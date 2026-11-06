using HousingCommunityManagement.Core;
namespace HousingCommunityManagement;
public partial class HouseholdEditForm : Form
{
    private readonly HouseholdService? service;
    private readonly Household? existing;
    public HouseholdEditForm() { InitializeComponent(); }
    public HouseholdEditForm(HouseholdService service, Household? existing) : this()
    {
        this.service = service;
        this.existing = existing is null ? null : service.GetAll().SingleOrDefault(h => h.Id == existing.Id)
            ?? throw new ArgumentException("This house no longer exists. Refresh the household list.");
        existing = this.existing;
        Text = existing is null ? "Add house" : "Edit house";
        house.Text = existing?.HouseNumber ?? "";
        owned.Checked = existing?.IsOwned ?? false;
        race.SelectedItem = existing?.Race ?? "";
        owner.Lines = existing?.Owners.ToArray() ?? [];
        contact.Lines = existing?.Contacts.ToArray() ?? [];
        active.Checked = existing?.IsActive ?? true;
        var manual = existing?.Flags.Where(f => f.Source == "Manual").Select(f => f.Name).ToList() ?? [];
        foreach (var name in service.GetAvailableFlags().Concat(manual).Distinct(StringComparer.OrdinalIgnoreCase))
            flags.Items.Add(name, manual.Contains(name, StringComparer.OrdinalIgnoreCase));
        ShowDerivedFlags();
    }
    private void AddFlagClicked(object? sender, EventArgs e)
    {
        if (service is null) return;
        try
        {
            var name = service.AddFlag(newFlag.Text);
            var index = flags.Items.Cast<string>().ToList().FindIndex(n => n.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (index < 0) index = flags.Items.Add(name);
            flags.SetItemChecked(index, true);
            flags.SelectedIndex = index;
            newFlag.Clear();
            newFlag.Focus();
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "Could not add flag", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
    }
    private void DeleteFlagClicked(object? sender, EventArgs e)
    {
        if (service is null) return;
        if (flags.SelectedItem is not string name)
        {
            MessageBox.Show(this, "Select a flag from the list to delete.", "Delete flag");
            return;
        }
        try
        {
            service.DeleteFlag(name, existing?.Id);
            existing?.Flags.RemoveAll(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            ShowDerivedFlags();
            flags.Items.RemoveAt(flags.SelectedIndex);
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "Could not delete flag", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
    }
    private void NewFlagKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Enter) return;
        e.SuppressKeyPress = true;
        AddFlagClicked(sender, EventArgs.Empty);
    }
    private void ShowDerivedFlags()
    {
        var values = existing?.Flags.Where(f => f.Source != "Manual").Select(f => $"{f.Name} ({f.Source})").ToList() ?? [];
        derived.Text = values.Count == 0 ? "No automatic flags currently apply." : string.Join(Environment.NewLine, values);
    }
    private void SaveClicked(object? sender, EventArgs e)
    {
        if (service is null) return;
        try
        {
            var selected = flags.CheckedItems.Cast<string>().Select(n => new HouseFlag { Name = n }).ToList();
            selected.AddRange(existing?.Flags.Where(f => f.Source != "Manual") ?? []);
            service.Save(new Household { Id = existing?.Id ?? 0, HouseNumber = house.Text, IsOwned = owned.Checked,
                Race = race.SelectedItem as string ?? "", Owners = owner.Lines.ToList(), Contacts = contact.Lines.ToList(), Flags = selected, IsActive = active.Checked });
            DialogResult = DialogResult.OK;
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "Could not save house", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
    }
}
