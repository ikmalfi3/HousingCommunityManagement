using HousingCommunityManagement.Core;
namespace HousingCommunityManagement;

public partial class AccountingSettingsForm : Form
{
    private readonly AccountingService? accounting;
    private List<Account> accounts = [];
    private int editingId;
    public AccountingSettingsForm() { InitializeComponent(); }
    public AccountingSettingsForm(AccountingService accounting) : this()
    {
        this.accounting = accounting;
        Reload();
    }
    private void Reload()
    {
        if (accounting is null) return;
        accounts = accounting.GetAccounts();
        var settings = accounting.GetSettings();
        accountGrid.DataSource = accounts.Select(a => new AccountRow(a.Id, a.Code, a.Name, a.Type, a.IsCashBank, a.IsActive)).ToList();
        accountGrid.Columns[nameof(AccountRow.Id)]!.Visible = false;
        accountGrid.Columns[nameof(AccountRow.Name)]!.MinimumWidth = 260;
        accountGrid.Columns[nameof(AccountRow.Name)]!.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        accountGrid.Columns[nameof(AccountRow.Code)]!.MinimumWidth = 85;
        accountGrid.Columns[nameof(AccountRow.Type)]!.MinimumWidth = 100;
        accountGrid.Columns[nameof(AccountRow.CashBank)]!.HeaderText = "Cash / bank";
        Bind(receivable, accounts.Where(a => a.IsActive && a.Type == "Asset" && !a.IsCashBank), settings.ReceivableAccountId);
        Bind(income, accounts.Where(a => a.IsActive && a.Type == "Income"), settings.IncomeAccountId);
        Bind(deposit, accounts.Where(a => a.IsActive && a.Type == "Liability"), settings.DepositAccountId);
        Bind(equity, accounts.Where(a => a.IsActive && a.Type == "Equity"), settings.OpeningEquityAccountId);
        Bind(receipt, accounts.Where(a => a.IsActive && a.IsCashBank), settings.ReceiptAccountId);
        Bind(payment, accounts.Where(a => a.IsActive && a.IsCashBank), settings.PaymentAccountId);
        Bind(payable, accounts.Where(a => a.IsActive && a.Type == "Liability"), settings.PayableAccountId);
        openingGrid.Rows.Clear();
        var opening = accounting.GetJournal().Where(e => e.Source == "OpeningCash").ToDictionary(e => e.SourceId, e => e.Lines.Sum(l => l.Debit));
        foreach (var a in accounts.Where(a => a.IsActive && a.IsCashBank))
        {
            var index = openingGrid.Rows.Add(a.DisplayName, opening.GetValueOrDefault(a.Id).ToString("N2")); openingGrid.Rows[index].Tag = a.Id;
        }
        openingGrid.ReadOnly = settings.StartedOn.HasValue;
        startButton.Enabled = !settings.StartedOn.HasValue;
        if (settings.StartedOn.HasValue) setupNote.Text = $"Accounting active since {settings.StartedOn:dd MMM yyyy}. Defaults apply to future entries. Receipts and refunds keep the original receivable/deposit account. Opening balances are locked. View the journal and balances from Accounting in the sidebar.";
    }
    private static void Bind(ComboBox control, IEnumerable<Account> values, int selected)
    {
        control.DisplayMember = nameof(Account.DisplayName); control.ValueMember = nameof(Account.Id);
        control.DataSource = values.ToList(); control.SelectedValue = selected;
    }
    private void AccountSelected(object? sender, EventArgs e)
    {
        if (accountGrid.CurrentRow?.DataBoundItem is not AccountRow row) return;
        var a = accounts.Single(x => x.Id == row.Id); editingId = a.Id;
        code.Text = a.Code; accountName.Text = a.Name; type.SelectedItem = a.Type; cashBank.Checked = a.IsCashBank; active.Checked = a.IsActive;
    }
    private void NewAccountClicked(object? sender, EventArgs e)
    {
        editingId = 0; code.Clear(); accountName.Clear(); type.SelectedItem = "Asset"; cashBank.Checked = false; active.Checked = true; code.Focus();
    }
    private void SaveAccountClicked(object? sender, EventArgs e)
    {
        if (accounting is null) return;
        try
        {
            accounting.SaveAccount(new Account { Id = editingId, Code = code.Text, Name = accountName.Text, Type = type.SelectedItem as string ?? "", IsCashBank = cashBank.Checked, IsActive = active.Checked });
            Reload();
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "Could not save account"); }
    }
    private static int AccountId(ComboBox control) => (control.SelectedItem as Account)?.Id ?? throw new ArgumentException("Select an account for every default.");
    private void SaveDefaults()
    {
        accounting?.SaveSettings(new AccountingSettings { ReceivableAccountId = AccountId(receivable), IncomeAccountId = AccountId(income), DepositAccountId = AccountId(deposit), ReceiptAccountId = AccountId(receipt), PaymentAccountId = AccountId(payment), OpeningEquityAccountId = AccountId(equity), PayableAccountId = AccountId(payable) });
    }
    private void SaveDefaultsClicked(object? sender, EventArgs e)
    {
        try { SaveDefaults(); MessageBox.Show(this, "Accounting defaults saved.", "Accounting"); }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "Could not save defaults"); }
    }
    private void StartClicked(object? sender, EventArgs e)
    {
        if (accounting is null) return;
        try
        {
            openingGrid.EndEdit();
            var balances = new Dictionary<int, decimal>();
            foreach (DataGridViewRow row in openingGrid.Rows)
            {
                if (!decimal.TryParse(row.Cells[1].Value?.ToString(), out var amount)) throw new ArgumentException("Enter a valid opening balance for each cash/bank account.");
                balances.Add((int)row.Tag!, amount);
            }
            if (MessageBox.Show(this, $"Start accounting today ({DateTime.Today:dd MMM yyyy})?\n\nCash/bank opening total: RM {balances.Values.Sum():N2}\nExisting outstanding maintenance and held deposits will be included once, against opening community funds.\n\nConfirm the entered balances include held deposit money. Opening balances cannot be re-entered after starting.", "Start accounting", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK) return;
            SaveDefaults(); accounting.Start(balances); Reload();
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "Could not start accounting"); }
    }
    private void ExpenseSetupClicked(object? sender, EventArgs e)
    {
        if (accounting?.Expenses is not { } expenses) return;
        using var dialog = new ExpenseSetupForm(expenses, showCategories: true); dialog.ShowDialog(this);
    }
    private sealed record AccountRow(int Id, string Code, string Name, string Type, bool CashBank, bool Active);
}
