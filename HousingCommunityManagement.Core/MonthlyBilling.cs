namespace HousingCommunityManagement.Core;

public sealed class MonthlyCharge
{
    public int Id { get; set; }
    public int HouseholdId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal Amount { get; set; }
    public DateTime? ChargeDate { get; set; }
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
    public int? CashAccountId { get; set; }
    public int? LegacyPaymentId { get; set; }
}
public interface IMonthlyBillingRepository
{
    List<MonthlyCharge> GetCharges();
    int GenerateMonths(int year, int[] months, decimal amountPerMonth, DateTime? sharedDueDate, int[]? householdIds, IReadOnlyDictionary<int, DateTime>? monthDueDates = null, DateTime? chargeDate = null);
    void RecordPayment(int chargeId, decimal amount, DateTime date, string reference, int? cashAccountId = null);
    int RecordPayments(IReadOnlyList<MaintenancePaymentRequest> payments, DateTime date, int? cashAccountId = null);
}
public sealed record MaintenancePaymentRequest(int ChargeId, decimal Amount, decimal ExpectedBalance, string Reference);
public sealed class MonthlyBillingService(IMonthlyBillingRepository repository)
{
    public static DateTime DueDate(int year, int month)
    {
        if (year is < 1900 or > 9999 || month is < 1 or > 12) throw new ArgumentException("Choose a valid year and month.");
        return new DateTime(year, month, 6);
    }
    public AccountingService? Accounting => repository is IAccountingRepository accounting ? new AccountingService(accounting) : null;
    public List<MonthlyCharge> GetCharges() => repository.GetCharges();
    public static void ValidateAmount(decimal amount)
    {
        if (amount <= 0 || amount > 1000000 || decimal.Round(amount, 2) != amount)
            throw new ArgumentException("Enter a maintenance amount greater than RM 0 and up to RM 1,000,000, with at most two decimal places.");
    }
    public static void ValidateMonthDueDates(int[] months, DateTime? sharedDueDate, IReadOnlyDictionary<int, DateTime>? monthDueDates)
    {
        if (monthDueDates is null) return;
        if (sharedDueDate.HasValue) throw new ArgumentException("Choose either one shared due date or individual monthly due dates.");
        if (monthDueDates.Count != months.Distinct().Count() || months.Any(m => !monthDueDates.ContainsKey(m)))
            throw new ArgumentException("Provide one due date for each selected month, and no unselected months.");
    }
    public int GenerateMonths(int year, IEnumerable<int> months, decimal amountPerMonth, DateTime? sharedDueDate = null, IEnumerable<int>? householdIds = null, IReadOnlyDictionary<int, DateTime>? monthDueDates = null, DateTime? chargeDate = null)
    {
        ValidateAmount(amountPerMonth);
        var selected = months.Distinct().Order().ToArray();
        if (selected.Length == 0) throw new ArgumentException("Select at least one month to charge.");
        foreach (var month in selected) DueDate(year, month);
        ValidateMonthDueDates(selected, sharedDueDate, monthDueDates);
        var houses = householdIds?.Distinct().ToArray();
        if (houses is { Length: 0 }) throw new ArgumentException("Select at least one house to charge.");
        var dates = monthDueDates?.ToDictionary(p => p.Key, p => p.Value.Date);
        var date = (chargeDate ?? DateTime.Today).Date;
        ValidateChargeDate(date);
        return repository.GenerateMonths(year, selected, amountPerMonth, sharedDueDate?.Date, houses, dates, date);
    }
    public static void ValidatePaymentChargeDates(IEnumerable<MonthlyCharge> charges, DateTime paymentDate)
    {
        var latest = charges.Where(c => c.ChargeDate.HasValue).Select(c => c.ChargeDate!.Value.Date).DefaultIfEmpty(DateTime.MinValue).Max();
        if (paymentDate.Date < latest) throw new ArgumentException($"Payment date must be on or after the latest selected charge date ({latest:dd MMM yyyy}).");
    }
    public static void ValidateChargeDate(DateTime date)
    {
        if (date.Year < 1900 || date.Date > DateTime.Today) throw new ArgumentException("Choose a charge date from 1900 through today.");
    }
    public void RecordPayment(int chargeId, decimal amount, DateTime date, string reference, int? cashAccountId = null)
    {
        if (amount <= 0 || decimal.Round(amount, 2) != amount) throw new ArgumentException("Enter a positive payment amount with at most two decimal places.");
        if (date.Date > DateTime.Today) throw new ArgumentException("Payment date cannot be in the future.");
        reference = reference.Trim();
        if (reference.Length > 150) throw new ArgumentException("Payment reference must be at most 150 characters.");
        repository.RecordPayment(chargeId, amount, date.Date, reference, cashAccountId);
    }
    public static List<MaintenancePaymentRequest> ValidatePaymentBatch(IEnumerable<MaintenancePaymentRequest> payments, DateTime date)
    {
        var selected = payments.Select(p => p with { Reference = p.Reference?.Trim() ?? "" }).ToList();
        if (selected.Count == 0) throw new ArgumentException("Select at least one unpaid monthly charge.");
        if (selected.Select(p => p.ChargeId).Distinct().Count() != selected.Count) throw new ArgumentException("Each monthly charge can appear only once in a payment batch.");
        if (date.Date > DateTime.Today) throw new ArgumentException("Payment date cannot be in the future.");
        foreach (var p in selected)
        {
            if (p.Amount <= 0 || decimal.Round(p.Amount, 2) != p.Amount || p.ExpectedBalance <= 0 || p.Amount > p.ExpectedBalance)
                throw new ArgumentException("Each selected row needs a positive payment with at most two decimals, no more than its displayed balance.");
            if (p.Reference.Length > 150) throw new ArgumentException("Payment references must be at most 150 characters.");
        }
        return selected;
    }
    public int RecordPayments(IEnumerable<MaintenancePaymentRequest> payments, DateTime date, int? cashAccountId = null)
        => repository.RecordPayments(ValidatePaymentBatch(payments, date), date.Date, cashAccountId);
}
