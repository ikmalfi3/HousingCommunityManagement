using HousingCommunityManagement.Core;

namespace HousingCommunityManagement;
public partial class AccountingForm : Form
{
    private readonly AccountingService? accounting;
    private List<Account> accounts = [];
    private List<JournalEntry> entries = [];
    private bool updatingFilters;
    private bool accountingStarted;
    public AccountingForm()
    {
        InitializeComponent();
    }

    public AccountingForm(AccountingService accounting) : this()
    {
        this.accounting = accounting;
        registersView.BindServices(accounting);
        adjustmentsView.BindServices(accounting);
        reconciliationsView.BindServices(accounting);
        locksView.BindServices(accounting);
    }

    private void AccountingShown(object? sender, EventArgs e) => Reload();
    private void AccountingTabChanged(object? sender, EventArgs e)
    {
        Reload();
        foreach (var control in accountingTabs.SelectedTab?.Controls.Cast<Control>() ?? [])
        {
            if (control is FinanceRegistersView registers)
                registers.RefreshData();
            if (control is AccountingAdjustmentsView adjustments)
                adjustments.RefreshData();
            if (control is BankReconciliationsView reconciliation)
                reconciliation.RefreshData();
            if (control is AccountingLocksView locks)
                locks.RefreshData();
        }
    }

