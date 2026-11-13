namespace HousingCommunityManagement;
public partial class RegisterActionForm : Form
{
    private Action? submit;
    public RegisterActionForm()
    {
        InitializeComponent();
        dateInput.MaxDate = DateTime.Today; dateInput.Value = DateTime.Today;
    }
    public ComboBox ActionInput => actionInput;
    public DateTimePicker DateInput => dateInput;
    public NumericUpDown AmountInput => amountInput;
    public ComboBox ExpenseInput => expenseInput;
    public ComboBox BillInput => billInput;
    public TextBox ReasonInput => reasonInput;
    public bool Run(IWin32Window owner, Action action) { submit = action; return ShowDialog(owner) == DialogResult.OK; }
    private void SaveClicked(object? sender, EventArgs e)
    {
        try { submit?.Invoke(); DialogResult = DialogResult.OK; }
        catch (Exception ex) { AccountingUi.Error(this, ex); }
    }
}
