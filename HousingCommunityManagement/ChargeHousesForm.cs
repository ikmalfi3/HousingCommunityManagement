using HousingCommunityManagement.Core;
namespace HousingCommunityManagement;
public partial class ChargeHousesForm : Form
{
    private readonly MonthlyBillingService? billing;
    private List<Household> available = [];
    private bool updatingMonths;
    private bool updatingHouses;
    private int dueDatesYear;
    public ChargeHousesForm() { InitializeComponent(); }
    public ChargeHousesForm(MonthlyBillingService billing, HouseholdService households) : this()
    {
        this.billing = billing;
        chargeDate.Value = DateTime.Today;
        available = households.GetAll().Where(h => h.IsActive).ToList();
        year.Value = DateTime.Today.Year;
        MonthSelectors[DateTime.Today.Month - 1].Checked = true;
        sharedDue.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 6);
        if (billing.Accounting is { } accounting)
        {
            var settings = accounting.GetSettings();
            var accounts = accounting.GetAccounts().ToDictionary(a => a.Id, a => a.DisplayName);
            receivableAccount.Text = accounts.GetValueOrDefault(settings.ReceivableAccountId, "Not configured");
            incomeAccount.Text = accounts.GetValueOrDefault(settings.IncomeAccountId, "Not configured");
            accountNote.Text = settings.StartedOn.HasValue
                ? "Default accounts from Settings > Accounting. New charges debit receivable and credit maintenance income."
                : "Accounting has not started. These defaults will apply once accounting is enabled in Settings > Accounting.";
        }
        ShowHouses();
        UpdateMonthlyDueDates();
    }
    private CheckBox[] MonthSelectors => [january, february, march, april, may, june, july, august, september, october, november, december];
    private int[] SelectedMonths => MonthSelectors.Select((box, i) => (box, i)).Where(x => x.box.Checked).Select(x => x.i + 1).ToArray();
    private DateTimePicker[] MonthDueEditors => [januaryDue, februaryDue, marchDue, aprilDue, mayDue, juneDue, julyDue, augustDue, septemberDue, octoberDue, novemberDue, decemberDue];
    private FlowLayoutPanel[] MonthDuePanels => [januaryDuePanel, februaryDuePanel, marchDuePanel, aprilDuePanel, mayDuePanel, juneDuePanel, julyDuePanel, augustDuePanel, septemberDuePanel, octoberDuePanel, novemberDuePanel, decemberDuePanel];
    private void SharedDueChanged(object? sender, EventArgs e)
    {
        if (useSharedDue.Checked) useMonthlyDue.Checked = false;
        sharedDue.Enabled = useSharedDue.Checked;
        UpdateMonthlyDueDates();
    }
    private void MonthlyDueChanged(object? sender, EventArgs e)
    {
        if (useMonthlyDue.Checked) useSharedDue.Checked = false;
        sharedDue.Enabled = useSharedDue.Checked;
        UpdateMonthlyDueDates();
    }
    private void UpdateMonthlyDueDates()
    {
        var selectedYear = (int)year.Value;
        // Native date spinners support years through 9998; ordinary charging still supports 9999.
        useMonthlyDue.Enabled = selectedYear <= 9998;
        if (!useMonthlyDue.Enabled && useMonthlyDue.Checked) useMonthlyDue.Checked = false;
        if (selectedYear <= 9998 && dueDatesYear != selectedYear)
        {
            MoveToYear(chargeDate, selectedYear);
            MoveToYear(sharedDue, selectedYear);
            for (var i = 0; i < MonthDueEditors.Length; i++)
            {
                if (dueDatesYear == 0) MonthDueEditors[i].Value = MonthlyBillingService.DueDate(selectedYear, i + 1);
                else MoveToYear(MonthDueEditors[i], selectedYear);
            }
            dueDatesYear = selectedYear;
        }
        var selected = SelectedMonths;
        monthlyDueGroup.Visible = useMonthlyDue.Checked;
        layout.RowStyles[5].Height = useMonthlyDue.Checked ? 30 + 36 * Math.Max(1, (selected.Length + 3) / 4) : 0;
        monthlyDueGroup.Text = selected.Length == 0 ? "Select months above to set their due dates" : "Due dates for selected months (default: the 6th of each month)";
        monthlyDueGrid.SuspendLayout();
        foreach (var panel in MonthDuePanels) panel.Visible = false;
        for (var i = 0; i < selected.Length; i++)
        {
            var panel = MonthDuePanels[selected[i] - 1];
            monthlyDueGrid.SetCellPosition(panel, new TableLayoutPanelCellPosition(i % 4, i / 4));
            panel.Visible = true;
        }
        monthlyDueGrid.ResumeLayout();
        MinimumSize = new Size(950, useMonthlyDue.Checked ? 640 : 560);
    }
    private static void MoveToYear(DateTimePicker picker, int selectedYear)
    {
        var value = picker.Value;
        picker.Value = new DateTime(selectedYear, value.Month, Math.Min(value.Day, DateTime.DaysInMonth(selectedYear, value.Month)));
    }
    private void AmountChanged(object? sender, EventArgs e) => UpdateHouseSelection();
    private void RefreshClicked(object? sender, EventArgs e)
    {
        if (updatingMonths) return;
        var count = SelectedMonths.Length;
        allMonths.CheckState = count == 0 ? CheckState.Unchecked : count == 12 ? CheckState.Checked : CheckState.Indeterminate;
        UpdateMonthlyDueDates();
    }
    private void AllMonthsClicked(object? sender, EventArgs e)
    {
        updatingMonths = true;
        var select = allMonths.CheckState != CheckState.Checked;
        foreach (var box in MonthSelectors) box.Checked = select;
        updatingMonths = false;
        RefreshClicked(sender, e);
    }
    private void OwnedFilterChanged(object? sender, EventArgs e) => ShowHouses();
    private void ShowHouses()
    {
        // Only visible checked houses can be charged; hidden unowned selections are discarded.
        var selected = houses.CheckedItems.Cast<HouseOption>().Select(h => h.Id).ToHashSet();
        updatingHouses = true;
        houses.Items.Clear();
        foreach (var house in available.Where(h => !onlyOwned.Checked || h.IsOwned))
            houses.Items.Add(new HouseOption(house.Id, $"{house.HouseNumber} / {(house.IsOwned ? "Owned" : "Unowned")} / {house.OwnerSummary}"), selected.Contains(house.Id));
        updatingHouses = false;
        UpdateHouseSelection();
    }
    private void AllHousesClicked(object? sender, EventArgs e)
    {
        var select = allHouses.CheckState != CheckState.Checked;
        updatingHouses = true;
        for (var i = 0; i < houses.Items.Count; i++) houses.SetItemChecked(i, select);
        updatingHouses = false;
        UpdateHouseSelection();
    }
    private void HouseItemCheck(object? sender, ItemCheckEventArgs e)
    {
        if (updatingHouses) return;
        // ItemCheck fires before CheckedItems changes.
        var count = houses.CheckedItems.Count + (e.NewValue == CheckState.Checked ? 1 : 0) - (e.CurrentValue == CheckState.Checked ? 1 : 0);
        UpdateHouseSelection(count);
    }
    private void UpdateHouseSelection(int? selectedCount = null)
    {
        var count = selectedCount ?? houses.CheckedItems.Count;
        allHouses.CheckState = count == 0 ? CheckState.Unchecked : count == houses.Items.Count ? CheckState.Checked : CheckState.Indeterminate;
        note.Text = $"{count} of {houses.Items.Count} visible active houses selected. Amount: RM {chargeAmount.Value:N2} per house per month. Existing month charges are skipped.";
    }
    private void GenerateClicked(object? sender, EventArgs e)
    {
        if (billing is null) return;
        try
        {
            var ids = houses.CheckedItems.Cast<HouseOption>().Select(h => h.Id).ToArray();
            var dates = useMonthlyDue.Checked ? SelectedMonths.ToDictionary(m => m, m => MonthDueEditors[m - 1].Value.Date) : null;
            var count = billing.GenerateMonths((int)year.Value, SelectedMonths, chargeAmount.Value, useSharedDue.Checked ? sharedDue.Value : null, ids, dates, chargeDate.Value.Date);
            MessageBox.Show(this, $"Created {count} monthly charge(s) at RM {chargeAmount.Value:N2} per house per month, dated {chargeDate.Value:dd MMM yyyy}. Existing charges were retained.", "Charges created");
            DialogResult = DialogResult.OK;
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "Could not charge houses", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
    }
    private sealed record HouseOption(int Id, string Label)
    {
        public override string ToString() => Label;
    }
}