    private void RefreshClicked(object? sender, EventArgs e) => Reload();
    private void Reload()
    {
        if (accounting is null)
            return;
        try
        {
            accounts = accounting.GetAccounts();
            entries = accounting.GetJournal();
            accountingStarted = accounting.GetSettings().StartedOn.HasValue;
            BindJournalFilters();
            ApplyJournalFilters();
            var totals = entries.SelectMany(e => e.Lines).GroupBy(l => l.AccountId).ToDictionary(g => g.Key, g => (Debit: g.Sum(l => l.Debit), Credit: g.Sum(l => l.Credit)));
            balanceGrid.DataSource = accounts.Select(a =>
            {
                var total = totals.GetValueOrDefault(a.Id);
                return new
                {
                    a.Code,
                    a.Name,
                    a.Type,
                    total.Debit,
                    total.Credit,
                    Balance = a.Type is "Asset" or "Expense" ? total.Debit - total.Credit : total.Credit - total.Debit
                };
            }).ToList();
            balancesNote.Text = "Balances include all posted dates. Assets and expenses: debit minus credit. Liabilities, equity and income: credit minus debit.";
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Could not load accounting", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BindJournalFilters()
    {
        var accountId = (accountFilter.SelectedItem as AccountChoice)?.Id ?? 0;
        var source = (sourceFilter.SelectedItem as SourceChoice)?.Code ?? "";
        updatingFilters = true;
        try
        {
            accountFilter.DisplayMember = nameof(AccountChoice.Label);
            accountFilter.ValueMember = nameof(AccountChoice.Id);
            accountFilter.DataSource = new[]
            {
                new AccountChoice(0, "All accounts")
            }.Concat(accounts.Select(a => new AccountChoice(a.Id, a.DisplayName + (a.IsActive ? "" : " (inactive)")))).ToList();
            accountFilter.SelectedValue = accounts.Any(a => a.Id == accountId) ? accountId : 0;
            sourceFilter.DisplayMember = nameof(SourceChoice.Label);
            sourceFilter.ValueMember = nameof(SourceChoice.Code);
            var sources = entries.Select(e => e.Source).Concat(source.Length == 0 ? [] : new[] { source }).Distinct().Select(code => new SourceChoice(code, SourceLabel(code))).OrderBy(s => s.Label).ToList();
            sourceFilter.DataSource = new[]
            {
                new SourceChoice("", "All transaction types")
            }.Concat(sources).ToList();
            sourceFilter.SelectedValue = source;
        }
        finally
        {
            updatingFilters = false;
        }
    }

    private void JournalDateFilterChanged(object? sender, EventArgs e)
    {
        journalFrom.Enabled = fromLabel.Checked;
        journalTo.Enabled = toLabel.Checked;
        JournalFilterChanged(sender, e);
    }

    private void JournalFilterChanged(object? sender, EventArgs e)
    {
        if (!updatingFilters && accounting is not null)
            ApplyJournalFilters();
    }

    private void ClearJournalFilters(object? sender, EventArgs e)
    {
        updatingFilters = true;
        try
        {
            fromLabel.Checked = false;
            toLabel.Checked = false;
            accountFilter.SelectedIndex = 0;
            sourceFilter.SelectedIndex = 0;
            journalSearch.Clear();
        }
        finally
        {
            updatingFilters = false;
        }

        if (accounting is not null)
            ApplyJournalFilters();
    }

    private void ApplyJournalFilters()
    {
        var names = accounts.ToDictionary(a => a.Id, a => a.DisplayName);
        var accountId = (accountFilter.SelectedItem as AccountChoice)?.Id ?? 0;
        var source = (sourceFilter.SelectedItem as SourceChoice)?.Code ?? "";
        var search = journalSearch.Text.Trim();
        var invalidDates = fromLabel.Checked && toLabel.Checked && journalFrom.Value.Date > journalTo.Value.Date;
        var rows = entries.Where(e => !invalidDates && (!fromLabel.Checked || e.Date.Date >= journalFrom.Value.Date) && (!toLabel.Checked || e.Date.Date <= journalTo.Value.Date) && (source.Length == 0 || e.Source == source)).SelectMany(e => e.Lines.Where(l => accountId == 0 || l.AccountId == accountId).Select(l => new JournalRow(e.Id, e.Date, e.Description, e.Source, e.SourceId, names.GetValueOrDefault(l.AccountId, "Unknown account"), l.Debit, l.Credit))).Where(r => search.Length == 0 || new[] { r.Description, r.Account, r.Source, SourceLabel(r.Source), r.Entry.ToString(), r.SourceId.ToString() }.Any(value => value.Contains(search, StringComparison.OrdinalIgnoreCase))).ToList();
        journalGrid.DataSource = rows;
        journalNote.Text = invalidDates ? "From date must be on or before To date. Clear filters to show all journal lines." : !accountingStarted ? "Accounting has not started. Configure accounts and opening balances in Settings > Accounting." : $"Showing {rows.Select(r => r.Entry).Distinct().Count():N0} of {entries.Count:N0} entries / {rows.Count:N0} lines. Debit: RM {rows.Sum(r => r.Debit):N2} / Credit: RM {rows.Sum(r => r.Credit):N2}." + Environment.NewLine + "Filters show matching lines only; filtered totals may differ. Account balances use the complete ledger.";
    }

    private static string SourceLabel(string source) => source switch
    {
        "MonthlyCharge" => "Maintenance charges",
        "MaintenancePayment" => "Maintenance payments",
        "RenovationCollection" => "Renovation deposit collections",
        "RenovationReturn" => "Renovation deposit returns",
        "OpeningAccount" => "Opening account balance",
        "OpeningRegister" => "Opening register item",
        "OpeningExpense" => "Opening supplier payable",
        "Adjustment" => "Noncash adjustment",
        "AdjustmentReversal" => "Adjustment reversal",
        "CashTransaction" => "Money in / out",
        "CashTransactionReversal" => "Money transaction reversal",
        "ExpenseBill" => "Expense bills / claims",
        "ExpensePayment" => "Expense payments",
        "ExpensePaymentReversal" => "Expense payment reversals",
        "ExpenseBillCancellation" => "Expense bill cancellations",
        "OpeningCash" => "Opening cash / bank balances",
        "OpeningCharge" => "Opening maintenance balances",
        "OpeningDeposit" => "Opening renovation deposits",
        _ => source
    };
    private sealed record AccountChoice(int Id, string Label);
    private sealed record SourceChoice(string Code, string Label);
    private sealed record JournalRow(int Entry, DateTime Date, string Description, string Source, int SourceId, string Account, decimal Debit, decimal Credit);
    private void DrawAccountingTab(object? sender, DrawItemEventArgs e)
    {
        var selected = e.Index == accountingTabs.SelectedIndex;
        var bounds = accountingTabs.GetTabRect(e.Index);
        using var background = new SolidBrush(selected ? Color.FromArgb(43, 65, 88) : Color.FromArgb(25, 39, 57));
        e.Graphics.FillRectangle(background, bounds);
        if (selected)
        {
            using var accent = new SolidBrush(Color.FromArgb(72, 184, 166));
            e.Graphics.FillRectangle(accent, bounds.Left, bounds.Bottom - 3, bounds.Width, 3);
        }

        TextRenderer.DrawText(e.Graphics, accountingTabs.TabPages[e.Index].Text, accountingTabs.Font, bounds, selected ? Color.White : Color.FromArgb(194, 207, 222), TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix);
        if (selected && accountingTabs.Focused)
            ControlPaint.DrawFocusRectangle(e.Graphics, Rectangle.Inflate(bounds, -5, -5), Color.White, background.Color);
    }
}
