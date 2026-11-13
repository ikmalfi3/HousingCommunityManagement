using HousingCommunityManagement.Core;

namespace HousingCommunityManagement;
public partial class AccountingAttachments : UserControl
{
    public List<ExpenseAttachment> Files { get; } = [];

    public AccountingAttachments(IEnumerable<ExpenseAttachment> files, bool readOnly = false) : this()
    {
        BindFiles(files, readOnly);
    }

    public void RefreshData() => Reload();
    public void BindFiles(IEnumerable<ExpenseAttachment> files, bool readOnly = false)
    {
        Files.Clear(); Files.AddRange(files); add.Visible = remove.Visible = !readOnly; Reload();
    }
    private void Reload()
    {
        list.Items.Clear();
        foreach (var file in Files)
            list.Items.Add(file.FileName);
    }

    private void AddClicked(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Filter = "Receipts|*.pdf;*.png;*.jpg;*.jpeg;*.txt",
            Multiselect = true
        };
        if (dialog.ShowDialog(this) != DialogResult.OK)
            return;
        try
        {
            var paths = dialog.FileNames.Select(p => new FileInfo(p)).ToList();
            if (Files.Count + paths.Count > 10 || paths.Any(p => p.Length > 10485760) || Files.Sum(f => (long)f.Content.Length) + paths.Sum(p => p.Length) > 30 * 1024 * 1024)
                throw new ArgumentException("Up to 10 files, 10 MB each, 30 MB total.");
            Files.AddRange(paths.Select(p => new ExpenseAttachment { FileName = p.Name, Content = File.ReadAllBytes(p.FullName) }));
            Reload();
        }
        catch (Exception ex)
        {
            AccountingUi.Error(this, ex);
        }
    }

    private void RemoveClicked(object? sender, EventArgs e)
    {
        if (list.SelectedIndex >= 0)
        {
            Files.RemoveAt(list.SelectedIndex);
            Reload();
        }
    }

    private void ExportClicked(object? sender, EventArgs e)
    {
        if (list.SelectedIndex < 0)
            return;
        var file = Files[list.SelectedIndex];
        using var dialog = new SaveFileDialog
        {
            FileName = file.FileName
        };
        if (dialog.ShowDialog(this) == DialogResult.OK)
            try
            {
                File.WriteAllBytes(dialog.FileName, file.Content);
            }
            catch (Exception ex)
            {
                AccountingUi.Error(this, ex);
            }
    }

    public AccountingAttachments()
    {
        InitializeComponent();
    }
}
