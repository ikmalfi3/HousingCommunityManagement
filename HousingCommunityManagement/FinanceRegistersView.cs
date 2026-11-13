using HousingCommunityManagement.Core;

namespace HousingCommunityManagement;
public partial class FinanceRegistersView : UserControl
{
    private AccountingService accounting = null !;
    private AccountingOperationsService operations = null !;
    private List<FinanceRegisterItem> items = [];
    public FinanceRegistersView(AccountingService accounting) : this()
    {
        BindServices(accounting);
    }

    private FinanceRegisterItem? Selected => grid.CurrentRow?.DataBoundItem is RegisterRow row ? items.SingleOrDefault(r => r.Id == row.Number) : null;

    private sealed record RegisterRow(int Number, string Kind, string Name, string Supplier, string Location, string Acquired, string Account, decimal Cost, decimal Depreciation, decimal Remaining, string Status);
    public void RefreshData() => Reload();
    private void Reload()
    {
        if (accounting is null || DesignMode) return;
        try
        {
            items = operations.Registers();
            var accounts = accounting.GetAccounts().ToDictionary(a => a.Id, a => a.DisplayName);
            var id = Selected?.Id;
            grid.DataSource = items.Where(r => r.AcquisitionDate.Date <= asOf.Value.Date && (filter.SelectedIndex <= 0 || r.Kind == filter.Text)).Select(r =>
            {
                var cost = r.Movements.Where(m => m.Date.Date <= asOf.Value.Date).Sum(m => m.CostChange);
                var depreciation = r.Movements.Where(m => m.Date.Date <= asOf.Value.Date).Sum(m => m.DepreciationChange);
                return new RegisterRow(r.Id, r.Kind, r.Name, r.Party, r.Location, r.AcquisitionDate.ToString("dd MMM yyyy"), accounts.GetValueOrDefault(r.AccountId) ?? "", cost, depreciation, cost - depreciation, r.Kind == "Asset" ? cost == 0 ? "Disposed / cleared" : cost == depreciation ? "Fully depreciated" : "In use" : cost == 0 ? "Cleared" : "Outstanding");
            }).ToList();
            foreach (DataGridViewRow row in grid.Rows)
                if ((row.DataBoundItem as RegisterRow)?.Number == id)
                {
                    grid.CurrentCell = row.Cells[0];
                    break;
                }

            footer.Text = "Register amounts follow the selected as-of date. Purchase/refund links do not post money twice. Use Balance check to find ledger amounts not yet assigned to individual items.";
            LoadHistory();
        }
        catch (Exception ex)
        {
            AccountingUi.Error(this, ex);
        }
    }

    private void RefreshClicked(object? sender, EventArgs e) => Reload();
    private void SelectedChanged(object? sender, EventArgs e) => LoadHistory();
    private void LoadHistory()
    {
        history.DataSource = (Selected?.Movements ?? []).OrderBy(m => m.Date).Select(m => new { m.Id, Date = m.Date.ToString("dd MMM yyyy"), m.Action, m.Description, m.CostChange, m.DepreciationChange, MoneyEntry = m.CashTransactionId, Adjustment = m.AdjustmentId }).ToList();
        link.Enabled = act.Enabled = Selected is not null;
    }

