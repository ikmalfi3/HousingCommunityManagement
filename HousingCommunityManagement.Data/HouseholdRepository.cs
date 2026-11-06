using System.Text.Json;
using HousingCommunityManagement.Core;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
namespace HousingCommunityManagement.Data;
public sealed partial class HouseholdDbContext(string databasePath) : DbContext
{
    public DbSet<Household> Households => Set<Household>();
    public DbSet<CommunitySettings> Settings => Set<CommunitySettings>();
    protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite(new SqliteConnectionStringBuilder { DataSource = databasePath }.ToString());
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureBilling(modelBuilder);
        ConfigureRenovations(modelBuilder);
        ConfigureAccounting(modelBuilder);
        ConfigureExpenses(modelBuilder);
        var h = modelBuilder.Entity<Household>();
        h.Property(x => x.HouseNumber).UseCollation("NOCASE"); h.HasIndex(x => x.HouseNumber).IsUnique();
        h.Ignore(x => x.OwnerSummary); h.Ignore(x => x.ContactSummary); h.Ignore(x => x.FlagSummary);
        JsonList(h.Property(x => x.Owners)); JsonList(h.Property(x => x.Contacts)); JsonList(h.Property(x => x.Flags));
        var s = modelBuilder.Entity<CommunitySettings>(); JsonList(s.Property(x => x.FlagNames));
        s.Property(x => x.MonthlyMaintenanceFee).HasConversion(v => (long)(v * 100m), v => v / 100m);
    }
    private static void JsonList<T>(Microsoft.EntityFrameworkCore.Metadata.Builders.PropertyBuilder<List<T>> property)
    {
        property.HasConversion(v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null), v => JsonSerializer.Deserialize<List<T>>(v, (JsonSerializerOptions?)null)!);
        property.Metadata.SetValueComparer(new ValueComparer<List<T>>(
            (a,b) => JsonSerializer.Serialize(a, (JsonSerializerOptions?)null) == JsonSerializer.Serialize(b, (JsonSerializerOptions?)null),
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null).GetHashCode(),
            v => JsonSerializer.Deserialize<List<T>>(JsonSerializer.Serialize(v, (JsonSerializerOptions?)null), (JsonSerializerOptions?)null)!));
    }
}
public sealed partial class HouseholdRepository : IHouseholdRepository, IMonthlyBillingRepository
{
    private readonly string databasePath;
    public HouseholdRepository(string databasePath)
    {
        this.databasePath = databasePath;
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(databasePath))!);
        using var db = new HouseholdDbContext(databasePath);
        db.Database.OpenConnection();
        using var inspect = db.Database.GetDbConnection().CreateCommand(); inspect.CommandText = "PRAGMA table_info('Households')";
        var columns = new List<string>();
        using (var reader = inspect.ExecuteReader()) while (reader.Read()) columns.Add(reader.GetString(1));
        if (columns.Contains("OwnerName")) UpgradeLegacy(db); else db.Database.EnsureCreated();
        if (columns.Count > 0 && !columns.Contains("Race"))
            db.Database.ExecuteSqlRaw("ALTER TABLE Households ADD COLUMN Race TEXT NOT NULL DEFAULT ''");
        EnsureAccountingSchema(db);
        EnsureBillingSchema(db);
        EnsureRenovationSchema(db);
        EnsureExpenseSchema(db);

        if (!db.Settings.Any()) { db.Settings.Add(new CommunitySettings()); db.SaveChanges(); }
        // Remove the original starter suggestions, retaining any already used on houses.
        var settings = db.Settings.Single();
        string[] originalDefaults = ["Occupied", "Vacant", "Rented", "Under renovation", "Payment due"];
        if (settings.FlagNames.SequenceEqual(originalDefaults))
        {
            var used = db.Households.AsNoTracking().ToList().SelectMany(h => h.Flags).Select(f => f.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
            settings.FlagNames = settings.FlagNames.Where(used.Contains).ToList();
            db.SaveChanges();
        }
    }
    private static void UpgradeLegacy(HouseholdDbContext db)
    {
        using var transaction = db.Database.BeginTransaction();
        db.Database.ExecuteSqlRaw("ALTER TABLE Households ADD COLUMN IsOwned INTEGER NOT NULL DEFAULT 0");
        db.Database.ExecuteSqlRaw("ALTER TABLE Households ADD COLUMN Owners TEXT NOT NULL DEFAULT '[]'");
        db.Database.ExecuteSqlRaw("ALTER TABLE Households ADD COLUMN Contacts TEXT NOT NULL DEFAULT '[]'");
        db.Database.ExecuteSqlRaw("ALTER TABLE Households ADD COLUMN Flags TEXT NOT NULL DEFAULT '[]'");
        db.Database.ExecuteSqlRaw("UPDATE Households SET IsOwned = length(trim(OwnerName)) > 0, Owners = CASE WHEN length(trim(OwnerName)) > 0 THEN json_array(OwnerName) ELSE '[]' END, Contacts = CASE WHEN length(trim(ContactNumber)) > 0 THEN json_array(ContactNumber) ELSE '[]' END");
        db.Database.ExecuteSqlRaw("CREATE TABLE LegacyHouseholdFees AS SELECT Id AS HouseholdId, MonthlyFee AS MonthlyFeeSen FROM Households");
        db.Database.ExecuteSqlRaw("CREATE TABLE Settings (Id INTEGER NOT NULL PRIMARY KEY, MonthlyMaintenanceFee INTEGER NOT NULL, FlagNames TEXT NOT NULL)");
        var settings = new CommunitySettings();
        using var command = db.Database.GetDbConnection().CreateCommand(); command.Transaction = transaction.GetDbTransaction();
        command.CommandText = "SELECT COALESCE(MAX(MonthlyFee),0) FROM Households";
        settings.MonthlyMaintenanceFee = Convert.ToInt64(command.ExecuteScalar()) / 100m;
        db.Settings.Add(settings); db.SaveChanges();
        db.Database.ExecuteSqlRaw("ALTER TABLE Households DROP COLUMN OwnerName");
        db.Database.ExecuteSqlRaw("ALTER TABLE Households DROP COLUMN ContactNumber");
        db.Database.ExecuteSqlRaw("ALTER TABLE Households DROP COLUMN MonthlyFee");
        transaction.Commit();
    }
    public List<Household> GetAll()
    {
        using var db = new HouseholdDbContext(databasePath);
        var houses = db.Households.AsNoTracking().OrderBy(h => h.HouseNumber).ToList();
        DerivedHouseFlags.Apply(houses, db.MonthlyCharges.Include(c => c.Payments).AsNoTracking().ToList(),
            db.Renovations.AsNoTracking().ToList(), DateTime.Today);
        return houses;
    }
    public void Save(Household household)
    {
        using var db = new HouseholdDbContext(databasePath);
        // Automatic flags are a live projection, never a saved snapshot that can go stale.
        var saved = new Household { Id = household.Id, HouseNumber = household.HouseNumber, IsOwned = household.IsOwned,
            Race = household.Race, Owners = household.Owners, Contacts = household.Contacts, IsActive = household.IsActive,
            Flags = household.Flags.Where(f => !DerivedHouseFlags.IsManaged(f)).ToList() };
        if (saved.Id == 0) db.Households.Add(saved); else db.Households.Update(saved);
        SaveChanges(db); household.Id = saved.Id;
    }
    public int AddMissing(List<Household> households)
    {
        using var db = new HouseholdDbContext(databasePath); using var transaction = db.Database.BeginTransaction();
        var existing = db.Households.Select(h => h.HouseNumber).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var missing = households.Where(h => existing.Add(h.HouseNumber)).ToList();
        db.Households.AddRange(missing); SaveChanges(db); transaction.Commit(); return missing.Count;
    }
    public void DeleteFlag(string name, int? currentHouseId)
    {
        using var db = new HouseholdDbContext(databasePath);
        using var transaction = db.Database.BeginTransaction();
        var houses = db.Households.ToList();
        var otherUses = houses.Count(h => h.Id != currentHouseId && h.Flags.Any(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase)));
        if (otherUses > 0)
            throw new ArgumentException($"This flag is used by {otherUses} other house(s), including inactive houses. Remove it from those houses and save them before deleting the option.");
        var current = houses.SingleOrDefault(h => h.Id == currentHouseId);
        current?.Flags.RemoveAll(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        var settings = db.Settings.Single();
        settings.FlagNames.RemoveAll(n => n.Equals(name, StringComparison.OrdinalIgnoreCase));
        db.SaveChanges();
        transaction.Commit();
    }
    public CommunitySettings GetSettings() { using var db = new HouseholdDbContext(databasePath); return db.Settings.AsNoTracking().Single(); }
    public void SaveSettings(CommunitySettings settings) { using var db = new HouseholdDbContext(databasePath); settings.Id = 1; db.Settings.Update(settings); db.SaveChanges(); }
    private static void SaveChanges(HouseholdDbContext db)
    {
        try { db.SaveChanges(); }
        catch (DbUpdateException ex) when (ex.InnerException is SqliteException { SqliteExtendedErrorCode: 2067 })
        { throw new ArgumentException("That house number already exists. Edit the existing house instead.", ex); }
    }
}
