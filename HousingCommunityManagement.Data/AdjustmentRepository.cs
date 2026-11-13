using HousingCommunityManagement.Core;
using Microsoft.EntityFrameworkCore;
namespace HousingCommunityManagement.Data;
public sealed partial class HouseholdRepository
{
    public void CorrectOpeningBank(int accountId, decimal change, string reason)
    {
        Money(change, true);
        if (string.IsNullOrWhiteSpace(reason) || reason.Length > 500) throw new ArgumentException("Enter an opening correction reason up to 500 characters.");
        using var db = new HouseholdDbContext(databasePath); using var tx = db.Database.BeginTransaction(); var setup = db.AccountingSetup.Single();
        if (!setup.StartedOn.HasValue) throw new ArgumentException("Use Start accounting for initial opening balances.");
        RequireStarted(db, setup.StartedOn.Value); RequireAccount(db, accountId, "Asset", true);
        var a = new AccountingAdjustment { Date = setup.StartedOn.Value, Action = "Opening bank correction", Description = reason.Trim(), IsPosted = true, Version = 1, Lines = [new() { AccountId = accountId, Debit = Math.Max(change, 0), Credit = Math.Max(-change, 0) }, new() { AccountId = setup.OpeningEquityAccountId, Debit = Math.Max(-change, 0), Credit = Math.Max(change, 0) }] };
        db.AccountingAdjustments.Add(a); db.SaveChanges(); AddJournal(db, a.Date, "Adjustment", a.Id, $"Opening cash/bank correction: {a.Description}", a.Lines.ToArray()); Audit(db, $"Opening bank account #{accountId} corrected by {change:N2}, adjustment #{a.Id}: {reason.Trim()}"); db.SaveChanges(); tx.Commit();
    }
    public List<AccountingAdjustment> GetAdjustments() { using var db = new HouseholdDbContext(databasePath); return db.AccountingAdjustments.AsNoTracking().OrderByDescending(a => a.Date).ThenByDescending(a => a.Id).ToList(); }
    private static void ValidateAdjustment(HouseholdDbContext db, AccountingAdjustment a, bool posting)
    {
        CashTransactionService.Validate(new CashTransaction { Date = a.Date, Direction = "Money out", Description = a.Description, Reference = a.Reference, Allocations = [new() { Amount = 1 }], Attachments = a.Attachments });
        if (a.Lines.Count is 0 or > 100) throw new ArgumentException("Enter 1-100 journal lines.");
        var controls = CashTransactionService.LinkedControlAccounts(db.AccountingSetup.Single(), db.JournalEntries.Include(e => e.Lines).ToList());
        var registered = db.FinanceRegisters.ToList().SelectMany(r => new[] { (int?)r.AccountId, r.DepreciationAccountId }).Where(i => i.HasValue).Select(i => i!.Value).ToHashSet();
        foreach (var line in a.Lines)
        {
            Money(line.Debit, zero: true); Money(line.Credit, zero: true);
            var account = db.Accounts.SingleOrDefault(x => x.Id == line.AccountId && x.IsActive) ?? throw new ArgumentException("Select active accounts.");
            if (account.IsCashBank || controls.Contains(account.Id) || registered.Contains(account.Id)) throw new ArgumentException("Use Money in/out for cash and guided actions for linked control/register accounts.");
            if (line.Debit > 0 && line.Credit > 0 || line.Debit + line.Credit == 0) throw new ArgumentException("Each line must have either a debit or credit.");
        }
        if (posting && (a.Lines.Count < 2 || a.Lines.Sum(l => l.Debit) != a.Lines.Sum(l => l.Credit))) throw new ArgumentException("Debits and credits must balance before posting.");
    }
    public int SaveAdjustment(AccountingAdjustment adjustment)
    {
        using var db = new HouseholdDbContext(databasePath); using var tx = db.Database.BeginTransaction();
        if (adjustment.Action != "Manual" || adjustment.IsPosted || adjustment.ReversedDate.HasValue || adjustment.RegisterItemId.HasValue || adjustment.BillId.HasValue) throw new ArgumentException("Use the guided actions for linked records.");
        ValidateAdjustment(db, adjustment, false);
        AccountingAdjustment saved;
        if (adjustment.Id == 0) { saved = new AccountingAdjustment(); db.AccountingAdjustments.Add(saved); }
        else
        {
            saved = db.AccountingAdjustments.SingleOrDefault(a => a.Id == adjustment.Id) ?? throw new ArgumentException("The draft no longer exists.");
            if (saved.IsPosted || saved.Version != adjustment.Version || saved.Action != "Manual") throw new ArgumentException("The adjustment changed or was posted. Reopen it before editing.");
        }
        saved.Date = adjustment.Date.Date; saved.Description = adjustment.Description.Trim(); saved.Reference = adjustment.Reference.Trim(); saved.Lines = adjustment.Lines.Select(l => new JournalLine { AccountId = l.AccountId, Debit = l.Debit, Credit = l.Credit }).ToList();
        saved.Attachments = adjustment.Attachments; saved.Version++;
        db.SaveChanges(); tx.Commit(); return saved.Id;
    }
    public void PostAdjustment(int id)
    {
        using var db = new HouseholdDbContext(databasePath); using var tx = db.Database.BeginTransaction(); var a = db.AccountingAdjustments.SingleOrDefault(a => a.Id == id) ?? throw new ArgumentException("Select an adjustment.");
        if (a.IsPosted || a.Action != "Manual") throw new ArgumentException("Only an unposted manual adjustment can be posted.");
        RequireStarted(db, a.Date); ValidateAdjustment(db, a, true);
        AddJournal(db, a.Date, "Adjustment", a.Id, $"Adjustment: {a.Description}. {a.Reference}", a.Lines.Select(l => new JournalLine { AccountId = l.AccountId, Debit = l.Debit, Credit = l.Credit }).ToArray());
        a.IsPosted = true; a.Version++; Audit(db, $"Adjustment #{a.Id} posted: {a.Description}"); db.SaveChanges(); tx.Commit();
    }
    public int RecordRegisterAction(RegisterActionRequest request)
    {
        CashTransactionService.ValidateDate(request.Date); Money(request.Amount, signed: request.Action.StartsWith("Opening "), zero: request.Action == "Dispose / write off asset");
        if (string.IsNullOrWhiteSpace(request.Reason) || request.Reason.Length > 500) throw new ArgumentException("Enter a description/reason up to 500 characters.");
        using var db = new HouseholdDbContext(databasePath); using var tx = db.Database.BeginTransaction(); RequireStarted(db, request.Date);
        var item = db.FinanceRegisters.Include(r => r.Movements).SingleOrDefault(r => r.Id == request.RegisterItemId) ?? throw new ArgumentException("Select a register item.");
        if (request.Date.Date < item.AcquisitionDate.Date || !request.Action.StartsWith("Opening ") && item.Movements.Any(m => m.Date.Date > request.Date.Date)) throw new ArgumentException("Use a date on or after acquisition and the item's latest movement.");
        var a = new AccountingAdjustment { Date = request.Date.Date, Action = request.Action, Description = request.Reason.Trim(), RegisterItemId = item.Id, IsPosted = true, Version = 1 };
        var movement = new RegisterMovement { RegisterItemId = item.Id, Date = a.Date, Action = a.Action, Description = a.Description };
        switch (request.Action)
        {
            case "Opening cost correction":
            case "Opening depreciation correction":
                var setup = db.AccountingSetup.Single();
                if (request.Date.Date != setup.StartedOn!.Value.Date) throw new ArgumentException("Opening corrections must use the accounting opening date. They add an audited entry instead of changing the original.");
                if (request.Action == "Opening depreciation correction")
                {
                    if (item.Kind != "Asset" || !item.DepreciationAccountId.HasValue) throw new ArgumentException("Choose an asset with accumulated depreciation.");
                    movement.DepreciationChange = request.Amount;
                    a.Lines = [new() { AccountId = setup.OpeningEquityAccountId, Debit = Math.Max(request.Amount, 0), Credit = Math.Max(-request.Amount, 0) }, new() { AccountId = item.DepreciationAccountId.Value, Debit = Math.Max(-request.Amount, 0), Credit = Math.Max(request.Amount, 0) }];
                }
                else
                {
                    movement.CostChange = request.Amount;
                    a.Lines = [new() { AccountId = item.AccountId, Debit = Math.Max(request.Amount, 0), Credit = Math.Max(-request.Amount, 0) }, new() { AccountId = setup.OpeningEquityAccountId, Debit = Math.Max(-request.Amount, 0), Credit = Math.Max(request.Amount, 0) }];
                }
                break;
            case "Apply to bill":
                if (item.Kind == "Asset") throw new ArgumentException("Only deposits or prepayments can be applied to a bill.");
                var bill = FindBill(db, request.BillId ?? 0);
                if (!bill.IsPosted || bill.CancelledDate.HasValue || request.Amount > bill.Balance || request.Date.Date < bill.AccountingDate.Date || bill.Settlements.Any(s => s.ReversedDate > request.Date) || bill.Payments.Any(p => p.ReversedDate > request.Date)) throw new ArgumentException("Choose a posted bill with enough balance and a date on/after posting and previous reversals.");
                if (item.Party.Length > 0 && !string.Equals(item.Party.Trim(), bill.PayeeName.Trim(), StringComparison.OrdinalIgnoreCase)) throw new ArgumentException("The bill supplier must match the deposit/prepayment supplier.");
                var payable = OriginalControlAccount(db, "ExpenseBill", "OpeningExpense", bill.Id, false);
                a.Lines = [new() { AccountId = payable, Debit = request.Amount }, new() { AccountId = item.AccountId, Credit = request.Amount }]; a.BillId = bill.Id; movement.CostChange = -request.Amount;
                db.AccountingAdjustments.Add(a); db.SaveChanges();
                bill.Settlements.Add(new ExpenseSettlement { AdjustmentId = a.Id, Date = a.Date, Amount = request.Amount }); bill.Version++;
                break;
            case "Depreciate":
                if (item.Kind != "Asset" || !item.DepreciationAccountId.HasValue) throw new ArgumentException("Select an asset with a separate accumulated-depreciation account.");
                RequireAccount(db, request.CounterAccountId, "Expense"); RequireAccount(db, item.DepreciationAccountId.Value, "Asset");
                a.Lines = [new() { AccountId = request.CounterAccountId, Debit = request.Amount }, new() { AccountId = item.DepreciationAccountId.Value, Credit = request.Amount }]; movement.DepreciationChange = request.Amount;
                break;
            case "Recognize expense":
            case "Write off":
                RequireAccount(db, request.CounterAccountId, "Expense");
                if (item.Kind == "Asset") throw new ArgumentException("Use Dispose / write off asset to remove equipment and its accumulated depreciation together.");
                a.Lines = [new() { AccountId = request.CounterAccountId, Debit = request.Amount }, new() { AccountId = item.AccountId, Credit = request.Amount }]; movement.CostChange = -request.Amount;
                break;
            case "Dispose / write off asset":
                if (item.Kind != "Asset" || request.Amount != item.Balance || item.Cost == 0) throw new ArgumentException("Dispose of the full remaining asset value. Record sale proceeds separately through Money in.");
                RequireAccount(db, request.CounterAccountId, "Expense");
                a.Lines = [new() { AccountId = item.AccountId, Credit = item.Cost }];
                if (item.Balance > 0) a.Lines.Add(new() { AccountId = request.CounterAccountId, Debit = item.Balance });
                if (item.AccumulatedDepreciation > 0) a.Lines.Add(new() { AccountId = item.DepreciationAccountId!.Value, Debit = item.AccumulatedDepreciation });
                movement.CostChange = -item.Cost; movement.DepreciationChange = -item.AccumulatedDepreciation;
                break;
            default: throw new ArgumentException("Choose a supported register action.");
        }
        item.Movements.Add(movement); ValidateRegisterTimeline(item);
        if (a.Id == 0) { db.AccountingAdjustments.Add(a); db.SaveChanges(); }
        movement.AdjustmentId = a.Id;
        AddJournal(db, a.Date, "Adjustment", a.Id, $"{a.Action}: {item.Name}. {a.Description}", a.Lines.ToArray());
        Audit(db, $"Register #{item.Id}: {a.Action}, adjustment #{a.Id}, RM {request.Amount:N2}"); db.SaveChanges(); tx.Commit(); return a.Id;
    }
    public void ReverseAdjustment(int id, DateTime date, string reason)
    {
        CashTransactionService.ValidateDate(date);
        if (string.IsNullOrWhiteSpace(reason) || reason.Length > 500) throw new ArgumentException("Enter a reversal reason up to 500 characters.");
        using var db = new HouseholdDbContext(databasePath); using var tx = db.Database.BeginTransaction(); RequireStarted(db, date);
        var a = db.AccountingAdjustments.SingleOrDefault(a => a.Id == id) ?? throw new ArgumentException("Select an adjustment.");
        if (!a.IsPosted || a.ReversedDate.HasValue || date.Date < a.Date.Date) throw new ArgumentException("Reverse a posted adjustment once, on or after its date.");
        if (a.RegisterItemId.HasValue)
        {
            var item = db.FinanceRegisters.Include(r => r.Movements).Single(r => r.Id == a.RegisterItemId);
            if (item.Movements.Any(m => m.Date.Date > date.Date)) throw new ArgumentException("Use a reversal date on or after the register's latest activity.");
            var original = item.Movements.Single(m => m.AdjustmentId == id && m.Action != "Reversal");
            item.Movements.Add(new RegisterMovement { RegisterItemId = item.Id, Date = date.Date, Action = "Reversal", Description = reason.Trim(), CostChange = -original.CostChange, DepreciationChange = -original.DepreciationChange, AdjustmentId = a.Id }); ValidateRegisterTimeline(item);
        }
        if (a.BillId.HasValue)
        {
            var bill = FindBill(db, a.BillId.Value);
            if (bill.CancelledDate.HasValue) throw new ArgumentException("This bill is cancelled.");
            bill.Settlements.Single(s => s.AdjustmentId == id).ReversedDate = date.Date; bill.Version++;
        }
        ReverseJournal(db, "Adjustment", id, "AdjustmentReversal", date.Date, reason.Trim()); a.ReversedDate = date.Date; a.ReversalReason = reason.Trim(); a.Version++;
        Audit(db, $"Adjustment #{id} reversed: {reason.Trim()}"); db.SaveChanges(); tx.Commit();
    }
}