    private sealed record SourceChoice(int Id, string Label);
    private void NewClicked(object? sender, EventArgs e)
    {
        try
        {
            using var dialog = new RegisterItemForm();
            var kind = dialog.KindInput;
            AccountingUi.Bind(kind, new[] { "Asset", "Supplier deposit", "Prepayment" });
            var name = dialog.NameInput;
            var party = dialog.PartyInput;
            var location = dialog.LocationInput;
            var acquired = dialog.AcquiredInput;
            var blocked = CashTransactionService.LinkedControlAccounts(accounting.GetSettings(), accounting.GetJournal());
            var settings = accounting.GetSettings();
            var accounts = accounting.GetAccounts().Where(a => a.IsActive && a.Type == "Asset" && !a.IsCashBank && !blocked.Contains(a.Id)).ToList();
            var account = dialog.AccountInput;
            AccountingUi.Bind(account, accounts, nameof(Account.DisplayName), nameof(Account.Id));
            var depChoices = new[]
            {
                new SourceChoice(0, "None")
            }.Concat(accounts.Select(a => new SourceChoice(a.Id, a.DisplayName)));
            var depreciation = dialog.DepreciationInput;
            AccountingUi.Bind(depreciation, depChoices, nameof(SourceChoice.Label), nameof(SourceChoice.Id));
            var sources = new[]
            {
                new SourceChoice(0, "Adopt an existing opening balance (no new journal)")
            }.Concat((accounting.Expenses?.Transactions?.GetAll() ?? []).Where(t => !t.ReversedDate.HasValue && t.Direction == "Money out" && t.Allocations.Any(l => accounts.Any(a => a.Id == l.AccountId))).Select(t => new SourceChoice(t.Id, $"#{t.Id} / {t.Date:dd MMM yyyy} / {t.Counterparty} / {t.Total:N2}")));
            var source = dialog.SourceInput;
            AccountingUi.Bind(source, sources, nameof(SourceChoice.Label), nameof(SourceChoice.Id));
            var amount = dialog.AmountInput;
            var initialDep = dialog.InitialDepInput;
            var notes = dialog.NotesInput;
            void SelectDefaults()
            {
                var linked = (source.SelectedItem as SourceChoice)?.Id > 0;
                if (!linked) account.SelectedValue = settings.RegisterAccount(kind.Text);
                depreciation.Enabled = kind.Text == "Asset";
                depreciation.SelectedValue = kind.Text == "Asset" ? settings.AccumulatedDepreciationAccountId : 0;
                initialDep.Enabled = kind.Text == "Asset" && !linked;
                if (!initialDep.Enabled) initialDep.Value = 0;
                notes.PlaceholderText = !linked && account.SelectedItem is null
                    ? "Set an active default account in Accounting settings > Default accounts." : "";
            }
            kind.SelectedIndexChanged += (_, _) => SelectDefaults();
            SelectDefaults();
            source.SelectedIndexChanged += (_, _) =>
            {
                SelectDefaults();
                if ((source.SelectedItem as SourceChoice)?.Id is not > 0) return;
                var transaction = accounting.Expenses!.Transactions!.GetAll().Single(t => t.Id == ((SourceChoice)source.SelectedItem).Id);
                acquired.Value = transaction.Date;
                if (party.Text.Length == 0)
                    party.Text = transaction.Counterparty;
                if (name.Text.Length == 0)
                    name.Text = transaction.Description;
                var allocation = transaction.Allocations.FirstOrDefault(l => accounts.Any(a => a.Id == l.AccountId));
                if (allocation is not null)
                {
                    account.SelectedValue = allocation.AccountId;
                    var used = operations.Registers().Where(r => r.AccountId == allocation.AccountId).SelectMany(r => r.Movements).Where(m => m.CashTransactionId == transaction.Id).Sum(m => Math.Abs(m.CostChange));
                    amount.Value = Math.Max(0, transaction.Allocations.Where(l => l.AccountId == allocation.AccountId).Sum(l => l.Amount) - used);
                }
            };
            if (dialog.Run(this, () => operations.AddRegister(new FinanceRegisterItem { Kind = kind.Text, Name = name.Text, Party = party.Text, Location = location.Text, AcquisitionDate = acquired.Value.Date, AccountId = ((Account? )account.SelectedItem)?.Id ?? 0, DepreciationAccountId = ((SourceChoice? )depreciation.SelectedItem)?.Id is> 0 and var dep ? dep : null, Notes = notes.Text }, ((SourceChoice? )source.SelectedItem)?.Id ?? 0, amount.Value, initialDep.Value)))
                Reload();
        }
        catch (Exception ex)
        {
            AccountingUi.Error(this, ex);
        }
    }

