using HousingCommunityManagement.Core;
namespace HousingCommunityManagement;
public partial class ExpenseSetupForm : Form
{
    private readonly ExpenseService? expenses;
    private int payeeId, categoryId;
    private List<Payee> payees = [];
    private List<ExpenseCategory> categories = [];
    public ExpenseSetupForm() { InitializeComponent(); }
    public ExpenseSetupForm(ExpenseService expenses, bool showCategories = false) : this()
    {
        this.expenses = expenses; Reload(); if (showCategories) tabs.SelectedTab = categoriesTab;
    }
    private void Reload()
    {
        if (expenses is null) return;
        payees = expenses.GetPayees(); categories = expenses.GetCategories();
        var accounts = expenses.Accounting?.GetAccounts() ?? [];
        expenseAccount.DisplayMember = nameof(Account.DisplayName); expenseAccount.ValueMember = nameof(Account.Id);
        expenseAccount.DataSource = accounts.Where(a => a.IsActive && a.Type == "Expense").ToList();
        payeeGrid.DataSource = payees.Select(p => new PayeeRow(p.Id, p.Name, p.Kind, p.Contact, p.IsActive)).ToList();
        categoryGrid.DataSource = categories.Select(c => new CategoryRow(c.Id, c.Name, accounts.Single(a => a.Id == c.AccountId).DisplayName, c.IsActive)).ToList();
        if (payeeGrid.Columns["Name"] is { } p) { p.MinimumWidth = 230; p.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; }
        if (categoryGrid.Columns["Name"] is { } c) c.MinimumWidth = 240;
        if (categoryGrid.Columns["Account"] is { } a) { a.MinimumWidth = 300; a.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; }
        if (payees.Count == 0) NewPayeeClicked(null, EventArgs.Empty);
        if (categories.Count == 0) NewCategoryClicked(null, EventArgs.Empty);
    }
    private void PayeeSelected(object? sender, EventArgs e)
    {
        if (payeeGrid.CurrentRow?.DataBoundItem is not PayeeRow row) return;
        var p = payees.Single(p => p.Id == row.Id); payeeId = p.Id; payeeName.Text = p.Name; payeeKind.SelectedItem = p.Kind; contact.Text = p.Contact; payeeActive.Checked = p.IsActive;
    }
    private void CategorySelected(object? sender, EventArgs e)
    {
        if (categoryGrid.CurrentRow?.DataBoundItem is not CategoryRow row) return;
        var c = categories.Single(c => c.Id == row.Id); categoryId = c.Id; categoryName.Text = c.Name; expenseAccount.SelectedValue = c.AccountId; categoryActive.Checked = c.IsActive;
    }
    private void NewPayeeClicked(object? sender, EventArgs e) { payeeId = 0; payeeName.Clear(); contact.Clear(); payeeKind.SelectedItem = "Supplier"; payeeActive.Checked = true; payeeName.Focus(); }
    private void NewCategoryClicked(object? sender, EventArgs e) { categoryId = 0; categoryName.Clear(); categoryActive.Checked = true; categoryName.Focus(); }
    private void SavePayeeClicked(object? sender, EventArgs e)
    {
        try { expenses?.SavePayee(new Payee { Id = payeeId, Name = payeeName.Text, Kind = payeeKind.Text, Contact = contact.Text, IsActive = payeeActive.Checked }); Reload(); }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "Could not save payee"); }
    }
    private void SaveCategoryClicked(object? sender, EventArgs e)
    {
        try
        {
            if (expenseAccount.SelectedItem is not Account a) throw new ArgumentException("Select an active expense account.");
            expenses?.SaveCategory(new ExpenseCategory { Id = categoryId, Name = categoryName.Text, AccountId = a.Id, IsActive = categoryActive.Checked }); Reload();
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "Could not save category"); }
    }
    private sealed record PayeeRow(int Id, string Name, string Type, string Contact, bool Active);
    private sealed record CategoryRow(int Id, string Name, string Account, bool Active);
}
