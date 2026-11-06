using HousingCommunityManagement.Core;
namespace HousingCommunityManagement;
public partial class PaymentForm : Form
{
    private readonly MonthlyBillingService? billing;
    private readonly int chargeId;
    public PaymentForm() { InitializeComponent(); }
    public PaymentForm(MonthlyBillingService billing, MonthlyCharge charge, string house) : this()
    {
        this.billing = billing; chargeId = charge.Id;
        heading.Text = $"House {house} / {new DateTime(charge.Year, charge.Month, 1):MMM yyyy}\nOutstanding: RM {charge.Balance:N2}";
        amount.Maximum = charge.Balance; amount.Value = charge.Balance;
        date.MaxDate = DateTime.Today;
        if (billing.Accounting is { } accounting)
        {
            cashAccount.DisplayMember = nameof(Account.DisplayName);
            cashAccount.ValueMember = nameof(Account.Id);
            cashAccount.DataSource = accounting.GetAccounts().Where(a => a.IsActive && a.IsCashBank).ToList();
            cashAccount.SelectedValue = accounting.GetSettings().ReceiptAccountId;
        }
    }
    private void SaveClicked(object? sender, EventArgs e)
    {
        if (billing is null) return;
        try
        {
            if (cashAccount.SelectedItem is not Account account) throw new ArgumentException("Select the cash or bank account that received the payment.");
            billing.RecordPayment(chargeId, amount.Value, date.Value, reference.Text, account.Id); DialogResult = DialogResult.OK;
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "Could not record payment", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
    }
}
