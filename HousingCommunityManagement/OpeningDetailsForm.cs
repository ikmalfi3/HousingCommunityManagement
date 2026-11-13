using HousingCommunityManagement.Core;

namespace HousingCommunityManagement;
public partial class OpeningDetailsForm : Form
{
    private AccountingService accounting = null!;
    private AccountingSettings? defaultAccounts;
    public OpeningDetails Details { get; private set; } = new();

    public OpeningDetailsForm(AccountingService accounting, OpeningDetails details) : this()
    {
        this.accounting = accounting;
        var accounts = accounting.GetAccounts().Where(a => a.IsActive && !a.IsCashBank).ToList();
        var settings = accounting.GetSettings();
        var controls = new[]
        {
            settings.ReceivableAccountId,
            settings.DepositAccountId,
            settings.PayableAccountId,
            settings.OpeningEquityAccountId
        };
        AccountingUi.BindColumn(openingAccountColumn, accounts.Where(a => !controls.Contains(a.Id) && a.Type is "Asset" or "Liability" or "Equity"), nameof(Account.DisplayName), nameof(Account.Id));
        AccountingUi.BindColumn(registerAccountColumn, accounts.Where(a => a.Type == "Asset" && !controls.Contains(a.Id)), nameof(Account.DisplayName), nameof(Account.Id));
        AccountingUi.BindColumn(registerDepAccountColumn, accounts.Where(a => a.Type == "Asset" && !controls.Contains(a.Id)), nameof(Account.DisplayName), nameof(Account.Id));
        AccountingUi.BindColumn(billPayeeColumn, accounting.Expenses?.GetPayees().Where(p => p.IsActive) ?? [], nameof(Payee.Name), nameof(Payee.Id));
        foreach (var a in details.Accounts)
            accountGrid.Rows.Add(a.AccountId, a.Debit, a.Credit);
        foreach (var r in details.Registers)
            registerGrid.Rows.Add(r.Kind, r.Name, r.Party, r.Location, r.AcquisitionDate.ToString("dd/MM/yyyy"), r.AccountId, r.Cost, r.AccumulatedDepreciation, (object? )r.DepreciationAccountId ?? null !);
        foreach (var b in details.SupplierBills)
            billGrid.Rows.Add(b.PayeeId, b.Reference, b.Description, b.DocumentDate.ToString("dd/MM/yyyy"), b.DueDate?.ToString("dd/MM/yyyy") ?? "", b.Amount);
        defaultAccounts = settings;
    }

    private void RegisterCellDirty(object? sender, EventArgs e)
    {
        if (registerGrid.IsCurrentCellDirty && registerGrid.CurrentCell?.ColumnIndex == 0)
            registerGrid.CommitEdit(DataGridViewDataErrorContexts.Commit);
    }
    private void RegisterKindChanged(object? sender, DataGridViewCellEventArgs e)
    {
        if (defaultAccounts is null || e.RowIndex < 0 || e.ColumnIndex != 0) return;
        var row = registerGrid.Rows[e.RowIndex];
        var kind = row.Cells[0].Value?.ToString() ?? "";
        var id = defaultAccounts.RegisterAccount(kind);
        var eligible = (registerAccountColumn.DataSource as IEnumerable<Account>)?.Any(a => a.Id == id) == true;
        row.Cells[5].Value = eligible ? id : null;
        row.Cells[8].Value = kind == "Asset" && (registerDepAccountColumn.DataSource as IEnumerable<Account>)?.Any(a => a.Id == defaultAccounts.AccumulatedDepreciationAccountId) == true ? defaultAccounts.AccumulatedDepreciationAccountId : null;
        if (kind != "Asset") row.Cells[7].Value = 0m;
    }

    private void SaveClicked(object? sender, EventArgs e)
    {
        try
        {
            accountGrid.EndEdit();
            registerGrid.EndEdit();
            billGrid.EndEdit();
            var details = new OpeningDetails();
            foreach (DataGridViewRow row in accountGrid.Rows)
                if (!row.IsNewRow)
                    details.Accounts.Add(new() { AccountId = AccountingUi.Id(row.Cells[0].Value, "an account"), Debit = AccountingUi.Amount(row.Cells[1].Value, "debit", true), Credit = AccountingUi.Amount(row.Cells[2].Value, "credit", true) });
            foreach (DataGridViewRow row in registerGrid.Rows)
                if (!row.IsNewRow)
                    details.Registers.Add(new() { Kind = row.Cells[0].Value?.ToString() ?? "", Name = row.Cells[1].Value?.ToString() ?? "", Party = row.Cells[2].Value?.ToString() ?? "", Location = row.Cells[3].Value?.ToString() ?? "", AcquisitionDate = AccountingUi.Date(row.Cells[4].Value, "acquisition date"), AccountId = AccountingUi.Id(row.Cells[5].Value, "an asset account"), Cost = AccountingUi.Amount(row.Cells[6].Value, "cost"), AccumulatedDepreciation = AccountingUi.Amount(row.Cells[7].Value, "depreciation", true), DepreciationAccountId = row.Cells[8].Value as int? });
            foreach (DataGridViewRow row in billGrid.Rows)
                if (!row.IsNewRow)
                    details.SupplierBills.Add(new() { PayeeId = AccountingUi.Id(row.Cells[0].Value, "a supplier"), Reference = row.Cells[1].Value?.ToString() ?? "", Description = row.Cells[2].Value?.ToString() ?? "", DocumentDate = AccountingUi.Date(row.Cells[3].Value, "document date"), DueDate = string.IsNullOrWhiteSpace(row.Cells[4].Value?.ToString()) ? null : AccountingUi.Date(row.Cells[4].Value, "due date"), Amount = AccountingUi.Amount(row.Cells[5].Value, "outstanding amount") });
            Details = details;
            DialogResult = DialogResult.OK;
        }
        catch (Exception ex)
        {
            AccountingUi.Error(this, ex);
        }
    }

    public OpeningDetailsForm()
    {
        InitializeComponent();
    }
}
