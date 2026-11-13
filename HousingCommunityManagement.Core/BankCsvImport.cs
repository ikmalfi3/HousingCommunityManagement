using System.Globalization;
using System.Text;
namespace HousingCommunityManagement.Core;
public static class BankCsvImport
{
    public static List<string[]> Parse(string text, char delimiter = ',')
    {
        if (text.Length > 20 * 1024 * 1024) throw new ArgumentException("CSV must be at most 20 MB.");
        var rows = new List<string[]>(); var row = new List<string>(); var cell = new StringBuilder(); var quoted = false;
        text = text.TrimStart('\uFEFF');
        for (var i = 0; i < text.Length; i++)
        {
            var c = text[i];
            if (c == '"') { if (quoted && i + 1 < text.Length && text[i + 1] == '"') { cell.Append('"'); i++; } else quoted = !quoted; }
            else if (!quoted && c == delimiter) { row.Add(cell.ToString()); cell.Clear(); }
            else if (!quoted && (c == '\r' || c == '\n'))
            {
                if (c == '\r' && i + 1 < text.Length && text[i + 1] == '\n') i++;
                row.Add(cell.ToString()); cell.Clear(); if (row.Any(v => v.Length > 0)) rows.Add(row.ToArray()); row.Clear();
            }
            else cell.Append(c);
        }
        if (quoted) throw new ArgumentException("CSV contains an unclosed quoted value.");
        row.Add(cell.ToString()); if (row.Any(v => v.Length > 0)) rows.Add(row.ToArray());
        if (rows.Count < 2 || rows.Count > 20001) throw new ArgumentException("CSV needs a header and 1-20,000 data rows.");
        return rows;
    }
    public static List<BankStatementLine> Map(List<string[]> rows, int dateColumn, int descriptionColumn, int referenceColumn, int amountColumn, int creditColumn, int debitColumn, string dateFormat)
    {
        string Cell(string[] row, int col) => col >= 0 && col < row.Length ? row[col].Trim() : "";
        decimal Amount(string value)
        {
            value = value.Replace("RM", "", StringComparison.OrdinalIgnoreCase).Trim();
            if (value.Length == 0 || value == "-") return 0;
            if (!decimal.TryParse(value, NumberStyles.Number | NumberStyles.AllowParentheses, CultureInfo.InvariantCulture, out var amount) || decimal.Round(amount, 2) != amount) throw new ArgumentException($"Invalid bank amount: {value}");
            return amount;
        }
        if (dateColumn < 0 || descriptionColumn < 0 || amountColumn < 0 && (creditColumn < 0 || debitColumn < 0)) throw new ArgumentException("Map date, description and either signed amount or both credit/debit columns.");
        if (amountColumn >= 0 && (creditColumn >= 0 || debitColumn >= 0)) throw new ArgumentException("Use either a signed amount column or separate credit/debit columns, not both.");
        var result = new List<BankStatementLine>();
        for (var i = 1; i < rows.Count; i++)
        {
            var row = rows[i];
            if (!DateTime.TryParseExact(Cell(row, dateColumn), dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)) throw new ArgumentException($"Row {i + 1}: invalid date for {dateFormat}.");
            var credit = amountColumn < 0 ? Amount(Cell(row, creditColumn)) : 0; var debit = amountColumn < 0 ? Amount(Cell(row, debitColumn)) : 0;
            if (credit < 0 || debit < 0 || credit > 0 && debit > 0) throw new ArgumentException($"Row {i + 1}: use positive credit OR debit.");
            var amount = amountColumn >= 0 ? Amount(Cell(row, amountColumn)) : credit - debit;
            if (amount == 0) throw new ArgumentException($"Row {i + 1}: statement amount must be nonzero. Remove balance/total rows before import.");
            result.Add(new BankStatementLine { Date = date.Date, Description = Cell(row, descriptionColumn), Reference = Cell(row, referenceColumn), Amount = amount });
        }
        return result;
    }
}
