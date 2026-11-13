using HousingCommunityManagement.Core;

namespace HousingCommunityManagement;
public partial class ExpenseCategoryEditForm : Form
{
    private ExpenseService expenses = null!;
    private readonly int recordId;
    public int SavedId { get; private set; }

    public ExpenseCategoryEditForm(ExpenseService expenses, ExpenseCategory? record = null) : this()
    {
        this.expenses = expenses;
        recordId = record?.Id ?? 0;
        Text = record is null ? "New category" : "Edit category";
        recordName.Text = record?.Name ?? "";
        active.Checked = record?.IsActive ?? true;
        var accounts = expenses.Accounting?.GetAccounts() ?? [];
        choice.DisplayMember = nameof(Account.DisplayName);
        choice.ValueMember = nameof(Account.Id);
        choice.DataSource = accounts.Where(a => a.Type == "Expense" && (a.IsActive || a.Id == record?.AccountId)).ToList();
        if (record is not null)
            choice.SelectedValue = record.AccountId;
    }

    private void SaveClicked(object? sender, EventArgs e)
    {
        try
        {
            if (choice.SelectedItem is not Account account || !account.IsActive)
                throw new ArgumentException("Select an active expense account.");
            var record = new ExpenseCategory
            {
                Id = recordId,
                Name = recordName.Text,
                IsActive = active.Checked,
                AccountId = account.Id
            };
            expenses.SaveCategory(record);
            SavedId = record.Id;
            DialogResult = DialogResult.OK;
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Could not save category", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    public ExpenseCategoryEditForm()
    {
        InitializeComponent();
    }
}
