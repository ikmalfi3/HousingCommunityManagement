using HousingCommunityManagement.Core;
using System.Text.Json;

namespace HousingCommunityManagement
{
    public partial class HouseholdForm : Form
    {
        private readonly HouseholdService? service;
        private List<Household> households = [];
        private DateTime flagsDate = DateTime.Today;
        private readonly string? columnSettingsPath;

        public HouseholdForm() : this(null, "") { }

        public HouseholdForm(HouseholdService? service, string databasePath)
        {
            this.service = service;
            InitializeComponent();
            databaseLabel.Text = $"Database: {databasePath}";
            if (!string.IsNullOrEmpty(databasePath))
                columnSettingsPath = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(databasePath))!, "house-table.json");
        }
        private void FilterChanged(object? sender, EventArgs e) => ApplyFilter();
        private void RefreshClicked(object? sender, EventArgs e) => Reload();
        private void DateFlagsTick(object? sender, EventArgs e)
        {
            if (service is not null && Visible && flagsDate != DateTime.Today) Reload();
        }
        private void AddClicked(object? sender, EventArgs e) => EditHousehold(null);
        private void EditClicked(object? sender, EventArgs e) => EditSelected();
        private void GridDoubleClicked(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) EditSelected();
        }
        private void RangeClicked(object? sender, EventArgs e)
        {
            if (service is null) return;
            try { using var dialog = new HouseRangeForm(service); if (dialog.ShowDialog(this) == DialogResult.OK) Reload(); }
            catch (Exception ex) { MessageBox.Show(this, ex.Message, "Could not open range"); }
        }
        private void SettingsClicked(object? sender, EventArgs e)
        {
            if (service is null) return;
            try { using var dialog = new CommunitySettingsForm(service); if (dialog.ShowDialog(this) == DialogResult.OK) Reload(); }
            catch (Exception ex) { MessageBox.Show(this, ex.Message, "Could not open settings"); }
        }
        private void FormShown(object? sender, EventArgs e)
        {
            RestoreColumnWidths();
            Reload();
        }

        private void GridCellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || grid.Rows[e.RowIndex].DataBoundItem is not Household household) return;
            var column = grid.Columns[e.ColumnIndex];
            if (column == ownerColumn) e.Value = string.Join(Environment.NewLine, household.Owners);
            else if (column == contactColumn) e.Value = string.Join(Environment.NewLine, household.Contacts);
            else if (column == flagsColumn) e.Value = string.Join(Environment.NewLine, household.Flags.Select(f => f.Name));
            else return;
            e.FormattingApplied = true;
        }

        private void RestoreColumnWidths()
        {
            if (columnSettingsPath is null || !File.Exists(columnSettingsPath)) return;
            try
            {
                var widths = JsonSerializer.Deserialize<Dictionary<string, int>>(File.ReadAllText(columnSettingsPath));
                if (widths is null) return;
                foreach (DataGridViewColumn column in grid.Columns)
                    if (widths.TryGetValue(column.Name, out var width) && width is >= 20 and <= 5000)
                        column.Width = Math.Clamp((int)Math.Round(width * DeviceDpi / 96d), column.MinimumWidth, 10000);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
            {
                // Invalid or unavailable layout preferences should not prevent opening the database.
            }
        }

        private void SaveColumnWidths(object? sender, FormClosingEventArgs e)
        {
            if (columnSettingsPath is null || e.Cancel) return;
            try
            {
                var widths = grid.Columns.Cast<DataGridViewColumn>().ToDictionary(c => c.Name,
                    c => (int)Math.Round(c.Width * 96d / DeviceDpi));
                Directory.CreateDirectory(Path.GetDirectoryName(columnSettingsPath)!);
                var temporary = columnSettingsPath + ".tmp";
                File.WriteAllText(temporary, JsonSerializer.Serialize(widths));
                File.Move(temporary, columnSettingsPath, true);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                MessageBox.Show(this, $"Column widths could not be remembered.\n\n{ex.Message}", "Table layout", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Reload()
        {
            try { households = service?.GetAll() ?? []; flagsDate = DateTime.Today; feeLabel.Text = "Maintenance amounts are entered in Charge houses for each charging run."; ApplyFilter(); }
            catch (Exception ex) { MessageBox.Show(this, $"Could not load households.\n\n{ex.Message}", "Load failed", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void ApplyFilter()
        {
            var term = search.Text.Trim();
            var filtered = households.Where(h => (includeInactive.Checked || h.IsActive) &&
                new[] { h.HouseNumber, h.OwnerSummary, h.ContactSummary, h.FlagSummary }.Any(v => v.Contains(term, StringComparison.OrdinalIgnoreCase))).ToList();
            grid.DataSource = filtered;
            count.Text = $"{filtered.Count} house(s) shown · {households.Count(h => h.IsActive)} active in total";
        }

        private void EditSelected()
        {
            if (grid.CurrentRow?.DataBoundItem is Household household) EditHousehold(household);
            else MessageBox.Show(this, "Select a house to edit.", "Houses");
        }

        private void EditHousehold(Household? household)
        {
            if (service is null) return;
            try { using var dialog = new HouseholdEditForm(service, household); dialog.ShowDialog(this); Reload(); }
            catch (Exception ex) { MessageBox.Show(this, ex.Message, "Could not open house"); }
        }
    }
}
