using HousingCommunityManagement.Core;

namespace HousingCommunityManagement;
public partial class AccountingSettingsForm : Form
{
    private readonly AccountingService? accounting;
    private List<Account> accounts = [];
    public AccountingSettingsForm()
    {
        InitializeComponent();
    }

    public AccountingSettingsForm(AccountingService accounting) : this()
    {
        this.accounting = accounting;
        Reload();
    }

    private void Reload()
    {
        if (accounting is null)
            return;
        accounts = accounting.GetAccounts();
        var settings = accounting.GetSettings();
        accountGrid.DataSource = accounts.Select(a => new AccountRow(a.Id, a.Code, a.Name, a.Type, a.IsCashBank, a.IsActive)).ToList();
        Bind(receivable, accounts.Where(a => a.IsActive && a.Type == "Asset" && !a.IsCashBank), settings.ReceivableAccountId);
        Bind(income, accounts.Where(a => a.IsActive && a.Type == "Income"), settings.IncomeAccountId);
        Bind(deposit, accounts.Where(a => a.IsActive && a.Type == "Liability"), settings.DepositAccountId);
        Bind(equity, accounts.Where(a => a.IsActive && a.Type == "Equity"), settings.OpeningEquityAccountId);
        Bind(receipt, accounts.Where(a => a.IsActive && a.IsCashBank), settings.ReceiptAccountId);
        Bind(payment, accounts.Where(a => a.IsActive && a.IsCashBank), settings.PaymentAccountId);
        Bind(payable, accounts.Where(a => a.IsActive && a.Type == "Liability"), settings.PayableAccountId);
        var blocked = CashTransactionService.LinkedControlAccounts(settings, accounting.GetJournal());
        var registerAccounts = accounts.Where(a => a.IsActive && a.Type == "Asset" && !a.IsCashBank && !blocked.Contains(a.Id)).ToList();
        Bind(assetPurchase, registerAccounts, settings.AssetPurchaseAccountId);
        Bind(supplierDeposit, registerAccounts, settings.SupplierDepositAccountId);
        Bind(prepayment, registerAccounts, settings.PrepaymentAccountId);
        Bind(accumulatedDepreciation, registerAccounts, settings.AccumulatedDepreciationAccountId);
        Bind(depreciationExpense, accounts.Where(a => a.IsActive && a.Type == "Expense"), settings.DepreciationExpenseAccountId);
        Bind(writeOffExpense, accounts.Where(a => a.IsActive && a.Type == "Expense"), settings.WriteOffExpenseAccountId);
        startButton.Visible = !settings.StartedOn.HasValue;
        startStatus.Text = settings.StartedOn.HasValue ? $"Accounting started on {settings.StartedOn:dd MMM yyyy}. Opening balances are locked." : "After saving your initial accounts, start accounting to set the opening date and cash/bank balances before recording transactions.";
        if (settings.StartedOn.HasValue)
            setupNote.Text = $"Accounting active since {settings.StartedOn:dd MMM yyyy}. Defaults apply to future entries. Opening balances are locked.";
    }

    private static void Bind(ComboBox control, IEnumerable<Account> values, int selected)
    {
        control.DisplayMember = nameof(Account.DisplayName);
        control.ValueMember = nameof(Account.Id);
        control.DataSource = values.ToList();
        control.SelectedValue = selected;
    }

    private void AccountSelected(object? sender, EventArgs e) => editButton.Enabled = accountGrid.SelectedRows.Count == 1;
    private void NewAccountClicked(object? sender, EventArgs e) => OpenAccountEditor(null);
    private void EditAccountClicked(object? sender, EventArgs e)
    {
        if (accountGrid.SelectedRows.Count != 1 || accountGrid.SelectedRows[0].DataBoundItem is not AccountRow row)
            return;
        OpenAccountEditor(accounts.Single(a => a.Id == row.Id));
    }

    private void OpenAccountEditor(Account? account)
    {
        if (accounting is null)
            return;
        using var dialog = new AccountEditForm(accounting, account);
        if (dialog.ShowDialog(this) != DialogResult.OK)
            return;
        Reload();
        foreach (DataGridViewRow row in accountGrid.Rows)
            if (row.DataBoundItem is AccountRow value && value.Id == dialog.SavedAccountId)
            {
                accountGrid.ClearSelection();
                accountGrid.CurrentCell = row.Cells[nameof(AccountRow.Code)];
                row.Selected = true;
                break;
            }
    }

    private static int AccountId(ComboBox control) => (control.SelectedItem as Account)?.Id ?? throw new ArgumentException("Select an account for every default.");
    private void SaveDefaults()
    {
        accounting?.SaveSettings(new AccountingSettings { ReceivableAccountId = AccountId(receivable), IncomeAccountId = AccountId(income), DepositAccountId = AccountId(deposit), ReceiptAccountId = AccountId(receipt), PaymentAccountId = AccountId(payment), OpeningEquityAccountId = AccountId(equity), PayableAccountId = AccountId(payable), AssetPurchaseAccountId = AccountId(assetPurchase), SupplierDepositAccountId = AccountId(supplierDeposit), PrepaymentAccountId = AccountId(prepayment), AccumulatedDepreciationAccountId = AccountId(accumulatedDepreciation), DepreciationExpenseAccountId = AccountId(depreciationExpense), WriteOffExpenseAccountId = AccountId(writeOffExpense) });
    }

    private void SaveDefaultsClicked(object? sender, EventArgs e)
    {
        try
        {
            SaveDefaults();
            MessageBox.Show(this, "Accounting defaults saved.", "Accounting");
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Could not save defaults");
        }
    }

    private void StartClicked(object? sender, EventArgs e)
    {
        if (accounting is null)
            return;
        try
        {
            using var dialog = new AccountingOpeningForm(accounting, SaveDefaults);
            if (dialog.ShowDialog(this) == DialogResult.OK)
                Reload();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Could not start accounting");
        }
    }

    private void ExpenseSetupClicked(object? sender, EventArgs e)
    {
        if (accounting?.Expenses is not { } expenses)
            return;
        using var dialog = new ExpenseSetupForm(expenses, showCategories: true);
        dialog.ShowDialog(this);
    }

    private sealed record AccountRow(int Id, string Code, string Name, string Type, bool CashBank, bool Active);
}
