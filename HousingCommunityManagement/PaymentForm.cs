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
    }
    private void SaveClicked(object? sender, EventArgs e)
    {
        if (billing is null) return;
        try { billing.RecordPayment(chargeId, amount.Value, date.Value, reference.Text); DialogResult = DialogResult.OK; }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "Could not record payment", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
    }
}
