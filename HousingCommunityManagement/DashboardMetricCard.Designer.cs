#nullable enable
namespace HousingCommunityManagement;
partial class DashboardMetricCard
{
    private TableLayoutPanel layout = null!;
    private Label titleLabel = null!, detailLabel = null!;
    private LinkLabel valueLabel = null!;
    private System.ComponentModel.IContainer? components = null;
    protected override void Dispose(bool disposing) { if (disposing) components?.Dispose(); base.Dispose(disposing); }
    private void InitializeComponent()
    {
        layout = new TableLayoutPanel(); titleLabel = new Label(); detailLabel = new Label(); valueLabel = new LinkLabel(); SuspendLayout(); layout.SuspendLayout();
        layout.Name = "layout"; layout.Dock = DockStyle.Fill; layout.Padding = new Padding(12); layout.ColumnCount = 1; layout.RowCount = 3; layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F)); layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F)); layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        titleLabel.Name = "titleLabel"; titleLabel.Text = "Metric"; titleLabel.UseMnemonic = false; titleLabel.Dock = DockStyle.Fill;
        valueLabel.Name = "valueLabel"; valueLabel.Text = "RM 0.00"; valueLabel.UseMnemonic = false; valueLabel.Dock = DockStyle.Fill; valueLabel.Font = new Font("Segoe UI", 16F, FontStyle.Bold); valueLabel.LinkColor = Color.FromArgb(23, 100, 147); valueLabel.LinkClicked += ValueClicked;
        detailLabel.Name = "detailLabel"; detailLabel.Text = "Details"; detailLabel.UseMnemonic = false; detailLabel.Dock = DockStyle.Fill;
        layout.Controls.Add(titleLabel, 0, 0); layout.Controls.Add(valueLabel, 0, 1); layout.Controls.Add(detailLabel, 0, 2); Controls.Add(layout);
        Name = "DashboardMetricCard"; BackColor = Color.White; Size = new Size(240, 140); Margin = new Padding(5, 10, 5, 8); AutoScaleDimensions = new SizeF(7F, 15F); AutoScaleMode = AutoScaleMode.Font;
        layout.ResumeLayout(false); ResumeLayout(false);
    }
}
