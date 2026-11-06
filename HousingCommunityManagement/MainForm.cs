using HousingCommunityManagement.Core;
namespace HousingCommunityManagement;

public partial class MainForm : Form
{
    private readonly HouseholdService? service;
    private readonly string databasePath = "";
    private HouseholdForm? householdPage;
    private BillingForm? billingPage;
    private RenovationsForm? renovationsPage;
    private ExpensesForm? expensesPage;
    private AccountingForm? accountingPage;
    private DashboardView? overview;
    private RenovationService? renovations;
    private readonly MonthlyBillingService? billing;
    public MainForm() { InitializeComponent(); }
    public MainForm(HouseholdService service, MonthlyBillingService billing, string databasePath) : this()
    {
        this.service = service;
        this.billing = billing;
        this.databasePath = databasePath;
        renovations = new RenovationService(new HousingCommunityManagement.Data.HouseholdRepository(databasePath));
    }
    private void MainShown(object? sender, EventArgs e) => ShowDashboard();
    private void DashboardClicked(object? sender, EventArgs e) => ShowDashboard();
    private void ShowDashboard()
    {
        CloseAccountingPage();
        CloseExpensesPage();
        CloseRenovationsPage();
        CloseHouseholdPage();
        CloseBillingPage();
        billingButton.BackColor = Color.FromArgb(25, 39, 57);
        dashboardPanel.Visible = true;
        dashboardButton.BackColor = Color.FromArgb(43, 65, 88);
        housesButton.BackColor = Color.FromArgb(25, 39, 57);
        if (service is null || billing is null || renovations is null) return;
        if (overview is null)
        {
            foreach (var control in dashboardPanel.Controls.Cast<Control>().ToArray()) control.Dispose();
            dashboardPanel.Controls.Clear();
            dashboardPanel.ColumnStyles.Clear(); dashboardPanel.RowStyles.Clear();
            dashboardPanel.ColumnCount = dashboardPanel.RowCount = 1;
            dashboardPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            dashboardPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            dashboardPanel.Padding = Padding.Empty;
            overview = new DashboardView(service, billing, renovations, NavigateDashboard);
            dashboardPanel.Controls.Add(overview, 0, 0);
        }
        overview.Reload();
    }
    private void NavigateDashboard(string page, string? filter)
    {
        switch (page)
        {
            case "Billing": BillingClicked(this, EventArgs.Empty); billingPage?.OpenDashboardFilter(filter); break;
            case "Expenses": ExpensesClicked(this, EventArgs.Empty); expensesPage?.OpenDashboardFilter(filter); break;
            case "Renovations": RenovationsClicked(this, EventArgs.Empty); renovationsPage?.OpenDashboardFilter(filter); break;
            case "Accounting": AccountingClicked(this, EventArgs.Empty); break;
        }
    }
    private void HousesClicked(object? sender, EventArgs e)
    {
        if (service is null || householdPage is not null) return;
        CloseAccountingPage();
        CloseExpensesPage();
        CloseRenovationsPage();
        CloseBillingPage();
        billingButton.BackColor = Color.FromArgb(25, 39, 57);
        householdPage = new HouseholdForm(service, databasePath) { TopLevel = false, FormBorderStyle = FormBorderStyle.None, Dock = DockStyle.Fill, MinimumSize = Size.Empty };
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
        CloseAccountingPage();
        CloseExpensesPage();
        CloseRenovationsPage();
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
                if (dialog.DataDeleted) { ShowDashboard(); return; }
                if (billingPage is not null) { CloseBillingPage(); BillingClicked(sender, e); }
                else if (accountingPage is not null) { CloseAccountingPage(); AccountingClicked(sender, e); }
                else if (expensesPage is not null) { CloseExpensesPage(); ExpensesClicked(sender, e); }
                else if (renovationsPage is not null) { CloseRenovationsPage(); RenovationsClicked(sender, e); }
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
    private void RenovationsClicked(object? sender, EventArgs e)
    {
        if (renovations is null || service is null || renovationsPage is not null) return;
        CloseAccountingPage();
        CloseExpensesPage();
        CloseHouseholdPage(); CloseBillingPage();
        dashboardPanel.Visible = false;
        renovationsPage = new RenovationsForm(renovations, service) { TopLevel = false, FormBorderStyle = FormBorderStyle.None, Dock = DockStyle.Fill };
        contentPanel.Controls.Add(renovationsPage); renovationsPage.Show(); renovationsPage.BringToFront();
        renovationsButton.BackColor = Color.FromArgb(43, 65, 88);
        dashboardButton.BackColor = housesButton.BackColor = billingButton.BackColor = Color.FromArgb(25, 39, 57);
    }
    private void CloseRenovationsPage()
    {
        if (renovationsPage is not null) { renovationsPage.Close(); renovationsPage.Dispose(); renovationsPage = null; }
        renovationsButton.BackColor = Color.FromArgb(25, 39, 57);
    }
    private void ExpensesClicked(object? sender, EventArgs e)
    {
        if (service?.Expenses is not { } expenses || expensesPage is not null) return;
        CloseAccountingPage();
        CloseHouseholdPage(); CloseBillingPage(); CloseRenovationsPage();
        dashboardPanel.Visible = false;
        expensesPage = new ExpensesForm(expenses) { TopLevel = false, FormBorderStyle = FormBorderStyle.None, Dock = DockStyle.Fill };
        contentPanel.Controls.Add(expensesPage); expensesPage.Show(); expensesPage.BringToFront();
        expensesButton.BackColor = Color.FromArgb(43, 65, 88);
        dashboardButton.BackColor = housesButton.BackColor = billingButton.BackColor = Color.FromArgb(25, 39, 57);
    }
    private void CloseExpensesPage()
    {
        if (expensesPage is not null) { expensesPage.Close(); expensesPage.Dispose(); expensesPage = null; }
        expensesButton.BackColor = Color.FromArgb(25, 39, 57);
    }
    private void AccountingClicked(object? sender, EventArgs e)
    {
        if (service?.Accounting is not { } accounting || accountingPage is not null) return;
        CloseHouseholdPage(); CloseBillingPage(); CloseRenovationsPage(); CloseExpensesPage();
        dashboardPanel.Visible = false;
        accountingPage = new AccountingForm(accounting) { TopLevel = false, FormBorderStyle = FormBorderStyle.None, Dock = DockStyle.Fill };
        contentPanel.Controls.Add(accountingPage); accountingPage.Show(); accountingPage.BringToFront();
        accountingButton.BackColor = Color.FromArgb(43, 65, 88);
        dashboardButton.BackColor = housesButton.BackColor = billingButton.BackColor = Color.FromArgb(25, 39, 57);
    }
    private void CloseAccountingPage()
    {
        if (accountingPage is not null) { accountingPage.Close(); accountingPage.Dispose(); accountingPage = null; }
        accountingButton.BackColor = Color.FromArgb(25, 39, 57);
    }
    private void MainClosing(object? sender, FormClosingEventArgs e) { CloseHouseholdPage(); CloseBillingPage(); CloseRenovationsPage(); CloseExpensesPage(); CloseAccountingPage(); }
}
