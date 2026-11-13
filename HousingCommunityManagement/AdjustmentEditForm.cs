using HousingCommunityManagement.Core;

namespace HousingCommunityManagement;
public partial class AdjustmentEditForm : Form
{
    private AccountingService accounting = null!;
    private readonly AccountingAdjustment? existing;
    public AdjustmentEditForm(AccountingService accounting, AccountingAdjustment? existing = null) : this()
    {
        this.accounting = accounting;
        this.existing = existing;
        var settings = accounting.GetSettings();
        var blocked = CashTransactionService.LinkedControlAccounts(settings, accounting.GetJournal());
        foreach (var item in accounting.Operations!.Registers())
        {
            blocked.Add(item.AccountId);
            if (item.DepreciationAccountId.HasValue)
                blocked.Add(item.DepreciationAccountId.Value);
        }

        var accounts = accounting.GetAccounts().Where(a => existing?.IsPosted == true || a.IsActive && !a.IsCashBank && !blocked.Contains(a.Id));
        allocationAccount.DisplayMember = nameof(Account.DisplayName); allocationAccount.ValueMember = nameof(Account.Id); allocationAccount.DataSource = accounts.ToList();
        if (existing is not null)
        {
            date.Value = existing.Date;
            description.Text = existing.Description;
            reference.Text = existing.Reference;
            foreach (var line in existing.Lines)
                lines.Rows.Add(line.AccountId, line.Debit, line.Credit);
        }

        attachments.BindFiles(existing?.Attachments ?? [], existing?.IsPosted == true);
        if (existing?.IsPosted == true)
        {
            lines.ReadOnly = true;
            lines.AllowUserToAddRows = lines.AllowUserToDeleteRows = false;
            date.Enabled = false;
            description.ReadOnly = reference.ReadOnly = true;
            save.Visible = false;
        }

        lines.CellValueChanged += (_, _) => UpdateTotals();
        lines.RowsRemoved += (_, _) => UpdateTotals();
        UpdateTotals();
    }

    private void UpdateTotals()
    {
        decimal debit = 0, credit = 0;
        foreach (DataGridViewRow row in lines.Rows)
            if (!row.IsNewRow)
            {
                if (decimal.TryParse(row.Cells[1].Value?.ToString(), out var d))
                    debit += d;
                if (decimal.TryParse(row.Cells[2].Value?.ToString(), out var c))
                    credit += c;
            }

        totals.Text = $"Debit RM {debit:N2} / Credit RM {credit:N2} / Difference RM {debit - credit:N2}";
    }

    private void SaveClicked(object? sender, EventArgs e)
    {
        try
        {
            lines.EndEdit();
            var values = new List<JournalLine>();
            foreach (DataGridViewRow row in lines.Rows)
                if (!row.IsNewRow)
                    values.Add(new() { AccountId = AccountingUi.Id(row.Cells[0].Value, "an account"), Debit = AccountingUi.Amount(row.Cells[1].Value, "debit", true), Credit = AccountingUi.Amount(row.Cells[2].Value, "credit", true) });
            accounting.Operations!.SaveAdjustment(new AccountingAdjustment { Id = existing?.Id ?? 0, Version = existing?.Version ?? 0, Date = date.Value.Date, Description = description.Text, Reference = reference.Text, Lines = values, Attachments = attachments.Files });
            DialogResult = DialogResult.OK;
        }
        catch (Exception ex)
        {
            AccountingUi.Error(this, ex);
        }
    }

    public AdjustmentEditForm()
    {
        InitializeComponent();
    }
}
