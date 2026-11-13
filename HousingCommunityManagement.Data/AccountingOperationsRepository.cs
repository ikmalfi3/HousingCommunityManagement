using HousingCommunityManagement.Core;
using Microsoft.EntityFrameworkCore;
namespace HousingCommunityManagement.Data;

public sealed partial class HouseholdDbContext
{
    public DbSet<FinanceRegisterItem> FinanceRegisters => Set<FinanceRegisterItem>();
    public DbSet<RegisterMovement> RegisterMovements => Set<RegisterMovement>();
    public DbSet<AccountingAdjustment> AccountingAdjustments => Set<AccountingAdjustment>();
    public DbSet<BankReconciliation> BankReconciliations => Set<BankReconciliation>();
    public DbSet<BankStatementLine> BankStatementLines => Set<BankStatementLine>();
    public DbSet<BankMatch> BankMatches => Set<BankMatch>();
    public DbSet<AccountingAuditEvent> AccountingAudit => Set<AccountingAuditEvent>();
    private static void ConfigureAccountingOperations(ModelBuilder builder)
    {
        var r = builder.Entity<FinanceRegisterItem>(); r.ToTable("FinanceRegisters"); r.Ignore(x => x.Cost); r.Ignore(x => x.AccumulatedDepreciation); r.Ignore(x => x.Balance); r.Ignore(x => x.Status);
        r.HasMany(x => x.Movements).WithOne().HasForeignKey(x => x.RegisterItemId).OnDelete(DeleteBehavior.Restrict);
        r.HasOne<Account>().WithMany().HasForeignKey(x => x.AccountId).OnDelete(DeleteBehavior.Restrict);
        var m = builder.Entity<RegisterMovement>(); m.Property(x => x.CostChange).HasConversion(v => (long)(v * 100m), v => v / 100m); m.Property(x => x.DepreciationChange).HasConversion(v => (long)(v * 100m), v => v / 100m);
        var a = builder.Entity<AccountingAdjustment>(); JsonList(a.Property(x => x.Lines)); JsonList(a.Property(x => x.Attachments));
        var b = builder.Entity<BankReconciliation>(); b.Property(x => x.From).HasColumnName("PeriodFrom"); b.Property(x => x.To).HasColumnName("PeriodTo");
        b.Property(x => x.OpeningBalance).HasConversion(v => (long)(v * 100m), v => v / 100m); b.Property(x => x.ClosingBalance).HasConversion(v => (long)(v * 100m), v => v / 100m);
        JsonList(b.Property(x => x.History)); b.HasMany(x => x.StatementLines).WithOne().HasForeignKey(x => x.ReconciliationId).OnDelete(DeleteBehavior.Restrict); b.HasMany(x => x.Matches).WithOne().HasForeignKey(x => x.ReconciliationId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<BankStatementLine>().Property(x => x.Amount).HasConversion(v => (long)(v * 100m), v => v / 100m);
        JsonList(builder.Entity<BankMatch>().Property(x => x.StatementLineIds)); JsonList(builder.Entity<BankMatch>().Property(x => x.JournalLineIds));
        builder.Entity<AccountingAuditEvent>().ToTable("AccountingAudit");
    }
}
public sealed partial class HouseholdRepository : IAccountingOperationsRepository
{
    private static void EnsureAccountingOperationsSchema(HouseholdDbContext db)
    {
        db.Database.ExecuteSqlRaw("CREATE TABLE IF NOT EXISTS FinanceRegisters (Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, Kind TEXT NOT NULL, Name TEXT NOT NULL, Party TEXT NOT NULL, Location TEXT NOT NULL, Notes TEXT NOT NULL, AcquisitionDate TEXT NOT NULL, AccountId INTEGER NOT NULL REFERENCES Accounts(Id), DepreciationAccountId INTEGER NULL REFERENCES Accounts(Id))");
        db.Database.ExecuteSqlRaw("CREATE TABLE IF NOT EXISTS RegisterMovements (Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, RegisterItemId INTEGER NOT NULL REFERENCES FinanceRegisters(Id), Date TEXT NOT NULL, Action TEXT NOT NULL, Description TEXT NOT NULL, CostChange INTEGER NOT NULL, DepreciationChange INTEGER NOT NULL, CashTransactionId INTEGER NULL, AdjustmentId INTEGER NULL)");
        db.Database.ExecuteSqlRaw("CREATE TABLE IF NOT EXISTS AccountingAdjustments (Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, Version INTEGER NOT NULL, Date TEXT NOT NULL, Description TEXT NOT NULL, Reference TEXT NOT NULL, Lines TEXT NOT NULL, Attachments TEXT NOT NULL, IsPosted INTEGER NOT NULL, ReversedDate TEXT NULL, ReversalReason TEXT NOT NULL, Action TEXT NOT NULL, RegisterItemId INTEGER NULL, BillId INTEGER NULL)");
        db.Database.ExecuteSqlRaw("CREATE TABLE IF NOT EXISTS BankReconciliations (Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, AccountId INTEGER NOT NULL REFERENCES Accounts(Id), PeriodFrom TEXT NOT NULL, PeriodTo TEXT NOT NULL, OpeningBalance INTEGER NOT NULL, ClosingBalance INTEGER NOT NULL, IsFinalized INTEGER NOT NULL, History TEXT NOT NULL)");
        db.Database.ExecuteSqlRaw("CREATE TABLE IF NOT EXISTS BankStatementLines (Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, ReconciliationId INTEGER NOT NULL REFERENCES BankReconciliations(Id), Date TEXT NOT NULL, Description TEXT NOT NULL, Reference TEXT NOT NULL, Amount INTEGER NOT NULL, ImportKey TEXT NOT NULL)");
        db.Database.ExecuteSqlRaw("CREATE TABLE IF NOT EXISTS BankMatches (Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, ReconciliationId INTEGER NOT NULL REFERENCES BankReconciliations(Id), StatementLineIds TEXT NOT NULL, JournalLineIds TEXT NOT NULL)");
        db.Database.ExecuteSqlRaw("CREATE TABLE IF NOT EXISTS AccountingAudit (Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, RecordedAt TEXT NOT NULL, Description TEXT NOT NULL)");
    }
    private static void Money(decimal amount, bool signed = false, bool zero = false)
    {
        if ((!signed && amount < 0) || (!zero && amount == 0) || Math.Abs(amount) > 1000000000m || decimal.Round(amount, 2) != amount) throw new ArgumentException("Enter a valid amount, at most RM 1 billion, with up to two decimals.");
    }
    private static void Audit(HouseholdDbContext db, string text) => db.AccountingAudit.Add(new AccountingAuditEvent { RecordedAt = DateTime.UtcNow, Description = text });
    private static void CheckReconciledEntry(HouseholdDbContext db, JournalEntry entry)
    {
        var ids = entry.Lines.Select(l => l.Id).ToHashSet();
        var final = db.BankReconciliations.Where(r => r.IsFinalized).Select(r => r.Id).ToList();
        if (db.BankMatches.Where(m => final.Contains(m.ReconciliationId)).AsEnumerable().Any(m => m.JournalLineIds.Any(ids.Contains))) throw new ArgumentException("This entry is matched to a finalized bank reconciliation. Reopen the reconciliation before correcting it.");
    }
    public List<AccountingAuditEvent> GetAccountingAudit() { using var db = new HouseholdDbContext(databasePath); return db.AccountingAudit.AsNoTracking().OrderByDescending(e => e.Id).ToList(); }
    public void SetAccountingLock(DateTime? through, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason) || reason.Length > 500) throw new ArgumentException("Enter a reason up to 500 characters for changing the lock.");
        if (through.HasValue) CashTransactionService.ValidateDate(through.Value);
        using var db = new HouseholdDbContext(databasePath); using var tx = db.Database.BeginTransaction(); var s = db.AccountingSetup.Single();
        if (!s.StartedOn.HasValue || through.HasValue && through.Value.Date < s.StartedOn.Value.Date) throw new ArgumentException("Start accounting first and choose a lock date on or after opening.");
        Audit(db, $"Transaction lock changed from {s.LockedThrough:dd MMM yyyy} to {(through.HasValue ? through.Value.ToString("dd MMM yyyy") : "unlocked")}: {reason.Trim()}");
        s.LockedThrough = through?.Date; db.SaveChanges(); tx.Commit();
    }
    private static void ValidateRegister(HouseholdDbContext db, FinanceRegisterItem item)
    {
        if (item.Id != 0 || item.Movements.Count > 0 || item.Kind is not ("Asset" or "Supplier deposit" or "Prepayment") || string.IsNullOrWhiteSpace(item.Name) || item.Name.Length > 200 || item.Party.Length > 150 || item.Location.Length > 200 || item.Notes.Length > 1000) throw new ArgumentException("Enter a new Asset, Supplier deposit or Prepayment, a name up to 200 characters, and valid details.");
        if (item.Kind != "Asset" && string.IsNullOrWhiteSpace(item.Party)) throw new ArgumentException("Enter the supplier holding this deposit or prepayment.");
        item.Name = item.Name.Trim(); item.Party = item.Party.Trim(); item.Location = item.Location.Trim(); item.Notes = item.Notes.Trim();
        CashTransactionService.ValidateDate(item.AcquisitionDate);
        RequireAccount(db, item.AccountId, "Asset");
        var controls = CashTransactionService.LinkedControlAccounts(db.AccountingSetup.Single(), db.JournalEntries.Include(e => e.Lines).ToList());
        if (controls.Contains(item.AccountId)) throw new ArgumentException("Choose a separate asset account, not a linked control account.");
        if (item.Kind == "Asset" && item.DepreciationAccountId.HasValue)
        {
            RequireAccount(db, item.DepreciationAccountId.Value, "Asset");
            if (item.DepreciationAccountId == item.AccountId || controls.Contains(item.DepreciationAccountId.Value)) throw new ArgumentException("Accumulated depreciation needs its own asset account.");
        }
        if (item.Kind != "Asset" && item.DepreciationAccountId.HasValue) throw new ArgumentException("Only equipment assets use accumulated depreciation.");
    }
    private static void ValidateRegisterTimeline(FinanceRegisterItem item)
    {
        decimal cost = 0, depreciation = 0;
        foreach (var day in item.Movements.GroupBy(m => m.Date.Date).OrderBy(g => g.Key))
        {
            cost += day.Sum(m => m.CostChange); depreciation += day.Sum(m => m.DepreciationChange);
            if (cost < 0 || depreciation < 0 || depreciation > cost) throw new ArgumentException("This date/amount would make the register balance or accumulated depreciation invalid. Review later activity.");
        }
    }
    public List<FinanceRegisterItem> GetFinanceRegisters() { using var db = new HouseholdDbContext(databasePath); return db.FinanceRegisters.Include(r => r.Movements).AsNoTracking().OrderBy(r => r.Kind).ThenBy(r => r.Name).ToList(); }
    public int AddFinanceRegister(FinanceRegisterItem item, int cashTransactionId, decimal amount, decimal openingDepreciation = 0)
    {
        using var db = new HouseholdDbContext(databasePath); using var tx = db.Database.BeginTransaction(); ValidateRegister(db, item);
        var saved = new FinanceRegisterItem { Kind = item.Kind, Name = item.Name.Trim(), Party = item.Party.Trim(), Location = item.Location.Trim(), Notes = item.Notes.Trim(), AcquisitionDate = item.AcquisitionDate.Date, AccountId = item.AccountId, DepreciationAccountId = item.DepreciationAccountId };
        db.FinanceRegisters.Add(saved); db.SaveChanges();
        if (cashTransactionId != 0)
        {
            if (openingDepreciation != 0) throw new ArgumentException("Purchase links start without accumulated depreciation.");
            LinkRegisterCashCore(db, saved, cashTransactionId, amount);
        }
        else
        {
            Money(amount); Money(openingDepreciation, zero: true);
            var opening = db.AccountingSetup.Single().StartedOn ?? throw new ArgumentException("Start accounting first.");
            if (item.AcquisitionDate.Date >= opening || openingDepreciation > amount || openingDepreciation > 0 && (item.Kind != "Asset" || !item.DepreciationAccountId.HasValue)) throw new ArgumentException("An existing opening item must be acquired before accounting starts, with valid accumulated depreciation.");
            var entries = db.JournalEntries.Include(e => e.Lines).Where(e => e.Date == opening && (e.Source == "OpeningAccount" || e.Source == "Adjustment")).ToList();
            var previous = db.FinanceRegisters.Include(r => r.Movements).ToList();
            var available = entries.SelectMany(e => e.Lines).Where(l => l.AccountId == item.AccountId).Sum(l => l.Debit - l.Credit) - previous.Where(r => r.AccountId == item.AccountId).SelectMany(r => r.Movements).Where(m => m.Action == "Adopt opening").Sum(m => m.CostChange);
            var availableDep = entries.SelectMany(e => e.Lines).Where(l => l.AccountId == item.DepreciationAccountId).Sum(l => l.Credit - l.Debit) - previous.Where(r => r.DepreciationAccountId == item.DepreciationAccountId).SelectMany(r => r.Movements).Where(m => m.Action == "Adopt opening").Sum(m => m.DepreciationChange);
            if (amount > available || openingDepreciation > availableDep && openingDepreciation > 0) throw new ArgumentException("The item exceeds the unregistered opening balance. Add missing opening balances first; adopting an existing balance creates no new journal.");
            saved.Movements.Add(new RegisterMovement { Date = opening, Action = "Adopt opening", Description = "Linked to existing opening ledger balance; no new money or journal", CostChange = amount, DepreciationChange = openingDepreciation });
            Audit(db, $"Register #{saved.Id} adopted from existing opening ledger: cost {amount:N2}, depreciation {openingDepreciation:N2}");
        }
        db.SaveChanges(); tx.Commit(); return saved.Id;
    }
    public void UpdateRegisterDetails(int id, string name, string party, string location, string notes, int? depreciationAccountId)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length > 200 || party.Length > 150 || location.Length > 200 || notes.Length > 1000) throw new ArgumentException("Enter valid register details within their length limits.");
        using var db = new HouseholdDbContext(databasePath); using var tx = db.Database.BeginTransaction(); var item = db.FinanceRegisters.SingleOrDefault(r => r.Id == id) ?? throw new ArgumentException("Select an existing item.");
        if (item.Kind != "Asset" && string.IsNullOrWhiteSpace(party)) throw new ArgumentException("Enter the supplier.");
        if (item.DepreciationAccountId != depreciationAccountId)
        {
            if (db.RegisterMovements.Any(m => m.RegisterItemId == id && m.DepreciationChange != 0)) throw new ArgumentException("The accumulated depreciation account is locked after depreciation history exists.");
            if (depreciationAccountId.HasValue)
            {
                RequireAccount(db, depreciationAccountId.Value, "Asset");
                var blocked = CashTransactionService.LinkedControlAccounts(db.AccountingSetup.Single(), db.JournalEntries.Include(e => e.Lines).ToList());
                if (item.Kind != "Asset" || depreciationAccountId.Value == item.AccountId || blocked.Contains(depreciationAccountId.Value)) throw new ArgumentException("Choose a separate accumulated depreciation asset account.");
            }
            Audit(db, $"Register #{id} accumulated depreciation account changed from {item.DepreciationAccountId} to {depreciationAccountId}"); item.DepreciationAccountId = depreciationAccountId;
        }
        Audit(db, $"Register #{id} details changed from [{item.Name}; {item.Party}; {item.Location}; {item.Notes}] to [{name.Trim()}; {party.Trim()}; {location.Trim()}; {notes.Trim()}]");
        item.Name = name.Trim(); item.Party = party.Trim(); item.Location = location.Trim(); item.Notes = notes.Trim(); db.SaveChanges(); tx.Commit();
    }
    public void LinkRegisterCash(int registerId, int cashTransactionId, decimal amount)
    {
        using var db = new HouseholdDbContext(databasePath); using var tx = db.Database.BeginTransaction();
        var item = db.FinanceRegisters.Include(r => r.Movements).Single(r => r.Id == registerId); LinkRegisterCashCore(db, item, cashTransactionId, amount); db.SaveChanges(); tx.Commit();
    }
    private static void LinkRegisterCashCore(HouseholdDbContext db, FinanceRegisterItem item, int cashId, decimal amount)
    {
        Money(amount);
        var cash = db.CashTransactions.SingleOrDefault(t => t.Id == cashId && !t.ReversedDate.HasValue) ?? throw new ArgumentException("Choose an unreversed money transaction.");
        CheckPostingDate(db.AccountingSetup.Single(), cash.Date);
        if (cash.Date.Date < item.AcquisitionDate.Date) throw new ArgumentException("The linked transaction cannot precede acquisition.");
        var available = cash.Allocations.Where(l => l.AccountId == item.AccountId).Sum(l => l.Amount);
        var allocated = db.RegisterMovements.Where(m => m.CashTransactionId == cashId).ToList().Where(m => db.FinanceRegisters.Find(m.RegisterItemId)!.AccountId == item.AccountId).Sum(m => Math.Abs(m.CostChange));
        if (allocated + amount > available) throw new ArgumentException("The amount exceeds the unlinked allocation to this register account.");
        if (item.Kind == "Asset" && cash.Direction == "Money in" && item.AccumulatedDepreciation > 0) throw new ArgumentException("Use disposal/write-off for a depreciated asset; record any sale proceeds separately against the disposal income account.");
        var movement = new RegisterMovement { Date = cash.Date.Date, RegisterItemId = item.Id, CashTransactionId = cash.Id, Action = cash.Direction == "Money out" ? "Purchase / paid" : "Refund", Description = $"Linked money transaction #{cash.Id}", CostChange = cash.Direction == "Money out" ? amount : -amount };
        item.Movements.Add(movement); ValidateRegisterTimeline(item);
        Audit(db, $"Register #{item.Id} linked to money transaction #{cash.Id}: RM {amount:N2}");
    }
    public void AddOpeningSchedules(OpeningDetails details, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason) || reason.Length > 500) throw new ArgumentException("Enter an opening-correction reason up to 500 characters.");
        using var db = new HouseholdDbContext(databasePath); using var tx = db.Database.BeginTransaction(); var settings = db.AccountingSetup.Single();
        if (!settings.StartedOn.HasValue) throw new ArgumentException("Use Start accounting for initial opening setup.");
        CheckPostingDate(settings, settings.StartedOn.Value);
        if (details.Accounts.Count + details.Registers.Count + details.SupplierBills.Count == 0) throw new ArgumentException("Enter at least one missing opening balance or supporting record.");
        AddExtendedOpening(db, settings, settings.StartedOn.Value.Date, details);
        Audit(db, $"Opening schedules supplemented without replacing original entries: {reason.Trim()}"); db.SaveChanges(); tx.Commit();
    }
    private static void AddExtendedOpening(HouseholdDbContext db, AccountingSettings settings, DateTime date, OpeningDetails details)
    {
        var controls = CashTransactionService.LinkedControlAccounts(settings, db.JournalEntries.Include(e => e.Lines).ToList());
        controls.Add(settings.OpeningEquityAccountId);
        if (details.Accounts.Select(l => l.AccountId).Distinct().Count() != details.Accounts.Count) throw new ArgumentException("Enter each additional opening account once.");
        foreach (var line in details.Accounts)
        {
            Money(line.Debit, zero: true); Money(line.Credit, zero: true);
            var account = db.Accounts.SingleOrDefault(a => a.Id == line.AccountId && a.IsActive && !a.IsCashBank && (a.Type == "Asset" || a.Type == "Liability" || a.Type == "Equity"));
            if (account is null || controls.Contains(line.AccountId) || line.Debit > 0 && line.Credit > 0) throw new ArgumentException("Use a separate asset/liability/equity opening account. Enter linked balances in their supporting schedules.");
            if (details.Registers.Any(r => r.AccountId == line.AccountId || r.DepreciationAccountId == line.AccountId)) throw new ArgumentException("A register opening already supplies this account. Do not enter its balance twice.");
            if (line.Debit + line.Credit > 0) AddJournal(db, date, "OpeningAccount", line.AccountId, $"Opening balance: {account.DisplayName}", new() { AccountId = line.AccountId, Debit = line.Debit, Credit = line.Credit }, new() { AccountId = settings.OpeningEquityAccountId, Debit = line.Credit, Credit = line.Debit });
        }
        foreach (var entry in details.Registers)
        {
            Money(entry.Cost); Money(entry.AccumulatedDepreciation, zero: true);
            if (db.JournalEntries.Any(e => e.Source == "OpeningAccount" && (e.SourceId == entry.AccountId || e.SourceId == entry.DepreciationAccountId))) throw new ArgumentException("This register account already has an opening balance. Adopt that balance from the register screen instead of entering it twice.");
            if (entry.AcquisitionDate.Date >= date || entry.AccumulatedDepreciation > entry.Cost || entry.Kind != "Asset" && entry.AccumulatedDepreciation != 0 || entry.AccumulatedDepreciation > 0 && !entry.DepreciationAccountId.HasValue) throw new ArgumentException("Opening items must be acquired before opening, with accumulated depreciation no greater than cost.");
            var item = new FinanceRegisterItem { Kind = entry.Kind, Name = entry.Name, Party = entry.Party, Location = entry.Location, AcquisitionDate = entry.AcquisitionDate.Date, AccountId = entry.AccountId, DepreciationAccountId = entry.DepreciationAccountId };
            ValidateRegister(db, item); db.FinanceRegisters.Add(item); db.SaveChanges();
            item.Movements.Add(new RegisterMovement { Date = date, Action = "Opening", CostChange = entry.Cost, DepreciationChange = entry.AccumulatedDepreciation, Description = "Brought forward at accounting opening" });
            var lines = new List<JournalLine> { new() { AccountId = item.AccountId, Debit = entry.Cost }, new() { AccountId = settings.OpeningEquityAccountId, Credit = entry.Cost - entry.AccumulatedDepreciation } };
            if (entry.AccumulatedDepreciation > 0) lines.Add(new() { AccountId = entry.DepreciationAccountId!.Value, Credit = entry.AccumulatedDepreciation });
            AddJournal(db, date, "OpeningRegister", item.Id, $"Opening {item.Kind}: {item.Name}", lines.ToArray());
        }
        foreach (var entry in details.SupplierBills)
        {
            Money(entry.Amount); CashTransactionService.ValidateDate(entry.DocumentDate);
            var payee = db.Payees.SingleOrDefault(p => p.Id == entry.PayeeId && p.IsActive) ?? throw new ArgumentException("Select an active supplier/claimant for opening bills.");
            if (entry.DocumentDate.Date >= date || string.IsNullOrWhiteSpace(entry.Description) || entry.Description.Length > 500 || entry.Reference.Length > 150) throw new ArgumentException("Opening bills need a document date before opening and a description/reference within the normal limits.");
            if (entry.Reference.Length > 0 && db.ExpenseBills.Any(b => b.PayeeId == entry.PayeeId && b.Reference == entry.Reference && b.CancelledDate == null)) throw new ArgumentException("An opening supplier reference already exists.");
            var category = db.ExpenseCategories.FirstOrDefault(c => c.IsActive) ?? throw new ArgumentException("Create at least one bill category first.");
            var bill = new ExpenseBill { PayeeId = payee.Id, PayeeName = payee.Name, Kind = payee.Kind == "Claimant" ? "Reimbursement claim" : "Supplier bill", Description = entry.Description.Trim(), Reference = entry.Reference.Trim(), DocumentDate = entry.DocumentDate.Date, AccountingDate = date, DueDate = entry.DueDate?.Date, IsPosted = true, IsOpeningBalance = true, Version = 1, Lines = [new() { CategoryId = category.Id, AccountId = settings.OpeningEquityAccountId, Description = "Opening amount owed (not a current expense)", Amount = entry.Amount }] };
            db.ExpenseBills.Add(bill); db.SaveChanges();
            AddJournal(db, date, "OpeningExpense", bill.Id, $"Opening payable: {payee.Name}. {entry.Reference}", new() { AccountId = settings.OpeningEquityAccountId, Debit = entry.Amount }, new() { AccountId = settings.PayableAccountId, Credit = entry.Amount });
        }
        Audit(db, $"Opening setup dated {date:dd MMM yyyy}: {details.Accounts.Count} additional accounts, {details.Registers.Count} register items, {details.SupplierBills.Count} supplier bills. Net opening funds balances automatically against the supporting schedules.");
    }
}
