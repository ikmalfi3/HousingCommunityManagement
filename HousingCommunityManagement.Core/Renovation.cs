namespace HousingCommunityManagement.Core;

public sealed class Renovation
{
    public int Id { get; set; }
    public bool IsArchived { get; set; }
    public int? DepositReceivedAccountId { get; set; }
    public int? DepositReturnedAccountId { get; set; }
    public int HouseholdId { get; set; }
    public string Description { get; set; } = "";
    public DateTime StartDate { get; set; }
    public int DurationDays { get; set; }
    public decimal DepositAmount { get; set; }
    public DateTime? DepositCollectedDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public DateTime? DepositReturnedDate { get; set; }
    public DateTime PlannedEndDate => StartDate.AddDays(DurationDays - 1);
    public string StatusOn(DateTime today) => IsArchived ? "Archived" : CompletedDate.HasValue ? "Completed" : today.Date < StartDate.Date ? "Planned" : today.Date > PlannedEndDate.Date ? "Overdue" : "In progress";
    public string DepositStatus => DepositAmount == 0 ? "No deposit" : DepositReturnedDate.HasValue ? "Returned" : !DepositCollectedDate.HasValue ? "Awaiting collection" : CompletedDate.HasValue || IsArchived ? "Awaiting return" : "Held";
}

public interface IRenovationRepository
{
    List<Renovation> GetRenovations(bool includeArchived = false);
    void AddRenovation(Renovation renovation);
    void DeleteRenovation(int id);
    void RecordRenovationAction(int id, string action, DateTime date, int? cashAccountId = null);
}

public sealed class RenovationService(IRenovationRepository repository)
{
    public AccountingService? Accounting => repository is IAccountingRepository accounting ? new AccountingService(accounting) : null;
    public List<Renovation> GetAll(bool includeArchived = false) => repository.GetRenovations(includeArchived);
    public static List<Renovation> Filter(IEnumerable<Renovation> records, DateTime today, int? startYear = null, string? status = null, string? depositStatus = null) => records
        .Where(r => (!startYear.HasValue || r.StartDate.Year == startYear.Value)
            && (status is null || r.StatusOn(today) == status)
            && (depositStatus is null || r.DepositStatus == depositStatus)).ToList();
    public void Delete(int id) => repository.DeleteRenovation(id);
    public void Add(Renovation renovation)
    {
        if (renovation.IsArchived || renovation.DepositReceivedAccountId.HasValue || renovation.DepositReturnedAccountId.HasValue) throw new ArgumentException("Create a new renovation without accounting history.");
        if (renovation.Id != 0) throw new ArgumentException("Create a new renovation record.");
        if (renovation.DurationDays is < 1 or > 3650) throw new ArgumentException("Duration must be between 1 and 3,650 days.");
        if (renovation.StartDate.Year < 1900 || renovation.StartDate.Date > DateTime.MaxValue.Date.AddDays(1 - renovation.DurationDays)) throw new ArgumentException("Choose a valid start date and duration.");
        if (renovation.DepositAmount < 0 || renovation.DepositAmount > 1000000 || decimal.Round(renovation.DepositAmount, 2) != renovation.DepositAmount) throw new ArgumentException("Enter a deposit amount from RM 0 to RM 1,000,000 with at most two decimal places.");
        renovation.Description = renovation.Description.Trim();
        if (renovation.Description.Length == 0 || renovation.Description.Length > 500) throw new ArgumentException("Enter a description of at most 500 characters.");
        if (renovation.DepositCollectedDate.HasValue || renovation.CompletedDate.HasValue || renovation.DepositReturnedDate.HasValue) throw new ArgumentException("Record collection and completion after creating the renovation.");
        renovation.StartDate = renovation.StartDate.Date;
        repository.AddRenovation(renovation);
    }
    public void RecordAction(int id, string action, DateTime date, int? cashAccountId = null)
    {
        if (date.Year < 1900 || date.Date > DateTime.Today) throw new ArgumentException("Choose a recorded date from 1900 through today.");
        repository.RecordRenovationAction(id, action, date.Date, cashAccountId);
    }
}
