using HousingCommunityManagement.Core;
namespace HousingCommunityManagement;
public partial class DeleteDataForm : Form
{
    private readonly DataResetService? reset;
    private bool updating;
    public DeleteDataForm() { InitializeComponent(); }
    public DeleteDataForm(DataResetService reset) : this()
    {
        this.reset = reset;
        var counts = reset.GetCounts();
        foreach (var item in DataResetService.Categories) choices.Items.Add(new Option(item.Category, $"{item.Label} — {counts[item.Category]}"));
        UpdateSelection(DataResetCategory.None);
    }
    private DataResetCategory Selected => choices.CheckedItems.Cast<Option>().Aggregate(DataResetCategory.None, (value, item) => value | item.Category);
    private void ChoiceChanged(object? sender, ItemCheckEventArgs e)
    {
        if (updating) return;
        var selected = Selected; var category = ((Option)choices.Items[e.Index]).Category;
        selected = e.NewValue == CheckState.Checked ? selected | category : selected & ~category;
        // ItemCheck occurs before the new checked state is applied.
        BeginInvoke(new Action(() => UpdateSelection(DataResetService.IncludeDependencies(selected))));
    }
    private void UpdateSelection(DataResetCategory selected)
    {
        if (IsDisposed) return;
        updating = true;
        try
        {
            for (var i = 0; i < choices.Items.Count; i++) choices.SetItemChecked(i, selected.HasFlag(((Option)choices.Items[i]).Category));
        }
        finally { updating = false; }
        delete.Enabled = selected != DataResetCategory.None;
        effects.Text = selected == DataResetCategory.None ? "No categories selected. Use Select all data for a clean start." : DescribeEffects(selected);
    }
    private static string DescribeEffects(DataResetCategory selected)
    {
        var lines = new List<string> { "Selected categories will be deleted/reset, including their associated journal entries." };
        if (selected.HasFlag(DataResetCategory.Households)) lines.Add("Households require maintenance charges/payments and renovations to be cleared too.");
        if (selected.HasFlag(DataResetCategory.Payees)) lines.Add("Suppliers/claimants require all expense records, payments and receipts to be cleared too.");
        if (selected.HasFlag(DataResetCategory.ExpenseCategories)) lines.Add("Expense categories require expenses to be cleared. Default categories will be restored.");
        if (selected.HasFlag(DataResetCategory.Accounting)) lines.Add("Resetting accounting clears all financial transactions and opening balances. Default accounts return; accounting must be started again.");
        else if ((selected & (DataResetCategory.Maintenance | DataResetCategory.Renovations | DataResetCategory.Expenses)) != 0) lines.Add("Opening cash/bank balances and other unselected transactions remain.");
        if (selected.HasFlag(DataResetCategory.CommunitySettings)) lines.Add("Saved flag definitions and assignments are removed. Automatic flags still reflect any remaining billing/renovation data.");
        lines.Add("Uncheck parent categories first if you want to keep their required linked data.");
        return string.Join(Environment.NewLine, lines);
    }
    private void SelectAllClicked(object? sender, EventArgs e) => UpdateSelection(DataResetCategory.All);
    private void ClearClicked(object? sender, EventArgs e) => UpdateSelection(DataResetCategory.None);
    private void DeleteClicked(object? sender, EventArgs e)
    {
        if (reset is null) return;
        try
        {
            using var confirmation = new DeleteDataConfirmForm(reset, reset.Prepare(Selected));
            if (confirmation.ShowDialog(this) == DialogResult.OK) DialogResult = DialogResult.OK;
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "Could not prepare deletion"); }
    }
    private sealed record Option(DataResetCategory Category, string Label) { public override string ToString() => Label; }
}
