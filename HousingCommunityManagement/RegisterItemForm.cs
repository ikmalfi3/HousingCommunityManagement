namespace HousingCommunityManagement;
public partial class RegisterItemForm : Form
{
    private Action? submit;
    public RegisterItemForm()
    {
        InitializeComponent();
        acquiredInput.MaxDate = DateTime.Today; acquiredInput.Value = DateTime.Today;
    }
    public ComboBox KindInput => kindInput;
    public TextBox NameInput => nameInput;
    public TextBox PartyInput => partyInput;
    public TextBox LocationInput => locationInput;
    public DateTimePicker AcquiredInput => acquiredInput;
    public ComboBox AccountInput => accountInput;
    public ComboBox DepreciationInput => depreciationInput;
    public ComboBox SourceInput => sourceInput;
    public NumericUpDown AmountInput => amountInput;
    public NumericUpDown InitialDepInput => initialDepInput;
    public TextBox NotesInput => notesInput;
    public bool Run(IWin32Window owner, Action action) { submit = action; return ShowDialog(owner) == DialogResult.OK; }
    private void SaveClicked(object? sender, EventArgs e)
    {
        try { submit?.Invoke(); DialogResult = DialogResult.OK; }
        catch (Exception ex) { AccountingUi.Error(this, ex); }
    }
}
