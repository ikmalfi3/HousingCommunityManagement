using HousingCommunityManagement.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
namespace HousingCommunityManagement.Data;

public sealed partial class HouseholdDbContext
{
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<AccountingSettings> AccountingSetup => Set<AccountingSettings>();
    public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();
    public DbSet<JournalLine> JournalLines => Set<JournalLine>();
    private static void ConfigureAccounting(ModelBuilder builder)
    {
        var a = builder.Entity<Account>(); a.Ignore(x => x.DisplayName);
        a.Property(x => x.Code).UseCollation("NOCASE"); a.HasIndex(x => x.Code).IsUnique();
        builder.Entity<AccountingSettings>().ToTable("AccountingSetup");
        var e = builder.Entity<JournalEntry>(); e.HasIndex(x => new { x.Source, x.SourceId }).IsUnique();
        e.HasMany(x => x.Lines).WithOne().HasForeignKey(x => x.JournalEntryId).OnDelete(DeleteBehavior.Restrict);
        var l = builder.Entity<JournalLine>();
        l.HasOne<Account>().WithMany().HasForeignKey(x => x.AccountId).OnDelete(DeleteBehavior.Restrict);
        l.Property(x => x.Debit).HasConversion(v => (long)(v * 100m), v => v / 100m);
        l.Property(x => x.Credit).HasConversion(v => (long)(v * 100m), v => v / 100m);
    }
}
public sealed partial class HouseholdRepository : IAccountingRepository
{
    private static void EnsureColumn(HouseholdDbContext db, string table, string column, string definition)
    {
        using var command = db.Database.GetDbConnection().CreateCommand();
        command.Transaction = db.Database.CurrentTransaction?.GetDbTransaction();
        command.CommandText = $"PRAGMA table_info('{table}')";
        var exists = false;
        using (var reader = command.ExecuteReader()) while (reader.Read()) if (reader.GetString(1) == column) exists = true;
        if (!exists)
        {
            // Identifiers and definitions come only from the fixed schema calls in this assembly.
            command.CommandText = $"ALTER TABLE {table} ADD COLUMN {column} {definition}";
            command.ExecuteNonQuery();
        }
    }
    private static void EnsureAccountingSchema(HouseholdDbContext db)
    {
        using var tx = db.Database.CurrentTransaction is null ? db.Database.BeginTransaction() : null;
        db.Database.ExecuteSqlRaw("CREATE TABLE IF NOT EXISTS Accounts (Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, Code TEXT NOT NULL COLLATE NOCASE, Name TEXT NOT NULL, Type TEXT NOT NULL, IsCashBank INTEGER NOT NULL, IsActive INTEGER NOT NULL)");
        db.Database.ExecuteSqlRaw("CREATE UNIQUE INDEX IF NOT EXISTS IX_Accounts_Code ON Accounts(Code)");
        db.Database.ExecuteSqlRaw("CREATE TABLE IF NOT EXISTS AccountingSetup (Id INTEGER NOT NULL PRIMARY KEY, ReceivableAccountId INTEGER NOT NULL, IncomeAccountId INTEGER NOT NULL, DepositAccountId INTEGER NOT NULL, ReceiptAccountId INTEGER NOT NULL, PaymentAccountId INTEGER NOT NULL, OpeningEquityAccountId INTEGER NOT NULL, StartedOn TEXT NULL)");
        db.Database.ExecuteSqlRaw("CREATE TABLE IF NOT EXISTS JournalEntries (Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, Date TEXT NOT NULL, Description TEXT NOT NULL, Source TEXT NOT NULL, SourceId INTEGER NOT NULL)");
        db.Database.ExecuteSqlRaw("CREATE UNIQUE INDEX IF NOT EXISTS IX_JournalEntries_Source_SourceId ON JournalEntries(Source,SourceId)");
        db.Database.ExecuteSqlRaw("CREATE TABLE IF NOT EXISTS JournalLines (Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, JournalEntryId INTEGER NOT NULL REFERENCES JournalEntries(Id) ON DELETE RESTRICT, AccountId INTEGER NOT NULL REFERENCES Accounts(Id) ON DELETE RESTRICT, Debit INTEGER NOT NULL, Credit INTEGER NOT NULL)");
        db.Database.ExecuteSqlRaw("CREATE INDEX IF NOT EXISTS IX_JournalLines_JournalEntryId ON JournalLines(JournalEntryId)");
        EnsureColumn(db, "AccountingSetup", "PayableAccountId", "INTEGER NOT NULL DEFAULT 0");
        if (!db.AccountingSetup.Any())
        {
            Account Seed(string code, string name, string type, bool cash = false)
            {
                var a = new Account { Code = code, Name = name, Type = type, IsCashBank = cash };
                db.Accounts.Add(a); return a;
            }
            var bank = Seed("1000", "Community bank account", "Asset", true);
            Seed("1010", "Cash on hand", "Asset", true);
            var receivable = Seed("1100", "Maintenance fees receivable", "Asset");
            var deposit = Seed("2000", "Refundable renovation deposits", "Liability");
            var equity = Seed("3000", "Opening community funds", "Equity");
            var income = Seed("4000", "Maintenance fee income", "Income");
            Seed("5000", "Security expenses", "Expense"); Seed("5100", "Cleaning expenses", "Expense");
            Seed("5200", "Repairs and maintenance", "Expense"); Seed("5300", "Utilities", "Expense");
            db.SaveChanges();
            db.AccountingSetup.Add(new AccountingSettings { ReceivableAccountId = receivable.Id, IncomeAccountId = income.Id, DepositAccountId = deposit.Id, ReceiptAccountId = bank.Id, PaymentAccountId = bank.Id, OpeningEquityAccountId = equity.Id });
            db.SaveChanges();
        }
        tx?.Commit();
    }
    public List<Account> GetAccounts() { using var db = new HouseholdDbContext(databasePath); return db.Accounts.AsNoTracking().OrderBy(a => a.Code).ToList(); }
    public AccountingSettings GetAccountingSettings() { using var db = new HouseholdDbContext(databasePath); return db.AccountingSetup.AsNoTracking().Single(); }
    public List<JournalEntry> GetJournal() { using var db = new HouseholdDbContext(databasePath); return db.JournalEntries.Include(e => e.Lines).AsNoTracking().OrderByDescending(e => e.Date).ThenByDescending(e => e.Id).ToList(); }
    private static Account RequireAccount(HouseholdDbContext db, int id, string type, bool cash = false)
    {
        var a = db.Accounts.SingleOrDefault(x => x.Id == id);
        if (a is null || !a.IsActive || a.Type != type || (cash && !a.IsCashBank) || (!cash && a.IsCashBank)) throw new ArgumentException($"Choose an active {type.ToLowerInvariant()} account{(cash ? " marked as cash/bank" : " that is not cash/bank")}.");
        return a;
    }
    private static void ValidateSetup(HouseholdDbContext db, AccountingSettings s)
    {
        RequireAccount(db, s.ReceivableAccountId, "Asset"); RequireAccount(db, s.IncomeAccountId, "Income");
        if (s.PayableAccountId != 0) RequireAccount(db, s.PayableAccountId, "Liability");
        RequireAccount(db, s.DepositAccountId, "Liability"); RequireAccount(db, s.OpeningEquityAccountId, "Equity");
        RequireAccount(db, s.ReceiptAccountId, "Asset", true); RequireAccount(db, s.PaymentAccountId, "Asset", true);
    }
    public void SaveAccountingSettings(AccountingSettings settings)
    {
        using var db = new HouseholdDbContext(databasePath); using var tx = db.Database.BeginTransaction();
        var savedPayable = db.AccountingSetup.Single().PayableAccountId;
        if (settings.PayableAccountId == 0) settings.PayableAccountId = savedPayable;
        ValidateSetup(db, settings);
        var current = db.AccountingSetup.Single();
        current.PayableAccountId = settings.PayableAccountId;
        current.ReceivableAccountId = settings.ReceivableAccountId; current.IncomeAccountId = settings.IncomeAccountId;
        current.DepositAccountId = settings.DepositAccountId; current.ReceiptAccountId = settings.ReceiptAccountId;
        current.PaymentAccountId = settings.PaymentAccountId; current.OpeningEquityAccountId = settings.OpeningEquityAccountId;
        db.SaveChanges(); tx.Commit();
    }
    public void SaveAccount(Account account)
    {
        using var db = new HouseholdDbContext(databasePath); using var tx = db.Database.BeginTransaction();
        if (db.Accounts.Any(a => a.Id != account.Id && a.Code == account.Code)) throw new ArgumentException("That account code already exists.");
        var old = db.Accounts.SingleOrDefault(a => a.Id == account.Id);
        if (account.Id != 0 && old is null) throw new ArgumentException("Account no longer exists.");
        if (old is not null && db.JournalLines.Any(l => l.AccountId == account.Id) && (old.Type != account.Type || old.IsCashBank != account.IsCashBank)) throw new ArgumentException("The type and cash/bank designation cannot change after an account is used.");
        if (old is null) db.Accounts.Add(account);
        else { old.Code = account.Code; old.Name = account.Name; old.Type = account.Type; old.IsCashBank = account.IsCashBank; old.IsActive = account.IsActive; }
        db.SaveChanges(); ValidateSetup(db, db.AccountingSetup.Single()); tx.Commit();
    }
    private static void AddJournal(HouseholdDbContext db, DateTime date, string source, int sourceId, string description, params JournalLine[] lines)
    {
        if (lines.Length < 2 || lines.Any(l => l.Debit < 0 || l.Credit < 0 || (l.Debit > 0 && l.Credit > 0) || decimal.Round(l.Debit, 2) != l.Debit || decimal.Round(l.Credit, 2) != l.Credit) || lines.Sum(l => l.Debit) != lines.Sum(l => l.Credit)) throw new ArgumentException("Journal entries must balance to the sen.");
        if (db.JournalEntries.Any(e => e.Source == source && e.SourceId == sourceId)) throw new ArgumentException("This transaction has already been posted.");
        db.JournalEntries.Add(new JournalEntry { Date = date.Date, Source = source, SourceId = sourceId, Description = description, Lines = lines.ToList() });
    }
    private static int CashAccount(HouseholdDbContext db, int? selected, bool receipt)
    {
        var s = db.AccountingSetup.Single();
        var id = selected ?? (receipt ? s.ReceiptAccountId : s.PaymentAccountId);
        RequireAccount(db, id, "Asset", true); return id;
    }
    private static void CheckPostingDate(AccountingSettings s, DateTime date)
    {
        if (s.StartedOn.HasValue && date.Date < s.StartedOn.Value.Date) throw new ArgumentException($"Accounting started on {s.StartedOn:dd MMM yyyy}. Use a date on or after that date.");
    }
    private static string HouseNumber(HouseholdDbContext db, int id) => db.Households.Where(h => h.Id == id).Select(h => h.HouseNumber).Single();
    private static int OriginalControlAccount(HouseholdDbContext db, string source, string openingSource, int sourceId, bool debit)
    {
        var entry = db.JournalEntries.Include(e => e.Lines).SingleOrDefault(e => e.Source == source && e.SourceId == sourceId)
            ?? db.JournalEntries.Include(e => e.Lines).SingleOrDefault(e => e.Source == openingSource && e.SourceId == sourceId)
            ?? throw new ArgumentException("The original accounting entry is missing. This transaction cannot be posted.");
        return entry.Lines.Single(l => debit ? l.Debit > 0 : l.Credit > 0).AccountId;
    }
    public void StartAccounting(Dictionary<int, decimal> openingCashBalances)
    {
        using var db = new HouseholdDbContext(databasePath); using var tx = db.Database.BeginTransaction();
        var s = db.AccountingSetup.Single();
        if (s.StartedOn.HasValue) throw new ArgumentException("Accounting has already started. Opening balances cannot be entered twice.");
        ValidateSetup(db, s);
        var cashIds = db.Accounts.Where(a => a.IsActive && a.IsCashBank).Select(a => a.Id).ToHashSet();
        if (!cashIds.SetEquals(openingCashBalances.Keys) || openingCashBalances.Values.Any(v => v < 0 || decimal.Round(v, 2) != v)) throw new ArgumentException("Enter an opening balance for every active cash/bank account.");
        var today = DateTime.Today;
        foreach (var pair in openingCashBalances.Where(p => p.Value > 0))
            AddJournal(db, today, "OpeningCash", pair.Key, "Opening cash/bank balance", new() { AccountId = pair.Key, Debit = pair.Value }, new() { AccountId = s.OpeningEquityAccountId, Credit = pair.Value });
        foreach (var c in db.MonthlyCharges.Include(c => c.Payments).ToList().Where(c => c.Balance > 0))
            AddJournal(db, today, "OpeningCharge", c.Id, $"Opening receivable: house {HouseNumber(db, c.HouseholdId)}, {c.Year}-{c.Month:00}", new() { AccountId = s.ReceivableAccountId, Debit = c.Balance }, new() { AccountId = s.OpeningEquityAccountId, Credit = c.Balance });
        foreach (var r in db.Renovations.ToList().Where(r => r.DepositCollectedDate.HasValue && !r.DepositReturnedDate.HasValue && r.DepositAmount > 0))
            AddJournal(db, today, "OpeningDeposit", r.Id, $"Opening held deposit: house {HouseNumber(db, r.HouseholdId)}, {r.Description}", new() { AccountId = s.OpeningEquityAccountId, Debit = r.DepositAmount }, new() { AccountId = s.DepositAccountId, Credit = r.DepositAmount });
        s.StartedOn = today; db.SaveChanges(); tx.Commit();
    }
}

