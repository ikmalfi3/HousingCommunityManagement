namespace HousingCommunityManagement;
public partial class StatementBalancesForm : Form
{
    private Action? submit;
    public StatementBalancesForm()
    {
        InitializeComponent();

    }
    public NumericUpDown OpeningInput => openingInput;
    public NumericUpDown ClosingInput => closingInput;
    public bool Run(IWin32Window owner, Action action) { submit = action; return ShowDialog(owner) == DialogResult.OK; }
    private void SaveClicked(object? sender, EventArgs e)
    {
        try { submit?.Invoke(); DialogResult = DialogResult.OK; }
        catch (Exception ex) { AccountingUi.Error(this, ex); }
    }
}
