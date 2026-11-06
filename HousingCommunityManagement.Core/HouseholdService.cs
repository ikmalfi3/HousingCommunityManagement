namespace HousingCommunityManagement.Core;
public sealed class HouseholdService(IHouseholdRepository repository)
{
    public DataResetService? DataReset => repository is IDataResetRepository reset ? new(reset) : null;
    public ExpenseService? Expenses => repository is IExpenseRepository expenses ? new(expenses) : null;
    public AccountingService? Accounting => repository is IAccountingRepository accounting ? new AccountingService(accounting) : null;
    public List<Household> GetAll() => repository.GetAll();
    public CommunitySettings GetSettings() => repository.GetSettings();
    public List<string> GetAvailableFlags() => repository.GetSettings().FlagNames
        .Concat(repository.GetAll().SelectMany(h => h.Flags).Where(f => !DerivedHouseFlags.IsManaged(f)).Select(f => f.Name))
        .Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(n => n, StringComparer.OrdinalIgnoreCase).ToList();

    public string AddFlag(string name)
    {
        name = name.Trim();
        if (DerivedHouseFlags.IsAutomaticName(name)) throw new ArgumentException("This flag is automatic. It is managed by billing or renovations and cannot be added manually.");
        if (name.Length is 0 or > 80) throw new ArgumentException("Enter a flag name (up to 80 characters).");
        var canonical = GetAvailableFlags().FirstOrDefault(n => n.Equals(name, StringComparison.OrdinalIgnoreCase)) ?? name;
        var settings = repository.GetSettings();
        settings.FlagNames.Add(canonical);
        SaveSettings(settings);
        return canonical;
    }
    public void DeleteFlag(string name, int? currentHouseId = null)
    {
        name = name.Trim();
        if (DerivedHouseFlags.IsAutomaticName(name)) throw new ArgumentException("Automatic flags cannot be deleted manually. Update the related billing or renovation record instead.");
        if (string.IsNullOrEmpty(name)) throw new ArgumentException("Select a flag to delete.");
        repository.DeleteFlag(name, currentHouseId);
    }

    public void SaveSettings(CommunitySettings settings)
    {
        if (settings.MonthlyMaintenanceFee < 0 || settings.MonthlyMaintenanceFee > 1000000 || decimal.Round(settings.MonthlyMaintenanceFee, 2) != settings.MonthlyMaintenanceFee)
            throw new ArgumentException("Enter a monthly fee between RM 0.00 and RM 1,000,000.00 with at most two decimal places.");
        settings.FlagNames = Clean(settings.FlagNames, 80, "Flag");
        if (settings.FlagNames.Any(DerivedHouseFlags.IsAutomaticName)) throw new ArgumentException("Automatic billing and renovation flags cannot be entered as manual flag options.");
        repository.SaveSettings(settings);
    }
    public int CreateRange(string prefix, int start, int end, int digits)
    {
        prefix = prefix.Trim();
        if (start < 0 || end < start || (long)end - start >= 10000 || digits < 1 || digits > 9)
            throw new ArgumentException("Enter an ascending range of up to 10,000 houses and 1-9 number digits.");
        var houses = Enumerable.Range(start, end - start + 1).Select(n => new Household { HouseNumber = prefix + n.ToString("D" + digits) }).ToList();
        foreach (var house in houses) Validate(house);
        return repository.AddMissing(houses);
    }
    public void Save(Household household) { Validate(household); repository.Save(household); }
    private static void Validate(Household household)
    {
        household.HouseNumber = household.HouseNumber.Trim();
        household.Race = household.Race.Trim();
        if (household.Race is not ("" or "Malay" or "Chinese" or "Indian" or "Others"))
            throw new ArgumentException("Choose Malay, Chinese, Indian or Others, or leave Race blank.");
        if (household.HouseNumber.Length is 0 or > 50) throw new ArgumentException("Enter a house number (up to 50 characters).");
        household.Owners = Clean(household.Owners, 150, "Owner");
        household.Contacts = Clean(household.Contacts, 80, "Contact");
        foreach (var flag in household.Flags)
        {
            flag.Name = flag.Name.Trim();
            if (flag.Source == "Manual" && DerivedHouseFlags.IsAutomaticName(flag.Name)) throw new ArgumentException("Automatic billing and renovation flags cannot be assigned manually.");
            if (flag.Name.Length is 0 or > 80 || string.IsNullOrWhiteSpace(flag.Source)) throw new ArgumentException("Flags require a name (up to 80 characters) and a source.");
        }
    }
    private static List<string> Clean(IEnumerable<string> values, int max, string label)
    {
        var result = values.Select(v => v.Trim()).Where(v => v.Length > 0).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        if (result.Any(v => v.Length > max)) throw new ArgumentException($"{label} entries must be at most {max} characters.");
        return result;
    }
}
