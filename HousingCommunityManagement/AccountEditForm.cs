using HousingCommunityManagement.Core;

namespace HousingCommunityManagement;
public partial class AccountEditForm : Form
{
    private AccountingService accounting = null!;
    private readonly int accountId;
    public int SavedAccountId { get; private set; }

    public AccountEditForm(AccountingService accounting, Account? account = null) : this()
    {
        this.accounting = accounting;
        accountId = account?.Id ?? 0;
        Text = account is null ? "New account" : "Edit account";
        code.Text = account?.Code ?? "";
        accountName.Text = account?.Name ?? "";
        type.SelectedItem = account?.Type ?? "Asset";
        cashBank.Checked = account?.IsCashBank ?? false;
        active.Checked = account?.IsActive ?? true;
        var used = account is not null && accounting.GetJournal().SelectMany(e => e.Lines).Any(l => l.AccountId == account.Id);
        type.Enabled = cashBank.Enabled = !used;
        usageNote.Visible = used;
    }

    private void SaveClicked(object? sender, EventArgs e)
    {
        try
        {
            var account = new Account
            {
                Id = accountId,
                Code = code.Text,
                Name = accountName.Text,
                Type = type.SelectedItem as string ?? "",
                IsCashBank = cashBank.Checked,
                IsActive = active.Checked
            };
            accounting.SaveAccount(account);
            SavedAccountId = account.Id;
            DialogResult = DialogResult.OK;
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Could not save account", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    public AccountEditForm()
    {
        InitializeComponent();
    }
}
