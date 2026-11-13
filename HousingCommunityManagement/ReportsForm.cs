using System.Drawing.Printing;
using System.Globalization;
using System.Text;
using HousingCommunityManagement.Core;

namespace HousingCommunityManagement;
public partial class ReportsForm : Form
{
    private AccountingService? accounting;
    private FinancialReport? report;
    private int printRow;
    public ReportsForm() { InitializeComponent(); }
    public ReportsForm(AccountingService accounting) : this()
    {
        this.accounting = accounting;
        fromDate.MaxDate = toDate.MaxDate = DateTime.Today;
        fromDate.Value = new DateTime(DateTime.Today.Year, 1, 1);
        toDate.Value = DateTime.Today;
        if (accounting.GetSettings().StartedOn is { } started && started.Date > fromDate.Value.Date)
            fromDate.Value = started.Date;
        GenerateReport();
    }
    private void TypeChanged(object? sender, EventArgs e)
    {
        fromDate.Enabled = reportType.Text == "Income Statement";
        InvalidateReport(sender, e);
    }
    private void InvalidateReport(object? sender, EventArgs e)
    {
        report = null; reportGrid.DataSource = null;
        heading.Text = "Reports";
        exportButton.Enabled = printButton.Enabled = false;
        summary.Text = "Choose the report and dates, then click Generate.";
    }
    private void GenerateClicked(object? sender, EventArgs e) => GenerateReport();
    private void GenerateReport()
    {
        if (accounting is null || DesignMode) return;
        try
        {
            if (accounting.GetSettings().StartedOn is not { } started)
            {
                InvalidateReport(this, EventArgs.Empty);
                summary.Text = "Start accounting and enter the opening balances before generating reports.";
                return;
            }
            if (toDate.Value.Date < started.Date || reportType.Text == "Income Statement" && fromDate.Value.Date < started.Date)
                throw new ArgumentException($"Accounting starts on {started:dd MMM yyyy}. Choose reporting dates on or after that date.");
            report = FinancialReports.Generate(reportType.Text, fromDate.Value, toDate.Value, accounting.GetAccounts(), accounting.GetJournal(), includeZero.Checked);
            reportGrid.DataSource = report.Rows;
            debitColumn.Visible = creditColumn.Visible = report.Title == "Trial Balance";
            amountColumn.Visible = report.Title != "Trial Balance";
            summary.Text = report.Summary; note.Text = report.Note;
            heading.Text = report.Title + (report.Title == "Income Statement" ? $" — {report.From:dd MMM yyyy} to {report.To:dd MMM yyyy}" : $" — as at {report.To:dd MMM yyyy}");
            exportButton.Enabled = printButton.Enabled = true;
        }
        catch (Exception ex) { AccountingUi.Error(this, ex); }
    }
    private static string CsvText(string text)
    {
        if (text.Length > 0 && "=+-@\t\r\n".Contains(text[0])) text = "'" + text;
        return "\"" + text.Replace("\"", "\"\"") + "\"";
    }
    private void FormatReportCell(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex >= 0 && reportGrid.Rows[e.RowIndex].DataBoundItem is FinancialReportRow { IsTotal: true })
            e.CellStyle.BackColor = Color.FromArgb(232, 240, 248);
    }
    private void ExportClicked(object? sender, EventArgs e)
    {
        if (report is null) return;
        exportDialog.FileName = report.Title.Replace(' ', '-') + "-" + report.To.ToString("yyyy-MM-dd") + ".csv";
        if (exportDialog.ShowDialog(this) != DialogResult.OK) return;
        try
        {
            var trial = report.Title == "Trial Balance";
            var text = new StringBuilder();
            text.AppendLine(CsvText(heading.Text)); text.AppendLine(CsvText(report.Note));
            text.AppendLine(trial ? "Section,Code,Account,Debit (RM),Credit (RM)" : "Section,Code,Account,Amount (RM)");
            string Number(decimal? value) => value?.ToString("0.00", CultureInfo.InvariantCulture) ?? "";
            foreach (var row in report.Rows)
                text.AppendLine(string.Join(",", CsvText(row.Section), CsvText(row.Code), CsvText(row.Account), trial ? Number(row.Debit) + "," + Number(row.Credit) : Number(row.Amount)));
            text.AppendLine(CsvText(report.Summary));
            File.WriteAllText(exportDialog.FileName, text.ToString(), new UTF8Encoding(true));
        }
        catch (Exception ex) { AccountingUi.Error(this, ex); }
    }
    private void PrintClicked(object? sender, EventArgs e)
    {
        if (report is null) return;
        try { printPreview.ShowDialog(this); }
        catch (Exception ex) { AccountingUi.Error(this, ex); }
    }
    private void BeginPrint(object? sender, PrintEventArgs e) => printRow = 0;
    private void PrintPage(object? sender, PrintPageEventArgs e)
    {
        if (report is null || e.Graphics is null) return;
        using var font = new Font("Segoe UI", 9);
        using var bold = new Font("Segoe UI", 9, FontStyle.Bold);
        var bounds = e.MarginBounds; float y = bounds.Top;
        e.Graphics.DrawString(heading.Text, bold, Brushes.Black, new RectangleF(bounds.Left, y, bounds.Width, 44)); y += 48;
        var trial = report.Title == "Trial Balance";
        var nameWidth = bounds.Width - (trial ? 200 : 110);
        using var right = new StringFormat { Alignment = StringAlignment.Far };
        e.Graphics.DrawString(report.Note, font, Brushes.Black, new RectangleF(bounds.Left, y, bounds.Width, 60)); y += 64;
        e.Graphics.DrawString("Account / section", bold, Brushes.Black, bounds.Left, y);
        if (trial)
        {
            e.Graphics.DrawString("Debit (RM)", bold, Brushes.Black, new RectangleF(bounds.Left + nameWidth, y, 95, 25), right);
            e.Graphics.DrawString("Credit (RM)", bold, Brushes.Black, new RectangleF(bounds.Right - 100, y, 100, 25), right);
        }
        else e.Graphics.DrawString("Amount (RM)", bold, Brushes.Black, new RectangleF(bounds.Right - 110, y, 110, 25), right);
        y += 30;
        while (printRow < report.Rows.Count && y + 44 <= bounds.Bottom - 60)
        {
            var row = report.Rows[printRow++]; var style = row.IsTotal ? bold : font;
            e.Graphics.DrawString(row.Section + " / " + row.Code + " " + row.Account, style, Brushes.Black, new RectangleF(bounds.Left, y, nameWidth, 40));
            if (trial)
            {
                e.Graphics.DrawString(row.Debit?.ToString("N2"), style, Brushes.Black, new RectangleF(bounds.Left + nameWidth, y, 95, 40), right);
                e.Graphics.DrawString(row.Credit?.ToString("N2"), style, Brushes.Black, new RectangleF(bounds.Right - 100, y, 100, 40), right);
            }
            else e.Graphics.DrawString(row.Amount?.ToString("N2"), style, Brushes.Black, new RectangleF(bounds.Right - 110, y, 110, 40), right);
            y += 44;
        }
        e.HasMorePages = printRow < report.Rows.Count;
        e.Graphics.DrawString(e.HasMorePages ? "Continued on next page" : report.Summary, font, Brushes.Black, new RectangleF(bounds.Left, bounds.Bottom - 55, bounds.Width, 50));
    }
}
