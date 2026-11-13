namespace HousingCommunityManagement;
public partial class AdjustmentReversalForm : Form
{
    private Action? submit;
    public AdjustmentReversalForm()
    {
        InitializeComponent();
        dateInput.MaxDate = DateTime.Today; dateInput.Value = DateTime.Today;
    }
    public DateTimePicker DateInput => dateInput;
    public TextBox ReasonInput => reasonInput;
    public bool Run(IWin32Window owner, Action action) { submit = action; return ShowDialog(owner) == DialogResult.OK; }
    private void SaveClicked(object? sender, EventArgs e)
    {
        try { submit?.Invoke(); DialogResult = DialogResult.OK; }
        catch (Exception ex) { AccountingUi.Error(this, ex); }
    }
}
