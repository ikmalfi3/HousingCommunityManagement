using HousingCommunityManagement.Core;
namespace HousingCommunityManagement;
public partial class CommunitySettingsForm : Form
{
    private readonly HouseholdService? service;
    public CommunitySettingsForm() { InitializeComponent(); }
    public CommunitySettingsForm(HouseholdService service) : this()
    {
        this.service = service;
        var settings = service.GetSettings(); fee.Value = settings.MonthlyMaintenanceFee; flagNames.Lines = settings.FlagNames.ToArray();
    }
    private void SaveClicked(object? sender, EventArgs e)
    {
        if (service is null) return;
        try
        {
            service.SaveSettings(new CommunitySettings { MonthlyMaintenanceFee = fee.Value, FlagNames = flagNames.Lines.ToList() });
            DialogResult = DialogResult.OK;
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "Could not save settings", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
    }
}
