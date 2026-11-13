#nullable enable
namespace HousingCommunityManagement;
partial class RegisterCashLinkForm
{
    private TableLayoutPanel layout = null!;
    private TableLayoutPanel fields = null!;
    private FlowLayoutPanel buttons = null!;
    private Button saveButton = null!;
    private Button cancelButton = null!;
    private Label sourceLabel = null!;
    private ComboBox sourceInput = null!;
    private Label amountLabel = null!;
    private Label helpLabel = null!;
    private NumericUpDown amountInput = null!;
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
        sourceLabel = new Label(); sourceInput = new ComboBox();
        amountLabel = new Label(); amountInput = new NumericUpDown();
        ((System.ComponentModel.ISupportInitialize)amountInput).BeginInit();
        SuspendLayout(); layout.SuspendLayout(); fields.SuspendLayout(); buttons.SuspendLayout();
        layout.Name = "layout"; layout.Dock = DockStyle.Fill; layout.Padding = new Padding(18); layout.ColumnCount = 1; layout.RowCount = 2;
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
        fields.Name = "fields"; fields.Dock = DockStyle.Fill; fields.ColumnCount = 2; fields.RowCount = 3;
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 185F)); fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        sourceLabel.Name = "sourceLabel"; sourceLabel.Text = "Money transaction"; sourceLabel.AutoSize = true; sourceLabel.Padding = new Padding(0, 6, 0, 0); sourceLabel.UseMnemonic = false;
        sourceInput.Name = "sourceInput"; sourceInput.Dock = DockStyle.Fill; sourceInput.TabIndex = 0;
        sourceInput.DropDownStyle = ComboBoxStyle.DropDownList;
        fields.Controls.Add(sourceLabel, 0, 0); fields.Controls.Add(sourceInput, 1, 0);
        amountLabel.Name = "amountLabel"; amountLabel.Text = "Amount for this item"; amountLabel.AutoSize = true; amountLabel.Padding = new Padding(0, 6, 0, 0); amountLabel.UseMnemonic = false;
        amountInput.Name = "amountInput"; amountInput.Dock = DockStyle.Fill; amountInput.TabIndex = 1;
        amountInput.Minimum = 0M;
        amountInput.Maximum = 1000000000M;
        amountInput.DecimalPlaces = 2;
        amountInput.ThousandsSeparator = true;
        fields.Controls.Add(amountLabel, 0, 1); fields.Controls.Add(amountInput, 1, 1);
        helpLabel = new Label();
        helpLabel.Name = "helpLabel";
        helpLabel.Dock = DockStyle.Fill;
        helpLabel.Padding = new Padding(0, 12, 0, 0);
        helpLabel.Text = "Record money once in Income & expenditure, then link its remaining allocation here.";
        fields.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        fields.Controls.Add(helpLabel, 0, 2);
        fields.SetColumnSpan(helpLabel, 2);
        buttons.Name = "buttons"; buttons.Dock = DockStyle.Fill; buttons.FlowDirection = FlowDirection.RightToLeft;
        saveButton.Name = "saveButton"; saveButton.Text = "Save"; saveButton.AutoSize = true; saveButton.Click += SaveClicked;
        cancelButton.Name = "cancelButton"; cancelButton.Text = "Cancel"; cancelButton.AutoSize = true; cancelButton.DialogResult = DialogResult.Cancel;
        buttons.Controls.Add(cancelButton); buttons.Controls.Add(saveButton); layout.Controls.Add(fields, 0, 0); layout.Controls.Add(buttons, 0, 1); Controls.Add(layout);
        Name = "RegisterCashLinkForm"; Text = "Link purchase / deposit refund"; ClientSize = new Size(760, 380); MinimumSize = new Size(650, 400); StartPosition = FormStartPosition.CenterParent; CancelButton = cancelButton;
        AutoScaleDimensions = new SizeF(7F, 15F); AutoScaleMode = AutoScaleMode.Font;
        ((System.ComponentModel.ISupportInitialize)amountInput).EndInit();
        buttons.ResumeLayout(false); buttons.PerformLayout(); fields.ResumeLayout(false); fields.PerformLayout(); layout.ResumeLayout(false); ResumeLayout(false);
    }
}
