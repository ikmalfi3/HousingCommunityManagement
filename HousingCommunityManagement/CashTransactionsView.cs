using HousingCommunityManagement.Core;

namespace HousingCommunityManagement;
public partial class CashTransactionsView : UserControl
{
    private CashTransactionService transactions = null !;
    private AccountingService accounting = null !;
    private List<CashTransaction> records = [];
    public CashTransactionsView(CashTransactionService transactions, AccountingService accounting) : this()
    {
        BindServices(transactions, accounting);
    }

    private void Reload()
    {
        if (accounting is null || DesignMode) return;
        try
        {
            records = transactions.GetAll();
            var choice = year.Text;
            var years = records.Select(t => t.Date.Year.ToString()).Append(DateTime.Today.Year.ToString()).Distinct().OrderDescending().ToArray();
            year.Items.Clear();
            year.Items.Add("All years");
            year.Items.AddRange(years);
            year.SelectedItem = year.Items.Contains(choice) ? choice : "All years";
            ApplyFilters();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Could not load money transactions");
        }
    }

    private void RefreshClicked(object? sender, EventArgs e) => Reload();
    private void FiltersChanged(object? sender, EventArgs e) => ApplyFilters();
    private CashTransaction? Selected => grid.CurrentRow?.DataBoundItem is TransactionRow row ? records.SingleOrDefault(t => t.Id == row.Number) : null;

    private void ApplyFilters()
    {
        if (accounting is null || DesignMode) return;
        var names = accounting.GetAccounts().ToDictionary(a => a.Id, a => a.DisplayName);
        var term = search.Text.Trim();
        var selectedId = Selected?.Id;
        var visible = records.Where(t => (year.SelectedIndex <= 0 || t.Date.Year.ToString() == year.Text) && (direction.SelectedIndex <= 0 || t.Direction == direction.Text) && (status.SelectedIndex <= 0 || (t.ReversedDate.HasValue ? "Reversed" : "Recorded") == status.Text) && (term.Length == 0 || $"{t.Id} {t.Counterparty} {t.Description} {t.Reference} {names.GetValueOrDefault(t.CashAccountId)} {string.Join(' ', t.Allocations.Select(l => names.GetValueOrDefault(l.AccountId) + " " + l.Description))}".Contains(term, StringComparison.OrdinalIgnoreCase))).ToList();
        grid.DataSource = visible.Select(t => new TransactionRow(t.Id, t.Date.ToString("dd MMM yyyy"), t.Direction, t.Counterparty, t.Description, names.GetValueOrDefault(t.CashAccountId) ?? "", t.Total, t.Reference, t.ReversedDate.HasValue ? "Reversed" : "Recorded", t.RelatedTransactionId, t.ReversedDate?.ToString("dd MMM yyyy") ?? "", t.ReversalReason)).ToList();
        foreach (DataGridViewRow row in grid.Rows)
            if ((row.DataBoundItem as TransactionRow)?.Number == selectedId)
            {
                grid.CurrentCell = row.Cells[0];
                break;
            }

        var incoming = visible.Where(t => !t.ReversedDate.HasValue && t.Direction == "Money in").Sum(t => t.Total);
        var outgoing = visible.Where(t => !t.ReversedDate.HasValue && t.Direction == "Money out").Sum(t => t.Total);
        summary.Text = $"{visible.Count} records | Unreversed displayed transactions: money in RM {incoming:N2} / money out RM {outgoing:N2} / net RM {incoming - outgoing:N2}";
        UpdateSelection();
    }

    private void SelectionChanged(object? sender, EventArgs e) => UpdateSelection();
    private void UpdateSelection()
    {
        view.Enabled = Selected is not null;
        reverse.Enabled = Selected is { ReversedDate: null };
    }

    private void MoneyInClicked(object? sender, EventArgs e) => Open("Money in");
    private void MoneyOutClicked(object? sender, EventArgs e) => Open("Money out");
    private void ViewClicked(object? sender, EventArgs e)
    {
        if (Selected is { } selected)
            Open(selected.Direction, selected);
    }

    private void Open(string direction, CashTransaction? existing = null)
    {
        try
        {
            using var dialog = new CashTransactionForm(transactions, accounting, direction, existing);
            if (dialog.ShowDialog(this) == DialogResult.OK)
                Reload();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Could not open transaction");
        }
    }

    private void ReverseClicked(object? sender, EventArgs e)
    {
        if (Selected is not { } original)
            return;
        using var dialog = new CashReversalForm(original, transactions);
        if (dialog.ShowDialog(this) == DialogResult.OK)
            Reload();
    }

    private sealed record TransactionRow(int Number, string Date, string Direction, string PayerPayee, string Description, string CashBank, decimal Amount, string Reference, string Status, int? RelatedEntry, string ReversedDate, string Reason);
    public void BindServices(CashTransactionService transactions, AccountingService accounting)
    {
        this.transactions = transactions;
        this.accounting = accounting;
        Reload();
    }

    public CashTransactionsView()
    {
        InitializeComponent();
    }
}
