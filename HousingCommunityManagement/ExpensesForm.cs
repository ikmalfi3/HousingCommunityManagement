using HousingCommunityManagement.Core;

namespace HousingCommunityManagement;
public partial class ExpensesForm : Form
{
    private readonly ExpenseService? expenses;
    private List<ExpenseBill> records = [];
    private ExpenseBill? selected;
    private bool reloading;
    public ExpensesForm()
    {
        InitializeComponent();
    }

    public ExpensesForm(ExpenseService expenses) : this()
    {
        this.expenses = expenses;
        if (expenses.Transactions is { } transactions && expenses.Accounting is { } accounting)
        {
            cashTransactionsView.BindServices(transactions, accounting);
        }

        Shown += PageShown;
    }

    private void PageShown(object? sender, EventArgs e) => Reload();
    private void RefreshClicked(object? sender, EventArgs e) => Reload();
    private void Reload()
    {
        if (expenses is null)
            return;
        try
        {
            records = expenses.GetAll();
            var year = yearFilter.SelectedItem as string;
            reloading = true;
            try
            {
                yearFilter.Items.Clear();
                yearFilter.Items.Add("All years");
                var years = records.Select(b => b.DocumentDate.Year.ToString()).Append(DateTime.Today.Year.ToString());
                if (year is not null && year != "All years")
                    years = years.Append(year);
                yearFilter.Items.AddRange(years.Distinct().OrderDescending().Cast<object>().ToArray());
                yearFilter.SelectedItem = year ?? "All years";
            }
            finally
            {
                reloading = false;
            }

            ApplyFilters();
            note.Text = expenses.Accounting?.GetSettings().StartedOn is null ? "Accounting has not started. Save drafts first; use Settings > Accounting before posting or paying." : "Payments record money paid outside the app. Posted history is preserved; corrections use reversals.";
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Could not load expenses");
        }
    }

    public void OpenDashboardFilter(string? filter)
    {
        if (transactionTabs is not null && billPage is not null)
            transactionTabs.SelectedTab = billPage;
        statusFilter.SelectedIndex = 0;
        kindFilter.SelectedIndex = 0;
        yearFilter.SelectedItem = "All years";
        overdueOnly.Checked = filter == "Overdue";
        ApplyFilters();
    }

    private void FiltersChanged(object? sender, EventArgs e)
    {
        if (!reloading && expenses is not null)
            ApplyFilters();
    }

    private void ApplyFilters()
    {
        var id = selected?.Id;
        var year = int.TryParse(yearFilter.SelectedItem as string, out var y) ? y : (int? )null;
        var visible = records.Where(b => (statusFilter.SelectedIndex <= 0 || b.Status == statusFilter.Text) && (kindFilter.SelectedIndex <= 0 || b.Kind == kindFilter.Text) && (!year.HasValue || b.DocumentDate.Year == year) && (!overdueOnly.Checked || b.IsOverdueOn(DateTime.Today))).ToList();
        reloading = true;
        try
        {
            bills.DataSource = visible.Select(b => new BillRow(b.Id, b.Kind, b.PayeeName, b.Description, b.Reference, b.DocumentDate.ToString("dd MMM yyyy"), b.DueDate?.ToString("dd MMM yyyy") ?? "", b.Status, b.IsOverdueOn(DateTime.Today), b.Total, b.Paid, b.Balance)).ToList();
            foreach (DataGridViewRow row in bills.Rows)
                if ((row.DataBoundItem as BillRow)?.Number == id)
                {
                    bills.CurrentCell = row.Cells[0];
                    break;
                }
        }
        finally
        {
            reloading = false;
        }

        summary.Text = $"{visible.Count} records  |  Outstanding: RM {visible.Sum(b => b.Balance):N2}  |  Overdue: RM {visible.Where(b => b.IsOverdueOn(DateTime.Today)).Sum(b => b.Balance):N2}  |  Drafts: {visible.Count(b => !b.IsPosted)}";
        LoadSelected();
    }

    private void BillSelected(object? sender, EventArgs e)
    {
        if (!reloading && expenses is not null)
            LoadSelected();
    }

