using HousingCommunityManagement.Core;
namespace HousingCommunityManagement;
public partial class RenovationForm : Form
{
    private readonly RenovationService? renovations;
    public RenovationForm() { InitializeComponent(); UpdatePreview(null, EventArgs.Empty); }
    public RenovationForm(RenovationService renovations, HouseholdService households) : this()
    {
        this.renovations = renovations;
        house.DisplayMember = nameof(Household.HouseNumber);
        house.DataSource = households.GetAll().Where(h => h.IsActive && h.IsOwned).ToList();
        save.Enabled = house.Items.Count > 0;
        if (!save.Enabled) preview.Text = "Create an active, owned house first.";
    }
    private void UpdatePreview(object? sender, EventArgs e)
    {
        try { preview.Text = $"Planned end: {start.Value.Date.AddDays((int)duration.Value - 1):dd MMM yyyy}. Deposit is a one-off amount."; }
        catch (ArgumentOutOfRangeException) { preview.Text = "Choose an earlier start date or shorter duration."; }
    }
    private void SaveClicked(object? sender, EventArgs e)
    {
        if (house.SelectedItem is not Household h || renovations is null) return;
        try
        {
            renovations.Add(new Renovation { HouseholdId = h.Id, Description = description.Text, StartDate = start.Value, DurationDays = (int)duration.Value, DepositAmount = deposit.Value });
            DialogResult = DialogResult.OK;
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "Could not save renovation"); }
    }
}
