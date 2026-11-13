using HousingCommunityManagement.Core;

namespace HousingCommunityManagement;
public partial class AccountingAdjustmentsView : UserControl
{
    private AccountingService accounting = null !;
    private List<AccountingAdjustment> records = [];
    public AccountingAdjustmentsView(AccountingService accounting) : this()
    {
        BindServices(accounting);
    }

    public void RefreshData() => Reload();
    private AccountingAdjustment? Selected => grid.CurrentRow?.DataBoundItem is AdjustmentRow row ? records.SingleOrDefault(a => a.Id == row.Number) : null;

    private sealed record AdjustmentRow(int Number, string Date, string Action, string Description, string Reference, string Status, decimal Debit, decimal Credit, int? Register, int? Bill, string Reversed, string Reason);
    private void Reload()
    {
        if (accounting is null || DesignMode) return;
        try
        {
            records = accounting.Operations!.Adjustments();
            grid.DataSource = records.Select(a => new AdjustmentRow(a.Id, a.Date.ToString("dd MMM yyyy"), a.Action, a.Description, a.Reference, a.ReversedDate.HasValue ? "Reversed" : a.IsPosted ? "Posted" : "Draft", a.Lines.Sum(l => l.Debit), a.Lines.Sum(l => l.Credit), a.RegisterItemId, a.BillId, a.ReversedDate?.ToString("dd MMM yyyy") ?? "", a.ReversalReason)).ToList();
        }
        catch (Exception ex)
        {
            AccountingUi.Error(this, ex);
        }
    }

    private void NewClicked(object? sender, EventArgs e)
    {
        using var dialog = new AdjustmentEditForm(accounting);
        if (dialog.ShowDialog(this) == DialogResult.OK)
            Reload();
    }

    private void ViewClicked(object? sender, EventArgs e)
    {
        if (Selected is not { } a)
            return;
        using var dialog = new AdjustmentEditForm(accounting, a);
        if (dialog.ShowDialog(this) == DialogResult.OK)
            Reload();
    }

    private void PostClicked(object? sender, EventArgs e)
    {
        if (Selected is not { IsPosted: false } a)
            return;
        if (MessageBox.Show(this, $"Post adjustment #{a.Id} dated {a.Date:dd MMM yyyy}? Posted details are locked.", "Post adjustment", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
            return;
        try
        {
            accounting.Operations!.PostAdjustment(a.Id);
            Reload();
        }
        catch (Exception ex)
        {
            AccountingUi.Error(this, ex);
        }
    }

    private void ReverseClicked(object? sender, EventArgs e)
    {
        if (Selected is not { IsPosted: true, ReversedDate: null } a)
            return;
        using var dialog = new AdjustmentReversalForm();
        var date = dialog.DateInput;
        var reason = dialog.ReasonInput;
        if (dialog.Run(this, () => accounting.Operations!.ReverseAdjustment(a.Id, date.Value.Date, reason.Text)))
            Reload();
    }

    private void RefreshClicked(object? sender, EventArgs e) => Reload();
    public void BindServices(AccountingService accounting)
    {
        this.accounting = accounting;
        Reload();
    }

    public AccountingAdjustmentsView()
    {
        InitializeComponent();
    }
}
