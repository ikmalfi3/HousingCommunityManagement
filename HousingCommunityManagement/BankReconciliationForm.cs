using HousingCommunityManagement.Core;

namespace HousingCommunityManagement;
public partial class BankReconciliationForm : Form
{
    private AccountingService accounting = null!;
    private readonly int reconciliationId;
    private BankReconciliationReview review = new();
    public BankReconciliationForm(AccountingService accounting, int id) : this()
    {
        this.accounting = accounting;
        reconciliationId = id;
        Reload();
    }

    private void Reload()
    {
        if (accounting is null || DesignMode) return;
        try
        {
            review = accounting.Operations!.Review(reconciliationId);
            var statement = review.Statement;
            bank.Rows.Clear();
            book.Rows.Clear();
            foreach (var line in statement.StatementLines)
            {
                var match = statement.Matches.FirstOrDefault(m => m.StatementLineIds.Contains(line.Id));
                var i = bank.Rows.Add(false, line.Id, line.Date.ToString("dd MMM yyyy"), line.Description, line.Amount, (object? )match?.Id ?? "");
                bank.Rows[i].Tag = line.Id;
            }

            foreach (var line in review.BookLines)
            {
                var match = statement.Matches.FirstOrDefault(m => m.JournalLineIds.Contains(line.Id));
                var i = book.Rows.Add(false, line.Id, line.Date.ToString("dd MMM yyyy"), line.Description, line.Amount, (object? )match?.Id ?? "");
                book.Rows[i].Tag = line.Id;
            }

            var name = accounting.GetAccounts().Single(a => a.Id == statement.AccountId).DisplayName;
            Text = $"Reconciliation #{statement.Id} / {name} / {statement.From:dd MMM yyyy} - {statement.To:dd MMM yyyy}";
            summary.Text = $"{(statement.IsFinalized ? "FINALIZED" : "In progress")} | Statement opening {statement.OpeningBalance:N2}, closing {statement.ClosingBalance:N2} | Book closing {review.BookClosing:N2}\nUncleared ledger net {review.Outstanding:N2} | Reconciliation difference {review.Difference:N2} | Statement arithmetic difference {review.StatementDifference:N2} | Unmatched statement lines {review.UnmatchedStatementCount}";
            summary.ForeColor = review.Difference != 0 || review.StatementDifference != 0 ? Color.Firebrick : SystemColors.ControlText;
            addLine.Enabled = removeLines.Enabled = balancesButton.Enabled = import.Enabled = matchButton.Enabled = unmatch.Enabled = createMoney.Enabled = finalize.Enabled = !statement.IsFinalized;
            reopen.Enabled = statement.IsFinalized;
        }
        catch (Exception ex)
        {
            AccountingUi.Error(this, ex);
        }
    }

    private int[] Checked(DataGridView grid)
    {
        grid.EndEdit();
        return grid.Rows.Cast<DataGridViewRow>().Where(r => r.Cells[0].Value is true).Select(r => (int)r.Tag!).ToArray();
    }

    private void ImportClicked(object? sender, EventArgs e)
    {
        using var file = new OpenFileDialog
        {
            Filter = "CSV statement|*.csv;*.txt"
        };
        if (file.ShowDialog(this) != DialogResult.OK)
            return;
        try
        {
            if (new FileInfo(file.FileName).Length > 20 * 1024 * 1024)
                throw new ArgumentException("CSV must be at most 20 MB.");
            using var dialog = new BankCsvImportForm(File.ReadAllText(file.FileName));
            if (dialog.ShowDialog(this) != DialogResult.OK)
                return;
            var added = accounting.Operations!.Import(reconciliationId, dialog.Lines);
            MessageBox.Show(this, $"Imported {added} new lines. Existing duplicates were skipped.");
            Reload();
        }
        catch (Exception ex)
        {
            AccountingUi.Error(this, ex);
        }
    }

    private void AddLineClicked(object? sender, EventArgs e)
    {
        using var dialog = new StatementLineForm();
        var date = dialog.DateInput; date.Value = review.Statement.From;
        var description = dialog.DescriptionInput;
        var reference = dialog.ReferenceInput;
        var amount = dialog.AmountInput;
        if (dialog.Run(this, () => accounting.Operations!.Import(reconciliationId, [new() { Date = date.Value.Date, Description = description.Text, Reference = reference.Text, Amount = amount.Value }])))
            Reload();
    }

