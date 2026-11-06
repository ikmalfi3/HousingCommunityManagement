using HousingCommunityManagement.Core;
namespace HousingCommunityManagement;
public partial class DeleteDataConfirmForm : Form
{
    private readonly DataResetService? reset;
    private readonly DataResetConfirmation? confirmation;
    public DeleteDataConfirmForm() { InitializeComponent(); }
    public DeleteDataConfirmForm(DataResetService reset, DataResetConfirmation confirmation) : this()
    {
        this.reset = reset; this.confirmation = confirmation;
        summary.Text = confirmation.Summary; codeLabel.Text = confirmation.Code;
        Shown += FocusAnswer;
    }
    private void FocusAnswer(object? sender, EventArgs e) => answer.Focus();
    private void AnswerChanged(object? sender, EventArgs e)
    {
        var matches = confirmation is not null && string.Equals(answer.Text, confirmation.Code, StringComparison.Ordinal);
        confirm.Enabled = matches;
        feedback.Text = matches ? "Code matches. Clicking Delete permanently will apply the changes above." : "Complete the code exactly as shown to enable deletion.";
    }
    private void ConfirmClicked(object? sender, EventArgs e)
    {
        if (reset is null || confirmation is null) return;
        confirm.Enabled = false;
        try { reset.Delete(confirmation, answer.Text); DialogResult = DialogResult.OK; }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message + "\n\nClose this confirmation and review the categories before trying again.", "Could not delete data");
        }
    }
}
