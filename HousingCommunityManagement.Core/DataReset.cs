using System.Security.Cryptography;
namespace HousingCommunityManagement.Core;

[Flags]
public enum DataResetCategory
{
    None = 0,
    Households = 1,
    Maintenance = 2,
    Renovations = 4,
    Expenses = 8,
    Payees = 16,
    ExpenseCategories = 32,
    Accounting = 64,
    CommunitySettings = 128,
    All = 255
}
public interface IDataResetRepository
{
    Dictionary<DataResetCategory, string> GetDataResetCounts();
    void DeleteData(DataResetCategory categories);
}
public sealed class DataResetConfirmation
{
    internal DataResetConfirmation(DataResetCategory categories, string summary)
    {
        Categories = categories; Summary = summary;
        const string alphabet = "abcdefghijklmnopqrstuvwxyz0123456789";
        char[] chars = new char[6];
        chars[0] = alphabet[RandomNumberGenerator.GetInt32(26)];
        chars[1] = alphabet[26 + RandomNumberGenerator.GetInt32(10)];
        for (var i = 2; i < chars.Length; i++) chars[i] = alphabet[RandomNumberGenerator.GetInt32(alphabet.Length)];
        for (var i = chars.Length - 1; i > 0; i--) { var j = RandomNumberGenerator.GetInt32(i + 1); (chars[i], chars[j]) = (chars[j], chars[i]); }
        Code = new string(chars);
    }
    public DataResetCategory Categories { get; }
    public string Summary { get; }
    public string Code { get; }
    private int used;
    internal void Confirm(string answer)
    {
        if (!string.Equals(answer, Code, StringComparison.Ordinal)) throw new ArgumentException("The code does not match. Enter the lowercase letters and numbers exactly as shown.");
        if (Interlocked.Exchange(ref used, 1) != 0) throw new ArgumentException("This confirmation has already been used. Review the categories again.");
    }
}
public sealed class DataResetService(IDataResetRepository repository)
{
    public static readonly IReadOnlyList<(DataResetCategory Category, string Label)> Categories = Array.AsReadOnly(new[]
    {
        (DataResetCategory.Households, "Households, owners & contacts"),
        (DataResetCategory.Maintenance, "Maintenance charges & payments"),
        (DataResetCategory.Renovations, "Renovations & deposits (including archived)"),
        (DataResetCategory.Expenses, "Expenses, payments & receipt attachments"),
        (DataResetCategory.Payees, "Suppliers & claimants"),
        (DataResetCategory.ExpenseCategories, "Reset expense categories to defaults"),
        (DataResetCategory.Accounting, "Reset accounts, defaults, opening balances & journal"),
        (DataResetCategory.CommunitySettings, "Reset saved house flags")
    });
    public static DataResetCategory IncludeDependencies(DataResetCategory selected)
    {
        if ((selected & ~DataResetCategory.All) != 0) throw new ArgumentException("Choose valid data categories.");
        if (selected.HasFlag(DataResetCategory.Accounting)) selected |= DataResetCategory.Maintenance | DataResetCategory.Renovations | DataResetCategory.Expenses | DataResetCategory.ExpenseCategories;
        if (selected.HasFlag(DataResetCategory.Households)) selected |= DataResetCategory.Maintenance | DataResetCategory.Renovations;
        if ((selected & (DataResetCategory.Payees | DataResetCategory.ExpenseCategories)) != 0) selected |= DataResetCategory.Expenses;
        return selected;
    }
    public Dictionary<DataResetCategory, string> GetCounts() => repository.GetDataResetCounts();
    public DataResetConfirmation Prepare(DataResetCategory selected)
    {
        selected = IncludeDependencies(selected);
        if (selected == DataResetCategory.None) throw new ArgumentException("Select at least one category to delete or reset.");
        var counts = GetCounts();
        var summary = string.Join(Environment.NewLine, Categories.Where(c => selected.HasFlag(c.Category)).Select(c => $"• {c.Label}: {counts[c.Category]}"));
        return new DataResetConfirmation(selected, summary);
    }
    public void Delete(DataResetConfirmation confirmation, string answer)
    {
        confirmation.Confirm(answer);
        repository.DeleteData(confirmation.Categories);
    }
}
