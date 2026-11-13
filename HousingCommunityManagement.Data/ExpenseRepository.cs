using HousingCommunityManagement.Core;
using Microsoft.EntityFrameworkCore;
namespace HousingCommunityManagement.Data;

public sealed partial class HouseholdDbContext
{
    public DbSet<Payee> Payees => Set<Payee>();
    public DbSet<ExpenseCategory> ExpenseCategories => Set<ExpenseCategory>();
    public DbSet<ExpenseBill> ExpenseBills => Set<ExpenseBill>();
    public DbSet<ExpenseLine> ExpenseLines => Set<ExpenseLine>();
    public DbSet<ExpensePayment> ExpensePayments => Set<ExpensePayment>();
    public DbSet<ExpenseAttachment> ExpenseAttachments => Set<ExpenseAttachment>();
    private static void ConfigureExpenses(ModelBuilder builder)
    {
        var p = builder.Entity<Payee>(); p.Property(x => x.Name).UseCollation("NOCASE"); p.HasIndex(x => x.Name).IsUnique();
        var c = builder.Entity<ExpenseCategory>(); c.Property(x => x.Name).UseCollation("NOCASE"); c.HasIndex(x => x.Name).IsUnique();
        c.HasOne<Account>().WithMany().HasForeignKey(x => x.AccountId).OnDelete(DeleteBehavior.Restrict);
        var b = builder.Entity<ExpenseBill>(); b.Ignore(x => x.Total); b.Ignore(x => x.Paid); b.Ignore(x => x.Balance); b.Ignore(x => x.Status);
        JsonList(b.Property(x => x.Settlements));
        b.Property(x => x.Reference).UseCollation("NOCASE");
        b.HasIndex(x => new { x.PayeeId, x.Reference }).IsUnique().HasFilter("CancelledDate IS NULL AND Reference <> ''");
        b.HasOne<Payee>().WithMany().HasForeignKey(x => x.PayeeId).OnDelete(DeleteBehavior.Restrict);
        b.HasMany(x => x.Lines).WithOne().HasForeignKey(x => x.ExpenseBillId).OnDelete(DeleteBehavior.Restrict);
        b.HasMany(x => x.Payments).WithOne().HasForeignKey(x => x.ExpenseBillId).OnDelete(DeleteBehavior.Restrict);
        b.HasMany(x => x.Attachments).WithOne().HasForeignKey(x => x.ExpenseBillId).OnDelete(DeleteBehavior.Restrict);
        var l = builder.Entity<ExpenseLine>();
        l.HasOne<ExpenseCategory>().WithMany().HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
        l.HasOne<Account>().WithMany().HasForeignKey(x => x.AccountId).OnDelete(DeleteBehavior.Restrict);
        l.Property(x => x.Amount).HasConversion(v => (long)(v * 100m), v => v / 100m);
        var payment = builder.Entity<ExpensePayment>();
        payment.Property(x => x.Amount).HasConversion(v => (long)(v * 100m), v => v / 100m);
        payment.HasOne<Account>().WithMany().HasForeignKey(x => x.CashAccountId).OnDelete(DeleteBehavior.Restrict);
    }
}
public sealed partial class HouseholdRepository : IExpenseRepository
{
    private static void EnsureExpenseSchema(HouseholdDbContext db)
    {
        using var tx = db.Database.CurrentTransaction is null ? db.Database.BeginTransaction() : null;
        EnsureColumn(db, "AccountingSetup", "PayableAccountId", "INTEGER NOT NULL DEFAULT 0");
        db.Database.ExecuteSqlRaw("CREATE TABLE IF NOT EXISTS Payees (Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, Name TEXT NOT NULL COLLATE NOCASE, Kind TEXT NOT NULL, Contact TEXT NOT NULL, IsActive INTEGER NOT NULL)");
        db.Database.ExecuteSqlRaw("CREATE UNIQUE INDEX IF NOT EXISTS IX_Payees_Name ON Payees(Name)");
        db.Database.ExecuteSqlRaw("CREATE TABLE IF NOT EXISTS ExpenseCategories (Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, Name TEXT NOT NULL COLLATE NOCASE, AccountId INTEGER NOT NULL REFERENCES Accounts(Id) ON DELETE RESTRICT, IsActive INTEGER NOT NULL)");
        db.Database.ExecuteSqlRaw("CREATE UNIQUE INDEX IF NOT EXISTS IX_ExpenseCategories_Name ON ExpenseCategories(Name)");
        db.Database.ExecuteSqlRaw("CREATE TABLE IF NOT EXISTS ExpenseBills (Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, Version INTEGER NOT NULL, PayeeId INTEGER NOT NULL REFERENCES Payees(Id) ON DELETE RESTRICT, PayeeName TEXT NOT NULL, Kind TEXT NOT NULL, Description TEXT NOT NULL, Reference TEXT NOT NULL COLLATE NOCASE, DocumentDate TEXT NOT NULL, AccountingDate TEXT NOT NULL, DueDate TEXT NULL, IsPosted INTEGER NOT NULL, CancelledDate TEXT NULL, CancellationReason TEXT NOT NULL)");
        db.Database.ExecuteSqlRaw("CREATE UNIQUE INDEX IF NOT EXISTS IX_ExpenseBills_PayeeId_Reference ON ExpenseBills(PayeeId, Reference) WHERE CancelledDate IS NULL AND Reference <> ''");
        db.Database.ExecuteSqlRaw("CREATE TABLE IF NOT EXISTS ExpenseLines (Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, ExpenseBillId INTEGER NOT NULL REFERENCES ExpenseBills(Id) ON DELETE RESTRICT, CategoryId INTEGER NOT NULL REFERENCES ExpenseCategories(Id) ON DELETE RESTRICT, AccountId INTEGER NOT NULL REFERENCES Accounts(Id) ON DELETE RESTRICT, Description TEXT NOT NULL, Amount INTEGER NOT NULL)");
        db.Database.ExecuteSqlRaw("CREATE TABLE IF NOT EXISTS ExpensePayments (Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, ExpenseBillId INTEGER NOT NULL REFERENCES ExpenseBills(Id) ON DELETE RESTRICT, CashAccountId INTEGER NOT NULL REFERENCES Accounts(Id) ON DELETE RESTRICT, Amount INTEGER NOT NULL, Date TEXT NOT NULL, Reference TEXT NOT NULL, ReversedDate TEXT NULL, ReversalReason TEXT NOT NULL)");
        db.Database.ExecuteSqlRaw("CREATE TABLE IF NOT EXISTS ExpenseAttachments (Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, ExpenseBillId INTEGER NOT NULL REFERENCES ExpenseBills(Id) ON DELETE RESTRICT, FileName TEXT NOT NULL, Content BLOB NOT NULL)");
        EnsureColumn(db, "ExpenseBills", "IsOpeningBalance", "INTEGER NOT NULL DEFAULT 0");
        EnsureColumn(db, "ExpenseBills", "Settlements", "TEXT NOT NULL DEFAULT '[]'");
        var setup = db.AccountingSetup.Single();
        if (setup.PayableAccountId == 0)
        {
            var code = "2100";
            while (db.Accounts.Any(a => a.Code == code)) code += "P";
            var payable = new Account { Code = code, Name = "Accounts payable", Type = "Liability" };
            db.Accounts.Add(payable); db.SaveChanges(); setup.PayableAccountId = payable.Id; db.SaveChanges();
        }
        if (!db.ExpenseCategories.Any())
        {
            foreach (var a in db.Accounts.Where(a => a.Type == "Expense" && a.IsActive).ToList())
                db.ExpenseCategories.Add(new ExpenseCategory { Name = a.Name, AccountId = a.Id });
            db.SaveChanges();
        }
        tx?.Commit();
    }
    public List<Payee> GetPayees() { using var db = new HouseholdDbContext(databasePath); return db.Payees.AsNoTracking().OrderBy(p => p.Name).ToList(); }
    public List<ExpenseCategory> GetExpenseCategories() { using var db = new HouseholdDbContext(databasePath); return db.ExpenseCategories.AsNoTracking().OrderBy(p => p.Name).ToList(); }
    public void SavePayee(Payee payee)
    {
        using var db = new HouseholdDbContext(databasePath); using var tx = db.Database.BeginTransaction();
        if (db.Payees.Any(p => p.Id != payee.Id && p.Name == payee.Name)) throw new ArgumentException("That payee name already exists.");
        if (payee.Id == 0) db.Payees.Add(payee);
        else
        {
            var old = db.Payees.SingleOrDefault(p => p.Id == payee.Id) ?? throw new ArgumentException("Payee no longer exists.");
            if (old.Kind != payee.Kind && db.ExpenseBills.Any(b => b.PayeeId == old.Id)) throw new ArgumentException("The type of a payee already used on expenses cannot change.");
            old.Name = payee.Name; old.Kind = payee.Kind; old.Contact = payee.Contact; old.IsActive = payee.IsActive;
        }
        db.SaveChanges(); tx.Commit();
    }
    public void SaveExpenseCategory(ExpenseCategory category)
    {
        using var db = new HouseholdDbContext(databasePath); using var tx = db.Database.BeginTransaction();
        RequireAccount(db, category.AccountId, "Expense");
        if (db.ExpenseCategories.Any(c => c.Id != category.Id && c.Name == category.Name)) throw new ArgumentException("That expense category name already exists.");
        if (category.Id == 0) db.ExpenseCategories.Add(category);
        else
        {
            var old = db.ExpenseCategories.SingleOrDefault(c => c.Id == category.Id) ?? throw new ArgumentException("Category no longer exists.");
            old.Name = category.Name; old.AccountId = category.AccountId; old.IsActive = category.IsActive;
        }
        db.SaveChanges(); tx.Commit();
    }
    private static IQueryable<ExpenseBill> BillQuery(HouseholdDbContext db) => db.ExpenseBills.Include(b => b.Lines).Include(b => b.Payments).AsSplitQuery();
    private static ExpenseBill FindBill(HouseholdDbContext db, int id) => BillQuery(db).SingleOrDefault(b => b.Id == id) ?? throw new ArgumentException("Select an existing expense.");
    public List<ExpenseBill> GetExpenses() { using var db = new HouseholdDbContext(databasePath); return BillQuery(db).AsNoTracking().OrderByDescending(b => b.DocumentDate).ThenByDescending(b => b.Id).ToList(); }
    public ExpenseBill GetExpense(int id)
    {
        using var db = new HouseholdDbContext(databasePath);
        return BillQuery(db).Include(b => b.Attachments).AsNoTracking().SingleOrDefault(b => b.Id == id) ?? throw new ArgumentException("Expense no longer exists.");
    }
    private static void ResolveBillDetails(HouseholdDbContext db, ExpenseBill bill)
    {
        var payee = db.Payees.SingleOrDefault(p => p.Id == bill.PayeeId && p.IsActive) ?? throw new ArgumentException("Select an active supplier or claimant.");
        if ((bill.Kind == "Reimbursement claim" && payee.Kind != "Claimant") || (bill.Kind == "Supplier bill" && payee.Kind != "Supplier")) throw new ArgumentException("Select a claimant for a reimbursement, or a supplier for a supplier bill.");
        bill.PayeeName = payee.Name;
        foreach (var line in bill.Lines)
        {
            var category = db.ExpenseCategories.SingleOrDefault(c => c.Id == line.CategoryId && c.IsActive) ?? throw new ArgumentException("Select an active category on every expense line.");
            RequireAccount(db, category.AccountId, "Expense"); line.AccountId = category.AccountId;
        }
        if (bill.Reference.Length > 0 && db.ExpenseBills.Any(b => b.Id != bill.Id && b.PayeeId == bill.PayeeId && b.Reference == bill.Reference && b.CancelledDate == null)) throw new ArgumentException("That invoice/receipt reference already exists for this payee. Check for a duplicate bill.");
    }
    public int SaveExpense(ExpenseBill draft, bool payImmediately, int? cashAccountId, DateTime paymentDate, string paymentReference)
    {
        using var db = new HouseholdDbContext(databasePath); using var tx = db.Database.BeginTransaction();
        ExpenseBill bill;
        if (draft.Id == 0) { bill = new ExpenseBill(); db.ExpenseBills.Add(bill); }
        else
        {
            bill = FindBill(db, draft.Id); db.Entry(bill).Collection(b => b.Attachments).Load();
            if (bill.IsPosted || bill.CancelledDate.HasValue || bill.Version != draft.Version) throw new ArgumentException("This draft has changed or been posted. Close and reopen it before editing.");
            db.ExpenseLines.RemoveRange(bill.Lines); db.ExpenseAttachments.RemoveRange(bill.Attachments);
            bill.Lines.Clear(); bill.Attachments.Clear();
        }
        bill.Version++; bill.PayeeId = draft.PayeeId; bill.Kind = draft.Kind; bill.Description = draft.Description; bill.Reference = draft.Reference;
        bill.DocumentDate = draft.DocumentDate.Date; bill.AccountingDate = draft.AccountingDate.Date; bill.DueDate = draft.DueDate?.Date;
        bill.Lines = draft.Lines.Select(l => new ExpenseLine { CategoryId = l.CategoryId, Description = l.Description, Amount = l.Amount }).ToList();
        bill.Attachments = draft.Attachments.Select(a => new ExpenseAttachment { FileName = a.FileName, Content = a.Content.ToArray() }).ToList();
        ResolveBillDetails(db, bill); db.SaveChanges();
        if (payImmediately) { PostBill(db, bill); AddExpensePayment(db, bill, bill.Total, paymentDate, cashAccountId, paymentReference); }
        db.SaveChanges(); tx.Commit(); return bill.Id;
    }
    private static void RequireStarted(HouseholdDbContext db, DateTime date)
    {
        var s = db.AccountingSetup.Single();
        if (!s.StartedOn.HasValue) throw new ArgumentException("Start accounting in Settings before posting expenses or payments. Drafts can be saved first.");
        CheckPostingDate(s, date);
        if (date.Date > DateTime.Today) throw new ArgumentException("Accounting dates cannot be in the future.");
    }
    private static void PostBill(HouseholdDbContext db, ExpenseBill bill)
    {
        if (bill.IsPosted || bill.CancelledDate.HasValue) throw new ArgumentException("Only a draft can be posted.");
        RequireStarted(db, bill.AccountingDate); ResolveBillDetails(db, bill);
        var payable = db.AccountingSetup.Single().PayableAccountId; RequireAccount(db, payable, "Liability");
        var lines = bill.Lines.GroupBy(l => l.AccountId).Select(g => new JournalLine { AccountId = g.Key, Debit = g.Sum(l => l.Amount) }).ToList();
        lines.Add(new JournalLine { AccountId = payable, Credit = bill.Total });
        AddJournal(db, bill.AccountingDate, "ExpenseBill", bill.Id, $"{bill.Kind}: {bill.PayeeName}. {bill.Reference}. {bill.Description}", lines.ToArray());
        bill.IsPosted = true; bill.Version++;
        db.SaveChanges();
    }
    public void PostExpense(int id)
    {
        using var db = new HouseholdDbContext(databasePath); using var tx = db.Database.BeginTransaction();
        PostBill(db, FindBill(db, id)); db.SaveChanges(); tx.Commit();
    }
    public void DeleteExpenseDraft(int id)
    {
        using var db = new HouseholdDbContext(databasePath); using var tx = db.Database.BeginTransaction();
        var bill = FindBill(db, id); db.Entry(bill).Collection(b => b.Attachments).Load();
        if (bill.IsPosted || bill.CancelledDate.HasValue) throw new ArgumentException("Only an unposted draft can be deleted.");
        db.ExpenseLines.RemoveRange(bill.Lines); db.ExpenseAttachments.RemoveRange(bill.Attachments); db.ExpenseBills.Remove(bill);
        db.SaveChanges(); tx.Commit();
    }
    private static void AddExpensePayment(HouseholdDbContext db, ExpenseBill bill, decimal amount, DateTime date, int? cashAccountId, string reference)
    {
        RequireStarted(db, date);
        if (!bill.IsPosted || bill.CancelledDate.HasValue || amount <= 0 || amount > bill.Balance) throw new ArgumentException("Payment must be positive and no more than the balance of a posted, uncancelled expense.");
        if (date.Date < bill.AccountingDate.Date || bill.Payments.Any(p => p.ReversedDate.HasValue && p.ReversedDate.Value.Date > date.Date) || bill.Settlements.Any(p => p.ReversedDate > date.Date)) throw new ArgumentException("Payment date must be on or after the expense accounting date and previous payment reversals.");
        var cash = CashAccount(db, cashAccountId, false);
        var payable = OriginalControlAccount(db, "ExpenseBill", "OpeningExpense", bill.Id, false);
        var payment = new ExpensePayment { ExpenseBillId = bill.Id, Amount = amount, Date = date.Date, CashAccountId = cash, Reference = reference };
        db.ExpensePayments.Add(payment); db.SaveChanges();
        AddJournal(db, date, "ExpensePayment", payment.Id, $"Payment to {bill.PayeeName}: {bill.Reference}. {reference}", new() { AccountId = payable, Debit = amount }, new() { AccountId = cash, Credit = amount });
        bill.Version++;
    }
    public void PayExpense(int id, decimal amount, DateTime date, int? cashAccountId, string reference)
    {
        using var db = new HouseholdDbContext(databasePath); using var tx = db.Database.BeginTransaction();
        AddExpensePayment(db, FindBill(db, id), amount, date, cashAccountId, reference); db.SaveChanges(); tx.Commit();
    }
    private static void ReverseJournal(HouseholdDbContext db, string source, int sourceId, string reversalSource, DateTime date, string reason)
    {
        var original = db.JournalEntries.Include(e => e.Lines).Single(e => e.Source == source && e.SourceId == sourceId);
        CheckReconciledEntry(db, original);
        AddJournal(db, date, reversalSource, sourceId, $"Reversal of entry {original.Id}: {reason}", original.Lines.Select(l => new JournalLine { AccountId = l.AccountId, Debit = l.Credit, Credit = l.Debit }).ToArray());
    }
    public void ReverseExpensePayment(int paymentId, DateTime date, string reason)
    {
        using var db = new HouseholdDbContext(databasePath); using var tx = db.Database.BeginTransaction();
        RequireStarted(db, date);
        var payment = db.ExpensePayments.SingleOrDefault(p => p.Id == paymentId) ?? throw new ArgumentException("Select a payment to reverse.");
        var bill = FindBill(db, payment.ExpenseBillId);
        if (payment.ReversedDate.HasValue || bill.CancelledDate.HasValue || date.Date < payment.Date.Date) throw new ArgumentException("Reverse an unreversed payment once, on or after its payment date.");
        ReverseJournal(db, "ExpensePayment", paymentId, "ExpensePaymentReversal", date, reason);
        payment.ReversedDate = date.Date; payment.ReversalReason = reason; bill.Version++;
        db.SaveChanges(); tx.Commit();
    }
    public void CancelExpense(int id, DateTime date, string reason)
    {
        using var db = new HouseholdDbContext(databasePath); using var tx = db.Database.BeginTransaction();
        RequireStarted(db, date); var bill = FindBill(db, id);
        if (!bill.IsPosted || bill.CancelledDate.HasValue || bill.Paid != 0) throw new ArgumentException("Cancel a posted expense once, after reversing all its payments. A genuine payment must be refunded before its reversal is recorded.");
        if (date.Date < bill.AccountingDate.Date || bill.Payments.Any(p => p.ReversedDate > date) || bill.Settlements.Any(p => p.ReversedDate > date)) throw new ArgumentException("Cancellation date must be on or after posting and all payment reversals.");
        ReverseJournal(db, bill.IsOpeningBalance ? "OpeningExpense" : "ExpenseBill", id, "ExpenseBillCancellation", date, reason);
        bill.CancelledDate = date.Date; bill.CancellationReason = reason; bill.Version++;
        db.SaveChanges(); tx.Commit();
    }
}
