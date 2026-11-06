using HousingCommunityManagement.Core;
namespace HousingCommunityManagement;
public partial class HouseRangeForm : Form
{
    private readonly HouseholdService? service;
    public HouseRangeForm()
    {
        InitializeComponent();
        prefix.TextChanged += PreviewChanged; start.ValueChanged += PreviewChanged;
        end.ValueChanged += PreviewChanged; digits.ValueChanged += PreviewChanged;
        save.Text = "Create houses";
    }
    public HouseRangeForm(HouseholdService service) : this() { this.service = service; }
    private void PreviewChanged(object? sender, EventArgs e) => preview.Text =
        prefix.Text.Trim() + ((int)start.Value).ToString("D" + (int)digits.Value) + " through " +
        prefix.Text.Trim() + ((int)end.Value).ToString("D" + (int)digits.Value);
    private void SaveClicked(object? sender, EventArgs e)
    {
        if (service is null) return;
        try
        {
            var added = service.CreateRange(prefix.Text, (int)start.Value, (int)end.Value, (int)digits.Value);
            MessageBox.Show(this, $"Created {added} house(s). Existing house numbers were skipped.", "Range created");
            DialogResult = DialogResult.OK;
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "Could not create range", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
    }
}
