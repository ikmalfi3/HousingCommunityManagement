using HousingCommunityManagement.Core;

namespace HousingCommunityManagement;
public partial class PayeeEditForm : Form
{
    private ExpenseService expenses = null!;
    private readonly int recordId;
    public int SavedId { get; private set; }

    public PayeeEditForm(ExpenseService expenses, Payee? record = null) : this()
    {
        this.expenses = expenses;
        recordId = record?.Id ?? 0;
        Text = record is null ? "New payee" : "Edit payee";
        recordName.Text = record?.Name ?? "";
        active.Checked = record?.IsActive ?? true;

        choice.SelectedItem = record?.Kind ?? "Supplier";
        contact.Text = record?.Contact ?? "";
    }

    private void SaveClicked(object? sender, EventArgs e)
    {
        try
        {
            var record = new Payee
            {
                Id = recordId,
                Name = recordName.Text,
                IsActive = active.Checked,
                Kind = choice.Text,
                Contact = contact.Text
            };
            expenses.SavePayee(record);
            SavedId = record.Id;
            DialogResult = DialogResult.OK;
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Could not save payee", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    public PayeeEditForm()
    {
        InitializeComponent();
    }
}
