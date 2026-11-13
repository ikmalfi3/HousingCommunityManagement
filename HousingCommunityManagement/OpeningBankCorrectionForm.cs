namespace HousingCommunityManagement;
public partial class OpeningBankCorrectionForm : Form
{
    private Action? submit;
    public OpeningBankCorrectionForm()
    {
        InitializeComponent();

    }
    public ComboBox AccountInput => accountInput;
    public NumericUpDown AmountInput => amountInput;
    public TextBox ReasonInput => reasonInput;
    public bool Run(IWin32Window owner, Action action) { submit = action; return ShowDialog(owner) == DialogResult.OK; }
    private void SaveClicked(object? sender, EventArgs e)
    {
        try { submit?.Invoke(); DialogResult = DialogResult.OK; }
        catch (Exception ex) { AccountingUi.Error(this, ex); }
    }
}
