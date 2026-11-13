using HousingCommunityManagement.Core;

namespace HousingCommunityManagement;
public partial class CashTransactionForm : Form
{
    private CashTransactionService transactions = null!;
    private readonly List<ExpenseAttachment> files = [];
    public CashTransactionForm(CashTransactionService transactions, AccountingService accounting, string direction, CashTransaction? existing = null, CashTransaction? draft = null) : this()
    {
        this.transactions = transactions;
        Text = existing is null ? direction : $"{existing.Direction} #{existing.Id}";
        heading.Text = Text;
        var accounts = accounting.GetAccounts();
        var settings = accounting.GetSettings();
        cashAccount.DisplayMember = nameof(Account.DisplayName);
        cashAccount.ValueMember = nameof(Account.Id);
        cashAccount.DataSource = accounts.Where(a => a.IsCashBank && (a.IsActive || a.Id == existing?.CashAccountId)).ToList();
        cashAccount.SelectedValue = existing?.CashAccountId ?? (direction == "Money in" ? settings.ReceiptAccountId : settings.PaymentAccountId);
        var controls = CashTransactionService.LinkedControlAccounts(settings, accounting.GetJournal());
        allocationAccount.DisplayMember = nameof(Account.DisplayName);
        allocationAccount.ValueMember = nameof(Account.Id);
        allocationAccount.DataSource = accounts.Where(a => existing is not null || a.IsActive && !controls.Contains(a.Id)).ToList();
        date.MaxDate = DateTime.Today;
        date.MinDate = new DateTime(1900, 1, 1);
        date.Value = existing?.Date ?? DateTime.Today;
        var choices = new List<RelatedChoice>
        {
            new(null, "None")
        };
        choices.AddRange(transactions.GetAll().Where(t => t.Id != existing?.Id).Select(t => new RelatedChoice(t.Id, $"#{t.Id} / {t.Date:dd MMM yyyy} / {t.Direction} / {t.Total:N2} / {t.Description}")));
        related.DisplayMember = nameof(RelatedChoice.Label);
        related.ValueMember = nameof(RelatedChoice.Id);
        related.DataSource = choices;
        if (existing?.RelatedTransactionId is { } relatedId)
            related.SelectedItem = choices.FirstOrDefault(c => c.Id == relatedId);
        if (existing is not null)
        {
            counterparty.Text = existing.Counterparty;
            description.Text = existing.Description;
            reference.Text = existing.Reference;
            foreach (var line in existing.Allocations)
                lines.Rows.Add(line.AccountId, line.Description, line.Amount);
            files.AddRange(existing.Attachments);
            date.Enabled = cashAccount.Enabled = related.Enabled = false;
            counterparty.ReadOnly = description.ReadOnly = reference.ReadOnly = true;
            lines.ReadOnly = true;
            lines.AllowUserToAddRows = lines.AllowUserToDeleteRows = false;
            save.Visible = addFile.Visible = removeFile.Visible = false;
            close.Text = "Close";
            if (existing.ReversedDate is { } reversed)
                heading.Text += $" / Reversed {reversed:dd MMM yyyy}: {existing.ReversalReason}";
        }
        else if (draft is not null)
        {
            date.Value = draft.Date;
            cashAccount.SelectedValue = draft.CashAccountId;
            description.Text = draft.Description;
            reference.Text = draft.Reference;
            counterparty.Text = draft.Counterparty;
            foreach (var line in draft.Allocations)
                lines.Rows.Add(line.AccountId > 0 ? (object)line.AccountId : null !, line.Description, line.Amount);
        }
        else
            lines.Rows.Add();
        save.Tag = direction;
        RefreshFiles();
        UpdateTotal();
    }

    private sealed record RelatedChoice(int? Id, string Label);
    private void UpdateTotal()
    {
        var total = 0m;
        foreach (DataGridViewRow row in lines.Rows)
            if (!row.IsNewRow && decimal.TryParse(row.Cells[2].Value?.ToString(), out var value))
                total += value;
        totalLabel.Text = $"Total: RM {total:N2}";
    }