    private void LoadSelected()
    {
        if (expenses is null)
            return;
        try
        {
            selected = bills.CurrentRow?.DataBoundItem is BillRow row ? expenses.Get(row.Number) : null;
            var accounts = expenses.Accounting?.GetAccounts().ToDictionary(a => a.Id, a => a.DisplayName) ?? [];
            var categories = expenses.GetCategories().ToDictionary(c => c.Id, c => c.Name);
            linesGrid.DataSource = (selected?.Lines ?? []).Select(l => new { Category = selected?.IsOpeningBalance == true ? "Opening payable" : categories.GetValueOrDefault(l.CategoryId), l.Description, Account = accounts.GetValueOrDefault(l.AccountId), l.Amount }).ToList();
            paymentsGrid.DataSource = (selected?.Payments ?? []).OrderByDescending(p => p.Date).ThenByDescending(p => p.Id).Select(p => new PaymentRow(p.Id, p.Date.ToString("dd MMM yyyy"), p.Amount, accounts.GetValueOrDefault(p.CashAccountId) ?? "", p.Reference, p.ReversedDate?.ToString("dd MMM yyyy") ?? "", p.ReversalReason)).Concat((selected?.Settlements ?? []).Select(s => new PaymentRow(-s.AdjustmentId, s.Date.ToString("dd MMM yyyy"), s.Amount, "Deposit / prepayment applied", $"Adjustment #{s.AdjustmentId}", s.ReversedDate?.ToString("dd MMM yyyy") ?? "", "Reverse through Accounting > Adjustments"))).ToList();
            attachments.Items.Clear();
            foreach (var a in selected?.Attachments ?? [])
                attachments.Items.Add(a.FileName);
            editButton.Enabled = deleteButton.Enabled = postButton.Enabled = selected is { IsPosted: false };
            payButton.Enabled = selected is { IsPosted: true, CancelledDate: null, Balance: > 0 };
            cancelButton.Enabled = selected is { IsPosted: true, CancelledDate: null, Paid: 0 };
            UpdatePaymentAction();
            exportButton.Enabled = false;
            details.TabPages[0].Text = selected?.CancelledDate is { } cancelled ? $"Expense lines / Cancelled {cancelled:dd MMM yyyy}" : "Expense lines";
            details.TabPages[0].ToolTipText = selected?.CancellationReason ?? "";
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Could not load expense details");
        }
    }

    private void PaymentSelected(object? sender, EventArgs e) => UpdatePaymentAction();
    private void UpdatePaymentAction() => reverseButton.Enabled = selected is { CancelledDate: null } && paymentsGrid.CurrentRow?.DataBoundItem is PaymentRow row && row.Number > 0 && row.Reversed == "";
    private void AttachmentSelected(object? sender, EventArgs e) => exportButton.Enabled = attachments.SelectedIndex >= 0;
    private void NewClicked(object? sender, EventArgs e)
    {
        if (expenses is null)
            return;
        using var dialog = new ExpenseEditForm(expenses);
        if (dialog.ShowDialog(this) == DialogResult.OK)
            Reload();
    }

    private void EditClicked(object? sender, EventArgs e)
    {
        if (expenses is null || selected is null)
            return;
        using var dialog = new ExpenseEditForm(expenses, selected);
        if (dialog.ShowDialog(this) == DialogResult.OK)
            Reload();
    }

    private void PostClicked(object? sender, EventArgs e)
    {
        if (expenses is null || selected is null)
            return;
        if (MessageBox.Show(this, $"Post {selected.Kind.ToLowerInvariant()} for {selected.PayeeName}?\nRM {selected.Total:N2} / Accounting date {selected.AccountingDate:dd MMM yyyy}\n\nThis records the expense and amount owed. Posted details cannot be edited.", "Post expense", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
            return;
        try
        {
            expenses.Post(selected.Id);
            Reload();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Could not post expense");
        }
    }

    private void PayClicked(object? sender, EventArgs e) => OpenAction("Pay");
    private void CancelClicked(object? sender, EventArgs e) => OpenAction("Cancel");
    private void ReverseClicked(object? sender, EventArgs e) => OpenAction("Reverse");
    private void OpenAction(string action)
    {
        if (expenses is null || selected is null)
            return;
        var payment = action == "Reverse" && paymentsGrid.CurrentRow?.DataBoundItem is PaymentRow row ? selected.Payments.Single(p => p.Id == row.Number) : null;
        using var dialog = new ExpenseActionForm(expenses, selected, action, payment);
        if (dialog.ShowDialog(this) == DialogResult.OK)
            Reload();
    }

    private void DeleteClicked(object? sender, EventArgs e)
    {
        if (expenses is null || selected is null)
            return;
        if (MessageBox.Show(this, "Permanently delete this draft and its attachments?", "Delete draft", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            return;
        try
        {
            expenses.DeleteDraft(selected.Id);
            Reload();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Could not delete draft");
        }
    }

    private void PayeesClicked(object? sender, EventArgs e)
    {
        if (expenses is null)
            return;
        using var dialog = new ExpenseSetupForm(expenses);
        dialog.ShowDialog(this);
        Reload();
    }

    private void ExportClicked(object? sender, EventArgs e)
    {
        if (selected is null || attachments.SelectedIndex < 0)
            return;
        var a = selected.Attachments[attachments.SelectedIndex];
        using var dialog = new SaveFileDialog
        {
            FileName = a.FileName,
            Filter = "All files|*.*"
        };
        if (dialog.ShowDialog(this) != DialogResult.OK)
            return;
        try
        {
            File.WriteAllBytes(dialog.FileName, a.Content);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Could not save attachment");
        }
    }

    private sealed record BillRow(int Number, string Type, string Payee, string Description, string Reference, string Date, string Due, string Status, bool Overdue, decimal Total, decimal Paid, decimal Balance);
    private sealed record PaymentRow(int Number, string Date, decimal Amount, string PaidFrom, string Reference, string Reversed, string Reason);
}
