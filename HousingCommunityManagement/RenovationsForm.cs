using HousingCommunityManagement.Core;
namespace HousingCommunityManagement;

public partial class RenovationsForm : Form
{
    private readonly RenovationService? renovations;
    private readonly HouseholdService? households;
    private List<Renovation> records = [];
    private bool updatingFilters;
    public RenovationsForm() { InitializeComponent(); }
    public RenovationsForm(RenovationService renovations, HouseholdService households) : this()
    {
        this.renovations = renovations; this.households = households;
        actionDate.MinDate = new DateTime(1900, 1, 1);
        actionDate.MaxDate = DateTime.Today;
        Shown += (_, _) => RefreshRecords();
    }
    private void RefreshClicked(object? sender, EventArgs e) => RefreshRecords();
    private void GridSelectionChanged(object? sender, EventArgs e) => UpdateActions();
    private void CollectClicked(object? sender, EventArgs e) => RecordAction("Collect");
    private void CompleteClicked(object? sender, EventArgs e) => RecordAction("Complete");
    private void ReturnClicked(object? sender, EventArgs e) => RecordAction("Return");
    private void RefreshRecords()
    {
        try
        {
            records = renovations?.GetAll(includeArchived: true) ?? [];
            if (renovations?.Accounting is { } accounting)
            {
                var accounts = accounting.GetAccounts().Where(a => a.IsActive && a.IsCashBank).ToList();
                var settings = accounting.GetSettings();
                BindCash(receivedInto, accounts, settings.ReceiptAccountId);
                BindCash(paidFrom, accounts, settings.PaymentAccountId);
            }
            var year = yearFilter.SelectedItem as string;
            updatingFilters = true;
            try
            {
                var years = records.Select(r => r.StartDate.Year.ToString()).Append(DateTime.Today.Year.ToString());
                if (year is not null && year != "All years") years = years.Append(year);
                yearFilter.Items.Clear();
                yearFilter.Items.Add("All years");
                yearFilter.Items.AddRange(years.Distinct().OrderDescending().Cast<object>().ToArray());
                yearFilter.SelectedItem = year ?? "All years";
            }
            finally { updatingFilters = false; }
            ApplyFilters();
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "Could not load renovations"); }
    }
    public void OpenDashboardFilter(string? filter)
    {
        statusFilter.SelectedIndex = 0; depositFilter.SelectedIndex = 0;
        yearFilter.SelectedItem = "All years";
        if (filter == "Overdue") statusFilter.SelectedItem = filter;
        else if (filter is "Awaiting return" or "Awaiting collection") depositFilter.SelectedItem = filter;
        ApplyFilters();
    }
    private void FiltersChanged(object? sender, EventArgs e)
    {
        if (!updatingFilters && renovations is not null) ApplyFilters();
    }
    private void ApplyFilters()
    {
        var selected = Selected?.Id;
        int? year = int.TryParse(yearFilter.SelectedItem as string, out var value) ? value : null;
        var status = statusFilter.SelectedIndex > 0 ? statusFilter.SelectedItem as string : null;
        var depositStatus = depositFilter.SelectedIndex > 0 ? depositFilter.SelectedItem as string : null;
        var candidates = records.Where(r => !r.IsArchived || showArchived.Checked || status == "Archived" || depositStatus == "Awaiting return");
        var visible = RenovationService.Filter(candidates, DateTime.Today, year, status, depositStatus);
        var houses = (households?.GetAll() ?? []).ToDictionary(h => h.Id);
        grid.Rows.Clear();
        foreach (var r in visible)
        {
            var house = houses.GetValueOrDefault(r.HouseholdId);
            var index = grid.Rows.Add(house?.HouseNumber ?? "", house?.OwnerSummary ?? "", r.Description, r.StartDate.ToString("dd MMM yyyy"), r.DurationDays, r.PlannedEndDate.ToString("dd MMM yyyy"), r.StatusOn(DateTime.Today), r.DepositAmount.ToString("N2"), r.DepositStatus, r.DepositCollectedDate?.ToString("dd MMM yyyy") ?? "", r.CompletedDate?.ToString("dd MMM yyyy") ?? "", r.DepositReturnedDate?.ToString("dd MMM yyyy") ?? "");
            grid.Rows[index].Tag = r;
            if (r.Id == selected) grid.CurrentCell = grid.Rows[index].Cells[0];
        }
        summary.Text = $"Showing {visible.Count} of {records.Count}  |  {visible.Count(r => !r.CompletedDate.HasValue && !r.IsArchived)} unfinished  |  Deposits held: RM {visible.Where(r => r.DepositCollectedDate.HasValue && !r.DepositReturnedDate.HasValue).Sum(r => r.DepositAmount):N2}  |  Awaiting return: {visible.Count(r => r.DepositStatus == "Awaiting return")}";
        UpdateActions();
    }
    private Renovation? Selected => grid.CurrentRow?.Tag as Renovation;
    private void UpdateActions()
    {
        var r = Selected;
        deleteButton.Enabled = r is { IsArchived: false };
        deleteButton.Text = r?.DepositCollectedDate is not null ? "Archive renovation" : "Delete renovation";
        collectButton.Enabled = r is { IsArchived: false, CompletedDate: null, DepositCollectedDate: null, DepositAmount: > 0 };
        completeButton.Enabled = r is { IsArchived: false, CompletedDate: null };
        returnButton.Enabled = r is { DepositCollectedDate: not null, DepositReturnedDate: null } && (r.IsArchived || r.CompletedDate.HasValue);
    }
    private void DeleteClicked(object? sender, EventArgs e)
    {
        if (Selected is not { } r || renovations is null) return;
        var house = grid.CurrentRow?.Cells[0].Value?.ToString();
        var archive = r.DepositCollectedDate.HasValue;
        var message = $"{(archive ? "Archive" : "Delete")} renovation for house {house}?\n{r.Description}\nStart: {r.StartDate:dd MMM yyyy}\n\n" + (archive ? "Deposit and accounting history will be preserved. Any held deposit still needs to be returned; use Awaiting return to find it." : "This permanently removes the renovation. This cannot be undone.");
        if (MessageBox.Show(this, message, archive ? "Archive renovation" : "Delete renovation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.Yes) return;
        try { renovations.Delete(r.Id); RefreshRecords(); }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "Could not delete renovation"); }
    }
    private void AddClicked(object? sender, EventArgs e)
    {
        if (renovations is null || households is null) return;
        using var dialog = new RenovationForm(renovations, households);
        if (dialog.ShowDialog(this) == DialogResult.OK) RefreshRecords();
    }
    private void RecordAction(string action)
    {
        if (Selected is not { } r || renovations is null) return;
        var message = action == "Return" ? $"Record RM {r.DepositAmount:N2} returned to the houseowner?" : action == "Collect" ? $"Record RM {r.DepositAmount:N2} collected?" : "Mark this renovation as completed?";
        if (action is "Collect" or "Return") message += $"\nAccount: {((action == "Collect" ? receivedInto : paidFrom).SelectedItem as Account)?.DisplayName}";
        if (MessageBox.Show(this, message + $"\nDate: {actionDate.Value:dd MMM yyyy}", "Renovations", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK) return;
        try
        {
            int? accountId = null;
            if (action is "Collect" or "Return")
            {
                var control = action == "Collect" ? receivedInto : paidFrom;
                if (control.SelectedItem is not Account account) throw new ArgumentException("Select a cash or bank account for this transaction.");
                accountId = account.Id;
            }
            renovations.RecordAction(r.Id, action, actionDate.Value, accountId); RefreshRecords();
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "Could not record action"); }
    }
    private static void BindCash(ComboBox control, List<Account> accounts, int defaultId)
    {
        var selected = (control.SelectedItem as Account)?.Id ?? defaultId;
        control.DisplayMember = nameof(Account.DisplayName); control.ValueMember = nameof(Account.Id);
        control.DataSource = accounts.ToList(); control.SelectedValue = accounts.Any(a => a.Id == selected) ? selected : defaultId;
    }
}
