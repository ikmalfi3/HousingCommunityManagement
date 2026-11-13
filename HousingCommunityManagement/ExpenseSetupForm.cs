using HousingCommunityManagement.Core;

namespace HousingCommunityManagement;
public partial class ExpenseSetupForm : Form
{
    private readonly ExpenseService? expenses;
    private List<Payee> payees = [];
    private List<ExpenseCategory> categories = [];
    public ExpenseSetupForm()
    {
        InitializeComponent();
    }

    public ExpenseSetupForm(ExpenseService expenses, bool showCategories = false) : this()
    {
        this.expenses = expenses;
        Reload();
        if (showCategories)
            tabs.SelectedTab = categoriesTab;
    }

    private void Reload()
    {
        if (expenses is null)
            return;
        payees = expenses.GetPayees();
        categories = expenses.GetCategories();
        var accounts = expenses.Accounting?.GetAccounts() ?? [];
        payeeGrid.DataSource = payees.Select(p => new PayeeRow(p.Id, p.Name, p.Kind, p.Contact, p.IsActive)).ToList();
        categoryGrid.DataSource = categories.Select(c => new CategoryRow(c.Id, c.Name, accounts.Single(a => a.Id == c.AccountId).DisplayName, c.IsActive)).ToList();
    }

    private void PayeeSelected(object? sender, EventArgs e) => editPayee.Enabled = payeeGrid.SelectedRows.Count == 1;
    private void CategorySelected(object? sender, EventArgs e) => editCategory.Enabled = categoryGrid.SelectedRows.Count == 1;
    private void NewPayeeClicked(object? sender, EventArgs e) => OpenPayee(null);
    private void EditPayeeClicked(object? sender, EventArgs e)
    {
        if (payeeGrid.SelectedRows.Count == 1 && payeeGrid.SelectedRows[0].DataBoundItem is PayeeRow row)
            OpenPayee(payees.Single(p => p.Id == row.Id));
    }

    private void OpenPayee(Payee? payee)
    {
        if (expenses is null)
            return;
        using var dialog = new PayeeEditForm(expenses, payee);
        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            Reload();
            SelectSaved(payeeGrid, dialog.SavedId);
        }
    }

    private void NewCategoryClicked(object? sender, EventArgs e) => OpenCategory(null);
    private void EditCategoryClicked(object? sender, EventArgs e)
    {
        if (categoryGrid.SelectedRows.Count == 1 && categoryGrid.SelectedRows[0].DataBoundItem is CategoryRow row)
            OpenCategory(categories.Single(c => c.Id == row.Id));
    }

    private void OpenCategory(ExpenseCategory? category)
    {
        if (expenses is null)
            return;
        using var dialog = new ExpenseCategoryEditForm(expenses, category);
        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            Reload();
            SelectSaved(categoryGrid, dialog.SavedId);
        }
    }

    private static void SelectSaved(DataGridView grid, int id)
    {
        foreach (DataGridViewRow row in grid.Rows)
            if ((row.DataBoundItem is PayeeRow p && p.Id == id) || (row.DataBoundItem is CategoryRow c && c.Id == id))
            {
                grid.ClearSelection();
                grid.CurrentCell = row.Cells["Name"];
                row.Selected = true;
                break;
            }
    }

    private sealed record PayeeRow(int Id, string Name, string Type, string Contact, bool Active);
    private sealed record CategoryRow(int Id, string Name, string Account, bool Active);
}
