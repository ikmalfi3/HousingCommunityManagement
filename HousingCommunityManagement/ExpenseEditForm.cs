using HousingCommunityManagement.Core;
namespace HousingCommunityManagement;
public partial class ExpenseEditForm : Form
{
    private readonly ExpenseService? expenses;
    private readonly ExpenseBill? existing;
    private List<ExpenseCategory> categories = [];
    private readonly List<ExpenseAttachment> files = [];
    public ExpenseEditForm() { InitializeComponent(); }
    public ExpenseEditForm(ExpenseService expenses, ExpenseBill? existing = null) : this()
    {
        this.expenses = expenses; this.existing = existing;
        kind.SelectedItem = existing?.Kind ?? "Supplier bill";
        BindChoices(existing?.PayeeId);
        if (existing is not null)
        {
            reference.Text = existing.Reference; description.Text = existing.Description;
            documentDate.Value = existing.DocumentDate; accountingDate.Value = existing.AccountingDate;
            dueDate.Value = existing.DueDate ?? DateTime.Today; dueDate.Checked = existing.DueDate.HasValue;
            foreach (var l in existing.Lines) lines.Rows.Add(l.CategoryId, l.Description, l.Amount);
            files.AddRange(existing.Attachments); RefreshFiles();
        }
        else
        {
            var row = lines.Rows[lines.Rows.Add()]; row.Cells[1].Value = ""; row.Cells[2].Value = 0m;
        }
        documentDate.MaxDate = accountingDate.MaxDate = paymentDate.MaxDate = DateTime.Today;
        documentDate.MinDate = accountingDate.MinDate = paymentDate.MinDate = new DateTime(1900, 1, 1);
    }
    private void BindChoices(int? selectedPayee = null)
    {
        if (expenses is null) return;
        var previous = selectedPayee ?? (payee.SelectedItem as Payee)?.Id;
        payee.DisplayMember = nameof(Payee.Name); payee.ValueMember = nameof(Payee.Id);
        payee.DataSource = expenses.GetPayees().Where(p => p.IsActive && (kind.Text == "Direct expense" || p.Kind == (kind.Text == "Reimbursement claim" ? "Claimant" : "Supplier"))).ToList();
        if (previous.HasValue) payee.SelectedValue = previous.Value;
        categories = expenses.GetCategories();
        categoryColumn.DisplayMember = nameof(ExpenseCategory.Name); categoryColumn.ValueMember = nameof(ExpenseCategory.Id);
        categoryColumn.DataSource = categories.Select(c => new { c.Id, Name = c.IsActive ? c.Name : c.Name + " (inactive)" }).ToList();
        if (expenses.Accounting is { } accounting)
        {
            var selectedCash = (cash.SelectedItem as Account)?.Id ?? accounting.GetSettings().PaymentAccountId;
            cash.DisplayMember = nameof(Account.DisplayName); cash.ValueMember = nameof(Account.Id); cash.DataSource = accounting.GetAccounts().Where(a => a.IsCashBank && a.IsActive).ToList(); cash.SelectedValue = selectedCash;
        }
    }
    private void KindChanged(object? sender, EventArgs e) => BindChoices();
    private void LineDefaults(object? sender, DataGridViewRowEventArgs e) { e.Row.Cells[2].Value = 0m; }
    private void SaveDraftClicked(object? sender, EventArgs e) => Save(false);
    private void SaveAndPayClicked(object? sender, EventArgs e) => Save(true);
    private void Save(bool payNow)
    {
        if (expenses is null) return;
        try
        {
            lines.EndEdit();
            if (payee.SelectedItem is not Payee p) throw new ArgumentException("Select a supplier or claimant. Use Payees & categories to add one first.");
            var bill = new ExpenseBill { Id = existing?.Id ?? 0, Version = existing?.Version ?? 0, Kind = kind.Text, PayeeId = p.Id, Description = description.Text, Reference = reference.Text, DocumentDate = documentDate.Value, AccountingDate = accountingDate.Value, DueDate = dueDate.Checked ? dueDate.Value : null, Attachments = files.ToList() };
            foreach (DataGridViewRow row in lines.Rows)
            {
                if (row.IsNewRow) continue;
                if (row.Cells[0].Value is not int category || !decimal.TryParse(row.Cells[2].Value?.ToString(), out var amount)) throw new ArgumentException("Choose a category and enter an amount on every expense line.");
                bill.Lines.Add(new ExpenseLine { CategoryId = category, Description = row.Cells[1].Value?.ToString() ?? "", Amount = amount });
            }
            int? accountId = null;
            if (payNow)
            {
                if (cash.SelectedItem is not Account a) throw new ArgumentException("Select the cash or bank account used to pay.");
                accountId = a.Id;
                if (MessageBox.Show(this, $"Post this expense and record RM {bill.Total:N2} paid to {p.Name}?\nPaid from: {a.DisplayName}\nPayment date: {paymentDate.Value:dd MMM yyyy}\n\nThis records a payment made outside the app.", "Post and record payment", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK) return;
            }
            expenses.Save(bill, payNow, accountId, paymentDate.Value, paymentReference.Text); DialogResult = DialogResult.OK;
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "Could not save expense"); }
    }
    private void AddAttachmentClicked(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog { Filter = "Receipts (PDF, images, text)|*.pdf;*.png;*.jpg;*.jpeg;*.txt", Multiselect = true };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        try
        {
            var additions = new List<ExpenseAttachment>();
            foreach (var path in dialog.FileNames)
            {
                var info = new FileInfo(path);
                if (info.Length is 0 or > 10485760) throw new ArgumentException("Each receipt must contain 1 byte to 10 MB.");
                additions.Add(new ExpenseAttachment { FileName = Path.GetFileName(path), Content = File.ReadAllBytes(path) });
            }
            if (files.Count + additions.Count > 10 || files.Concat(additions).Sum(a => (long)a.Content.Length) > 30 * 1024 * 1024) throw new ArgumentException("Attach up to 10 files, no more than 30 MB total.");
            files.AddRange(additions); RefreshFiles();
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "Could not attach receipt"); }
    }
    private void RemoveAttachmentClicked(object? sender, EventArgs e) { if (attachments.SelectedIndex >= 0) { files.RemoveAt(attachments.SelectedIndex); RefreshFiles(); } }
    private void RefreshFiles() { attachments.Items.Clear(); foreach (var a in files) attachments.Items.Add($"{a.FileName} ({a.Content.Length / 1024.0:N1} KB)"); }
    private void SetupClicked(object? sender, EventArgs e) { if (expenses is null) return; using var dialog = new ExpenseSetupForm(expenses); dialog.ShowDialog(this); BindChoices(); }
}
