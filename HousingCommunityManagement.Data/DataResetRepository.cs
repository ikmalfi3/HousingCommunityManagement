using HousingCommunityManagement.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
namespace HousingCommunityManagement.Data;

public sealed partial class HouseholdRepository : IDataResetRepository
{
    public Dictionary<DataResetCategory, string> GetDataResetCounts()
    {
        using var db = new HouseholdDbContext(databasePath);
        return new()
        {
            [DataResetCategory.Households] = $"{db.Households.Count()} houses",
            [DataResetCategory.Maintenance] = $"{db.MonthlyCharges.Count()} charges, {db.MonthlyPayments.Count()} payments (plus legacy archives)",
            [DataResetCategory.Renovations] = $"{db.Renovations.Count()} records",
            [DataResetCategory.Expenses] = $"{db.CashTransactions.Count()} money in/out transactions, {db.ExpenseBills.Count()} bills, {db.ExpensePayments.Count()} payments, {db.ExpenseAttachments.Count()} attachments",
            [DataResetCategory.Payees] = $"{db.Payees.Count()} payees",
            [DataResetCategory.ExpenseCategories] = $"{db.ExpenseCategories.Count()} categories; restore defaults",
            [DataResetCategory.Accounting] = $"{db.Accounts.Count()} accounts, {db.JournalEntries.Count()} journal entries, {db.FinanceRegisters.Count()} register items, {db.AccountingAdjustments.Count()} adjustments, {db.BankReconciliations.Count()} reconciliations; accounting will stop",
            [DataResetCategory.CommunitySettings] = "saved flag definitions and assignments are cleared"
        };
    }
    public void DeleteData(DataResetCategory categories)
    {
        if (categories == DataResetCategory.None || categories != DataResetService.IncludeDependencies(categories)) throw new ArgumentException("Review all required data categories before deleting.");
        using var db = new HouseholdDbContext(databasePath);
        db.Database.OpenConnection();
        using var tx = db.Database.BeginTransaction();
        if (!categories.HasFlag(DataResetCategory.Accounting) && (categories & (DataResetCategory.Maintenance | DataResetCategory.Renovations | DataResetCategory.Expenses)) != 0 && (db.FinanceRegisters.Any() || db.AccountingAdjustments.Any() || db.BankReconciliations.Any()))
            throw new ArgumentException("Accounting registers, adjustments or reconciliations depend on these records. Select Reset accounts, defaults, opening balances & journal to reset the complete accounting history together.");
        if (categories.HasFlag(DataResetCategory.Accounting))
        {
            db.BankMatches.ExecuteDelete(); db.BankStatementLines.ExecuteDelete(); db.BankReconciliations.ExecuteDelete();
            db.RegisterMovements.ExecuteDelete(); db.FinanceRegisters.ExecuteDelete(); db.AccountingAdjustments.ExecuteDelete(); db.AccountingAudit.ExecuteDelete();
        }
        // Clear journal entries for selected modules before their source records.
        var sources = new List<string>();
        if (categories.HasFlag(DataResetCategory.Maintenance)) sources.AddRange(["MonthlyCharge", "MaintenancePayment", "OpeningCharge"]);
        if (categories.HasFlag(DataResetCategory.Renovations)) sources.AddRange(["RenovationCollection", "RenovationReturn", "OpeningDeposit"]);
        if (categories.HasFlag(DataResetCategory.Expenses)) sources.AddRange(["ExpenseBill", "ExpensePayment", "ExpensePaymentReversal", "ExpenseBillCancellation", "OpeningExpense", "CashTransaction", "CashTransactionReversal"]);
        var entries = db.JournalEntries.Where(e => categories.HasFlag(DataResetCategory.Accounting) || sources.Contains(e.Source));
        db.JournalLines.Where(l => entries.Select(e => e.Id).Contains(l.JournalEntryId)).ExecuteDelete();
        entries.ExecuteDelete();
        if (categories.HasFlag(DataResetCategory.Expenses))
        {
            db.CashTransactions.ExecuteDelete();
            db.ExpensePayments.ExecuteDelete(); db.ExpenseAttachments.ExecuteDelete(); db.ExpenseLines.ExecuteDelete(); db.ExpenseBills.ExecuteDelete();
        }
        if (categories.HasFlag(DataResetCategory.Maintenance))
        {
            db.MonthlyPayments.ExecuteDelete(); db.MonthlyCharges.ExecuteDelete();
            ClearLegacyTable(db, "MaintenancePayments"); ClearLegacyTable(db, "QuarterlyCharges");
            // Keep BillingUpgrades: archived charges must never be migrated back after a reset.
        }
        if (categories.HasFlag(DataResetCategory.Renovations)) db.Renovations.ExecuteDelete();
        if (categories.HasFlag(DataResetCategory.Payees)) db.Payees.ExecuteDelete();
        if (categories.HasFlag(DataResetCategory.ExpenseCategories)) db.ExpenseCategories.ExecuteDelete();
        if (categories.HasFlag(DataResetCategory.Households))
        {
            ClearLegacyTable(db, "LegacyHouseholdFees"); db.Households.ExecuteDelete();
        }
        if (categories.HasFlag(DataResetCategory.CommunitySettings))
        {
            var settings = db.Settings.Single(); settings.MonthlyMaintenanceFee = 0; settings.FlagNames = [];
            foreach (var house in db.Households.ToList()) house.Flags = [];
            db.SaveChanges();
        }
        if (categories.HasFlag(DataResetCategory.Accounting))
        {
            db.AccountingSetup.ExecuteDelete(); db.Accounts.ExecuteDelete();
            db.Database.ExecuteSqlRaw("DELETE FROM AccountingDefaultUpgrades");
            EnsureAccountingSchema(db);
        }
        if (categories.HasFlag(DataResetCategory.ExpenseCategories)) EnsureExpenseSchema(db);
        db.SaveChanges(); tx.Commit();
    }
    private static void ClearLegacyTable(HouseholdDbContext db, string table)
    {
        // Only the three fixed archive table names above are passed to this helper.
        using var command = db.Database.GetDbConnection().CreateCommand(); command.Transaction = db.Database.CurrentTransaction!.GetDbTransaction();
        command.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name=$table";
        var parameter = command.CreateParameter(); parameter.ParameterName = "$table"; parameter.Value = table; command.Parameters.Add(parameter);
        if (Convert.ToInt32(command.ExecuteScalar()) == 0) return;
        command.CommandText = $"DELETE FROM \"{table}\""; command.Parameters.Clear(); command.ExecuteNonQuery();
    }
}