    private void RemoveLinesClicked(object? sender, EventArgs e)
    {
        if (MessageBox.Show(this, "Remove the checked unmatched statement lines? The removal remains in statement history.", "Remove statement lines", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
            return;
        try
        {
            accounting.Operations!.RemoveStatementLines(reconciliationId, Checked(bank));
            Reload();
        }
        catch (Exception ex)
        {
            AccountingUi.Error(this, ex);
        }
    }

    private void MatchClicked(object? sender, EventArgs e)
    {
        try
        {
            accounting.Operations!.Match(reconciliationId, Checked(bank), Checked(book));
            Reload();
        }
        catch (Exception ex)
        {
            AccountingUi.Error(this, ex);
        }
    }

    private void UnmatchClicked(object? sender, EventArgs e)
    {
        try
        {
            var bankIds = Checked(bank).ToHashSet();
            var bookIds = Checked(book).ToHashSet();
            var groups = review.Statement.Matches.Where(m => m.StatementLineIds.Any(bankIds.Contains) || m.JournalLineIds.Any(bookIds.Contains)).Select(m => m.Id).ToList();
            if (groups.Count == 0)
                throw new ArgumentException("Check a matched line on either side to remove its match group.");
            foreach (var id in groups)
                accounting.Operations!.Unmatch(reconciliationId, id);
            Reload();
        }
        catch (Exception ex)
        {
            AccountingUi.Error(this, ex);
        }
    }

    private void CreateMoneyClicked(object? sender, EventArgs e)
    {
        if (bank.CurrentRow?.Tag is not int id)
            return;
        var line = review.Statement.StatementLines.Single(l => l.Id == id);
        if (review.Statement.Matches.Any(m => m.StatementLineIds.Contains(id)))
        {
            MessageBox.Show(this, "This statement line is already matched.");
            return;
        }

        using var dialog = new CashTransactionForm(accounting.Expenses!.Transactions!, accounting, line.Amount > 0 ? "Money in" : "Money out", draft: new CashTransaction { Date = line.Date, CashAccountId = review.Statement.AccountId, Description = line.Description, Reference = line.Reference, Allocations = [new() { Amount = Math.Abs(line.Amount) }] });
        if (dialog.ShowDialog(this) == DialogResult.OK)
            Reload();
    }

    private void FinalizeClicked(object? sender, EventArgs e)
    {
        if (MessageBox.Show(this, "Finalize this statement? Its matches will be protected until explicitly reopened.", "Finalize reconciliation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
            return;
        try
        {
            accounting.Operations!.Finalize(reconciliationId);
            Reload();
        }
        catch (Exception ex)
        {
            AccountingUi.Error(this, ex);
        }
    }

    private void ReopenClicked(object? sender, EventArgs e)
    {
        using var dialog = new ReasonEntryForm("Reopen reconciliation");
        var reason = dialog.ReasonInput;
        if (dialog.Run(this, () => accounting.Operations!.Reopen(reconciliationId, reason.Text)))
            Reload();
    }

    private void BalancesClicked(object? sender, EventArgs e)
    {
        using var dialog = new StatementBalancesForm();
        var opening = dialog.OpeningInput; opening.Value = review.Statement.OpeningBalance;
        var closing = dialog.ClosingInput; closing.Value = review.Statement.ClosingBalance;
        if (dialog.Run(this, () => accounting.Operations!.SaveReconciliation(new BankReconciliation { Id = reconciliationId, AccountId = review.Statement.AccountId, From = review.Statement.From, To = review.Statement.To, OpeningBalance = opening.Value, ClosingBalance = closing.Value })))
            Reload();
    }

    private void HistoryClicked(object? sender, EventArgs e) => AccountingUi.ShowRows(this, "Reconciliation history (UTC timestamps)", review.Statement.History.Select(h => new { Event = h }).ToList());
    private void RefreshClicked(object? sender, EventArgs e) => Reload();
    public BankReconciliationForm()
    {
        InitializeComponent();
    }
}
