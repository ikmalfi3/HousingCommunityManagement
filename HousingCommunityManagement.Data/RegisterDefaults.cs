using HousingCommunityManagement.Core;
using Microsoft.EntityFrameworkCore;
namespace HousingCommunityManagement.Data;
public sealed partial class HouseholdRepository
{
    private static void EnsureRegisterDefaults(HouseholdDbContext db)
    {
        db.Database.ExecuteSqlRaw("CREATE TABLE IF NOT EXISTS AccountingDefaultUpgrades (Version INTEGER NOT NULL PRIMARY KEY)");
        if (db.Database.SqlQueryRaw<int>("SELECT COUNT(*) AS Value FROM AccountingDefaultUpgrades WHERE Version = 1").Single() != 0) return;
        Account Seed(int suggestedCode, string name, string type)
        {
            var existing = db.Accounts.FirstOrDefault(a => a.Name == name && a.Type == type && a.IsActive && !a.IsCashBank);
            if (existing is not null) return existing;
            var code = suggestedCode.ToString();
            while (db.Accounts.Any(a => a.Code == code)) code = (++suggestedCode).ToString();
            var account = new Account { Code = code, Name = name, Type = type };
            db.Accounts.Add(account); db.SaveChanges(); return account;
        }
        var setup = db.AccountingSetup.Single();
        if (setup.AssetPurchaseAccountId == 0) setup.AssetPurchaseAccountId = Seed(1500, "Equipment & furniture", "Asset").Id;
        if (setup.SupplierDepositAccountId == 0) setup.SupplierDepositAccountId = Seed(1200, "Supplier deposits paid", "Asset").Id;
        if (setup.PrepaymentAccountId == 0) setup.PrepaymentAccountId = Seed(1300, "Prepayments", "Asset").Id;
        if (setup.AccumulatedDepreciationAccountId == 0) setup.AccumulatedDepreciationAccountId = Seed(1590, "Accumulated depreciation", "Asset").Id;
        if (setup.DepreciationExpenseAccountId == 0) setup.DepreciationExpenseAccountId = Seed(5600, "Depreciation expense", "Expense").Id;
        if (setup.WriteOffExpenseAccountId == 0) setup.WriteOffExpenseAccountId = Seed(5700, "Write-offs & asset disposal expense", "Expense").Id;
        Seed(4100, "Other income", "Income");
        Seed(4200, "Bank interest income", "Income");
        Seed(4300, "Asset disposal proceeds", "Income");
        Seed(5400, "Insurance expense", "Expense");
        Seed(5500, "Bank charges", "Expense");
        db.SaveChanges();
        db.Database.ExecuteSqlRaw("INSERT INTO AccountingDefaultUpgrades (Version) VALUES (1)");
    }
}
