using HousingCommunityManagement.Core;

namespace HousingCommunityManagement;
public partial class AccountingLocksView : UserControl
{
    private AccountingService accounting = null!;
    public AccountingLocksView(AccountingService accounting) : this()
    { BindServices(accounting); }

    private void Reload()
    {
        if (accounting is null || DesignMode) return;
        try
        {
            var setting = accounting.GetSettings();
            enabled.Checked = setting.LockedThrough.HasValue;
            if (setting.LockedThrough.HasValue)
                through.Value = setting.LockedThrough.Value.Date;
            grid.DataSource = accounting.Operations!.Audit().Select(a => new { a.Id, RecordedUTC = a.RecordedAt.ToString("yyyy-MM-dd HH:mm:ss"), a.Description }).ToList();
            if (grid.Columns["Description"] is { } column)
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            status.Text = setting.LockedThrough.HasValue ? $"Transactions dated on or before {setting.LockedThrough:dd MMM yyyy} are locked. Statement finalization also protects matched cash entries independently." : "No transaction lock is set. Finalized statements still protect their cash entries.";
        }
        catch (Exception ex)
        {
            AccountingUi.Error(this, ex);
        }
    }

    public void RefreshData() => Reload();
    private void ApplyClicked(object? sender, EventArgs e)
    {
        try
        {
            accounting.Operations!.SetLock(enabled.Checked ? through.Value.Date : null, reason.Text);
            reason.Clear();
            Reload();
        }
        catch (Exception ex)
        {
            AccountingUi.Error(this, ex);
        }
    }

    private void RefreshClicked(object? sender, EventArgs e) => Reload();
    public void BindServices(AccountingService accounting) { this.accounting = accounting; through.MaxDate = DateTime.Today; Reload(); }
    
    public AccountingLocksView()
    {
        InitializeComponent();
    }
}
