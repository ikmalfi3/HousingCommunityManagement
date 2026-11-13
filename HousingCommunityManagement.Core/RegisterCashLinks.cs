namespace HousingCommunityManagement.Core;

public sealed record RegisterCashLinkOption(int Id, DateTime Date, string Direction, string Counterparty, decimal Available)
{
    public string Label => $"#{Id} / {Date:dd MMM yyyy} / {Direction} / {Counterparty} / available RM {Available:N2}";
}

public static class RegisterCashLinks
{
    public static List<RegisterCashLinkOption> Available(FinanceRegisterItem item, IEnumerable<FinanceRegisterItem> registers, IEnumerable<CashTransaction> transactions)
    {
        var used = registers.Where(r => r.AccountId == item.AccountId).SelectMany(r => r.Movements)
            .Where(m => m.CashTransactionId.HasValue).GroupBy(m => m.CashTransactionId!.Value)
            .ToDictionary(g => g.Key, g => g.Sum(m => Math.Abs(m.CostChange)));
        return transactions.Where(t => !t.ReversedDate.HasValue && t.Date.Date >= item.AcquisitionDate.Date)
            .Where(t => t.Direction == "Money out" || item.Balance > 0 && !(item.Kind == "Asset" && item.AccumulatedDepreciation > 0))
            .Select(t => new RegisterCashLinkOption(t.Id, t.Date, t.Direction, t.Counterparty,
                Math.Max(0, t.Allocations.Where(a => a.AccountId == item.AccountId).Sum(a => a.Amount) - used.GetValueOrDefault(t.Id))))
            .Where(t => t.Available > 0).ToList();
    }
}
