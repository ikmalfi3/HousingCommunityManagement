using HousingCommunityManagement.Core;
namespace HousingCommunityManagement;
public partial class BillingForm : Form
{
    private readonly MonthlyBillingService? billing;
    private readonly HouseholdService? households;
    private void DrawBillingTab(object? sender, DrawItemEventArgs e)
    {
        var selected = e.Index == billingTabs.SelectedIndex;
        var bounds = billingTabs.GetTabRect(e.Index);
        using var background = new SolidBrush(selected ? Color.FromArgb(43, 65, 88) : Color.FromArgb(25, 39, 57));
        e.Graphics.FillRectangle(background, bounds);
        if (selected)
        {
            using var accent = new SolidBrush(Color.FromArgb(72, 184, 166));
            e.Graphics.FillRectangle(accent, bounds.Left, bounds.Bottom - 3, bounds.Width, 3);
        }
        TextRenderer.DrawText(e.Graphics, billingTabs.TabPages[e.Index].Text, billingTabs.Font,
            bounds, selected ? Color.White : Color.FromArgb(194, 207, 222),
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);
        if (selected && billingTabs.Focused)
            ControlPaint.DrawFocusRectangle(e.Graphics, Rectangle.Inflate(bounds, -5, -5), Color.White, background.Color);
    }
    public BillingForm() { InitializeComponent(); }
    public BillingForm(MonthlyBillingService billing, HouseholdService households) : this()
    {
        this.billing = billing; this.households = households;
        year.Value = DateTime.Today.Year;
        summaryYear.Value = DateTime.Today.Year;
        MonthSelectors[DateTime.Today.Month - 1].Checked = true;

    }
    private CheckBox[] MonthSelectors => [january, february, march, april, may, june, july, august, september, october, november, december];
    private int[] SelectedMonths => MonthSelectors.Select((box, i) => (box, i)).Where(x => x.box.Checked).Select(x => x.i + 1).ToArray();
    private bool dashboardOverdueOnly;
    private bool dashboardAllYears;
    public void OpenDashboardFilter(string? filter)
    {
        dashboardAllYears = true;
        dashboardOverdueOnly = filter == "Overdue";
        billingTabs.SelectedIndex = 0;
        updatingMonths = true;
        foreach (var box in MonthSelectors) box.Checked = true;
        updatingMonths = false;
        allMonths.CheckState = CheckState.Checked;
        Reload();
    }
    private bool updatingMonths;
    private void AllMonthsClicked(object? sender, EventArgs e)
    {
        updatingMonths = true;
        var select = allMonths.CheckState != CheckState.Checked;
        foreach (var box in MonthSelectors) box.Checked = select;
        updatingMonths = false;
        RefreshClicked(sender, e);
    }
    private void RefreshClicked(object? sender, EventArgs e)
    {
        if (updatingMonths) return;
        dashboardOverdueOnly = false;
        dashboardAllYears = false;
        var count = SelectedMonths.Length;
        allMonths.CheckState = count == 0 ? CheckState.Unchecked : count == 12 ? CheckState.Checked : CheckState.Indeterminate;
        Reload();
    }
    private void Reload()
    {
        if (billing is null || households is null) return;
        try
        {
            var names = households.GetAll().ToDictionary(h => h.Id, h => h.HouseNumber);
            grid.DataSource = billing.GetCharges().Where(c => dashboardOverdueOnly ? c.StatusOn(DateTime.Today) == "Overdue" : (dashboardAllYears || c.Year == (int)year.Value) && SelectedMonths.Contains(c.Month))
                .OrderBy(c => names.GetValueOrDefault(c.HouseholdId)).ThenBy(c => c.Month).Select(c => new ChargeRow(c, names.GetValueOrDefault(c.HouseholdId, "Unknown"))).ToList();
            note.Text = dashboardOverdueOnly ? "Overdue charges across all years. Change year or month filters to return to normal billing view." : dashboardAllYears ? "All charges across all years. Change year or month filters to return to normal billing view." : "Select months above to filter records. No months selected: no records. Use Charge houses to choose billing months and houses separately.";
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "Could not load billing"); }
    }
    private void GenerateClicked(object? sender, EventArgs e)
    {
        if (billing is null || households is null) return;
        try
        {
            using var dialog = new ChargeHousesForm(billing, households);
            if (dialog.ShowDialog(this) == DialogResult.OK) Reload();
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "Could not generate charges"); }
    }
    private void BillingTabChanged(object? sender, EventArgs e)
    {
        if (billingTabs.SelectedTab == summaryTab) ReloadSummary();
        else Reload();
    }
    private void SummaryRefreshClicked(object? sender, EventArgs e) => ReloadSummary();
    private void ReloadSummary()
    {
        if (billing is null || households is null) return;
        try
        {
            var charges = billing.GetCharges().Where(c => c.Year == (int)summaryYear.Value)
                .ToDictionary(c => (c.HouseholdId, c.Month));
            summaryGrid.Rows.Clear();
            foreach (var house in households.GetAll())
            {
                var index = summaryGrid.Rows.Add();
                var row = summaryGrid.Rows[index];
                row.Cells[0].Value = house.HouseNumber;
                row.Cells[0].ToolTipText = house.IsActive ? "Active house" : "Inactive house";
                row.Cells[1].Value = string.Join(Environment.NewLine, house.Owners);
                row.Cells[2].Value = string.Join(Environment.NewLine, house.Contacts);
                for (var month = 1; month <= 12; month++)
                {
                    var cell = row.Cells[month + 2];
                    charges.TryGetValue((house.Id, month), out var charge);
                    var status = charge?.StatusOn(DateTime.Today);
                    var (background, foreground) = status switch
                    {
                        "Paid" => (Color.FromArgb(220, 252, 231), Color.FromArgb(22, 101, 52)),
                        "Overdue" => (Color.FromArgb(254, 226, 226), Color.FromArgb(153, 27, 27)),
                        "Payment due" => (Color.FromArgb(219, 234, 254), Color.FromArgb(30, 64, 175)),
                        _ => (Color.FromArgb(243, 244, 246), Color.FromArgb(75, 85, 99))
                    };
                    cell.Style.BackColor = background;
                    cell.Style.ForeColor = foreground;
                    cell.Style.SelectionBackColor = background;
                    cell.Style.SelectionForeColor = foreground;
                    cell.Value = charge is null ? "—" : status == "Paid" ? $"Paid\nRM {charge.Paid:N2}"
                        : $"{(status == "Overdue" ? "Overdue" : "Due")}\nRM {charge.Balance:N2}";
                    cell.ToolTipText = charge is null ? "Not charged" :
                        $"Charged: RM {charge.Amount:N2}\nPaid: RM {charge.Paid:N2}\nRemaining: RM {charge.Balance:N2}\nDue: {charge.DueDate:d MMM yyyy}\n{status}";
                }
            }
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "Could not load summary", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }
    private void BatchPayClicked(object? sender, EventArgs e)
    {
        if (billing is null || households is null) return;
        using var dialog = new BatchMaintenancePaymentForm(billing, households, (int)year.Value, SelectedMonths);
        if (dialog.ShowDialog(this) == DialogResult.OK) Reload();
    }
    private void PayClicked(object? sender, EventArgs e)
    {
        if (billing is null) return;
        if (grid.CurrentRow?.DataBoundItem is not ChargeRow row) { MessageBox.Show(this, "Select a house's monthly charge first."); return; }
        if (row.Balance <= 0) { MessageBox.Show(this, "This month is already paid."); return; }
        using var dialog = new PaymentForm(billing, row.Charge, row.House);
        if (dialog.ShowDialog(this) == DialogResult.OK) Reload();
    }
    private sealed class ChargeRow(MonthlyCharge charge, string house)
    {
        public MonthlyCharge Charge { get; } = charge;
        public string House => house;
        public string Month => new DateTime(Charge.Year, Charge.Month, 1).ToString("MMM yyyy");
        public decimal Charged => Charge.Amount;
        public decimal Paid => Charge.Paid;
        public decimal Balance => Charge.Balance;
        public string Due => Charge.DueDate.ToString("d MMM yyyy");
        public string Status => Charge.StatusOn(DateTime.Today);
    }
}
