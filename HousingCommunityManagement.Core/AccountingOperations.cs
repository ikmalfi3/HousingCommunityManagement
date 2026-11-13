namespace HousingCommunityManagement.Core;

public sealed class OpeningDetails
{
    public List<OpeningAccountBalance> Accounts { get; set; } = [];
    public List<RegisterOpening> Registers { get; set; } = [];
    public List<OpeningSupplierBill> SupplierBills { get; set; } = [];
}
public sealed class OpeningAccountBalance
{
    public int AccountId { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
}
public sealed class RegisterOpening
{
    public string Kind { get; set; } = "Asset";
    public string Name { get; set; } = "";
    public string Party { get; set; } = "";
    public string Location { get; set; } = "";
    public DateTime AcquisitionDate { get; set; }
    public int AccountId { get; set; }
    public int? DepreciationAccountId { get; set; }
    public decimal Cost { get; set; }
    public decimal AccumulatedDepreciation { get; set; }
}
public sealed class OpeningSupplierBill
{
    public int PayeeId { get; set; }
    public string Reference { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime DocumentDate { get; set; }
    public DateTime? DueDate { get; set; }
    public decimal Amount { get; set; }
}
public sealed class FinanceRegisterItem
{
    public int Id { get; set; }
    public string Kind { get; set; } = "Asset";
    public string Name { get; set; } = "";
    public string Party { get; set; } = "";
    public string Location { get; set; } = "";
    public string Notes { get; set; } = "";
    public DateTime AcquisitionDate { get; set; }
    public int AccountId { get; set; }
    public int? DepreciationAccountId { get; set; }
    public List<RegisterMovement> Movements { get; set; } = [];
    public decimal Cost => Movements.Sum(m => m.CostChange);
    public decimal AccumulatedDepreciation => Movements.Sum(m => m.DepreciationChange);
    public decimal Balance => Cost - AccumulatedDepreciation;
    public string Status => Kind == "Asset" ? Cost == 0 ? "Disposed / cleared" : Balance == 0 ? "Fully depreciated" : "In use" : Balance == 0 ? "Cleared" : "Outstanding";
}
public sealed class RegisterMovement
{
    public int Id { get; set; }
    public int RegisterItemId { get; set; }
    public DateTime Date { get; set; }
    public string Action { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal CostChange { get; set; }
    public decimal DepreciationChange { get; set; }
    public int? CashTransactionId { get; set; }
    public int? AdjustmentId { get; set; }
}
public sealed class AccountingAdjustment
{
    public int Id { get; set; }
    public int Version { get; set; }
    public DateTime Date { get; set; } = DateTime.Today;
    public string Description { get; set; } = "";
    public string Reference { get; set; } = "";
    public List<JournalLine> Lines { get; set; } = [];
    public List<ExpenseAttachment> Attachments { get; set; } = [];
    public bool IsPosted { get; set; }
    public DateTime? ReversedDate { get; set; }
    public string ReversalReason { get; set; } = "";
    public string Action { get; set; } = "Manual";
    public int? RegisterItemId { get; set; }
    public int? BillId { get; set; }
}
public sealed class RegisterActionRequest
{
    public int RegisterItemId { get; set; }
    public string Action { get; set; } = "Depreciate";
    public DateTime Date { get; set; } = DateTime.Today;
    public decimal Amount { get; set; }
    public int CounterAccountId { get; set; }
    public int? BillId { get; set; }
    public string Reason { get; set; } = "";
}
public sealed class BankStatementLine
{
    public int Id { get; set; }
    public int ReconciliationId { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; } = "";
    public string Reference { get; set; } = "";
    public decimal Amount { get; set; }
    public string ImportKey { get; set; } = "";
}
public sealed class BankMatch
{
    public int Id { get; set; }
    public int ReconciliationId { get; set; }
    public List<int> StatementLineIds { get; set; } = [];
    public List<int> JournalLineIds { get; set; } = [];
}
public sealed class BankReconciliation
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal ClosingBalance { get; set; }
    public bool IsFinalized { get; set; }
    public List<string> History { get; set; } = [];
    public List<BankStatementLine> StatementLines { get; set; } = [];
    public List<BankMatch> Matches { get; set; } = [];
}
public sealed record BankBookLine(int Id, DateTime Date, string Description, decimal Amount, bool Matched);
public sealed class BankReconciliationReview
{
    public BankReconciliation Statement { get; set; } = new();
    public List<BankBookLine> BookLines { get; set; } = [];
    public decimal BookOpening { get; set; }
    public decimal BookClosing { get; set; }
    public decimal Outstanding => BookLines.Where(l => !l.Matched).Sum(l => l.Amount);
    public decimal Difference => BookClosing - (Statement.ClosingBalance + Outstanding);
    public decimal StatementDifference => Statement.OpeningBalance + Statement.StatementLines.Sum(l => l.Amount) - Statement.ClosingBalance;
    public int UnmatchedStatementCount => Statement.StatementLines.Count(l => !Statement.Matches.Any(m => m.StatementLineIds.Contains(l.Id)));
}
public sealed class AccountingAuditEvent
{
    public int Id { get; set; }
    public DateTime RecordedAt { get; set; }
    public string Description { get; set; } = "";
}
public interface IAccountingOperationsRepository
{
    List<JournalLine> PreviewOpening(Dictionary<int, decimal> cash, DateTime date, OpeningDetails details);
    void AddOpeningSchedules(OpeningDetails details, string reason);
    void CorrectOpeningBank(int accountId, decimal change, string reason);
    List<FinanceRegisterItem> GetFinanceRegisters();
    int AddFinanceRegister(FinanceRegisterItem item, int cashTransactionId, decimal amount, decimal openingDepreciation = 0);
    void UpdateRegisterDetails(int id, string name, string party, string location, string notes, int? depreciationAccountId);
    void LinkRegisterCash(int registerId, int cashTransactionId, decimal amount);
    List<AccountingAdjustment> GetAdjustments();
    int SaveAdjustment(AccountingAdjustment adjustment);
    void PostAdjustment(int id);
    void ReverseAdjustment(int id, DateTime date, string reason);
    int RecordRegisterAction(RegisterActionRequest request);
    List<BankReconciliation> GetReconciliations();
    int SaveReconciliation(BankReconciliation statement);
    int ImportStatementLines(int id, List<BankStatementLine> lines);
    void RemoveStatementLines(int id, int[] lineIds);
    void DeleteReconciliationDraft(int id, string reason);
    BankReconciliationReview ReviewReconciliation(int id);
    void MatchBankLines(int id, int[] statementIds, int[] journalLineIds);
    void UnmatchBankLines(int id, int matchId);
    void FinalizeReconciliation(int id);
    void ReopenReconciliation(int id, string reason);
    void SetAccountingLock(DateTime? through, string reason);
    List<AccountingAuditEvent> GetAccountingAudit();
}
public sealed class AccountingOperationsService(IAccountingOperationsRepository repository)
{
    public List<JournalLine> PreviewOpening(Dictionary<int, decimal> cash, DateTime date, OpeningDetails details) => repository.PreviewOpening(cash, date.Date, details);
    public void AddOpeningSchedules(OpeningDetails details, string reason) => repository.AddOpeningSchedules(details, reason);
    public void CorrectOpeningBank(int accountId, decimal change, string reason) => repository.CorrectOpeningBank(accountId, change, reason);
    public List<FinanceRegisterItem> Registers() => repository.GetFinanceRegisters();
    public int AddRegister(FinanceRegisterItem item, int transactionId, decimal amount, decimal openingDepreciation = 0) => repository.AddFinanceRegister(item, transactionId, amount, openingDepreciation);
    public void UpdateRegister(int id, string name, string party, string location, string notes, int? depreciationAccountId) => repository.UpdateRegisterDetails(id, name, party, location, notes, depreciationAccountId);
    public void LinkCash(int registerId, int transactionId, decimal amount) => repository.LinkRegisterCash(registerId, transactionId, amount);
    public List<AccountingAdjustment> Adjustments() => repository.GetAdjustments();
    public int SaveAdjustment(AccountingAdjustment adjustment) => repository.SaveAdjustment(adjustment);
    public void PostAdjustment(int id) => repository.PostAdjustment(id);
    public void ReverseAdjustment(int id, DateTime date, string reason) => repository.ReverseAdjustment(id, date.Date, reason);
    public int RegisterAction(RegisterActionRequest request) => repository.RecordRegisterAction(request);
    public List<BankReconciliation> Reconciliations() => repository.GetReconciliations();
    public int SaveReconciliation(BankReconciliation statement) => repository.SaveReconciliation(statement);
    public int Import(int id, List<BankStatementLine> lines) => repository.ImportStatementLines(id, lines);
    public void RemoveStatementLines(int id, int[] lineIds) => repository.RemoveStatementLines(id, lineIds);
    public void DeleteReconciliationDraft(int id, string reason) => repository.DeleteReconciliationDraft(id, reason);
    public BankReconciliationReview Review(int id) => repository.ReviewReconciliation(id);
    public void Match(int id, int[] statementIds, int[] bookIds) => repository.MatchBankLines(id, statementIds, bookIds);
    public void Unmatch(int id, int matchId) => repository.UnmatchBankLines(id, matchId);
    public void Finalize(int id) => repository.FinalizeReconciliation(id);
    public void Reopen(int id, string reason) => repository.ReopenReconciliation(id, reason);
    public void SetLock(DateTime? through, string reason) => repository.SetAccountingLock(through, reason);
    public List<AccountingAuditEvent> Audit() => repository.GetAccountingAudit();
}
