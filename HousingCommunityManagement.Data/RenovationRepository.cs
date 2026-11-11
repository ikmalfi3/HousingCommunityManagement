using HousingCommunityManagement.Core;
using Microsoft.EntityFrameworkCore;
namespace HousingCommunityManagement.Data;

public sealed partial class HouseholdDbContext
{
    public DbSet<Renovation> Renovations => Set<Renovation>();
    private static void ConfigureRenovations(ModelBuilder builder)
    {
        var r = builder.Entity<Renovation>();
        r.HasOne<Household>().WithMany().HasForeignKey(x => x.HouseholdId).OnDelete(DeleteBehavior.Restrict);
        r.Property(x => x.DepositAmount).HasConversion(v => (long)(v * 100m), v => v / 100m);
        r.Ignore(x => x.PlannedEndDate); r.Ignore(x => x.DepositStatus);
    }
}

public sealed partial class HouseholdRepository : IRenovationRepository
{
    private static void EnsureRenovationSchema(HouseholdDbContext db)
    {
        db.Database.ExecuteSqlRaw("CREATE TABLE IF NOT EXISTS Renovations (Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, HouseholdId INTEGER NOT NULL REFERENCES Households(Id) ON DELETE RESTRICT, Description TEXT NOT NULL, StartDate TEXT NOT NULL, DurationDays INTEGER NOT NULL, DepositAmount INTEGER NOT NULL, DepositCollectedDate TEXT NULL, CompletedDate TEXT NULL, DepositReturnedDate TEXT NULL)");
        db.Database.ExecuteSqlRaw("CREATE INDEX IF NOT EXISTS IX_Renovations_HouseholdId ON Renovations(HouseholdId)");
        EnsureColumn(db, "Renovations", "IsArchived", "INTEGER NOT NULL DEFAULT 0");
        EnsureColumn(db, "Renovations", "DepositReceivedAccountId", "INTEGER NULL REFERENCES Accounts(Id) ON DELETE RESTRICT");
        EnsureColumn(db, "Renovations", "DepositReturnedAccountId", "INTEGER NULL REFERENCES Accounts(Id) ON DELETE RESTRICT");
    }
    public List<Renovation> GetRenovations(bool includeArchived = false)
    {
        using var db = new HouseholdDbContext(databasePath);
        return db.Renovations.AsNoTracking().Where(x => includeArchived || !x.IsArchived).OrderByDescending(x => x.StartDate).ThenByDescending(x => x.Id).ToList();
    }
    public void AddRenovation(Renovation renovation)
    {
        using var db = new HouseholdDbContext(databasePath);
        if (!db.Households.Any(h => h.Id == renovation.HouseholdId && h.IsActive && h.IsOwned)) throw new ArgumentException("Select an active, owned house. Refresh if its details have changed.");
        db.Renovations.Add(renovation); db.SaveChanges();
    }
    public void DeleteRenovation(int id)
    {
        using var db = new HouseholdDbContext(databasePath);
        var renovation = db.Renovations.SingleOrDefault(r => r.Id == id)
            ?? throw new ArgumentException("Select an existing renovation. Refresh if it has already been deleted.");
        if (renovation.IsArchived) throw new ArgumentException("This renovation is already archived.");
        if (renovation.DepositCollectedDate.HasValue) renovation.IsArchived = true;
        else db.Renovations.Remove(renovation);
        db.SaveChanges();
    }
    public void RecordRenovationAction(int id, string action, DateTime date, int? cashAccountId = null)
    {
        using var db = new HouseholdDbContext(databasePath);
        using var tx = db.Database.BeginTransaction();
        var r = db.Renovations.SingleOrDefault(x => x.Id == id) ?? throw new ArgumentException("Select an existing renovation.");
        var setup = db.AccountingSetup.Single();
        date = date.Date;
        if (date.Year < 1900 || date > DateTime.Today) throw new ArgumentException("Choose a recorded date from 1900 through today.");
        if (r.IsArchived && action != "Return") throw new ArgumentException("Only a deposit return can be recorded for an archived renovation.");
        switch (action)
        {
            case "Collect":
                if (r.DepositAmount <= 0 || r.DepositCollectedDate.HasValue || r.CompletedDate.HasValue) throw new ArgumentException("Only an outstanding deposit on an unfinished renovation can be collected.");
                CheckPostingDate(setup, date);
                r.DepositReceivedAccountId = CashAccount(db, cashAccountId, true);
                r.DepositCollectedDate = date;
                if (setup.StartedOn.HasValue)
                    AddJournal(db, date, "RenovationCollection", r.Id, $"Renovation deposit: house {HouseNumber(db, r.HouseholdId)}, {r.Description}",
                        new() { AccountId = r.DepositReceivedAccountId.Value, Debit = r.DepositAmount }, new() { AccountId = setup.DepositAccountId, Credit = r.DepositAmount });
                break;
            case "Complete":
                if (r.DepositAmount > 0 && !r.DepositCollectedDate.HasValue) throw new ArgumentException("Collect the deposit before completing this renovation.");
                if (r.CompletedDate.HasValue || date < r.StartDate || date < r.DepositCollectedDate) throw new ArgumentException("Completion must be on or after the start and deposit collection dates, and can only be recorded once.");
                r.CompletedDate = date; break;
            case "Return":
                if ((!r.CompletedDate.HasValue && !r.IsArchived) || !r.DepositCollectedDate.HasValue || r.DepositReturnedDate.HasValue || date < r.CompletedDate || date < r.DepositCollectedDate) throw new ArgumentException("Return a collected deposit once, after completion or archiving, and not before collection.");
                CheckPostingDate(setup, date);
                r.DepositReturnedAccountId = CashAccount(db, cashAccountId, false);
                r.DepositReturnedDate = date;
                if (setup.StartedOn.HasValue)
                {
                    var control = OriginalControlAccount(db, "RenovationCollection", "OpeningDeposit", r.Id, false);
                    AddJournal(db, date, "RenovationReturn", r.Id, $"Renovation deposit returned: house {HouseNumber(db, r.HouseholdId)}, {r.Description}",
                        new() { AccountId = control, Debit = r.DepositAmount }, new() { AccountId = r.DepositReturnedAccountId.Value, Credit = r.DepositAmount });
                }
                break;
            default: throw new ArgumentException("Choose a valid renovation action.");
        }
        db.SaveChanges(); tx.Commit();
    }
}
