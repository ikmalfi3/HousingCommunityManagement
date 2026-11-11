using HousingCommunityManagement.Core;
namespace HousingCommunityManagement;

public partial class AccountingOpeningForm : Form
{
    private readonly AccountingService accounting;
    private readonly Action saveDefaults;
    public AccountingOpeningForm(AccountingService accounting, Action saveDefaults)
    {
        this.accounting = accounting;
        this.saveDefaults = saveDefaults;
        InitializeComponent();
        openingDate.MaxDate = DateTime.Today;
        openingDate.Value = DateTime.Today;
        foreach (var account in accounting.GetAccounts().Where(a => a.IsActive && a.IsCashBank))
        {
            var index = balances.Rows.Add(account.DisplayName, "0.00");
            balances.Rows[index].Tag = account.Id;
        }
    }
    private void StartClicked(object? sender, EventArgs e)
    {
        try
        {
            balances.EndEdit();
            var values = new Dictionary<int, decimal>();
            foreach (DataGridViewRow row in balances.Rows)
            {
                if (!decimal.TryParse(row.Cells[1].Value?.ToString(), out var amount) || amount < 0 || amount > 1000000000m || decimal.Round(amount, 2) != amount)
                    throw new ArgumentException("Enter an opening balance from RM 0 to RM 1 billion with at most two decimals for each cash/bank account.");
                values.Add((int)row.Tag!, amount);
            }
            if (MessageBox.Show(this, $"Start accounting on {openingDate.Value:dd MMM yyyy}?\n\nCash/bank opening total: RM {values.Values.Sum():N2}\nMaintenance outstanding and deposits held before this date will be brought forward. Existing charges, receipts and deposit activity on or after this date will be posted at their original dates.\n\nEnter cash/bank balances before activity on the opening date, including deposit money held then. The opening date and balances are locked after starting.", "Start accounting", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK) return;
            saveDefaults();
            accounting.Start(values, openingDate.Value);
            DialogResult = DialogResult.OK;
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "Could not start accounting"); }
    }
}
