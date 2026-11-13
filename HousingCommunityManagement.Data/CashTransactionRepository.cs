using HousingCommunityManagement.Core;
using Microsoft.EntityFrameworkCore;
namespace HousingCommunityManagement.Data;

public sealed partial class HouseholdDbContext
{
    public DbSet<CashTransaction> CashTransactions => Set<CashTransaction>();
    private static void ConfigureCashTransactions(ModelBuilder builder)
    {
        var t = builder.Entity<CashTransaction>();
        t.Ignore(x => x.Total);
        JsonList(t.Property(x => x.Allocations));
        JsonList(t.Property(x => x.Attachments));
        t.HasOne<Account>().WithMany().HasForeignKey(x => x.CashAccountId).OnDelete(DeleteBehavior.Restrict);
    }
}
public sealed partial class HouseholdRepository : ICashTransactionRepository
{
    private static void EnsureCashTransactionSchema(HouseholdDbContext db) => db.Database.ExecuteSqlRaw("CREATE TABLE IF NOT EXISTS CashTransactions (Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, Date TEXT NOT NULL, Direction TEXT NOT NULL, CashAccountId INTEGER NOT NULL REFERENCES Accounts(Id) ON DELETE RESTRICT, Counterparty TEXT NOT NULL, Description TEXT NOT NULL, Reference TEXT NOT NULL, RelatedTransactionId INTEGER NULL, Allocations TEXT NOT NULL, Attachments TEXT NOT NULL, ReversedDate TEXT NULL, ReversalReason TEXT NOT NULL)");
    public List<CashTransaction> GetCashTransactions()
    {
        using var db = new HouseholdDbContext(databasePath);
        return db.CashTransactions.AsNoTracking().OrderByDescending(t => t.Date).ThenByDescending(t => t.Id).ToList();
    }
    public int RecordCashTransaction(CashTransaction transaction)
    {
        CashTransactionService.Validate(transaction);
        using var db = new HouseholdDbContext(databasePath); using var tx = db.Database.BeginTransaction();
        RequireStarted(db, transaction.Date);
        RequireAccount(db, transaction.CashAccountId, "Asset", true);
        var settings = db.AccountingSetup.Single();
        var controls = CashTransactionService.LinkedControlAccounts(settings, db.JournalEntries.Include(e => e.Lines).Where(e => e.Source == "MonthlyCharge" || e.Source == "OpeningCharge" || e.Source == "ExpenseBill" || e.Source == "OpeningExpense" || e.Source == "RenovationCollection" || e.Source == "OpeningDeposit").ToList());
        foreach (var allocation in transaction.Allocations)
        {
            var account = db.Accounts.SingleOrDefault(a => a.Id == allocation.AccountId && a.IsActive) ?? throw new ArgumentException("Select an active account on every line.");
            if (account.Id == transaction.CashAccountId) throw new ArgumentException("The account on a line must differ from the selected cash/bank account.");
            if (controls.Contains(account.Id)) throw new ArgumentException("Use maintenance, renovations, or bills and payments for their linked receivable, deposit and payable accounts.");
        }
        if (transaction.Reference.Length > 0 && db.CashTransactions.Any(t => t.ReversedDate == null && t.CashAccountId == transaction.CashAccountId && t.Direction == transaction.Direction && t.Counterparty.ToUpper() == transaction.Counterparty.ToUpper() && t.Reference.ToUpper() == transaction.Reference.ToUpper()))
            throw new ArgumentException("That reference already exists for this payer/payee, direction and bank account. Check for a duplicate transaction.");
        if (transaction.RelatedTransactionId.HasValue)
        {
            var original = db.CashTransactions.SingleOrDefault(t => t.Id == transaction.RelatedTransactionId) ?? throw new ArgumentException("The related transaction does not exist.");
            if (transaction.Date.Date < original.Date.Date) throw new ArgumentException("The transaction date cannot precede the related transaction.");
        }
        // Snapshot caller data so a rolled-back save does not leave the input with a generated ID.
        var saved = new CashTransaction { Date = transaction.Date.Date, Direction = transaction.Direction, CashAccountId = transaction.CashAccountId, Counterparty = transaction.Counterparty, Description = transaction.Description, Reference = transaction.Reference, RelatedTransactionId = transaction.RelatedTransactionId, Allocations = transaction.Allocations, Attachments = transaction.Attachments };
        db.CashTransactions.Add(saved); db.SaveChanges();
        var incoming = saved.Direction == "Money in";
        var lines = saved.Allocations.Select(l => new JournalLine { AccountId = l.AccountId, Debit = incoming ? 0 : l.Amount, Credit = incoming ? l.Amount : 0 }).ToList();
        lines.Add(new JournalLine { AccountId = saved.CashAccountId, Debit = incoming ? saved.Total : 0, Credit = incoming ? 0 : saved.Total });
        AddJournal(db, saved.Date, "CashTransaction", saved.Id, $"{saved.Direction}: {saved.Counterparty}. {saved.Description}. {saved.Reference}", lines.ToArray());
        db.SaveChanges(); tx.Commit(); return saved.Id;
    }
    public void ReverseCashTransaction(int id, DateTime date, string reason)
    {
        CashTransactionService.ValidateDate(date); reason = reason.Trim();
        if (reason.Length is 0 or > 500) throw new ArgumentException("Enter a reversal reason up to 500 characters.");
        using var db = new HouseholdDbContext(databasePath); using var tx = db.Database.BeginTransaction();
        RequireStarted(db, date);
        var original = db.CashTransactions.SingleOrDefault(t => t.Id == id) ?? throw new ArgumentException("Select an existing transaction.");
        if (original.ReversedDate.HasValue || date.Date < original.Date.Date) throw new ArgumentException("Reverse a transaction once, on or after its original date.");
        foreach (var register in db.FinanceRegisters.Include(r => r.Movements).ToList().Where(r => r.Movements.Any(m => m.CashTransactionId == id)))
        {
            if (register.Movements.Any(m => m.Date.Date > date.Date)) throw new ArgumentException("Reverse on or after the register's latest activity.");
            var linked = register.Movements.Where(m => m.CashTransactionId == id).ToList();
            register.Movements.Add(new RegisterMovement { Date = date.Date, Action = "Cash reversal", Description = reason, CostChange = -linked.Sum(m => m.CostChange), DepreciationChange = -linked.Sum(m => m.DepreciationChange), CashTransactionId = id });
            ValidateRegisterTimeline(register);
        }
        ReverseJournal(db, "CashTransaction", id, "CashTransactionReversal", date.Date, reason);
        original.ReversedDate = date.Date; original.ReversalReason = reason;
        db.SaveChanges(); tx.Commit();
    }
}
