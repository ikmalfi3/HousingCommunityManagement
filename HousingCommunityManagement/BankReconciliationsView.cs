using HousingCommunityManagement.Core;

namespace HousingCommunityManagement;
public partial class BankReconciliationsView : UserControl
{
    private AccountingService accounting = null!;
    private List<BankReconciliation> records = [];
    public BankReconciliationsView(AccountingService accounting) : this()
    { BindServices(accounting); }

    private BankReconciliation? Selected => grid.CurrentRow?.DataBoundItem is StatementRow row ? records.SingleOrDefault(r => r.Id == row.Number) : null;

    private sealed record StatementRow(int Number, string Account, string From, string To, decimal Opening, decimal Closing, string Status);
    public void RefreshData() => Reload();
    private void Reload()
    {
        if (accounting is null || DesignMode) return;
        try
        {
            var accounts = accounting.GetAccounts().ToDictionary(a => a.Id, a => a.DisplayName);
            records = accounting.Operations!.Reconciliations();
            grid.DataSource = records.Select(r => new StatementRow(r.Id, accounts.GetValueOrDefault(r.AccountId) ?? "", r.From.ToString("dd MMM yyyy"), r.To.ToString("dd MMM yyyy"), r.OpeningBalance, r.ClosingBalance, r.IsFinalized ? "Finalized" : "In progress")).ToList();
        }
        catch (Exception ex)
        {
            AccountingUi.Error(this, ex);
        }
    }

    private void NewClicked(object? sender, EventArgs e)
    {
        using var dialog = new StatementDetailsForm();
        var account = dialog.AccountInput; AccountingUi.Bind(account, accounting.GetAccounts().Where(a => a.IsCashBank && a.IsActive), nameof(Account.DisplayName), nameof(Account.Id));
        var from = dialog.FromInput; from.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        var to = dialog.ToInput;
        var opening = dialog.OpeningInput;
        var closing = dialog.ClosingInput;
        var id = 0;
        if (dialog.Run(this, () => id = accounting.Operations!.SaveReconciliation(new BankReconciliation { AccountId = (account.SelectedItem as Account)?.Id ?? 0, From = from.Value.Date, To = to.Value.Date, OpeningBalance = opening.Value, ClosingBalance = closing.Value })))
        {
            Reload();
            using var detail = new BankReconciliationForm(accounting, id);
            detail.ShowDialog(this);
            Reload();
        }
    }

    private void DeleteClicked(object? sender, EventArgs e)
    {
        if (Selected is not { } r)
            return;
        using var dialog = new ReasonEntryForm("Delete unfinished statement");
        var reason = dialog.ReasonInput;
        if (dialog.Run(this, () => accounting.Operations!.DeleteReconciliationDraft(r.Id, reason.Text)))
            Reload();
    }

    private void OpenClicked(object? sender, EventArgs e)
    {
        if (Selected is not { } r)
            return;
        using var detail = new BankReconciliationForm(accounting, r.Id);
        detail.ShowDialog(this);
        Reload();
    }

    private void RefreshClicked(object? sender, EventArgs e) => Reload();
    public void BindServices(AccountingService accounting) { this.accounting = accounting; Reload(); }
    
    public BankReconciliationsView()
    {
        InitializeComponent();
    }
}
