namespace HousingCommunityManagement;
public partial class StatementLineForm : Form
{
    private Action? submit;
    public StatementLineForm()
    {
        InitializeComponent();
        dateInput.MaxDate = DateTime.Today; dateInput.Value = DateTime.Today;
    }
    public DateTimePicker DateInput => dateInput;
    public TextBox DescriptionInput => descriptionInput;
    public TextBox ReferenceInput => referenceInput;
    public NumericUpDown AmountInput => amountInput;
    public bool Run(IWin32Window owner, Action action) { submit = action; return ShowDialog(owner) == DialogResult.OK; }
    private void SaveClicked(object? sender, EventArgs e)
    {
        try { submit?.Invoke(); DialogResult = DialogResult.OK; }
        catch (Exception ex) { AccountingUi.Error(this, ex); }
    }
}
