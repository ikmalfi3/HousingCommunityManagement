using System.Security.Cryptography;
using System.Text;
using HousingCommunityManagement.Core;
using Microsoft.EntityFrameworkCore;
namespace HousingCommunityManagement.Data;
public sealed partial class HouseholdRepository
{
    private static IQueryable<BankReconciliation> Statements(HouseholdDbContext db) => db.BankReconciliations.Include(r => r.StatementLines).Include(r => r.Matches);
    public List<BankReconciliation> GetReconciliations() { using var db = new HouseholdDbContext(databasePath); return Statements(db).AsNoTracking().OrderByDescending(r => r.To).ToList(); }
    public int SaveReconciliation(BankReconciliation statement)
    {
        CashTransactionService.ValidateDate(statement.From); CashTransactionService.ValidateDate(statement.To); Money(statement.OpeningBalance, true, true); Money(statement.ClosingBalance, true, true);
        if (statement.From.Date > statement.To.Date || statement.IsFinalized || statement.StatementLines.Count > 0 || statement.Matches.Count > 0) throw new ArgumentException("Choose a valid statement period and enter opening/closing balances. Import and match lines after saving.");
        using var db = new HouseholdDbContext(databasePath); using var tx = db.Database.BeginTransaction();
        var setup = db.AccountingSetup.Single();
        if (!setup.StartedOn.HasValue || statement.From.Date < setup.StartedOn.Value.Date) throw new ArgumentException("The statement must start on or after accounting opening.");
        RequireAccount(db, statement.AccountId, "Asset", true);
        if (db.BankReconciliations.Any(r => r.Id != statement.Id && r.AccountId == statement.AccountId && r.From <= statement.To.Date && r.To >= statement.From.Date)) throw new ArgumentException("A statement already covers this period. Open it instead.");
        var previous = db.BankReconciliations.Where(r => r.AccountId == statement.AccountId && r.Id != statement.Id).OrderByDescending(r => r.To).FirstOrDefault();
        if (statement.Id == 0 && previous is not null && (!previous.IsFinalized || statement.From.Date != previous.To.Date.AddDays(1) || statement.OpeningBalance != previous.ClosingBalance)) throw new ArgumentException("Finalize the previous statement first. The next statement starts the following day and uses its closing balance as opening.");
        BankReconciliation saved;
        if (statement.Id == 0) { saved = new BankReconciliation(); db.BankReconciliations.Add(saved); }
        else
        {
            saved = Statements(db).SingleOrDefault(r => r.Id == statement.Id) ?? throw new ArgumentException("The statement no longer exists.");
            if (saved.IsFinalized) throw new ArgumentException("Reopen the statement before editing balances.");
            if (saved.StatementLines.Any(l => l.Date.Date > statement.To.Date)) throw new ArgumentException("The period must include all imported statement dates.");
            if (saved.AccountId != statement.AccountId || saved.From.Date != statement.From.Date) throw new ArgumentException("The account and start date are fixed after creation.");
            var earlier = db.BankReconciliations.Where(r => r.AccountId == saved.AccountId && r.To < saved.From).OrderByDescending(r => r.To).FirstOrDefault();
            if (earlier is not null && statement.OpeningBalance != earlier.ClosingBalance) throw new ArgumentException("Opening must equal the previous statement closing balance.");
        }
        saved.AccountId = statement.AccountId; saved.From = statement.From.Date; saved.To = statement.To.Date; saved.OpeningBalance = statement.OpeningBalance; saved.ClosingBalance = statement.ClosingBalance;
        saved.History.Add($"{DateTime.UtcNow:O} Statement details saved"); db.SaveChanges(); tx.Commit(); return saved.Id;
    }
    public int ImportStatementLines(int id, List<BankStatementLine> lines)
    {
        if (lines.Count is 0 or > 20000) throw new ArgumentException("Import 1-20,000 statement lines.");
        using var db = new HouseholdDbContext(databasePath); using var tx = db.Database.BeginTransaction(); var r = Statements(db).Single(x => x.Id == id);
        if (r.IsFinalized) throw new ArgumentException("Reopen the statement before importing.");
        var occurrences = new Dictionary<string, int>(); var count = 0;
        foreach (var line in lines)
        {
            Money(line.Amount, true); CashTransactionService.ValidateDate(line.Date);
            if (line.Date.Date < r.From.Date || line.Date.Date > r.To.Date || line.Description.Length > 500 || line.Reference.Length > 150) throw new ArgumentException("Every statement line must fall in the statement period, with description/reference within normal limits.");
            var raw = $"{line.Date:yyyy-MM-dd}|{line.Amount:0.00}|{line.Description.Trim()}|{line.Reference.Trim()}";
            occurrences[raw] = occurrences.GetValueOrDefault(raw) + 1;
            var key = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(raw + "|" + occurrences[raw])));
            if (r.StatementLines.Any(l => l.ImportKey == key)) continue;
            r.StatementLines.Add(new BankStatementLine { Date = line.Date.Date, Amount = line.Amount, Description = line.Description.Trim(), Reference = line.Reference.Trim(), ImportKey = key }); count++;
        }
        r.History.Add($"{DateTime.UtcNow:O} Imported {count} statement lines; duplicates skipped"); db.SaveChanges(); tx.Commit(); return count;
    }
    public void RemoveStatementLines(int id, int[] lineIds)
    {
        using var db = new HouseholdDbContext(databasePath); using var tx = db.Database.BeginTransaction(); var r = Statements(db).Single(r => r.Id == id);
        if (r.IsFinalized || lineIds.Length == 0 || lineIds.Distinct().Count() != lineIds.Length) throw new ArgumentException("Select unmatched bank lines in an open statement.");
        var lines = r.StatementLines.Where(l => lineIds.Contains(l.Id)).ToList();
        if (lines.Count != lineIds.Length || r.Matches.Any(m => m.StatementLineIds.Any(lineIds.Contains))) throw new ArgumentException("Unmatch these bank lines before removing them.");
        foreach (var line in lines) r.History.Add($"{DateTime.UtcNow:O} Removed statement line #{line.Id}: {line.Date:yyyy-MM-dd}, {line.Amount:N2}, {line.Description}, {line.Reference}");
        db.BankStatementLines.RemoveRange(lines); db.SaveChanges(); tx.Commit();
    }
    public void DeleteReconciliationDraft(int id, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason) || reason.Length > 500) throw new ArgumentException("Enter a deletion reason up to 500 characters.");
        using var db = new HouseholdDbContext(databasePath); using var tx = db.Database.BeginTransaction(); var r = Statements(db).Single(r => r.Id == id);
        if (r.IsFinalized || r.History.Any(h => h.Contains(" Finalized;")) || db.BankReconciliations.Any(x => x.AccountId == r.AccountId && x.To > r.To)) throw new ArgumentException("Only a latest statement that has never been finalized can be deleted. Reviewed history must be preserved.");
        Audit(db, $"Deleted unfinished reconciliation #{id}, {r.From:yyyy-MM-dd} to {r.To:yyyy-MM-dd}, opening {r.OpeningBalance:N2}, closing {r.ClosingBalance:N2}: {reason.Trim()}");
        db.BankMatches.RemoveRange(r.Matches); db.BankStatementLines.RemoveRange(r.StatementLines); db.BankReconciliations.Remove(r); db.SaveChanges(); tx.Commit();
    }
    public BankReconciliationReview ReviewReconciliation(int id) { using var db = new HouseholdDbContext(databasePath); return ReviewBank(db, id); }
    private static BankReconciliationReview ReviewBank(HouseholdDbContext db, int id)
    {
        var r = Statements(db).Single(x => x.Id == id);
        var previous = db.BankReconciliations.Where(x => x.AccountId == r.AccountId && x.To < r.From).OrderByDescending(x => x.To).ToList();
        var priorIds = previous.Where(x => x.IsFinalized).Select(x => x.Id).ToList();
        var cleared = db.BankMatches.Where(m => priorIds.Contains(m.ReconciliationId)).AsEnumerable().SelectMany(m => m.JournalLineIds).ToHashSet();
        var matched = r.Matches.SelectMany(m => m.JournalLineIds).ToHashSet();
        var earliest = previous.Select(x => x.From).Append(r.From).Min();
        var ledger = db.JournalEntries.Include(e => e.Lines).Where(e => e.Date <= r.To).ToList();
        var openingCorrections = db.AccountingAdjustments.Where(a => a.Action == "Opening bank correction").Select(a => a.Id).ToHashSet();
        var accountingOpening = db.AccountingSetup.Single().StartedOn;
        bool OpeningEntry(JournalEntry e, DateTime boundary) => e.Date == boundary && boundary == accountingOpening && (e.Source == "OpeningCash" || (e.Source == "Adjustment" || e.Source == "AdjustmentReversal") && openingCorrections.Contains(e.SourceId));
        var opening = ledger.Where(e => e.Date < r.From || OpeningEntry(e, r.From)).SelectMany(e => e.Lines).Where(l => l.AccountId == r.AccountId).Sum(l => l.Debit - l.Credit);
        var book = ledger.Where(e => e.Date >= earliest && !OpeningEntry(e, earliest))
            .SelectMany(e => e.Lines.Where(l => l.AccountId == r.AccountId).Select(l => new BankBookLine(l.Id, e.Date, e.Description, l.Debit - l.Credit, matched.Contains(l.Id))))
            .Where(l => !cleared.Contains(l.Id)).OrderBy(l => l.Date).ThenBy(l => l.Id).ToList();
        return new BankReconciliationReview { Statement = r, BookLines = book, BookOpening = opening, BookClosing = ledger.SelectMany(e => e.Lines).Where(l => l.AccountId == r.AccountId).Sum(l => l.Debit - l.Credit) };
    }
    public void MatchBankLines(int id, int[] statementIds, int[] journalLineIds)
    {
        if (statementIds.Length == 0 || journalLineIds.Length == 0 || statementIds.Distinct().Count() != statementIds.Length || journalLineIds.Distinct().Count() != journalLineIds.Length) throw new ArgumentException("Select one or more distinct lines on each side.");
        using var db = new HouseholdDbContext(databasePath); using var tx = db.Database.BeginTransaction(); var review = ReviewBank(db, id); var r = review.Statement;
        if (r.IsFinalized) throw new ArgumentException("Reopen this statement before changing matches.");
        var bank = r.StatementLines.Where(l => statementIds.Contains(l.Id)).ToList(); var book = review.BookLines.Where(l => journalLineIds.Contains(l.Id) && !l.Matched).ToList();
        if (bank.Count != statementIds.Length || book.Count != journalLineIds.Length || r.Matches.Any(m => m.StatementLineIds.Any(statementIds.Contains))) throw new ArgumentException("A selected line is already matched or is not available in this statement.");
        if (bank.Sum(l => l.Amount) != book.Sum(l => l.Amount)) throw new ArgumentException("Selected statement and ledger amounts must agree exactly.");
        r.Matches.Add(new BankMatch { StatementLineIds = statementIds.ToList(), JournalLineIds = journalLineIds.ToList() });
        r.History.Add($"{DateTime.UtcNow:O} Matched {bank.Count} statement / {book.Count} ledger lines, RM {bank.Sum(l => l.Amount):N2}"); db.SaveChanges(); tx.Commit();
    }
    public void UnmatchBankLines(int id, int matchId)
    {
        using var db = new HouseholdDbContext(databasePath); using var tx = db.Database.BeginTransaction(); var r = Statements(db).Single(r => r.Id == id);
        if (r.IsFinalized) throw new ArgumentException("Reopen before changing matches.");
        var match = r.Matches.SingleOrDefault(m => m.Id == matchId) ?? throw new ArgumentException("Select an existing match."); db.BankMatches.Remove(match);
        r.History.Add($"{DateTime.UtcNow:O} Removed match #{matchId}"); db.SaveChanges(); tx.Commit();
    }
    public void FinalizeReconciliation(int id)
    {
        using var db = new HouseholdDbContext(databasePath); using var tx = db.Database.BeginTransaction(); var review = ReviewBank(db, id); var r = review.Statement;
        if (r.IsFinalized || review.StatementDifference != 0 || review.Difference != 0 || review.UnmatchedStatementCount != 0) throw new ArgumentException("Match all statement lines and resolve both statement and reconciliation differences before finalizing.");
        var earlier = db.BankReconciliations.Where(x => x.AccountId == r.AccountId && x.To < r.From).OrderByDescending(x => x.To).FirstOrDefault();
        if (earlier is null && review.BookOpening != r.OpeningBalance) throw new ArgumentException("The first statement opening balance must equal the ledger opening balance. Start with an earlier statement or correct the opening records.");
        if (earlier is not null && (!earlier.IsFinalized || earlier.ClosingBalance != r.OpeningBalance || earlier.To.AddDays(1) != r.From)) throw new ArgumentException("The previous statement must be finalized and continue into this statement with matching balances.");
        if (r.Matches.Any(m => m.JournalLineIds.Any(i => !review.BookLines.Any(l => l.Id == i)) || m.StatementLineIds.Any(i => !r.StatementLines.Any(l => l.Id == i)))) throw new ArgumentException("A match is no longer available after earlier statement changes. Unmatch it and review again.");
        r.IsFinalized = true; r.History.Add($"{DateTime.UtcNow:O} Finalized; book closing {review.BookClosing:N2}, outstanding {review.Outstanding:N2}"); Audit(db, $"Bank reconciliation #{id} finalized through {r.To:dd MMM yyyy}"); db.SaveChanges(); tx.Commit();
    }
    public void ReopenReconciliation(int id, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason) || reason.Length > 500) throw new ArgumentException("Enter a reason up to 500 characters.");
        using var db = new HouseholdDbContext(databasePath); using var tx = db.Database.BeginTransaction(); var r = Statements(db).Single(x => x.Id == id);
        if (!r.IsFinalized || db.BankReconciliations.Any(x => x.AccountId == r.AccountId && x.To > r.To && x.IsFinalized)) throw new ArgumentException("Only the latest finalized statement for an account can be reopened. Remove/review later statements first.");
        r.IsFinalized = false; r.History.Add($"{DateTime.UtcNow:O} Reopened: {reason.Trim()}"); Audit(db, $"Bank reconciliation #{id} reopened: {reason.Trim()}"); db.SaveChanges(); tx.Commit();
    }
}
