using HousingCommunityManagement.Core;
namespace HousingCommunityManagement;
public partial class ChargeHousesForm : Form
{
    private readonly MonthlyBillingService? billing;
    private List<Household> available = [];
    private bool updatingMonths;
    private bool updatingHouses;
    public ChargeHousesForm() { InitializeComponent(); }
    public ChargeHousesForm(MonthlyBillingService billing, HouseholdService households) : this()
    {
        this.billing = billing;
        available = households.GetAll().Where(h => h.IsActive).ToList();
        year.Value = DateTime.Today.Year;
        MonthSelectors[DateTime.Today.Month - 1].Checked = true;
        sharedDue.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 6);
        ShowHouses();
    }
    private CheckBox[] MonthSelectors => [january, february, march, april, may, june, july, august, september, october, november, december];
    private int[] SelectedMonths => MonthSelectors.Select((box, i) => (box, i)).Where(x => x.box.Checked).Select(x => x.i + 1).ToArray();
    private void SharedDueChanged(object? sender, EventArgs e) => sharedDue.Enabled = useSharedDue.Checked;
    private void RefreshClicked(object? sender, EventArgs e)
    {
        if (updatingMonths) return;
        var count = SelectedMonths.Length;
        allMonths.CheckState = count == 0 ? CheckState.Unchecked : count == 12 ? CheckState.Checked : CheckState.Indeterminate;
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
        note.Text = $"{count} of {houses.Items.Count} visible active houses selected. Only selected houses will be charged; existing month charges are skipped.";
    }
    private void GenerateClicked(object? sender, EventArgs e)
    {
        if (billing is null) return;
        try
        {
            var ids = houses.CheckedItems.Cast<HouseOption>().Select(h => h.Id).ToArray();
            var count = billing.GenerateMonths((int)year.Value, SelectedMonths, useSharedDue.Checked ? sharedDue.Value : null, ids);
            MessageBox.Show(this, $"Created {count} monthly charge(s). Existing charges were retained.", "Charges created");
            DialogResult = DialogResult.OK;
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "Could not charge houses", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
    }
    private sealed record HouseOption(int Id, string Label)
    {
        public override string ToString() => Label;
    }
}