    private void EditDetailsClicked(object? sender, EventArgs e)
    {
        if (Selected is not { } item)
            return;
        using var dialog = new RegisterDetailsForm();
        var name = dialog.NameInput;
        name.Text = item.Name;
        var party = dialog.PartyInput;
        party.Text = item.Party;
        var location = dialog.LocationInput;
        location.Text = item.Location;
        var notes = dialog.NotesInput;
        notes.Text = item.Notes;
        var choices = new[]
        {
            new SourceChoice(0, "None")
        }.Concat(accounting.GetAccounts().Where(a => a.Type == "Asset" && !a.IsCashBank && (a.IsActive || a.Id == item.DepreciationAccountId)).Select(a => new SourceChoice(a.Id, a.DisplayName)));
        var depreciation = dialog.DepreciationInput;
        AccountingUi.Bind(depreciation, choices, nameof(SourceChoice.Label), nameof(SourceChoice.Id));
        depreciation.SelectedValue = item.DepreciationAccountId ?? 0;
        depreciation.Enabled = item.Kind == "Asset" && !item.Movements.Any(m => m.DepreciationChange != 0);
        if (dialog.Run(this, () => operations.UpdateRegister(item.Id, name.Text, party.Text, location.Text, notes.Text, (depreciation.SelectedItem as SourceChoice)?.Id is> 0 and var value ? value : null)))
            Reload();
    }

    private void LinkClicked(object? sender, EventArgs e)
    {
        if (Selected is not { } item)
            return;
        using var dialog = new RegisterCashLinkForm();
        var choices = RegisterCashLinks.Available(item, operations.Registers(), accounting.Expenses?.Transactions?.GetAll() ?? []);
        var source = dialog.SourceInput;
        AccountingUi.Bind(source, choices, nameof(RegisterCashLinkOption.Label), nameof(RegisterCashLinkOption.Id));
        var amount = dialog.AmountInput;
        void UpdateAmount()
        {
            var choice = source.SelectedItem as RegisterCashLinkOption;
            amount.Value = 0;
            amount.Maximum = choice is null ? 0 : choice.Direction == "Money in" ? Math.Min(choice.Available, item.Balance) : choice.Available;
            amount.Value = amount.Maximum;
            dialog.SetAvailable(choice is not null);
        }
        source.SelectedIndexChanged += (_, _) => UpdateAmount();
        UpdateAmount();
        if (dialog.Run(this, () => operations.LinkCash(item.Id, ((RegisterCashLinkOption?)source.SelectedItem)?.Id ?? 0, amount.Value)))
            Reload();
    }

    private void ActionClicked(object? sender, EventArgs e)
    {
        if (Selected is not { } item)
            return;
        using var dialog = new RegisterActionForm
        {
            Text = $"Register action / {item.Name}"};
        var actions = item.Kind == "Asset" ? new[]
        {
            "Depreciate",
            "Dispose / write off asset",
            "Opening cost correction",
            "Opening depreciation correction"
        }

        : new[]
        {
            "Apply to bill",
            "Recognize expense",
            "Write off",
            "Opening cost correction"
        };
        var action = dialog.ActionInput;
        AccountingUi.Bind(action, actions);
        var date = dialog.DateInput;
        var amount = dialog.AmountInput;
        var expense = dialog.ExpenseInput;
        AccountingUi.Bind(expense, accounting.GetAccounts().Where(a => a.IsActive && a.Type == "Expense"), nameof(Account.DisplayName), nameof(Account.Id));
        var bills = (accounting.Expenses?.GetAll() ?? []).Where(b => b.IsPosted && b.CancelledDate is null && b.Balance > 0).Select(b => new SourceChoice(b.Id, $"#{b.Id} / {b.PayeeName} / {b.Reference} / owing {b.Balance:N2}"));
        var bill = dialog.BillInput;
        AccountingUi.Bind(bill, bills, nameof(SourceChoice.Label), nameof(SourceChoice.Id));
        var reason = dialog.ReasonInput;
        void ConfigureAction()
        {
            var apply = action.Text == "Apply to bill";
            var correction = action.Text.StartsWith("Opening ");
            if (action.Text == "Depreciate" || action.Text == "Write off" || action.Text == "Dispose / write off asset")
                expense.SelectedValue = accounting.GetSettings().RegisterExpenseAccount(action.Text);
            else expense.SelectedIndex = -1;
            expense.Enabled = !apply && !correction;
            bill.Enabled = apply;
            amount.Value = action.Text == "Dispose / write off asset" ? item.Balance : apply && bill.SelectedItem is SourceChoice choice ? Math.Min(item.Balance, accounting.Expenses!.Get(choice.Id).Balance) : 0;
            date.Value = correction ? accounting.GetSettings().StartedOn!.Value : DateTime.Today;
        }

        action.SelectedIndexChanged += (_, _) => ConfigureAction();
        bill.SelectedIndexChanged += (_, _) =>
        {
            if (action.Text == "Apply to bill")
                ConfigureAction();
        };
        ConfigureAction();
        if (dialog.Run(this, () => operations.RegisterAction(new RegisterActionRequest { RegisterItemId = item.Id, Action = action.Text, Date = date.Value.Date, Amount = amount.Value, CounterAccountId = (expense.SelectedItem as Account)?.Id ?? 0, BillId = (bill.SelectedItem as SourceChoice)?.Id, Reason = reason.Text })))
            Reload();
    }

