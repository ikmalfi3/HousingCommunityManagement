using HousingCommunityManagement.Core;

namespace HousingCommunityManagement;
public partial class BankCsvImportForm : Form
{
    private readonly string text = "";
    private ComboBox[] mappings => new ComboBox[] { dateColumn, descriptionColumn, referenceColumn, amountColumn, creditColumn, debitColumn };
    private List<string[]> rows = [];
    public List<BankStatementLine> Lines { get; private set; } = [];

    public BankCsvImportForm(string text) : this()
    {
        this.text = text;
        ReloadColumns();
    }

    private sealed record ColumnChoice(int Index, string Label);
    private void ReloadColumns()
    {
        if (text.Length == 0 || DesignMode) return;
        try
        {
            rows = BankCsvImport.Parse(text, delimiter.SelectedIndex == 1 ? ';' : delimiter.SelectedIndex == 2 ? '\t' : ',');
            var columns = new[]
            {
                new ColumnChoice(-1, "Not used")
            }.Concat(rows[0].Select((h, i) => new ColumnChoice(i, $"{i + 1}: {h}"))).ToList();
            foreach (var box in mappings)
            {
                box.DisplayMember = nameof(ColumnChoice.Label);
                box.ValueMember = nameof(ColumnChoice.Index);
                box.DataSource = columns.ToList();
            }

            preview.Columns.Clear();
            foreach (var c in columns.Skip(1))
                preview.Columns.Add($"C{c.Index}", c.Label);
            foreach (var row in rows.Skip(1).Take(50))
                preview.Rows.Add(row.Take(rows[0].Length).Cast<object>().ToArray());
        }
        catch (Exception ex)
        {
            AccountingUi.Error(this, ex);
        }
    }

    private void MappingChanged(object? sender, EventArgs e) => mapped = false;
    private void DelimiterChanged(object? sender, EventArgs e) => ReloadColumns();
    private int Column(int index) => (mappings[index].SelectedItem as ColumnChoice)?.Index ?? -1;
    private void PreviewClicked(object? sender, EventArgs e)
    {
        try
        {
            Lines = BankCsvImport.Map(rows, Column(0), Column(1), Column(2), Column(3), Column(4), Column(5), format.Text);
            AccountingUi.ShowRows(this, $"Mapped preview / {Lines.Count} lines / net RM {Lines.Sum(l => l.Amount):N2}", Lines.Take(200).Select(l => new { Date = l.Date.ToString("dd MMM yyyy"), l.Description, l.Reference, l.Amount }).ToList());
            mapped = true;
        }
        catch (Exception ex)
        {
            mapped = false;
            AccountingUi.Error(this, ex);
        }
    }

    private bool mapped;
    private void ImportClicked(object? sender, EventArgs e)
    {
        try
        {
            if (!mapped)
                throw new ArgumentException("Preview the mapped lines before importing.");
            Lines = BankCsvImport.Map(rows, Column(0), Column(1), Column(2), Column(3), Column(4), Column(5), format.Text);
            DialogResult = DialogResult.OK;
        }
        catch (Exception ex)
        {
            AccountingUi.Error(this, ex);
        }
    }

    public BankCsvImportForm()
    {
        InitializeComponent();
    }
}
