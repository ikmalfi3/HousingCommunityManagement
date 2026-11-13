using HousingCommunityManagement.Core;

namespace HousingCommunityManagement;
public partial class CashReversalForm : Form
{
    private CashTransaction original = null!;
    private CashTransactionService transactions = null!;
    public CashReversalForm(CashTransaction original, CashTransactionService transactions) : this()
    {
        this.original = original;
        this.transactions = transactions;
        date.MinDate = original.Date;
        date.MaxDate = DateTime.Today;
    }

    private void SaveClicked(object? sender, EventArgs e)
    {
        try
        {
            if (MessageBox.Show(this, $"Reverse transaction #{original.Id} (RM {original.Total:N2})?\n\nUse this to correct an incorrect entry. An actual refund is a new Money in/out transaction linked to the original. This does not move money.", "Reverse transaction", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK)
                return;
            transactions.Reverse(original.Id, date.Value, reason.Text);
            DialogResult = DialogResult.OK;
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Could not reverse transaction");
        }
    }

    public CashReversalForm()
    {
        InitializeComponent();
    }
}
