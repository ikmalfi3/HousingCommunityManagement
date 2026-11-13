namespace HousingCommunityManagement.Core;

public sealed class CashTransaction
{
    public int Id { get; set; }
    public DateTime Date { get; set; } = DateTime.Today;
    public string Direction { get; set; } = "Money out";
    public int CashAccountId { get; set; }
    public string Counterparty { get; set; } = "";
    public string Description { get; set; } = "";
    public string Reference { get; set; } = "";
    public int? RelatedTransactionId { get; set; }
    public List<CashAllocation> Allocations { get; set; } = [];
    public List<ExpenseAttachment> Attachments { get; set; } = [];
    public DateTime? ReversedDate { get; set; }
    public string ReversalReason { get; set; } = "";
    public decimal Total => Allocations.Sum(l => l.Amount);
}
public sealed class CashAllocation
{
    public int AccountId { get; set; }
    public string Description { get; set; } = "";
    public decimal Amount { get; set; }
}
public interface ICashTransactionRepository
{
    List<CashTransaction> GetCashTransactions();
    int RecordCashTransaction(CashTransaction transaction);
    void ReverseCashTransaction(int id, DateTime date, string reason);
}
public sealed class CashTransactionService(ICashTransactionRepository repository)
{
    public static HashSet<int> LinkedControlAccounts(AccountingSettings settings, IEnumerable<JournalEntry> journal)
    {
        var ids = new HashSet<int> { settings.ReceivableAccountId, settings.DepositAccountId, settings.PayableAccountId };
        foreach (var entry in journal)
        {
            if (entry.Source is "MonthlyCharge" or "OpeningCharge") ids.UnionWith(entry.Lines.Where(l => l.Debit > 0).Select(l => l.AccountId));
            if (entry.Source is "ExpenseBill" or "OpeningExpense" or "RenovationCollection" or "OpeningDeposit") ids.UnionWith(entry.Lines.Where(l => l.Credit > 0).Select(l => l.AccountId));
        }
        return ids;
    }
    public List<CashTransaction> GetAll() => repository.GetCashTransactions();
    public static void Validate(CashTransaction transaction)
    {
        if (transaction.Id != 0 || transaction.ReversedDate.HasValue || transaction.ReversalReason.Length > 0) throw new ArgumentException("Create a new transaction without posted history.");
        ValidateDate(transaction.Date);
        if (transaction.Direction is not ("Money in" or "Money out")) throw new ArgumentException("Choose Money in or Money out.");
        transaction.Counterparty = transaction.Counterparty.Trim(); transaction.Description = transaction.Description.Trim(); transaction.Reference = transaction.Reference.Trim();
        if (transaction.Description.Length is 0 or > 500 || transaction.Counterparty.Length > 150 || transaction.Reference.Length > 150) throw new ArgumentException("Enter a description up to 500 characters, and payer/payee and reference up to 150 characters.");
        if (transaction.Allocations.Count is 0 or > 100 || transaction.Allocations.Any(l => l.Amount <= 0 || l.Amount > 1000000000m || decimal.Round(l.Amount, 2) != l.Amount || l.Description.Length > 250) || transaction.Total > 1000000000m) throw new ArgumentException("Enter 1-100 account lines with positive amounts, at most two decimals, and a total up to RM 1 billion.");
        if (transaction.Attachments.Count > 10 || transaction.Attachments.Sum(a => (long)a.Content.Length) > 30 * 1024 * 1024) throw new ArgumentException("Attach up to 10 files, at most 30 MB total.");
        foreach (var a in transaction.Attachments)
        {
            a.FileName = Path.GetFileName(a.FileName);
            if (a.FileName.Length is 0 or > 200 || a.Content.Length is 0 or > 10485760 || !new[] { ".pdf", ".png", ".jpg", ".jpeg", ".txt" }.Contains(Path.GetExtension(a.FileName).ToLowerInvariant())) throw new ArgumentException("Attach PDF, PNG, JPEG or text files, up to 10 MB each.");
        }
    }
    public static void ValidateDate(DateTime date)
    {
        if (date.Year < 1900 || date.Date > DateTime.Today) throw new ArgumentException("Choose a date from 1900 through today.");
    }
    public int Record(CashTransaction transaction) { Validate(transaction); return repository.RecordCashTransaction(transaction); }
    public void Reverse(int id, DateTime date, string reason)
    {
        ValidateDate(date); reason = reason.Trim();
        if (reason.Length is 0 or > 500) throw new ArgumentException("Enter a reversal reason up to 500 characters.");
        repository.ReverseCashTransaction(id, date.Date, reason);
    }
}
