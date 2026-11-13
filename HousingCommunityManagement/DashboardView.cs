using HousingCommunityManagement.Core;

namespace HousingCommunityManagement;
public partial class DashboardView : UserControl
{
    private HouseholdService households = null!;
    private MonthlyBillingService billing = null!;
    private RenovationService renovations = null!;
    private Action<string, string?> navigate = (_, _) => { };
    private bool loading;
    public DashboardView() { InitializeComponent(); }
    public DashboardView(HouseholdService households, MonthlyBillingService billing, RenovationService renovations, Action<string, string?> navigate) : this() => BindServices(households, billing, renovations, navigate);
    public void BindServices(HouseholdService households, MonthlyBillingService billing, RenovationService renovations, Action<string, string?> navigate)
    {
        this.households = households; this.billing = billing; this.renovations = renovations; this.navigate = navigate; year.Value = DateTime.Today.Year;
    }
    private void RefreshClicked(object? sender, EventArgs e) => Reload();
    private void YearChanged(object? sender, EventArgs e) { if (!loading) Reload(); }
    private void CardsSizeChanged(object? sender, EventArgs e) => ResizeCards();
    private void BillingLinkClicked(object? sender, EventArgs e) => navigate("Billing", null);
    private void RenovationsLinkClicked(object? sender, EventArgs e) => navigate("Renovations", null);
    private void ExpensesLinkClicked(object? sender, EventArgs e) => navigate("Expenses", null);
    private void AccountingLinkClicked(object? sender, EventArgs e) => navigate("Accounting", null);
    private static Label TextLabel(string text) => new()
    {
        Text = text,
        AutoSize = true,
        MaximumSize = new Size(430, 0),
        UseMnemonic = false,
        Margin = new Padding(3, 6, 3, 7),
        ForeColor = Color.FromArgb(58, 72, 88)
    };
    private void MaintenanceOverdueClicked(object? sender, LinkLabelLinkClickedEventArgs e) => navigate("Billing", "Overdue");
    private void ExpensesOverdueClicked(object? sender, LinkLabelLinkClickedEventArgs e) => navigate("Expenses", "Overdue");
    private void RenovationOverdueClicked(object? sender, LinkLabelLinkClickedEventArgs e) => navigate("Renovations", "Overdue");
    private void DepositReturnClicked(object? sender, LinkLabelLinkClickedEventArgs e) => navigate("Renovations", "Awaiting return");
    private void DepositCollectionClicked(object? sender, LinkLabelLinkClickedEventArgs e) => navigate("Renovations", "Awaiting collection");
    private static void Clear(Control control)
    {
        foreach (var child in control.Controls.Cast<Control>().ToArray())
            child.Dispose();
        control.Controls.Clear();
    }

    private void ResizeCards()
    {
        var columns = cards.ClientSize.Width >= 900 ? 4 : 2;
        foreach (Control card in cards.Controls)
        {
            card.Width = Math.Max(180, cards.ClientSize.Width / columns - 10);

        }
    }

