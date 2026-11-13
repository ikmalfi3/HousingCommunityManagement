using HousingCommunityManagement.Core;

namespace HousingCommunityManagement;
public partial class AccountingOpeningForm : Form
{
    private AccountingService accounting = null!;
    private OpeningDetails details = new();
    private readonly Action saveDefaults = () => { };
    public AccountingOpeningForm(AccountingService accounting, Action saveDefaults) : this()
    {
        this.accounting = accounting;
        this.saveDefaults = saveDefaults;
        openingDate.MaxDate = DateTime.Today;
        openingDate.Value = DateTime.Today;
        foreach (var account in accounting.GetAccounts().Where(a => a.IsActive && a.IsCashBank))
        {
            var index = balances.Rows.Add(account.DisplayName, "0.00");
            balances.Rows[index].Tag = account.Id;
        }
    }

    private void OtherBalancesClicked(object? sender, EventArgs e)
    {
        using var dialog = new OpeningDetailsForm(accounting, details);
        if (dialog.ShowDialog(this) == DialogResult.OK)
            details = dialog.Details;
    }

    private void PreviewClicked(object? sender, EventArgs e)
    {
        try
        {
            ShowPreview(ReadBalances());
        }
        catch (Exception ex)
        {
            AccountingUi.Error(this, ex);
        }
    }

    private Dictionary<int, decimal> ReadBalances()
    {
        balances.EndEdit();
        var values = new Dictionary<int, decimal>();
        foreach (DataGridViewRow row in balances.Rows)
            values.Add((int)row.Tag!, AccountingUi.Amount(row.Cells[1].Value, "cash/bank balance"));
        return values;
    }

    private void ShowPreview(Dictionary<int, decimal> values)
    {
        saveDefaults();
        var lines = accounting.Operations!.PreviewOpening(values, openingDate.Value, details);
        var accounts = accounting.GetAccounts().ToDictionary(a => a.Id, a => a.DisplayName);
        AccountingUi.ShowRows(this, $"Opening review / Debit RM {lines.Sum(l => l.Debit):N2} / Credit RM {lines.Sum(l => l.Credit):N2} / Net opening community funds automatically balances", lines.Select(l => new { Account = accounts[l.AccountId], l.Debit, l.Credit, NetDebit = l.Debit - l.Credit }).ToList());
    }

    private void StartClicked(object? sender, EventArgs e)
    {
        try
        {
            balances.EndEdit();
            var values = new Dictionary<int, decimal>();
            foreach (DataGridViewRow row in balances.Rows)
            {
                if (!decimal.TryParse(row.Cells[1].Value?.ToString(), out var amount) || Math.Abs(amount) > 1000000000m || decimal.Round(amount, 2) != amount)
                    throw new ArgumentException("Enter an opening balance between minus and plus RM 1 billion with at most two decimals for each cash/bank account.");
                values.Add((int)row.Tag!, amount);
            }

            ShowPreview(values);
            if (MessageBox.Show(this, $"Start accounting on {openingDate.Value:dd MMM yyyy}?\n\nCash/bank opening total: RM {values.Values.Sum():N2}\nMaintenance outstanding and deposits held before this date will be brought forward. Existing charges, receipts and deposit activity on or after this date will be posted at their original dates.\n\nEnter cash/bank balances before activity on the opening date, including deposit money held then. The opening date and balances are locked after starting.", "Start accounting", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                return;
            saveDefaults();
            accounting.Start(values, openingDate.Value, details);
            DialogResult = DialogResult.OK;
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Could not start accounting");
        }
    }

    public AccountingOpeningForm()
    {
        InitializeComponent();
    }
}
