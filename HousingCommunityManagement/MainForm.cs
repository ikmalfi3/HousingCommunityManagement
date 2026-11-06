using HousingCommunityManagement.Core;
namespace HousingCommunityManagement;

public partial class MainForm : Form
{
    private readonly HouseholdService? service;
    private readonly string databasePath = "";
    private Form1? householdPage;
    private BillingForm? billingPage;
    private readonly MonthlyBillingService? billing;
    public MainForm() { InitializeComponent(); }
    public MainForm(HouseholdService service, MonthlyBillingService billing, string databasePath) : this()
    {
        this.service = service;
        this.billing = billing;
        this.databasePath = databasePath;
    }
    private void MainShown(object? sender, EventArgs e) => ShowDashboard();
    private void DashboardClicked(object? sender, EventArgs e) => ShowDashboard();
    private void ShowDashboard()
    {
        CloseHouseholdPage();
        CloseBillingPage();
        billingButton.BackColor = Color.FromArgb(25, 39, 57);
        dashboardPanel.Visible = true;
        dashboardButton.BackColor = Color.FromArgb(43, 65, 88);
        housesButton.BackColor = Color.FromArgb(25, 39, 57);
        try
        {
            var houses = service?.GetAll() ?? [];
            totalValue.Text = houses.Count.ToString("N0");
            houseDetails.Text = $"{houses.Count(h => h.IsActive):N0} active / {houses.Count(h => h.IsOwned):N0} owned";
            var today = DateTime.Today;
            var month = today.Month;
            var charges = billing?.GetCharges() ?? [];
            var current = charges.Where(c => c.Year == today.Year && c.Month == month).ToList();
            var charged = current.Sum(c => c.Amount);
            var paid = current.Sum(c => c.Paid);
            monthlyFeeValue.Text = $"RM {current.Sum(c => c.Balance):N2}";
            collectedValue.Text = $"RM {paid:N2}";
            claimsValue.Text = $"RM {charges.Where(c => c.StatusOn(today) == "Overdue").Sum(c => c.Balance):N2}";
            var due = MonthlyBillingService.DueDate(today.Year, month);
            dateLabel.Text = $"{today:MMMM yyyy} / Monthly charges";
            feeDetails.Text = $"{current.Count} house(s) billed this month";
            collectedDetails.Text = "Payments allocated to this month";
            claimsDetails.Text = "Unpaid balances past their due dates / all months";
            collectionNote.Text = current.Count == 0 ? "No charges generated for this month. Open Billing & payments to generate them."
                : $"{(charged == 0 ? 100 : paid / charged * 100):N1}% collected / RM {charged:N2} charged. Overdue status follows each charge's saved due date.";
            collectionProgress.Value = charged == 0 ? 0 : Math.Clamp((int)(paid / charged * 100), 0, 100);
            var names = houses.ToDictionary(h => h.Id, h => h.HouseNumber);
            var recent = charges.SelectMany(c => c.Payments.Select(p => new { Charge = c, Payment = p }))
                .OrderByDescending(x => x.Payment.Date).ThenByDescending(x => x.Payment.Id).Take(8).ToList();
            recentEmpty.Text = recent.Count == 0 ? "No maintenance payments recorded yet."
                : string.Join(Environment.NewLine + Environment.NewLine, recent.Select(x =>
                    $"{x.Payment.Date:d MMM yyyy}    House {names.GetValueOrDefault(x.Charge.HouseholdId)}    {new DateTime(x.Charge.Year, x.Charge.Month, 1):MMM yyyy}    RM {x.Payment.Amount:N2}    {x.Payment.Reference}"));
            footerNote.Text = $"Monthly rate: RM {service?.GetSettings().MonthlyMaintenanceFee ?? 0:N2} / Charge any selection of months. Existing charges retain their original rate.";
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "Could not load dashboard", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }
    private void HousesClicked(object? sender, EventArgs e)
    {
        if (service is null || householdPage is not null) return;
        CloseBillingPage();
        billingButton.BackColor = Color.FromArgb(25, 39, 57);
        householdPage = new Form1(service, databasePath) { TopLevel = false, FormBorderStyle = FormBorderStyle.None, Dock = DockStyle.Fill, MinimumSize = Size.Empty };
        contentPanel.Controls.Add(householdPage);
        dashboardPanel.Visible = false;
        householdPage.Show();
        householdPage.BringToFront();
        dashboardButton.BackColor = Color.FromArgb(25, 39, 57);
        housesButton.BackColor = Color.FromArgb(43, 65, 88);
    }
    private void BillingClicked(object? sender, EventArgs e)
    {
        if (billing is null || service is null || billingPage is not null) return;
        CloseHouseholdPage();
        dashboardPanel.Visible = false;
        billingPage = new BillingForm(billing, service) { TopLevel = false, FormBorderStyle = FormBorderStyle.None, Dock = DockStyle.Fill };
        contentPanel.Controls.Add(billingPage); billingPage.Show(); billingPage.BringToFront();
        billingButton.BackColor = Color.FromArgb(43, 65, 88);
        dashboardButton.BackColor = Color.FromArgb(25, 39, 57);
        housesButton.BackColor = Color.FromArgb(25, 39, 57);
    }
    private void CloseBillingPage()
    {
        if (billingPage is null) return;
        billingPage.Close(); billingPage.Dispose(); billingPage = null;
    }
    private void SettingsClicked(object? sender, EventArgs e)
    {
        if (service is null) return;
        try
        {
            using var dialog = new CommunitySettingsForm(service);
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                if (billingPage is not null) { CloseBillingPage(); BillingClicked(sender, e); }
                else if (householdPage is null) ShowDashboard();
                else { CloseHouseholdPage(); HousesClicked(sender, e); }
            }
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "Could not open settings"); }
    }
    private void CloseHouseholdPage()
    {
        if (householdPage is null) return;
        householdPage.Close();
        householdPage.Dispose();
        householdPage = null;
    }
    private void MainClosing(object? sender, FormClosingEventArgs e) { CloseHouseholdPage(); CloseBillingPage(); }
}
