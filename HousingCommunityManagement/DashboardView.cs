using HousingCommunityManagement.Core;

namespace HousingCommunityManagement;

public sealed class DashboardView : UserControl
{
    private readonly HouseholdService households;
    private readonly MonthlyBillingService billing;
    private readonly RenovationService renovations;
    private readonly Action<string, string?> navigate;
    private readonly NumericUpDown year = new() { Minimum = 1900, Maximum = 9999, Width = 85, Value = DateTime.Today.Year };
    private readonly TableLayoutPanel body = new() { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2, Padding = new Padding(24), BackColor = Color.FromArgb(245, 247, 250) };
    private readonly FlowLayoutPanel cards = new() { Dock = DockStyle.Fill, AutoSize = true, WrapContents = true };
    private readonly FlowLayoutPanel attention = Stack();
    private readonly FlowLayoutPanel maintenance = Stack();
    private readonly FlowLayoutPanel renovation = Stack();
    private readonly FlowLayoutPanel expenses = Stack();
    private readonly FlowLayoutPanel activity = Stack();
    private readonly MoneyChart chart = new() { Dock = DockStyle.Fill, Height = 210 };
    private readonly Label community = TextLabel("");
    private bool loading;

    public DashboardView(HouseholdService households, MonthlyBillingService billing, RenovationService renovations, Action<string, string?> navigate)
    {
        this.households = households; this.billing = billing; this.renovations = renovations; this.navigate = navigate;
        Dock = DockStyle.Fill; AutoScroll = true; BackColor = body.BackColor;
        body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50)); body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        var header = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = false };
        header.Controls.Add(new Label { Text = "Dashboard", AutoSize = true, Font = new Font("Segoe UI", 22, FontStyle.Bold), Margin = new Padding(0, 0, 30, 8) });
        header.Controls.Add(TextLabel("Yearly totals")); header.Controls.Add(year);
        var refresh = new Button { Text = "Refresh", AutoSize = true }; refresh.Click += (_, _) => Reload(); header.Controls.Add(refresh);
        AddWide(header); AddWide(community); AddWide(cards);
        AddSection("Money received & paid · selected year", chart, "", null);
        AddSection("Needs attention · current", attention, "", null);
        AddSection("Maintenance fees · selected billing year", maintenance, "View all charges", "Billing");
        AddSection("Renovations · current", renovation, "View all renovations", "Renovations");
        AddSection("Expenses · current", expenses, "View all expenses", "Expenses");
        AddSection("Recent activity · all years", activity, "View journal", "Accounting");
        AddWide(TextLabel("Balances and attention items cover all years. Yearly collections follow payment dates; maintenance progress follows the charge's billing year. Deposits are refundable liabilities, separate from income."));
        Controls.Add(body);
        cards.SizeChanged += (_, _) => ResizeCards();
        year.ValueChanged += (_, _) => { if (!loading) Reload(); };
    }

    private static FlowLayoutPanel Stack() => new() { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoSize = true };
    private static Label TextLabel(string text) => new() { Text = text, AutoSize = true, MaximumSize = new Size(430, 0), UseMnemonic = false, Margin = new Padding(3, 6, 3, 7), ForeColor = Color.FromArgb(58, 72, 88) };
    private void AddWide(Control control) { var row = body.RowCount++; body.RowStyles.Add(new RowStyle(SizeType.AutoSize)); body.Controls.Add(control, 0, row); body.SetColumnSpan(control, 2); }
    private void AddSection(string title, Control content, string link, string? destination)
    {
        var panel = new TableLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, ColumnCount = 1, Padding = new Padding(14), BackColor = Color.White, Margin = new Padding(5, 8, 5, 8), MinimumSize = new Size(0, 240) };
        var heading = TextLabel(title); heading.Font = new Font("Segoe UI", 11, FontStyle.Bold);
        panel.Controls.Add(heading); panel.Controls.Add(content);
        if (destination is not null) panel.Controls.Add(Link(link, destination));
        // Each pair occupies one row, keeping both columns the same height.
        var index = body.Controls.OfType<TableLayoutPanel>().Count();
        if (index % 2 == 0) { body.RowCount++; body.RowStyles.Add(new RowStyle(SizeType.AutoSize)); }
        body.Controls.Add(panel, index % 2, body.RowCount - 1);
    }
    private LinkLabel Link(string text, string page, string? filter = null)
    {
        var label = new LinkLabel { Text = text, AutoSize = true, MaximumSize = new Size(430, 0), Margin = new Padding(3, 7, 3, 7), UseMnemonic = false, LinkColor = Color.FromArgb(23, 100, 147) };
        label.LinkClicked += (_, _) => navigate(page, filter); return label;
    }
    private static void Clear(Control control) { foreach (var child in control.Controls.Cast<Control>().ToArray()) child.Dispose(); control.Controls.Clear(); }
    private void ResizeCards()
    {
        var columns = cards.ClientSize.Width >= 900 ? 4 : 2;
        foreach (Control card in cards.Controls)
        {
            card.Width = Math.Max(180, cards.ClientSize.Width / columns - 10);
            foreach (Control label in card.Controls) label.MaximumSize = new Size(Math.Max(150, card.Width - 30), 0);
        }
    }
    private void Card(string title, string value, string detail, string page)
    {
        var card = new FlowLayoutPanel { FlowDirection = FlowDirection.TopDown, WrapContents = false, Width = 205, Height = 140, BackColor = Color.White, Padding = new Padding(12), Margin = new Padding(5, 10, 5, 8) };
        card.Controls.Add(TextLabel(title)); var number = Link(value, page); number.Font = new Font("Segoe UI", 16, FontStyle.Bold); card.Controls.Add(number); card.Controls.Add(TextLabel(detail)); cards.Controls.Add(card);
    }

    public void Reload()
    {
        try
        {
            loading = true;
            var today = DateTime.Today; var selectedYear = (int)year.Value;
            var houses = households.GetAll(); var charges = billing.GetCharges(); var works = renovations.GetAll(true);
            var bills = households.Expenses?.GetAll() ?? [];
            var accounting = households.Accounting; var started = accounting?.GetSettings().StartedOn;
            var accounts = accounting?.GetAccounts() ?? []; var journal = accounting?.GetJournal() ?? [];
            var cashIds = accounts.Where(a => a.IsCashBank).Select(a => a.Id).ToHashSet();
            var cash = journal.Where(e => e.Date.Date <= today).SelectMany(e => e.Lines).Where(l => cashIds.Contains(l.AccountId)).Sum(l => l.Debit - l.Credit);
            var held = works.Where(r => r.DepositCollectedDate.HasValue && !r.DepositReturnedDate.HasValue).ToList();
            Clear(cards);
            Card("Cash & bank balance", started.HasValue ? $"RM {cash:N2}" : "Not configured", started.HasValue ? "Current ledger balance" : "Start accounting in Settings", "Accounting");
            Card("Maintenance outstanding", $"RM {charges.Sum(c => c.Balance):N2}", $"{charges.Where(c => c.Balance > 0).Select(c => c.HouseholdId).Distinct().Count()} houses · all years", "Billing");
            Card("Unpaid expenses", $"RM {bills.Sum(b => b.Balance):N2}", $"{bills.Count(b => b.Balance > 0)} bills / claims", "Expenses");
            Card("Deposits held", $"RM {held.Sum(r => r.DepositAmount):N2}", $"{held.Count} renovations · refundable", "Renovations");
            ResizeCards();
            community.Text = $"As of {today:dd MMM yyyy}   ·   {houses.Count} houses / {houses.Count(h => h.IsActive)} active / {houses.Count(h => h.IsOwned)} owned";
            Clear(attention);
            attention.Controls.Add(Link($"Maintenance overdue: {charges.Where(c => c.StatusOn(today) == "Overdue").Select(c => c.HouseholdId).Distinct().Count()} houses · RM {charges.Where(c => c.StatusOn(today) == "Overdue").Sum(c => c.Balance):N2}", "Billing", "Overdue"));
            attention.Controls.Add(Link($"Expenses overdue: {bills.Count(b => b.IsOverdueOn(today))} · RM {bills.Where(b => b.IsOverdueOn(today)).Sum(b => b.Balance):N2}", "Expenses", "Overdue"));
            attention.Controls.Add(Link($"Renovations past planned end: {works.Count(r => r.StatusOn(today) == "Overdue")}", "Renovations", "Overdue"));
            attention.Controls.Add(Link($"Deposits awaiting return: {works.Count(r => r.DepositStatus == "Awaiting return")}", "Renovations", "Awaiting return"));
            Clear(maintenance);
            var annual = charges.Where(c => c.Year == selectedYear).ToList(); var charged = annual.Sum(c => c.Amount); var collected = annual.Sum(c => c.Paid);
            maintenance.Controls.Add(TextLabel($"Charged: RM {charged:N2}   ·   Collected: RM {collected:N2}\nRemaining: RM {annual.Sum(c => c.Balance):N2}"));
            maintenance.Controls.Add(new ProgressBar { Width = 350, Height = 18, Value = charged == 0 ? 0 : Math.Clamp((int)(collected / charged * 100), 0, 100) });
            maintenance.Controls.Add(TextLabel(charged == 0 ? "No charges for this year." : $"{collected / charged:P1} collected for {selectedYear} charges."));
            Clear(renovation);
            renovation.Controls.Add(TextLabel($"Upcoming: {works.Count(r => r.StatusOn(today) == "Planned")}   ·   In progress: {works.Count(r => r.StatusOn(today) == "In progress")}   ·   Overdue: {works.Count(r => r.StatusOn(today) == "Overdue")}"));
            renovation.Controls.Add(Link($"Deposits awaiting collection: {works.Count(r => !r.IsArchived && !r.CompletedDate.HasValue && r.DepositStatus == "Awaiting collection")}", "Renovations", "Awaiting collection"));
            var upcoming = works.Where(r => !r.IsArchived && !r.CompletedDate.HasValue).OrderBy(r => r.PlannedEndDate).Take(3).ToList();
            foreach (var r in upcoming) renovation.Controls.Add(TextLabel($"House {houses.FirstOrDefault(h => h.Id == r.HouseholdId)?.HouseNumber ?? "?"} · planned end {r.PlannedEndDate:dd MMM yyyy} · {r.StatusOn(today)}"));
            if (upcoming.Count == 0) renovation.Controls.Add(TextLabel("No unfinished renovations."));
            Clear(expenses);
            expenses.Controls.Add(TextLabel($"Unpaid: {bills.Count(b => b.Status == "Unpaid")}   ·   Partially paid: {bills.Count(b => b.Status == "Partially paid")}   ·   Drafts: {bills.Count(b => b.Status == "Draft")}"));
            var monthPaid = bills.SelectMany(b => b.Payments).Where(p => p.Date.Year == today.Year && p.Date.Month == today.Month).Sum(p => p.Amount) - bills.SelectMany(b => b.Payments).Where(p => p.ReversedDate?.Year == today.Year && p.ReversedDate?.Month == today.Month).Sum(p => p.Amount);
            expenses.Controls.Add(TextLabel($"Net paid in {today:MMM yyyy}: RM {monthPaid:N2}"));
            var dueBills = bills.Where(b => b.Balance > 0).OrderBy(b => b.DueDate ?? DateTime.MaxValue).Take(3).ToList();
            foreach (var b in dueBills) expenses.Controls.Add(TextLabel($"{b.PayeeName} · RM {b.Balance:N2} · {(b.DueDate.HasValue ? b.DueDate.Value.ToString("dd MMM yyyy") : "No due date")}"));
            if (dueBills.Count == 0) expenses.Controls.Add(TextLabel("No unpaid bills or claims."));
            var events = new List<(DateTime Date, string Text)>();
            foreach (var c in charges) foreach (var p in c.Payments) events.Add((p.Date, $"Maintenance received · House {houses.FirstOrDefault(h => h.Id == c.HouseholdId)?.HouseNumber ?? "?"} · RM {p.Amount:N2}"));
            foreach (var b in bills) foreach (var p in b.Payments) { events.Add((p.Date, $"Expense paid · {b.PayeeName} · RM {p.Amount:N2}")); if (p.ReversedDate.HasValue) events.Add((p.ReversedDate.Value, $"Expense payment reversed · {b.PayeeName} · RM {p.Amount:N2}")); }
            foreach (var r in works) { if (r.DepositCollectedDate.HasValue) events.Add((r.DepositCollectedDate.Value, $"Deposit received · House {houses.FirstOrDefault(h => h.Id == r.HouseholdId)?.HouseNumber ?? "?"} · RM {r.DepositAmount:N2}")); if (r.DepositReturnedDate.HasValue) events.Add((r.DepositReturnedDate.Value, $"Deposit returned · House {houses.FirstOrDefault(h => h.Id == r.HouseholdId)?.HouseNumber ?? "?"} · RM {r.DepositAmount:N2}")); }
            Clear(activity);
            foreach (var item in events.OrderByDescending(x => x.Date).ThenBy(x => x.Text).Take(5)) activity.Controls.Add(TextLabel($"{item.Date:dd MMM yyyy} · {item.Text}"));
            if (events.Count == 0) activity.Controls.Add(TextLabel("No payments or deposit activity yet."));
            var received = new decimal[12]; var paid = new decimal[12]; var deposits = new decimal[12]; var returned = new decimal[12];
            foreach (var p in charges.SelectMany(c => c.Payments).Where(p => p.Date.Year == selectedYear)) received[p.Date.Month - 1] += p.Amount;
            foreach (var p in bills.SelectMany(b => b.Payments)) { if (p.Date.Year == selectedYear) paid[p.Date.Month - 1] += p.Amount; if (p.ReversedDate?.Year == selectedYear) paid[p.ReversedDate.Value.Month - 1] -= p.Amount; }
            foreach (var r in works) { if (r.DepositCollectedDate?.Year == selectedYear) deposits[r.DepositCollectedDate.Value.Month - 1] += r.DepositAmount; if (r.DepositReturnedDate?.Year == selectedYear) returned[r.DepositReturnedDate.Value.Month - 1] += r.DepositAmount; }
            chart.SetValues(received, paid, deposits, returned);
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "Could not load dashboard", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        finally { loading = false; }
    }
}

