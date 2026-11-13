namespace HousingCommunityManagement;
public partial class RegisterCashLinkForm : Form
{
    private Action? submit;
    public RegisterCashLinkForm()
    {
        InitializeComponent();

    }
    public ComboBox SourceInput => sourceInput;
    public NumericUpDown AmountInput => amountInput;
    public void SetAvailable(bool available)
    {
        saveButton.Enabled = available;
        amountInput.Enabled = available;
        helpLabel.Text = available
            ? "Only unlinked amounts are shown. Money out adds a purchase/deposit; Money in links a refund already recorded in Income & expenditure."
            : "No unlinked transactions are available for this account. The original payment may already be linked. To refund a deposit, first record Money in against the same deposit account, then return here.";
    }
    public bool Run(IWin32Window owner, Action action) { submit = action; return ShowDialog(owner) == DialogResult.OK; }
    private void SaveClicked(object? sender, EventArgs e)
    {
        try { submit?.Invoke(); DialogResult = DialogResult.OK; }
        catch (Exception ex) { AccountingUi.Error(this, ex); }
    }
}
