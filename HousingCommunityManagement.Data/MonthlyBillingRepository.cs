using HousingCommunityManagement.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
namespace HousingCommunityManagement.Data;

public sealed partial class HouseholdDbContext
{
    public DbSet<MonthlyCharge> MonthlyCharges => Set<MonthlyCharge>();
    public DbSet<MaintenancePayment> MonthlyPayments => Set<MaintenancePayment>();
    private static void ConfigureBilling(ModelBuilder builder)
    {
        var charge = builder.Entity<MonthlyCharge>();
        charge.HasIndex(c => new { c.HouseholdId, c.Year, c.Month }).IsUnique();
        charge.HasOne<Household>().WithMany().HasForeignKey(c => c.HouseholdId).OnDelete(DeleteBehavior.Restrict);
        charge.HasMany(c => c.Payments).WithOne().HasForeignKey(p => p.MonthlyChargeId).OnDelete(DeleteBehavior.Restrict);
        charge.Property(c => c.Amount).HasConversion(v => (long)(v * 100m), v => v / 100m);
        charge.Ignore(c => c.Paid); charge.Ignore(c => c.Balance);
        builder.Entity<MaintenancePayment>().Property(p => p.Amount).HasConversion(v => (long)(v * 100m), v => v / 100m);
    }
}
public sealed partial class HouseholdRepository
{
    private static void EnsureBillingSchema(HouseholdDbContext db)
    {
        using var tx = db.Database.BeginTransaction();
        db.Database.ExecuteSqlRaw("CREATE TABLE IF NOT EXISTS MonthlyCharges (Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, HouseholdId INTEGER NOT NULL REFERENCES Households(Id) ON DELETE RESTRICT, Year INTEGER NOT NULL, Month INTEGER NOT NULL, Amount INTEGER NOT NULL, DueDate TEXT NOT NULL)");
        db.Database.ExecuteSqlRaw("CREATE UNIQUE INDEX IF NOT EXISTS IX_MonthlyCharges_HouseholdId_Year_Month ON MonthlyCharges(HouseholdId, Year, Month)");
        db.Database.ExecuteSqlRaw("CREATE TABLE IF NOT EXISTS MonthlyPayments (Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, MonthlyChargeId INTEGER NOT NULL REFERENCES MonthlyCharges(Id) ON DELETE RESTRICT, Amount INTEGER NOT NULL, Date TEXT NOT NULL, Reference TEXT NOT NULL, LegacyPaymentId INTEGER NULL)");
        db.Database.ExecuteSqlRaw("CREATE INDEX IF NOT EXISTS IX_MonthlyPayments_MonthlyChargeId ON MonthlyPayments(MonthlyChargeId)");
        db.Database.ExecuteSqlRaw("CREATE TABLE IF NOT EXISTS BillingUpgrades (Version INTEGER PRIMARY KEY)");
        using var command = db.Database.GetDbConnection().CreateCommand();
        command.Transaction = tx.GetDbTransaction();
        command.CommandText = "SELECT COUNT(*) FROM BillingUpgrades WHERE Version=1";
        if (Convert.ToInt32(command.ExecuteScalar()) == 0)
        {
            command.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='QuarterlyCharges'";
            if (Convert.ToInt32(command.ExecuteScalar()) > 0) MigrateQuarterly(db);
            db.Database.ExecuteSqlRaw("INSERT INTO BillingUpgrades(Version) VALUES(1)");
        }
        tx.Commit();
    }
    private static void MigrateQuarterly(HouseholdDbContext db)
    {
        // Retain original quarterly tables as an archive. Split exact sen and allocate old payments oldest month first.
        var oldCharges = new List<(int Id, int House, int Year, int Quarter, long Amount, DateTime Due)>();
        using var command = db.Database.GetDbConnection().CreateCommand();
        command.Transaction = db.Database.CurrentTransaction!.GetDbTransaction();
        command.CommandText = "SELECT Id, HouseholdId, Year, Quarter, Amount, DueDate FROM QuarterlyCharges ORDER BY Id";
        using (var reader = command.ExecuteReader())
            while (reader.Read()) oldCharges.Add((reader.GetInt32(0),reader.GetInt32(1),reader.GetInt32(2),reader.GetInt32(3),reader.GetInt64(4),reader.GetDateTime(5)));
        foreach (var old in oldCharges)
        {
            var months = Enumerable.Range(0, 3).Select(i => new MonthlyCharge { HouseholdId = old.House, Year = old.Year, Month = (old.Quarter - 1) * 3 + i + 1,
                Amount = (old.Amount / 3 + (i < old.Amount % 3 ? 1 : 0)) / 100m, DueDate = old.Due }).ToList();
            db.MonthlyCharges.AddRange(months); db.SaveChanges();
            var payments = new List<(int Id, long Amount, DateTime Date, string Reference)>();
            command.CommandText = "SELECT Id, Amount, Date, Reference FROM MaintenancePayments WHERE QuarterlyChargeId = $id ORDER BY Date, Id";
            command.Parameters.Clear(); var parameter = command.CreateParameter(); parameter.ParameterName = "$id"; parameter.Value = old.Id; command.Parameters.Add(parameter);
            using (var reader = command.ExecuteReader())
                while (reader.Read()) payments.Add((reader.GetInt32(0),reader.GetInt64(1),reader.GetDateTime(2),reader.GetString(3)));
            foreach (var payment in payments)
            {
                var remaining = payment.Amount / 100m;
                foreach (var month in months)
                {
                    var allocation = Math.Min(remaining, month.Balance);
                    if (allocation <= 0) continue;
                    month.Payments.Add(new MaintenancePayment { Amount = allocation, Date = payment.Date, Reference = payment.Reference, LegacyPaymentId = payment.Id });
                    remaining -= allocation;
                }
                if (remaining != 0) throw new InvalidOperationException("An original payment exceeds its quarterly charge. Migration was rolled back.");
            }
            db.SaveChanges();
        }
    }
    public List<MonthlyCharge> GetCharges()
    {
        using var db = new HouseholdDbContext(databasePath);
        return db.MonthlyCharges.Include(c => c.Payments).AsNoTracking().ToList();
    }
    public int GenerateMonths(int year, int[] months, DateTime? sharedDueDate, int[]? householdIds)
    {
        foreach (var month in months) MonthlyBillingService.DueDate(year, month);
        using var db = new HouseholdDbContext(databasePath);
        using var tx = db.Database.BeginTransaction();
        var fee = db.Settings.Single().MonthlyMaintenanceFee;
        if (fee <= 0) throw new ArgumentException("Set a monthly maintenance rate greater than zero in Community settings before generating charges.");
        var billed = db.MonthlyCharges.Where(c => c.Year == year).Select(c => new { c.HouseholdId, c.Month }).ToList().Select(c => (c.HouseholdId, c.Month)).ToHashSet();
        var houses = db.Households.Where(h => h.IsActive).ToList();
        if (householdIds is not null)
        {
            if (householdIds.Length == 0 || householdIds.Any(id => houses.All(h => h.Id != id)))
                throw new ArgumentException("Select existing active houses to charge. Reopen the dialog if a house has changed.");
            houses = houses.Where(h => householdIds.Contains(h.Id)).ToList();
        }
        var charges = houses.SelectMany(h => months.Distinct().Where(m => !billed.Contains((h.Id, m))).Select(m => new MonthlyCharge {
            HouseholdId = h.Id, Year = year, Month = m, Amount = fee, DueDate = sharedDueDate ?? MonthlyBillingService.DueDate(year, m) })).ToList();
        db.MonthlyCharges.AddRange(charges); db.SaveChanges(); tx.Commit(); return charges.Count;
    }
    public void RecordPayment(int chargeId, decimal amount, DateTime date, string reference)
    {
        using var db = new HouseholdDbContext(databasePath);
        using var tx = db.Database.BeginTransaction();
        var charge = db.MonthlyCharges.Include(c => c.Payments).SingleOrDefault(c => c.Id == chargeId) ?? throw new ArgumentException("Select an existing monthly charge.");
        if (amount <= 0 || amount > charge.Balance) throw new ArgumentException("Payment must be greater than zero and no more than the outstanding balance for this month.");
        db.MonthlyPayments.Add(new MaintenancePayment { MonthlyChargeId = chargeId, Amount = amount, Date = date, Reference = reference });
        db.SaveChanges(); tx.Commit();
    }
}