internal sealed class MoneyChart : Control
{
    private decimal[][] values = [new decimal[12], new decimal[12], new decimal[12], new decimal[12]];
    private readonly Color[] colors = [Color.SeaGreen, Color.SteelBlue, Color.DarkGoldenrod, Color.MediumPurple];
    private readonly string[] names = ["Maintenance received", "Expenses paid (net)", "Deposits received", "Deposits returned"];
    private readonly ToolTip tooltip = new();
    private int hoveredMonth = -1;
    public MoneyChart() { DoubleBuffered = true; ResizeRedraw = true; }
    protected override void Dispose(bool disposing) { if (disposing) tooltip.Dispose(); base.Dispose(disposing); }
    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        var month = e.Y < 65 ? -1 : Math.Clamp((int)((e.X - 6) / ((Width - 12f) / 12)), 0, 11);
        if (month == hoveredMonth) return;
        hoveredMonth = month;
        tooltip.SetToolTip(this, month < 0 ? "" : new DateTime(2000, month + 1, 1).ToString("MMMM") + "\n" + string.Join("\n", names.Select((name, i) => $"{name}: RM {values[i][month]:N2}")));
    }
    public void SetValues(params decimal[][] series) { values = series; Invalidate(); }
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var max = Math.Max(1m, values.SelectMany(v => v).Max(v => Math.Abs(v)));
        for (var s = 0; s < 4; s++) { using var brush = new SolidBrush(colors[s]); e.Graphics.FillRectangle(brush, s % 2 * Width / 2 + 2, s / 2 * 22 + 5, 9, 9); TextRenderer.DrawText(e.Graphics, names[s], Font, new Point(s % 2 * Width / 2 + 15, s / 2 * 22 + 1), ForeColor); }
        var negative = values.SelectMany(v => v).Any(v => v < 0);
        var top = 65; var baseline = negative ? (Height - 22 + top) / 2 : Height - 32; var scale = Math.Max(1, baseline - top); var group = (Width - 12f) / 12;
        TextRenderer.DrawText(e.Graphics, $"Scale: RM {max:N0} · hover over a month for amounts", Font, new Point(2, 44), ForeColor);
        e.Graphics.DrawLine(Pens.LightGray, 2, baseline, Width - 2, baseline);
        for (var m = 0; m < 12; m++)
        {
            for (var s = 0; s < 4; s++) { var height = (float)(Math.Abs(values[s][m]) / max) * scale; using var brush = new SolidBrush(colors[s]); e.Graphics.FillRectangle(brush, 6 + m * group + s * group / 5, values[s][m] < 0 ? baseline : baseline - height, Math.Max(1, group / 5 - 1), height); }
            TextRenderer.DrawText(e.Graphics, new DateTime(2000, m + 1, 1).ToString("MMM"), Font, new Point((int)(4 + m * group), Height - 22), ForeColor);
        }
    }
}
