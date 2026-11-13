namespace HousingCommunityManagement.Core;

public sealed class Payee
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Kind { get; set; } = "Supplier";
    public string Contact { get; set; } = "";
    public bool IsActive { get; set; } = true;
}
public sealed class ExpenseCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int AccountId { get; set; }
    public bool IsActive { get; set; } = true;
}
public sealed class ExpenseBill
{
    public int Id { get; set; }
    public int Version { get; set; }
    public int PayeeId { get; set; }
    public string PayeeName { get; set; } = "";
    public string Kind { get; set; } = "Supplier bill";
    public string Description { get; set; } = "";
    public string Reference { get; set; } = "";
    public DateTime DocumentDate { get; set; } = DateTime.Today;
    public DateTime AccountingDate { get; set; } = DateTime.Today;
    public DateTime? DueDate { get; set; }
    public bool IsPosted { get; set; }
    public bool IsOpeningBalance { get; set; }
    public List<ExpenseSettlement> Settlements { get; set; } = [];
    public DateTime? CancelledDate { get; set; }
    public string CancellationReason { get; set; } = "";
    public List<ExpenseLine> Lines { get; set; } = [];
    public List<ExpensePayment> Payments { get; set; } = [];
    public List<ExpenseAttachment> Attachments { get; set; } = [];
    public decimal Total => Lines.Sum(l => l.Amount);
    public decimal Paid => Payments.Where(p => !p.ReversedDate.HasValue).Sum(p => p.Amount) + Settlements.Where(p => !p.ReversedDate.HasValue).Sum(p => p.Amount);
    public decimal Balance => IsPosted && !CancelledDate.HasValue ? Total - Paid : 0;
    public string Status => CancelledDate.HasValue ? "Cancelled" : !IsPosted ? "Draft" : Paid == 0 ? "Unpaid" : Balance == 0 ? "Paid" : "Partially paid";
    public bool IsOverdueOn(DateTime today) => Balance > 0 && DueDate.HasValue && DueDate.Value.Date < today.Date;
}
public sealed class ExpenseSettlement
{
    public int AdjustmentId { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public DateTime? ReversedDate { get; set; }
}
public sealed class ExpenseLine
{
    public int Id { get; set; }
    public int ExpenseBillId { get; set; }
    public int CategoryId { get; set; }
    public int AccountId { get; set; }
    public string Description { get; set; } = "";
    public decimal Amount { get; set; }
}
public sealed class ExpensePayment
{
    public int Id { get; set; }
    public int ExpenseBillId { get; set; }
    public int CashAccountId { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Reference { get; set; } = "";
    public DateTime? ReversedDate { get; set; }
    public string ReversalReason { get; set; } = "";
}
public sealed class ExpenseAttachment
{
    public int Id { get; set; }
    public int ExpenseBillId { get; set; }
    public string FileName { get; set; } = "";
    public byte[] Content { get; set; } = [];
}
public interface IExpenseRepository
{
    List<Payee> GetPayees();
    void SavePayee(Payee payee);
    List<ExpenseCategory> GetExpenseCategories();
    void SaveExpenseCategory(ExpenseCategory category);
    List<ExpenseBill> GetExpenses();
    ExpenseBill GetExpense(int id);
    int SaveExpense(ExpenseBill bill, bool payImmediately, int? cashAccountId, DateTime paymentDate, string paymentReference);
    void PostExpense(int id);
    void DeleteExpenseDraft(int id);
    void PayExpense(int id, decimal amount, DateTime date, int? cashAccountId, string reference);
    void ReverseExpensePayment(int paymentId, DateTime date, string reason);
    void CancelExpense(int id, DateTime date, string reason);
}
public sealed class ExpenseService(IExpenseRepository repository)
{
    public CashTransactionService? Transactions => repository is ICashTransactionRepository cash ? new(cash) : null;
    public AccountingService? Accounting => repository is IAccountingRepository accounting ? new(accounting) : null;
    public List<Payee> GetPayees() => repository.GetPayees();
    public List<ExpenseCategory> GetCategories() => repository.GetExpenseCategories();
    public List<ExpenseBill> GetAll() => repository.GetExpenses();
    public ExpenseBill Get(int id) => repository.GetExpense(id);
    private static string Text(string text, int max, string label, bool required = false)
    {
        text = text.Trim();
        if (text.Length > max || (required && text.Length == 0)) throw new ArgumentException($"{label} must be {(required ? "between 1 and " : "at most ")}{max} characters.");
        return text;
    }
    public void SavePayee(Payee payee)
    {
        payee.Name = Text(payee.Name, 150, "Payee name", true); payee.Contact = Text(payee.Contact, 200, "Contact");
        if (payee.Kind is not ("Supplier" or "Claimant")) throw new ArgumentException("Choose Supplier or Claimant.");
        repository.SavePayee(payee);
    }
    public void SaveCategory(ExpenseCategory category)
    {
        category.Name = Text(category.Name, 100, "Category name", true); repository.SaveExpenseCategory(category);
    }
    public int Save(ExpenseBill bill, bool payImmediately = false, int? cashAccountId = null, DateTime? paymentDate = null, string paymentReference = "")
    {
        if (bill.IsPosted || bill.CancelledDate.HasValue) throw new ArgumentException("Posted and cancelled expenses cannot be edited. Use a reversal or cancellation.");
        if (bill.Kind is not ("Supplier bill" or "Reimbursement claim" or "Direct expense")) throw new ArgumentException("Choose a valid expense type.");
        bill.Description = Text(bill.Description, 500, "Description", true); bill.Reference = Text(bill.Reference, 150, "Invoice/receipt reference");
        if (bill.DocumentDate.Year < 1900 || bill.AccountingDate.Year < 1900 || bill.DocumentDate.Date > DateTime.Today || bill.AccountingDate.Date > DateTime.Today) throw new ArgumentException("Document and accounting dates must be from 1900 through today.");
        if (bill.DueDate?.Date < bill.DocumentDate.Date) throw new ArgumentException("Due date cannot precede the document date.");
        if (bill.Lines.Count is < 1 or > 100) throw new ArgumentException("Add between 1 and 100 expense lines.");
        foreach (var line in bill.Lines)
        {
            line.Description = Text(line.Description, 250, "Line description"); ValidateMoney(line.Amount);
        }
        if (bill.Total > 1000000000m) throw new ArgumentException("Total expense cannot exceed RM 1 billion.");
        if (bill.Attachments.Count > 10 || bill.Attachments.Sum(a => (long)a.Content.Length) > 30 * 1024 * 1024) throw new ArgumentException("Attach up to 10 files, no more than 30 MB in total.");
        foreach (var attachment in bill.Attachments)
        {
            attachment.FileName = Path.GetFileName(attachment.FileName);
            if (attachment.FileName.Length is 0 or > 200 || attachment.Content.Length is 0 or > 10485760) throw new ArgumentException("Each attachment needs a filename and must contain 1 byte to 10 MB.");
            if (!new[] { ".pdf", ".png", ".jpg", ".jpeg", ".txt" }.Contains(Path.GetExtension(attachment.FileName).ToLowerInvariant())) throw new ArgumentException("Attach PDF, PNG, JPEG or text receipts.");
        }
        var date = (paymentDate ?? DateTime.Today).Date;
        if (payImmediately) ValidateDate(date);
        return repository.SaveExpense(bill, payImmediately, cashAccountId, date, Text(paymentReference, 150, "Payment reference"));
    }
    public void Post(int id) => repository.PostExpense(id);
    public void DeleteDraft(int id) => repository.DeleteExpenseDraft(id);
    private static void ValidateMoney(decimal amount)
    {
        if (amount <= 0 || amount > 1000000000 || decimal.Round(amount, 2) != amount) throw new ArgumentException("Enter a positive amount up to RM 1 billion with at most two decimals.");
    }
    private static void ValidateDate(DateTime date)
    {
        if (date.Year < 1900 || date.Date > DateTime.Today) throw new ArgumentException("Choose a recorded date from 1900 through today.");
    }
    public void Pay(int id, decimal amount, DateTime date, int? cashAccountId, string reference)
    {
        ValidateMoney(amount); ValidateDate(date);
        repository.PayExpense(id, amount, date.Date, cashAccountId, Text(reference, 150, "Payment reference"));
    }
    public void ReversePayment(int paymentId, DateTime date, string reason)
    {
        ValidateDate(date); repository.ReverseExpensePayment(paymentId, date.Date, Text(reason, 500, "Reversal reason", true));
    }
    public void Cancel(int id, DateTime date, string reason)
    {
        ValidateDate(date); repository.CancelExpense(id, date.Date, Text(reason, 500, "Cancellation reason", true));
    }
}
