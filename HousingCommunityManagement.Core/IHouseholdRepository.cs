namespace HousingCommunityManagement.Core;

public interface IHouseholdRepository
{
    List<Household> GetAll();
    void Save(Household household);
    int AddMissing(List<Household> households);
    CommunitySettings GetSettings();
    void SaveSettings(CommunitySettings settings);
    void DeleteFlag(string name, int? currentHouseId);
}
