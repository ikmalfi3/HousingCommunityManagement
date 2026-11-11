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
        EnsureColumn(db, "MonthlyCharges", "ChargeDate", "TEXT NULL");
        EnsureColumn(db, "MonthlyPayments", "CashAccountId", "INTEGER NULL REFERENCES Accounts(Id) ON DELETE RESTRICT");
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
    public int GenerateMonths(int year, int[] months, decimal amountPerMonth, DateTime? sharedDueDate, int[]? householdIds, IReadOnlyDictionary<int, DateTime>? monthDueDates = null, DateTime? chargeDate = null)
    {
        var postingDate = (chargeDate ?? DateTime.Today).Date;
        MonthlyBillingService.ValidateChargeDate(postingDate);
        MonthlyBillingService.ValidateAmount(amountPerMonth);
        foreach (var month in months) MonthlyBillingService.DueDate(year, month);
        MonthlyBillingService.ValidateMonthDueDates(months, sharedDueDate, monthDueDates);
        using var db = new HouseholdDbContext(databasePath);
        using var tx = db.Database.BeginTransaction();
        CheckPostingDate(db.AccountingSetup.Single(), postingDate);
        var billed = db.MonthlyCharges.Where(c => c.Year == year).Select(c => new { c.HouseholdId, c.Month }).ToList().Select(c => (c.HouseholdId, c.Month)).ToHashSet();
        var houses = db.Households.Where(h => h.IsActive).ToList();
        if (householdIds is not null)
        {
            if (householdIds.Length == 0 || householdIds.Any(id => houses.All(h => h.Id != id)))
                throw new ArgumentException("Select existing active houses to charge. Reopen the dialog if a house has changed.");
            houses = houses.Where(h => householdIds.Contains(h.Id)).ToList();
        }
        var charges = houses.SelectMany(h => months.Distinct().Where(m => !billed.Contains((h.Id, m))).Select(m => new MonthlyCharge {
            HouseholdId = h.Id, Year = year, Month = m, Amount = amountPerMonth, ChargeDate = postingDate,
            DueDate = monthDueDates is null ? sharedDueDate?.Date ?? MonthlyBillingService.DueDate(year, m) : monthDueDates[m].Date })).ToList();
        db.MonthlyCharges.AddRange(charges); db.SaveChanges();
        var setup = db.AccountingSetup.Single();
        if (setup.StartedOn.HasValue)
        {
            ValidateSetup(db, setup);
            foreach (var c in charges)
                AddJournal(db, postingDate, "MonthlyCharge", c.Id, $"Maintenance charge: house {HouseNumber(db, c.HouseholdId)}, {c.Year}-{c.Month:00}",
                    new() { AccountId = setup.ReceivableAccountId, Debit = c.Amount }, new() { AccountId = setup.IncomeAccountId, Credit = c.Amount });
            db.SaveChanges();
        }
        tx.Commit(); return charges.Count;
    }
    public int RecordPayments(IReadOnlyList<MaintenancePaymentRequest> payments, DateTime date, int? cashAccountId = null)
    {
        var selected = MonthlyBillingService.ValidatePaymentBatch(payments, date);
        using var db = new HouseholdDbContext(databasePath);
        using var tx = db.Database.BeginTransaction();
        var ids = selected.Select(p => p.ChargeId).ToArray();
        var charges = db.MonthlyCharges.Include(c => c.Payments).Where(c => ids.Contains(c.Id)).ToDictionary(c => c.Id);
        if (charges.Count != selected.Count) throw new ArgumentException("A selected charge no longer exists. Refresh unpaid charges and review the batch.");
        MonthlyBillingService.ValidatePaymentChargeDates(charges.Values, date);
        // Check every balance before inserting any source payment or journal entry.
        foreach (var request in selected)
            if (charges[request.ChargeId].Balance != request.ExpectedBalance)
                throw new ArgumentException("A selected charge's balance has changed. Refresh unpaid charges, review the amounts, and try again. No payments were saved.");
        var setup = db.AccountingSetup.Single(); CheckPostingDate(setup, date);
        var cash = CashAccount(db, cashAccountId, true);
        var recorded = selected.Select(p => new MaintenancePayment { MonthlyChargeId = p.ChargeId, Amount = p.Amount, Date = date.Date, Reference = p.Reference, CashAccountId = cash }).ToList();
        db.MonthlyPayments.AddRange(recorded); db.SaveChanges();
        if (setup.StartedOn.HasValue)
        {
            foreach (var payment in recorded)
            {
                var charge = charges[payment.MonthlyChargeId];
                var control = OriginalControlAccount(db, "MonthlyCharge", "OpeningCharge", charge.Id, true);
                AddJournal(db, date, "MaintenancePayment", payment.Id, $"Maintenance payment: house {HouseNumber(db, charge.HouseholdId)}, {charge.Year}-{charge.Month:00}. {payment.Reference}",
                    new() { AccountId = cash, Debit = payment.Amount }, new() { AccountId = control, Credit = payment.Amount });
            }
            db.SaveChanges();
        }
        tx.Commit(); return recorded.Count;
    }
    public void RecordPayment(int chargeId, decimal amount, DateTime date, string reference, int? cashAccountId = null)
    {
        using var db = new HouseholdDbContext(databasePath);
        using var tx = db.Database.BeginTransaction();
        var charge = db.MonthlyCharges.Include(c => c.Payments).SingleOrDefault(c => c.Id == chargeId) ?? throw new ArgumentException("Select an existing monthly charge.");
        MonthlyBillingService.ValidatePaymentChargeDates([charge], date);
        if (amount <= 0 || amount > charge.Balance) throw new ArgumentException("Payment must be greater than zero and no more than the outstanding balance for this month.");
        var setup = db.AccountingSetup.Single(); CheckPostingDate(setup, date);
        var cash = CashAccount(db, cashAccountId, true);
        var payment = new MaintenancePayment { MonthlyChargeId = chargeId, Amount = amount, Date = date, Reference = reference, CashAccountId = cash };
        db.MonthlyPayments.Add(payment); db.SaveChanges();
        if (setup.StartedOn.HasValue)
        {
            var control = OriginalControlAccount(db, "MonthlyCharge", "OpeningCharge", charge.Id, true);
            AddJournal(db, date, "MaintenancePayment", payment.Id, $"Maintenance payment: house {HouseNumber(db, charge.HouseholdId)}, {charge.Year}-{charge.Month:00}. {reference}",
                new() { AccountId = cash, Debit = amount }, new() { AccountId = control, Credit = amount });
            db.SaveChanges();
        }
        tx.Commit();
    }
}
