using System.Globalization;
using HousingCommunityManagement.Core;

namespace HousingCommunityManagement;
internal static class AccountingUi
{
    public static void Bind<T>(ComboBox control, IEnumerable<T> values, string display = "", string key = "")
    {
        control.DisplayMember = display; control.ValueMember = key; control.DataSource = values.ToList();
    }
    public static void BindColumn<T>(DataGridViewComboBoxColumn column, IEnumerable<T> values, string display, string key)
    {
        column.DisplayMember = display; column.ValueMember = key; column.DataSource = values.ToList();
    }
    public static decimal Amount(object? value, string label, bool allowBlank = false) => allowBlank && string.IsNullOrWhiteSpace(value?.ToString()) ? 0m : decimal.TryParse(value?.ToString(), NumberStyles.Number, CultureInfo.CurrentCulture, out var amount) ? amount : throw new ArgumentException($"Enter a valid amount for {label}.");
    public static DateTime Date(object? value, string label)
    {
        if (value is DateTime date)
            return date.Date;
        if (DateTime.TryParseExact(value?.ToString(), new[] { "dd/MM/yyyy", "d/M/yyyy", "yyyy-MM-dd", "dd MMM yyyy" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            return date.Date;
        throw new ArgumentException($"Enter {label} as dd/MM/yyyy or yyyy-MM-dd.");
    }

    public static int Id(object? value, string label) => value is int id ? id : throw new ArgumentException($"Select {label}.");
    public static void Error(Control owner, Exception ex)
    {
        // Automated layout checks must fail with the original stack instead of
        // opening an unattended warning on the user's desktop.
        if (AppContext.TryGetSwitch("HousingCommunityManagement.ThrowOnUiError", out var enabled) && enabled)
            throw new InvalidOperationException("An accounting UI action failed.", ex);
        MessageBox.Show(owner, ex.Message, "Could not complete action", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
    public static void ShowRows(IWin32Window owner, string title, object rows)
    {
        using var form = new AccountingRowsForm(title, rows);
        form.ShowDialog(owner);
    }
}
