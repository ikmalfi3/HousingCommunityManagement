namespace HousingCommunityManagement.Core;

public sealed record FinancialReportRow(string Section, string Code, string Account, decimal? Debit = null, decimal? Credit = null, decimal? Amount = null, bool IsTotal = false);
public sealed record FinancialReport(string Title, DateTime From, DateTime To, List<FinancialReportRow> Rows, string Summary, string Note);

public static class FinancialReports
{
    public static FinancialReport Generate(string kind, DateTime from, DateTime to, IEnumerable<Account> accounts, IEnumerable<JournalEntry> journal, bool includeZero = false)
    {
        from = from.Date; to = to.Date;
        if (kind is not ("Balance Sheet" or "Income Statement" or "Trial Balance")) throw new ArgumentException("Choose a report type.");
        if (kind == "Income Statement" && from > to) throw new ArgumentException("The start date cannot be after the end date.");
        var chart = accounts.OrderBy(a => a.Code, StringComparer.OrdinalIgnoreCase).ToList();
        var entries = journal.Where(e => e.Date.Date <= to).ToList();
        var lines = entries.SelectMany(e => e.Lines).ToList();
        if (lines.Any(l => chart.All(a => a.Id != l.AccountId))) throw new ArgumentException("A posted account is missing from the chart of accounts.");
        decimal Net(int id, IEnumerable<JournalLine> source) => source.Where(l => l.AccountId == id).Sum(l => l.Debit - l.Credit);
        var rows = new List<FinancialReportRow>();
        var difference = lines.Sum(l => l.Debit - l.Credit);
        string note = "Posted entries only, including openings, adjustments and reversals. Drafts are excluded. Amounts in RM.";
        if (kind == "Trial Balance")
        {
            foreach (var a in chart)
            {
                var net = Net(a.Id, lines);
                if (includeZero || net != 0) rows.Add(new(a.Type, a.Code, a.Name, Math.Max(net, 0), Math.Max(-net, 0)));
            }
            var debit = rows.Sum(r => r.Debit ?? 0); var credit = rows.Sum(r => r.Credit ?? 0);
            rows.Add(new("Total", "", "Total", debit, credit, IsTotal: true));
            return new(kind, to, to, rows, $"Debits RM {debit:N2} | Credits RM {credit:N2} | Difference RM {debit - credit:N2}", note + " Cumulative account balances as at the selected date; no annual closing is assumed.");
        }
        decimal Group(string type, IEnumerable<JournalLine> source, string section)
        {
            decimal total = 0;
            foreach (var a in chart.Where(a => a.Type == type))
            {
                var amount = Net(a.Id, source) * (type is "Liability" or "Equity" or "Income" ? -1 : 1);
                total += amount;
                if (includeZero || amount != 0) rows.Add(new(section, a.Code, a.Name, Amount: amount));
            }
            rows.Add(new(section, "", "Total " + section.ToLowerInvariant(), Amount: total, IsTotal: true));
            return total;
        }
        decimal Surplus(IEnumerable<JournalLine> source)
        {
            var values = source.ToList();
            return chart.Where(a => a.Type is "Income" or "Expense").Sum(a => -Net(a.Id, values));
        }
        if (kind == "Income Statement")
        {
            var period = entries.Where(e => e.Date.Date >= from).SelectMany(e => e.Lines).ToList();
            var income = Group("Income", period, "Income"); var expenses = Group("Expense", period, "Expenses");
            var surplus = income - expenses;
            rows.Add(new("Result", "", "Surplus / (deficit)", Amount: surplus, IsTotal: true));
            return new(kind, from, to, rows, $"Income RM {income:N2} | Expenses RM {expenses:N2} | Surplus / (deficit) RM {surplus:N2}", note + " Income and expenses are based on posting dates, including unpaid posted bills and charges.");
        }
        var assets = Group("Asset", lines, "Assets");
        var liabilities = Group("Liability", lines, "Liabilities");
        var equity = Group("Equity", lines, "Equity");
        var yearStart = new DateTime(to.Year, 1, 1);
        var prior = Surplus(entries.Where(e => e.Date.Date < yearStart).SelectMany(e => e.Lines));
        var current = Surplus(entries.Where(e => e.Date.Date >= yearStart).SelectMany(e => e.Lines));
        rows.Add(new("Equity", "", "Accumulated surplus / (deficit) before " + to.Year, Amount: prior));
        rows.Add(new("Equity", "", "Current year surplus / (deficit)", Amount: current));
        rows.Add(new("Equity", "", "Total equity including surplus / (deficit)", Amount: equity + prior + current, IsTotal: true));
        rows.Add(new("Total", "", "Total liabilities and equity", Amount: liabilities + equity + prior + current, IsTotal: true));
        rows.Add(new("Check", "", "Balance difference", Amount: difference, IsTotal: true));
        return new(kind, to, to, rows, $"Assets RM {assets:N2} | Liabilities and equity RM {liabilities + equity + prior + current:N2} | Difference RM {difference:N2}", note + " Accumulated depreciation reduces assets. Unclosed income/expense balances are included in equity as surplus; no closing journal is created.");
    }
}
