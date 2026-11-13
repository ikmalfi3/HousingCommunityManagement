namespace HousingCommunityManagement;
public partial class RegisterDetailsForm : Form
{
    private Action? submit;
    public RegisterDetailsForm()
    {
        InitializeComponent();

    }
    public TextBox NameInput => nameInput;
    public TextBox PartyInput => partyInput;
    public TextBox LocationInput => locationInput;
    public TextBox NotesInput => notesInput;
    public ComboBox DepreciationInput => depreciationInput;
    public bool Run(IWin32Window owner, Action action) { submit = action; return ShowDialog(owner) == DialogResult.OK; }
    private void SaveClicked(object? sender, EventArgs e)
    {
        try { submit?.Invoke(); DialogResult = DialogResult.OK; }
        catch (Exception ex) { AccountingUi.Error(this, ex); }
    }
}
