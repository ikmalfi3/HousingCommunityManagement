namespace HousingCommunityManagement.Core;

public sealed class MonthlyCharge
{
    public int Id { get; set; }
    public int HouseholdId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
    public List<MaintenancePayment> Payments { get; set; } = [];
    public decimal Paid => Payments.Sum(p => p.Amount);
    public decimal Balance => Amount - Paid;
    public string StatusOn(DateTime date) => Balance <= 0 ? "Paid" : date.Date > DueDate.Date ? "Overdue" : "Payment due";
}
public sealed class MaintenancePayment
{
    public int Id { get; set; }
    public int MonthlyChargeId { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Reference { get; set; } = "";
    public int? LegacyPaymentId { get; set; }
}
public interface IMonthlyBillingRepository
{
    List<MonthlyCharge> GetCharges();
    int GenerateMonths(int year, int[] months, DateTime? sharedDueDate, int[]? householdIds);
    void RecordPayment(int chargeId, decimal amount, DateTime date, string reference);
}
public sealed class MonthlyBillingService(IMonthlyBillingRepository repository)
{
    public static DateTime DueDate(int year, int month)
    {
        if (year is < 1900 or > 9999 || month is < 1 or > 12) throw new ArgumentException("Choose a valid year and month.");
        return new DateTime(year, month, 6);
    }
    public List<MonthlyCharge> GetCharges() => repository.GetCharges();
    public int GenerateMonths(int year, IEnumerable<int> months, DateTime? sharedDueDate = null, IEnumerable<int>? householdIds = null)
    {
        var selected = months.Distinct().Order().ToArray();
        if (selected.Length == 0) throw new ArgumentException("Select at least one month to charge.");
        foreach (var month in selected) DueDate(year, month);
        var houses = householdIds?.Distinct().ToArray();
        if (houses is { Length: 0 }) throw new ArgumentException("Select at least one house to charge.");
        return repository.GenerateMonths(year, selected, sharedDueDate?.Date, houses);
    }
    public void RecordPayment(int chargeId, decimal amount, DateTime date, string reference)
    {
        if (amount <= 0 || decimal.Round(amount, 2) != amount) throw new ArgumentException("Enter a positive payment amount with at most two decimal places.");
        if (date.Date > DateTime.Today) throw new ArgumentException("Payment date cannot be in the future.");
        reference = reference.Trim();
        if (reference.Length > 150) throw new ArgumentException("Payment reference must be at most 150 characters.");
        repository.RecordPayment(chargeId, amount, date.Date, reference);
    }
}
