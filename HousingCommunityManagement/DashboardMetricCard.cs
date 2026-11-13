using System.ComponentModel;
namespace HousingCommunityManagement;
public partial class DashboardMetricCard : UserControl
{
    public DashboardMetricCard() { InitializeComponent(); }
    [DefaultValue("Metric")] public string TitleText { get => titleLabel.Text; set => titleLabel.Text = value; }
    [DefaultValue("RM 0.00")] public string ValueText { get => valueLabel.Text; set => valueLabel.Text = value; }
    [DefaultValue("Details")] public string DetailText { get => detailLabel.Text; set => detailLabel.Text = value; }
    public event EventHandler? NavigateRequested;
    private void ValueClicked(object? sender, LinkLabelLinkClickedEventArgs e) => NavigateRequested?.Invoke(this, EventArgs.Empty);
}
