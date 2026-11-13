namespace HousingCommunityManagement;
public partial class AccountingRowsForm : Form
{
    public AccountingRowsForm() { InitializeComponent(); }
    public AccountingRowsForm(string title, object rows) : this()
    {
        Text = title; rowsGrid.DataSource = rows;
        foreach (DataGridViewColumn column in rowsGrid.Columns) if (column.ValueType == typeof(decimal)) column.DefaultCellStyle.Format = "N2";
    }
}
