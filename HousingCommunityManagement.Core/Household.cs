namespace HousingCommunityManagement.Core;

public sealed class Household
{
    public int Id { get; set; }
    public string HouseNumber { get; set; } = "";
    public bool IsOwned { get; set; }
    public string Race { get; set; } = "";
    public List<string> Owners { get; set; } = [];
    public List<string> Contacts { get; set; } = [];
    public List<HouseFlag> Flags { get; set; } = [];
    public bool IsActive { get; set; } = true;
    public string OwnerSummary => string.Join(", ", Owners);
    public string ContactSummary => string.Join(", ", Contacts);
    public string FlagSummary => string.Join(", ", Flags.Select(f => f.Name));
}

public sealed class HouseFlag
{
    public string Name { get; set; } = "";
    // Manual flags and flags owned by future modules can coexist.
    public string Source { get; set; } = "Manual";
}

public sealed class CommunitySettings
{
    public int Id { get; set; } = 1;
    // Retained for compatibility with existing databases; charging always uses an explicit amount.
    public decimal MonthlyMaintenanceFee { get; set; }
    public List<string> FlagNames { get; set; } = [];
}