    public void Reload()
    {
        if (households is null || billing is null || renovations is null || DesignMode) return;
        try
        {
            loading = true;
            var today = DateTime.Today;
            var selectedYear = (int)year.Value;
            var houses = households.GetAll();
            var charges = billing.GetCharges();
            var works = renovations.GetAll(true);
            var bills = households.Expenses?.GetAll() ?? [];
            var money = households.Expenses?.Transactions?.GetAll() ?? [];
            var accounting = households.Accounting;
            var started = accounting?.GetSettings().StartedOn;
            var accounts = accounting?.GetAccounts() ?? [];
            var journal = accounting?.GetJournal() ?? [];
            var cashIds = accounts.Where(a => a.IsCashBank).Select(a => a.Id).ToHashSet();
            var cash = journal.Where(e => e.Date.Date <= today).SelectMany(e => e.Lines).Where(l => cashIds.Contains(l.AccountId)).Sum(l => l.Debit - l.Credit);
            decimal ExternalAmount(CashTransaction t) => t.Allocations.Where(l => !cashIds.Contains(l.AccountId)).Sum(l => l.Amount);
            decimal MonthMoney(string direction) => money.Where(t => t.Direction == direction).Sum(t => (t.Date.Year == today.Year && t.Date.Month == today.Month ? ExternalAmount(t) : 0m) - (t.ReversedDate?.Year == today.Year && t.ReversedDate?.Month == today.Month ? ExternalAmount(t) : 0m));
            var held = works.Where(r => r.DepositCollectedDate.HasValue && !r.DepositReturnedDate.HasValue).ToList();
            cashCard.ValueText = started.HasValue ? $"RM {cash:N2}" : "Not configured"; cashCard.DetailText = started.HasValue ? "Current ledger balance" : "Start accounting in Settings";
            maintenanceCard.ValueText = $"RM {charges.Sum(c => c.Balance):N2}"; maintenanceCard.DetailText = $"{charges.Where(c => c.Balance > 0).Select(c => c.HouseholdId).Distinct().Count()} houses · all years";
            unpaidCard.ValueText = $"RM {bills.Sum(b => b.Balance):N2}"; unpaidCard.DetailText = $"{bills.Count(b => b.Balance > 0)} bills / claims";
            depositCard.ValueText = $"RM {held.Sum(r => r.DepositAmount):N2}"; depositCard.DetailText = $"{held.Count} renovations · refundable";
            ResizeCards();
            community.Text = $"As of {today:dd MMM yyyy}   ·   {houses.Count} houses / {houses.Count(h => h.IsActive)} active / {houses.Count(h => h.IsOwned)} owned";

            maintenanceOverdueLink.Text = $"Maintenance overdue: {charges.Where(c => c.StatusOn(today) == "Overdue").Select(c => c.HouseholdId).Distinct().Count()} houses · RM {charges.Where(c => c.StatusOn(today) == "Overdue").Sum(c => c.Balance):N2}";
            expensesOverdueLink.Text = $"Expenses overdue: {bills.Count(b => b.IsOverdueOn(today))} · RM {bills.Where(b => b.IsOverdueOn(today)).Sum(b => b.Balance):N2}";
            renovationOverdueLink.Text = $"Renovations past planned end: {works.Count(r => r.StatusOn(today) == "Overdue")}";
            depositReturnLink.Text = $"Deposits awaiting return: {works.Count(r => r.DepositStatus == "Awaiting return")}";

            var annual = charges.Where(c => c.Year == selectedYear).ToList();
            var charged = annual.Sum(c => c.Amount);
            var collected = annual.Sum(c => c.Paid);
            maintenanceSummary.Text = $"Charged: RM {charged:N2}   ·   Collected: RM {collected:N2}\nRemaining: RM {annual.Sum(c => c.Balance):N2}";
            maintenanceProgress.Value = charged == 0 ? 0 : Math.Clamp((int)(collected / charged * 100), 0, 100);
            maintenanceRate.Text = charged == 0 ? "No charges for this year." : $"{collected / charged:P1} collected for {selectedYear} charges.";
            Clear(upcomingRenovations);
            renovationSummary.Text = $"Upcoming: {works.Count(r => r.StatusOn(today) == "Planned")}   ·   In progress: {works.Count(r => r.StatusOn(today) == "In progress")}   ·   Overdue: {works.Count(r => r.StatusOn(today) == "Overdue")}";
            depositCollectionLink.Text = $"Deposits awaiting collection: {works.Count(r => !r.IsArchived && !r.CompletedDate.HasValue && r.DepositStatus == "Awaiting collection")}";
            var upcoming = works.Where(r => !r.IsArchived && !r.CompletedDate.HasValue).OrderBy(r => r.PlannedEndDate).Take(3).ToList();
            foreach (var r in upcoming)
                upcomingRenovations.Controls.Add(TextLabel($"House {houses.FirstOrDefault(h => h.Id == r.HouseholdId)?.HouseNumber ?? "?"} · planned end {r.PlannedEndDate:dd MMM yyyy} · {r.StatusOn(today)}"));
            if (upcoming.Count == 0)
                upcomingRenovations.Controls.Add(TextLabel("No unfinished renovations."));
            Clear(dueBillsPanel);
            expenseSummary.Text = $"Unpaid: {bills.Count(b => b.Status == "Unpaid")}   ·   Partially paid: {bills.Count(b => b.Status == "Partially paid")}   ·   Drafts: {bills.Count(b => b.Status == "Draft")}";
            var monthPaid = bills.SelectMany(b => b.Payments).Where(p => p.Date.Year == today.Year && p.Date.Month == today.Month).Sum(p => p.Amount) - bills.SelectMany(b => b.Payments).Where(p => p.ReversedDate?.Year == today.Year && p.ReversedDate?.Month == today.Month).Sum(p => p.Amount);
            expenseMonthPaid.Text = $"Net paid in {today:MMM yyyy}: RM {monthPaid + MonthMoney("Money out"):N2} / Other receipts: RM {MonthMoney("Money in"):N2}";
            var dueBills = bills.Where(b => b.Balance > 0).OrderBy(b => b.DueDate ?? DateTime.MaxValue).Take(3).ToList();
            foreach (var b in dueBills)
                dueBillsPanel.Controls.Add(TextLabel($"{b.PayeeName} · RM {b.Balance:N2} · {(b.DueDate.HasValue ? b.DueDate.Value.ToString("dd MMM yyyy") : "No due date")}"));
            if (dueBills.Count == 0)
                dueBillsPanel.Controls.Add(TextLabel("No unpaid bills or claims."));
            var events = new List<(DateTime Date, string Text)>();
            foreach (var t in money)
            {
                events.Add((t.Date, $"{t.Direction} · {t.Counterparty} · RM {t.Total:N2}"));
                if (t.ReversedDate is { } reversed)
                    events.Add((reversed, $"Reversed {t.Direction.ToLowerInvariant()} #{t.Id} · RM {t.Total:N2}"));
            }

            foreach (var c in charges)
                foreach (var p in c.Payments)
                    events.Add((p.Date, $"Maintenance received · House {houses.FirstOrDefault(h => h.Id == c.HouseholdId)?.HouseNumber ?? "?"} · RM {p.Amount:N2}"));
            foreach (var b in bills)
                foreach (var p in b.Payments)
                {
                    events.Add((p.Date, $"Expense paid · {b.PayeeName} · RM {p.Amount:N2}"));
                    if (p.ReversedDate.HasValue)
                        events.Add((p.ReversedDate.Value, $"Expense payment reversed · {b.PayeeName} · RM {p.Amount:N2}"));
                }

            foreach (var r in works)
            {
                if (r.DepositCollectedDate.HasValue)
                    events.Add((r.DepositCollectedDate.Value, $"Deposit received · House {houses.FirstOrDefault(h => h.Id == r.HouseholdId)?.HouseNumber ?? "?"} · RM {r.DepositAmount:N2}"));
                if (r.DepositReturnedDate.HasValue)
                    events.Add((r.DepositReturnedDate.Value, $"Deposit returned · House {houses.FirstOrDefault(h => h.Id == r.HouseholdId)?.HouseNumber ?? "?"} · RM {r.DepositAmount:N2}"));
            }

            Clear(activity);
            foreach (var item in events.OrderByDescending(x => x.Date).ThenBy(x => x.Text).Take(5))
                activity.Controls.Add(TextLabel($"{item.Date:dd MMM yyyy} · {item.Text}"));
            if (events.Count == 0)
                activity.Controls.Add(TextLabel("No payments or deposit activity yet."));
            var received = new decimal[12];
            var paid = new decimal[12];
            var deposits = new decimal[12];
            var returned = new decimal[12];
            foreach (var p in charges.SelectMany(c => c.Payments).Where(p => p.Date.Year == selectedYear))
                received[p.Date.Month - 1] += p.Amount;
            foreach (var p in bills.SelectMany(b => b.Payments))
            {
                if (p.Date.Year == selectedYear)
                    paid[p.Date.Month - 1] += p.Amount;
                if (p.ReversedDate?.Year == selectedYear)
                    paid[p.ReversedDate.Value.Month - 1] -= p.Amount;
            }

            foreach (var r in works)
            {
                if (r.DepositCollectedDate?.Year == selectedYear)
                    deposits[r.DepositCollectedDate.Value.Month - 1] += r.DepositAmount;
                if (r.DepositReturnedDate?.Year == selectedYear)
                    returned[r.DepositReturnedDate.Value.Month - 1] += r.DepositAmount;
            }

            foreach (var t in money)
            {
                var series = t.Direction == "Money in" ? received : paid;
                if (t.Date.Year == selectedYear)
                    series[t.Date.Month - 1] += ExternalAmount(t);
                if (t.ReversedDate is { } reversed && reversed.Year == selectedYear)
                    series[reversed.Month - 1] -= ExternalAmount(t);
            }

            chart.SetValues(received, paid, deposits, returned);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Could not load dashboard", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            loading = false;
        }
    }
}
