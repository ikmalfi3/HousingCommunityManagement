namespace HousingCommunityManagement;
public partial class ReasonEntryForm : Form
{
    private Action? submit;
    public ReasonEntryForm()
    {
        InitializeComponent();

    }
    public ReasonEntryForm(string title) : this() { Text = title; }
    public TextBox ReasonInput => reasonInput;
    public bool Run(IWin32Window owner, Action action) { submit = action; return ShowDialog(owner) == DialogResult.OK; }
    private void SaveClicked(object? sender, EventArgs e)
    {
        try { submit?.Invoke(); DialogResult = DialogResult.OK; }
        catch (Exception ex) { AccountingUi.Error(this, ex); }
    }
}