    private void CheckClicked(object? sender, EventArgs e)
    {
        var accounts = accounting.GetAccounts().ToDictionary(a => a.Id, a => a.DisplayName);
        var journal = accounting.GetJournal().Where(e => e.Date.Date <= asOf.Value.Date).SelectMany(e => e.Lines).ToList();
        var accountIds = items.SelectMany(r => new[] { (int? )r.AccountId, r.DepreciationAccountId }).Where(i => i.HasValue).Select(i => i!.Value).Distinct();
        AccountingUi.ShowRows(this, "Register / ledger balance check", accountIds.Select(id =>
        {
            var registered = items.Sum(r => (r.AccountId == id ? r.Movements.Where(m => m.Date <= asOf.Value.Date).Sum(m => m.CostChange) : 0) - (r.DepreciationAccountId == id ? r.Movements.Where(m => m.Date <= asOf.Value.Date).Sum(m => m.DepreciationChange) : 0));
            var ledger = journal.Where(l => l.AccountId == id).Sum(l => l.Debit - l.Credit);
            return new
            {
                Account = accounts[id],
                RegisteredNetDebit = registered,
                LedgerNetDebit = ledger,
                UnassignedDifference = ledger - registered
            };
        }).ToList());
    }

    private void BankOpeningClicked(object? sender, EventArgs e)
    {
        using var dialog = new OpeningBankCorrectionForm();
        var account = dialog.AccountInput;
        AccountingUi.Bind(account, accounting.GetAccounts().Where(a => a.IsCashBank && a.IsActive), nameof(Account.DisplayName), nameof(Account.Id));
        var amount = dialog.AmountInput;
        var reason = dialog.ReasonInput;
        if (dialog.Run(this, () => operations.CorrectOpeningBank((account.SelectedItem as Account)?.Id ?? 0, amount.Value, reason.Text)))
            Reload();
    }

    private void OpeningClicked(object? sender, EventArgs e)
    {
        if (accounting.GetSettings().StartedOn is null)
        {
            MessageBox.Show(this, "Use Start accounting in Settings for initial opening balances.");
            return;
        }

        using var details = new OpeningDetailsForm(accounting, new OpeningDetails());
        if (details.ShowDialog(this) != DialogResult.OK)
            return;
        using var reason = new ReasonEntryForm("Supplement opening schedules");
        var text = reason.ReasonInput;
        if (reason.Run(this, () => operations.AddOpeningSchedules(details.Details, text.Text)))
            Reload();
    }

    public void BindServices(AccountingService accounting)
    {
        this.accounting = accounting;
        operations = accounting.Operations!;
        asOf.MaxDate = DateTime.Today;
        asOf.Value = DateTime.Today;
        Reload();
    }

    public FinanceRegistersView()
    {
        InitializeComponent();
    }
}
