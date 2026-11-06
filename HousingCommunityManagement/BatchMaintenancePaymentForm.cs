using HousingCommunityManagement.Core;
namespace HousingCommunityManagement;

public partial class BatchMaintenancePaymentForm : Form
{
    private readonly MonthlyBillingService? billing;
    private readonly HouseholdService? households;
    private readonly Dictionary<int, PaymentDraft> drafts = [];
    private bool updatingFilters;
    private bool updatingGrid;
    private bool loaded;
    private int? initialYear;
    public BatchMaintenancePaymentForm() { InitializeComponent(); }
    public BatchMaintenancePaymentForm(MonthlyBillingService billing, HouseholdService households, int? initialYear = null, IEnumerable<int>? initialMonths = null) : this()
    {
        this.billing = billing; this.households = households; this.initialYear = initialYear ?? DateTime.Today.Year;
        paymentDate.MaxDate = DateTime.Today;
        updatingFilters = true;
        try
        {
            var months = (initialMonths ?? Enumerable.Range(1, 12)).ToHashSet();
            for (var i = 0; i < MonthSelectors.Length; i++) MonthSelectors[i].Checked = months.Contains(i + 1);
        }
        finally { updatingFilters = false; }
        Shown += PageShown;
    }
    private CheckBox[] MonthSelectors => [january, february, march, april, may, june, july, august, september, october, november, december];
    private int[] SelectedMonths => MonthSelectors.Select((box, i) => (box, i)).Where(x => x.box.Checked).Select(x => x.i + 1).ToArray();
    private void PageShown(object? sender, EventArgs e) { if (!loaded) Reload(); }
    private void Reload()
    {
        if (billing is null || households is null) return;
        try
        {
            var charges = billing.GetCharges().Where(c => c.Balance > 0).ToList();
            var houses = households.GetAll().ToDictionary(h => h.Id);
            var selectedYear = (yearFilter.SelectedItem as YearChoice)?.Year ?? initialYear ?? 0;
            var selectedAccount = (cashAccount.SelectedItem as Account)?.Id;
            updatingFilters = updatingGrid = true;
            try
            {
                grid.Rows.Clear(); drafts.Clear();
                foreach (var charge in charges)
                {
                    var house = houses.GetValueOrDefault(charge.HouseholdId);
                    drafts.Add(charge.Id, new PaymentDraft(charge, house?.HouseNumber ?? "Unknown house", house?.OwnerSummary ?? ""));
                }
                yearFilter.DisplayMember = nameof(YearChoice.Label);
                var years = charges.Select(c => c.Year).Append(DateTime.Today.Year);
                if (selectedYear > 0) years = years.Append(selectedYear);
                yearFilter.DataSource = new[] { new YearChoice(0, "All years") }.Concat(years.Distinct().OrderDescending().Select(y => new YearChoice(y, y.ToString()))).ToList();
                yearFilter.SelectedItem = yearFilter.Items.Cast<YearChoice>().First(y => y.Year == selectedYear);
                if (billing.Accounting is { } accounting)
                {
                    var settings = accounting.GetSettings(); var accounts = accounting.GetAccounts().Where(a => a.IsActive && a.IsCashBank).ToList();
                    cashAccount.DisplayMember = nameof(Account.DisplayName); cashAccount.ValueMember = nameof(Account.Id); cashAccount.DataSource = accounts;
                    cashAccount.SelectedValue = accounts.Any(a => a.Id == selectedAccount) ? selectedAccount!.Value : settings.ReceiptAccountId;
                }
                loaded = true;
            }
            finally { updatingFilters = updatingGrid = false; }
            ApplyFilters();
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "Could not load unpaid charges"); }
    }
    private void CaptureGrid()
    {
        foreach (DataGridViewRow row in grid.Rows)
            if (row.Tag is PaymentDraft draft)
            {
                draft.Selected = row.Cells[0].Value is true;
                draft.AmountText = row.Cells[5].Value?.ToString() ?? "";
                draft.Reference = row.Cells[6].Value?.ToString() ?? "";
            }
    }
    private void FiltersChanged(object? sender, EventArgs e) { if (!updatingFilters && loaded) ApplyFilters(); }
    private void ApplyFilters()
    {
        grid.EndEdit(); CaptureGrid();
        var months = SelectedMonths.ToHashSet(); var year = (yearFilter.SelectedItem as YearChoice)?.Year ?? 0; var term = houseSearch.Text.Trim();
        var visible = drafts.Values.Where(d => (year == 0 || d.Charge.Year == year) && months.Contains(d.Charge.Month)
            && (term.Length == 0 || d.House.Contains(term, StringComparison.OrdinalIgnoreCase) || d.Owners.Contains(term, StringComparison.OrdinalIgnoreCase)))
            .OrderBy(d => d.House, StringComparer.CurrentCultureIgnoreCase).ThenBy(d => d.Charge.Year).ThenBy(d => d.Charge.Month).ToList();
        var visibleIds = visible.Select(d => d.Charge.Id).ToHashSet();
        foreach (var draft in drafts.Values.Where(d => !visibleIds.Contains(d.Charge.Id))) draft.Selected = false;
        updatingGrid = true;
        try
        {
            grid.Rows.Clear();
            foreach (var draft in visible)
            {
                var c = draft.Charge;
                var index = grid.Rows.Add(draft.Selected, draft.House, new DateTime(c.Year, c.Month, 1).ToString("MMM yyyy"), c.DueDate.ToString("dd MMM yyyy"), c.Balance, draft.AmountText, draft.Reference);
                grid.Rows[index].Tag = draft;
            }
            allMonths.CheckState = months.Count == 0 ? CheckState.Unchecked : months.Count == 12 ? CheckState.Checked : CheckState.Indeterminate;
        }
        finally { updatingGrid = false; }
        UpdateSummary();
    }
    private void AllMonthsClicked(object? sender, EventArgs e)
    {
        updatingFilters = true;
        try { var selected = allMonths.CheckState != CheckState.Checked; foreach (var box in MonthSelectors) box.Checked = selected; }
        finally { updatingFilters = false; }
        if (loaded) ApplyFilters();
    }
    private void GridDirtyChanged(object? sender, EventArgs e)
    {
        if (grid.CurrentCell?.ColumnIndex == 0 && grid.IsCurrentCellDirty) grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
    }
    private void GridValueChanged(object? sender, DataGridViewCellEventArgs e) { if (!updatingGrid) { CaptureGrid(); UpdateSummary(); } }
    private void PaymentDetailsChanged(object? sender, EventArgs e) { if (!updatingFilters) UpdateSummary(); }
    private static bool TryAmount(PaymentDraft draft, out decimal amount) => decimal.TryParse(draft.AmountText, out amount) && amount > 0 && amount <= draft.Charge.Balance && decimal.Round(amount, 2) == amount;
    private void UpdateSummary()
    {
        var selected = drafts.Values.Where(d => d.Selected).ToList(); var total = 0m; var invalid = 0;
        foreach (var d in selected) { if (TryAmount(d, out var amount)) total += amount; else invalid++; }
        summary.Text = $"{selected.Count} payments selected / RM {total:N2} total" + (invalid > 0 ? $" / {invalid} invalid amount(s)" : "")
            + Environment.NewLine + "Only selected visible rows save. Filter changes clear hidden selections. Edit amounts for partial payments.";
        summary.ForeColor = invalid > 0 ? Color.Firebrick : SystemColors.ControlText;
        save.Enabled = selected.Count > 0 && invalid == 0 && cashAccount.SelectedItem is Account;
    }
    private void SetVisibleSelection(bool selected)
    {
        grid.EndEdit(); updatingGrid = true;
        try { foreach (DataGridViewRow row in grid.Rows) row.Cells[0].Value = selected; }
        finally { updatingGrid = false; }
        CaptureGrid(); UpdateSummary();
    }
    private void SelectAllClicked(object? sender, EventArgs e) => SetVisibleSelection(true);
    private void ClearSelectionClicked(object? sender, EventArgs e) => SetVisibleSelection(false);
    private void FillBalancesClicked(object? sender, EventArgs e)
    {
        grid.EndEdit(); CaptureGrid(); updatingGrid = true;
        try { foreach (DataGridViewRow row in grid.Rows) if (row.Tag is PaymentDraft { Selected: true } draft) row.Cells[5].Value = draft.Charge.Balance.ToString("N2"); }
        finally { updatingGrid = false; }
        CaptureGrid(); UpdateSummary();
    }
    private void RefreshClicked(object? sender, EventArgs e)
    {
        grid.EndEdit(); CaptureGrid();
        if (drafts.Values.Any(d => d.Selected || d.Reference.Length > 0 || d.AmountText != d.Charge.Balance.ToString("N2"))
            && MessageBox.Show(this, "Refresh unpaid charges? This clears selections and any payment amounts or references entered in this dialog.", "Refresh unpaid charges", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes) return;
        Reload();
    }
    private void SaveClicked(object? sender, EventArgs e)
    {
        if (billing is null) return;
        try
        {
            grid.EndEdit(); CaptureGrid(); UpdateSummary();
            if (cashAccount.SelectedItem is not Account account) throw new ArgumentException("Select the cash or bank account that received these payments.");
            var selected = drafts.Values.Where(d => d.Selected).ToList(); var payments = new List<MaintenancePaymentRequest>();
            foreach (var d in selected)
            {
                if (!TryAmount(d, out var amount)) throw new ArgumentException($"Enter a valid payment no greater than RM {d.Charge.Balance:N2} for house {d.House}, {new DateTime(d.Charge.Year, d.Charge.Month, 1):MMM yyyy}.");
                payments.Add(new MaintenancePaymentRequest(d.Charge.Id, amount, d.Charge.Balance, d.Reference));
            }
            MonthlyBillingService.ValidatePaymentBatch(payments, paymentDate.Value);
            if (MessageBox.Show(this, $"Record {payments.Count} payments totalling RM {payments.Sum(p => p.Amount):N2} for {selected.Select(d => d.Charge.HouseholdId).Distinct().Count()} house(s)?\nPayment date: {paymentDate.Value:dd MMM yyyy}\nReceived into: {account.DisplayName}\n\nThis records money already received outside the app. All selected rows will be saved together.", "Record multiple payments", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.OK) return;
            save.Enabled = false;
            var count = billing.RecordPayments(payments, paymentDate.Value, account.Id);
            MessageBox.Show(this, $"Recorded {count} payments totalling RM {payments.Sum(p => p.Amount):N2}.", "Payments recorded"); DialogResult = DialogResult.OK;
        }
        catch (Exception ex) { MessageBox.Show(this, "No payments were saved.\n\n" + ex.Message, "Could not record payment batch"); UpdateSummary(); }
    }
    private sealed record YearChoice(int Year, string Label);
    private sealed class PaymentDraft(MonthlyCharge charge, string house, string owners)
    {
        public MonthlyCharge Charge { get; } = charge;
        public string House { get; } = house;
        public string Owners { get; } = owners;
        public bool Selected { get; set; }
        public string AmountText { get; set; } = charge.Balance.ToString("N2");
        public string Reference { get; set; } = "";
    }
}
