using HousingCommunityManagement.Core;
namespace HousingCommunityManagement;
public partial class ExpenseActionForm : Form
{
    private readonly ExpenseService? expenses;
    private readonly ExpenseBill? bill;
    private readonly ExpensePayment? payment;
    private readonly string action = "Pay";
    public ExpenseActionForm() { InitializeComponent(); }
    public ExpenseActionForm(ExpenseService expenses, ExpenseBill bill, string action = "Pay", ExpensePayment? payment = null) : this()
    {
        this.expenses = expenses; this.bill = bill; this.action = action; this.payment = payment;
        Text = action == "Pay" ? "Record expense payment" : action == "Reverse" ? "Reverse expense payment" : "Cancel posted expense";
        heading.Text = $"{bill.PayeeName} / {bill.Reference}\nOutstanding: RM {bill.Balance:N2}";
        date.MaxDate = DateTime.Today;
        if (expenses.Accounting is { } accounting)
        {
            account.DisplayMember = nameof(Account.DisplayName); account.ValueMember = nameof(Account.Id);
            account.DataSource = accounting.GetAccounts().Where(a => a.IsCashBank && a.IsActive).ToList(); account.SelectedValue = accounting.GetSettings().PaymentAccountId;
        }
        var earliest = action == "Reverse" && payment is not null ? payment.Date.Date : bill.AccountingDate.Date;
        if (action != "Reverse") earliest = bill.Payments.Where(p => p.ReversedDate.HasValue).Select(p => p.ReversedDate!.Value.Date).Concat(bill.Settlements.Where(s => s.ReversedDate.HasValue).Select(s => s.ReversedDate!.Value.Date)).Append(earliest).Max();
        var started = expenses.Accounting?.GetSettings().StartedOn;
        if (started.HasValue && started.Value.Date > earliest) earliest = started.Value.Date;
        date.MinDate = earliest;
        amount.Maximum = action == "Pay" ? bill.Balance : 1000000000;
        amount.Value = action == "Pay" ? bill.Balance : payment?.Amount ?? bill.Total;
        amount.Enabled = account.Enabled = action == "Pay";
        if (action != "Pay") account.Visible = accountLabel.Visible = false;
        if (action != "Pay") { referenceLabel.Text = "Reason (required)"; save.Text = action == "Reverse" ? "Reverse payment" : "Cancel expense"; }
        else reference.MaxLength = 150;
    }
    private void SaveClicked(object? sender, EventArgs e)
    {
        if (expenses is null || bill is null) return;
        try
        {
            if (action == "Pay")
            {
                if (account.SelectedItem is not Account a) throw new ArgumentException("Select a cash or bank account.");
                expenses.Pay(bill.Id, amount.Value, date.Value, a.Id, reference.Text);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(reference.Text)) throw new ArgumentException("Enter the reason for this correction.");
                var warning = action == "Reverse" ? "Reverse this payment in the records? Only do this for an incorrect payment entry or after the money has actually been returned. This does not recover money from the payee." : "Cancel this expense and reverse its posted accounting entry? The original and cancellation history will remain.";
                if (MessageBox.Show(this, warning, Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK) return;
                if (action == "Reverse" && payment is not null) expenses.ReversePayment(payment.Id, date.Value, reference.Text);
                else expenses.Cancel(bill.Id, date.Value, reference.Text);
            }
            DialogResult = DialogResult.OK;
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "Could not record expense action"); }
    }
}
