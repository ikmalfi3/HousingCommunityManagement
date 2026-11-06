namespace HousingCommunityManagement.Core;

public static class DerivedHouseFlags
{
    public const string MaintenanceOutstanding = "Maintenance fee outstanding";
    public const string MaintenanceOverdue = "Maintenance fee overdue";
    public const string UpcomingRenovation = "Upcoming renovation";
    public const string CurrentRenovation = "Currently in renovation";
    public const string OverdueRenovation = "Renovation overdue";
    public const string DepositCollection = "Renovation deposit awaiting collection";
    public const string DepositHeld = "Renovation deposit held";
    public const string DepositReturn = "Renovation deposit awaiting return";
    public static readonly IReadOnlyList<(string Name, string Source)> Definitions = Array.AsReadOnly(new[]
    {
        (MaintenanceOutstanding, "Billing"), (MaintenanceOverdue, "Billing"),
        (UpcomingRenovation, "Renovations"), (CurrentRenovation, "Renovations"),
        (OverdueRenovation, "Renovations"), (DepositCollection, "Renovations"),
        (DepositHeld, "Renovations"), (DepositReturn, "Renovations")
    });
    public static bool IsAutomaticName(string name) => Definitions.Any(d => d.Name.Equals(name.Trim(), StringComparison.OrdinalIgnoreCase));
    public static bool IsManaged(HouseFlag flag) => Definitions.Any(d => d.Name.Equals(flag.Name, StringComparison.OrdinalIgnoreCase) && d.Source.Equals(flag.Source, StringComparison.OrdinalIgnoreCase));
    public static void Apply(IEnumerable<Household> houses, IEnumerable<MonthlyCharge> charges, IEnumerable<Renovation> renovations, DateTime today)
    {
        today = today.Date;
        var unpaid = charges.Where(c => c.Balance > 0).ToLookup(c => c.HouseholdId);
        var work = renovations.ToLookup(r => r.HouseholdId);
        foreach (var house in houses)
        {
            // Replace only this calculator's flags. Custom flags and other module sources remain independent.
            house.Flags.RemoveAll(IsManaged);
            void Add(string name, string source) => house.Flags.Add(new HouseFlag { Name = name, Source = source });
            if (unpaid[house.Id].Any()) Add(MaintenanceOutstanding, "Billing");
            if (unpaid[house.Id].Any(c => today > c.DueDate.Date)) Add(MaintenanceOverdue, "Billing");
            var records = work[house.Id].ToList();
            var unfinished = records.Where(r => !r.IsArchived && !r.CompletedDate.HasValue).ToList();
            if (unfinished.Any(r => r.StartDate.Date > today)) Add(UpcomingRenovation, "Renovations");
            if (unfinished.Any(r => r.StartDate.Date <= today)) Add(CurrentRenovation, "Renovations");
            if (unfinished.Any(r => r.PlannedEndDate.Date < today)) Add(OverdueRenovation, "Renovations");
            if (unfinished.Any(r => r.DepositAmount > 0 && !r.DepositCollectedDate.HasValue)) Add(DepositCollection, "Renovations");
            var held = records.Where(r => r.DepositAmount > 0 && r.DepositCollectedDate.HasValue && !r.DepositReturnedDate.HasValue).ToList();
            if (held.Count > 0) Add(DepositHeld, "Renovations");
            if (held.Any(r => r.CompletedDate.HasValue || r.IsArchived)) Add(DepositReturn, "Renovations");
        }
    }
}
