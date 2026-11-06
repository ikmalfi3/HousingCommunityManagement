namespace HousingCommunityManagement.Core;

public sealed class Account
{
    public int Id { get; set; }
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public string Type { get; set; } = "Asset";
    public bool IsCashBank { get; set; }
    public bool IsActive { get; set; } = true;
    public string DisplayName => $"{Code} - {Name}";
}
public sealed class AccountingSettings
{
    public int Id { get; set; } = 1;
    public int ReceivableAccountId { get; set; }
    public int IncomeAccountId { get; set; }
    public int DepositAccountId { get; set; }
    public int ReceiptAccountId { get; set; }
    public int PaymentAccountId { get; set; }
    public int PayableAccountId { get; set; }
    public int OpeningEquityAccountId { get; set; }
    public DateTime? StartedOn { get; set; }
}
public sealed class JournalEntry
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; } = "";
    public string Source { get; set; } = "";
    public int SourceId { get; set; }
    public List<JournalLine> Lines { get; set; } = [];
}
public sealed class JournalLine
{
    public int Id { get; set; }
    public int JournalEntryId { get; set; }
    public int AccountId { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
}
public interface IAccountingRepository
{
    List<Account> GetAccounts();
    AccountingSettings GetAccountingSettings();
    void SaveAccount(Account account);
    void SaveAccountingSettings(AccountingSettings settings);
    List<JournalEntry> GetJournal();
    void StartAccounting(Dictionary<int, decimal> openingCashBalances);
}
public sealed class AccountingService(IAccountingRepository repository)
{
    public ExpenseService? Expenses => repository is IExpenseRepository expenses ? new(expenses) : null;
    public List<Account> GetAccounts() => repository.GetAccounts();
    public AccountingSettings GetSettings() => repository.GetAccountingSettings();
    public List<JournalEntry> GetJournal() => repository.GetJournal();
    public void SaveAccount(Account account)
    {
        account.Code = account.Code.Trim(); account.Name = account.Name.Trim();
        if (account.Code.Length is 0 or > 20 || account.Name.Length is 0 or > 100) throw new ArgumentException("Enter an account code (up to 20 characters) and name (up to 100 characters).");
        if (account.Type is not ("Asset" or "Liability" or "Equity" or "Income" or "Expense")) throw new ArgumentException("Choose a valid account type.");
        if (account.IsCashBank && account.Type != "Asset") throw new ArgumentException("Cash and bank accounts must be assets.");
        repository.SaveAccount(account);
    }
    public void SaveSettings(AccountingSettings settings) => repository.SaveAccountingSettings(settings);
    public void Start(Dictionary<int, decimal> openingCashBalances)
    {
        if (openingCashBalances.Values.Any(v => v < 0 || v > 1000000000 || decimal.Round(v, 2) != v)) throw new ArgumentException("Opening cash/bank balances must be from RM 0 to RM 1 billion with at most two decimals.");
        repository.StartAccounting(openingCashBalances);
    }
}

