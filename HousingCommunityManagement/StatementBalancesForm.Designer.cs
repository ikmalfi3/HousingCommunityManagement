#nullable enable
namespace HousingCommunityManagement;
partial class StatementBalancesForm
{
    private TableLayoutPanel layout = null!;
    private TableLayoutPanel fields = null!;
    private FlowLayoutPanel buttons = null!;
    private Button saveButton = null!;
    private Button cancelButton = null!;
    private Label openingLabel = null!;
    private NumericUpDown openingInput = null!;
    private Label closingLabel = null!;
    private NumericUpDown closingInput = null!;
    private System.ComponentModel.IContainer? components = null;
    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }
    private void InitializeComponent()
    {
        layout = new TableLayoutPanel(); fields = new TableLayoutPanel(); buttons = new FlowLayoutPanel();
        saveButton = new Button(); cancelButton = new Button();
        openingLabel = new Label(); openingInput = new NumericUpDown();
        closingLabel = new Label(); closingInput = new NumericUpDown();
        ((System.ComponentModel.ISupportInitialize)openingInput).BeginInit();
        ((System.ComponentModel.ISupportInitialize)closingInput).BeginInit();
        SuspendLayout(); layout.SuspendLayout(); fields.SuspendLayout(); buttons.SuspendLayout();
        layout.Name = "layout"; layout.Dock = DockStyle.Fill; layout.Padding = new Padding(18); layout.ColumnCount = 1; layout.RowCount = 2;
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
        fields.Name = "fields"; fields.Dock = DockStyle.Fill; fields.ColumnCount = 2; fields.RowCount = 2;
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 185F)); fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        openingLabel.Name = "openingLabel"; openingLabel.Text = "Statement opening"; openingLabel.AutoSize = true; openingLabel.Padding = new Padding(0, 6, 0, 0); openingLabel.UseMnemonic = false;
        openingInput.Name = "openingInput"; openingInput.Dock = DockStyle.Fill; openingInput.TabIndex = 0;
        openingInput.Minimum = -1000000000M;
        openingInput.Maximum = 1000000000M;
        openingInput.DecimalPlaces = 2;
        openingInput.ThousandsSeparator = true;
        fields.Controls.Add(openingLabel, 0, 0); fields.Controls.Add(openingInput, 1, 0);
        closingLabel.Name = "closingLabel"; closingLabel.Text = "Statement closing"; closingLabel.AutoSize = true; closingLabel.Padding = new Padding(0, 6, 0, 0); closingLabel.UseMnemonic = false;
        closingInput.Name = "closingInput"; closingInput.Dock = DockStyle.Fill; closingInput.TabIndex = 1;
        closingInput.Minimum = -1000000000M;
        closingInput.Maximum = 1000000000M;
        closingInput.DecimalPlaces = 2;
        closingInput.ThousandsSeparator = true;
        fields.Controls.Add(closingLabel, 0, 1); fields.Controls.Add(closingInput, 1, 1);
        buttons.Name = "buttons"; buttons.Dock = DockStyle.Fill; buttons.FlowDirection = FlowDirection.RightToLeft;
        saveButton.Name = "saveButton"; saveButton.Text = "Save"; saveButton.AutoSize = true; saveButton.Click += SaveClicked;
        cancelButton.Name = "cancelButton"; cancelButton.Text = "Cancel"; cancelButton.AutoSize = true; cancelButton.DialogResult = DialogResult.Cancel;
        buttons.Controls.Add(cancelButton); buttons.Controls.Add(saveButton); layout.Controls.Add(fields, 0, 0); layout.Controls.Add(buttons, 0, 1); Controls.Add(layout);
        Name = "StatementBalancesForm"; Text = "Edit statement balances"; ClientSize = new Size(760, 380); MinimumSize = new Size(650, 400); StartPosition = FormStartPosition.CenterParent; CancelButton = cancelButton;
        AutoScaleDimensions = new SizeF(7F, 15F); AutoScaleMode = AutoScaleMode.Font;
        ((System.ComponentModel.ISupportInitialize)openingInput).EndInit();
        ((System.ComponentModel.ISupportInitialize)closingInput).EndInit();
        buttons.ResumeLayout(false); buttons.PerformLayout(); fields.ResumeLayout(false); fields.PerformLayout(); layout.ResumeLayout(false); ResumeLayout(false);
    }
}