    private void TotalRowsRemoved(object? sender, DataGridViewRowsRemovedEventArgs e) => UpdateTotal();
    private void LineChanged(object? sender, DataGridViewCellEventArgs e) => UpdateTotal();
    private void RefreshFiles()
    {
        attachments.Items.Clear();
        foreach (var file in files)
            attachments.Items.Add(file.FileName);
    }

    private void AddFileClicked(object? sender, EventArgs e)
    {
        using var picker = new OpenFileDialog
        {
            Filter = "Receipts (PDF, images, text)|*.pdf;*.png;*.jpg;*.jpeg;*.txt",
            Multiselect = true
        };
        if (picker.ShowDialog(this) != DialogResult.OK)
            return;
        try
        {
            var added = picker.FileNames.Select(path => new FileInfo(path)).ToList();
            if (files.Count + added.Count > 10 || added.Any(f => f.Length > 10485760) || files.Sum(f => (long)f.Content.Length) + added.Sum(f => f.Length) > 30 * 1024 * 1024)
                throw new ArgumentException("Attach up to 10 files, at most 10 MB each and 30 MB total.");
            files.AddRange(added.Select(f => new ExpenseAttachment { FileName = f.Name, Content = File.ReadAllBytes(f.FullName) }));
            RefreshFiles();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Could not attach file");
        }
    }

    private void RemoveFileClicked(object? sender, EventArgs e)
    {
        if (attachments.SelectedIndex >= 0)
        {
            files.RemoveAt(attachments.SelectedIndex);
            RefreshFiles();
        }
    }

    private void ExportFileClicked(object? sender, EventArgs e)
    {
        if (attachments.SelectedIndex < 0)
            return;
        var file = files[attachments.SelectedIndex];
        using var picker = new SaveFileDialog
        {
            FileName = file.FileName
        };
        if (picker.ShowDialog(this) != DialogResult.OK)
            return;
        try
        {
            File.WriteAllBytes(picker.FileName, file.Content);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Could not save file");
        }
    }

    private void SaveClicked(object? sender, EventArgs e)
    {
        try
        {
            lines.EndEdit();
            if (cashAccount.SelectedItem is not Account cash)
                throw new ArgumentException("Select a cash/bank account.");
            var allocations = new List<CashAllocation>();
            foreach (DataGridViewRow row in lines.Rows)
            {
                if (row.IsNewRow)
                    continue;
                if (row.Cells[0].Value is not int id || !decimal.TryParse(row.Cells[2].Value?.ToString(), out var amount))
                    throw new ArgumentException("Select an account and enter an amount on every line. Remove unused rows.");
                allocations.Add(new CashAllocation { AccountId = id, Description = row.Cells[1].Value?.ToString() ?? "", Amount = amount });
            }

            var transaction = new CashTransaction
            {
                Date = date.Value.Date,
                Direction = (string)save.Tag!,
                CashAccountId = cash.Id,
                Counterparty = counterparty.Text,
                Description = description.Text,
                Reference = reference.Text,
                RelatedTransactionId = (related.SelectedItem as RelatedChoice)?.Id,
                Allocations = allocations,
                Attachments = files
            };
            CashTransactionService.Validate(transaction);
            if (MessageBox.Show(this, $"Record {transaction.Direction.ToLowerInvariant()} of RM {transaction.Total:N2}?\nDate: {transaction.Date:dd MMM yyyy}\nCash/bank: {cash.DisplayName}\n\nThis records money already moved outside the app. Posted records are locked; corrections use reversal.", "Record transaction", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                return;
            transactions.Record(transaction);
            DialogResult = DialogResult.OK;
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Could not record transaction", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    public CashTransactionForm()
    {
        InitializeComponent();
    }
}
