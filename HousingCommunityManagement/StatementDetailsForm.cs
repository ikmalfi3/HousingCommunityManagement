namespace HousingCommunityManagement;
public partial class StatementDetailsForm : Form
{
    private Action? submit;
    public StatementDetailsForm()
    {
        InitializeComponent();
        fromInput.MaxDate = DateTime.Today; fromInput.Value = DateTime.Today;
        toInput.MaxDate = DateTime.Today; toInput.Value = DateTime.Today;
    }
    public ComboBox AccountInput => accountInput;
    public DateTimePicker FromInput => fromInput;
    public DateTimePicker ToInput => toInput;
    public NumericUpDown OpeningInput => openingInput;
    public NumericUpDown ClosingInput => closingInput;
    public bool Run(IWin32Window owner, Action action) { submit = action; return ShowDialog(owner) == DialogResult.OK; }
    private void SaveClicked(object? sender, EventArgs e)
    {
        try { submit?.Invoke(); DialogResult = DialogResult.OK; }
        catch (Exception ex) { AccountingUi.Error(this, ex); }
    }
}
