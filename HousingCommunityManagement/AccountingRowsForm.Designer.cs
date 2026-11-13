#nullable enable
namespace HousingCommunityManagement;
partial class AccountingRowsForm
{
    private DataGridView rowsGrid = null!;
    private System.ComponentModel.IContainer? components = null;
    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }
    private void InitializeComponent()
    {
        rowsGrid = new DataGridView(); ((System.ComponentModel.ISupportInitialize)rowsGrid).BeginInit(); SuspendLayout();
        rowsGrid.Name = "rowsGrid"; rowsGrid.Dock = DockStyle.Fill; rowsGrid.BackgroundColor = Color.White; rowsGrid.ReadOnly = true; rowsGrid.AllowUserToAddRows = false; rowsGrid.AllowUserToDeleteRows = false; rowsGrid.RowHeadersVisible = false; rowsGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
        Controls.Add(rowsGrid); Name = "AccountingRowsForm"; Text = "Accounting review"; ClientSize = new Size(1000, 550); StartPosition = FormStartPosition.CenterParent;
        Name = "AccountingRowsForm";
        AutoScaleDimensions = new SizeF(7F, 15F); AutoScaleMode = AutoScaleMode.Font; ((System.ComponentModel.ISupportInitialize)rowsGrid).EndInit(); ResumeLayout(false);
    }
}
