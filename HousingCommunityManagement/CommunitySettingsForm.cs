using HousingCommunityManagement.Core;
namespace HousingCommunityManagement;
public partial class CommunitySettingsForm : Form
{
    private readonly HouseholdService? service;
    public bool DataDeleted { get; private set; }
    public CommunitySettingsForm() { InitializeComponent(); }
    public CommunitySettingsForm(HouseholdService service) : this()
    {
        this.service = service;
        flagNames.Lines = service.GetSettings().FlagNames.ToArray();
    }
    private void SaveClicked(object? sender, EventArgs e)
    {
        if (service is null) return;
        try
        {
            var settings = service.GetSettings();
            settings.FlagNames = flagNames.Lines.ToList();
            service.SaveSettings(settings);
            DialogResult = DialogResult.OK;
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "Could not save settings", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
    }
    private void AccountingClicked(object? sender, EventArgs e)
    {
        if (service?.Accounting is not { } accounting) return;
        try { using var dialog = new AccountingSettingsForm(accounting); dialog.ShowDialog(this); }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "Could not open accounting"); }
    }
    private void DeleteDataClicked(object? sender, EventArgs e)
    {
        if (service?.DataReset is not { } reset) return;
        try
        {
            using var dialog = new DeleteDataForm(reset);
            if (dialog.ShowDialog(this) != DialogResult.OK) return;
            DataDeleted = true;
            DialogResult = DialogResult.OK;
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "Could not open data deletion"); }
    }
}
