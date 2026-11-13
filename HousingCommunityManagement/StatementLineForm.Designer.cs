#nullable enable
namespace HousingCommunityManagement;
partial class StatementLineForm
{
    private TableLayoutPanel layout = null!;
    private TableLayoutPanel fields = null!;
    private FlowLayoutPanel buttons = null!;
    private Button saveButton = null!;
    private Button cancelButton = null!;
    private Label dateLabel = null!;
    private DateTimePicker dateInput = null!;
    private Label descriptionLabel = null!;
    private TextBox descriptionInput = null!;
    private Label referenceLabel = null!;
    private TextBox referenceInput = null!;
    private Label amountLabel = null!;
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
        dateLabel = new Label(); dateInput = new DateTimePicker();
        descriptionLabel = new Label(); descriptionInput = new TextBox();
        referenceLabel = new Label(); referenceInput = new TextBox();
        amountLabel = new Label(); amountInput = new NumericUpDown();
        ((System.ComponentModel.ISupportInitialize)amountInput).BeginInit();
        SuspendLayout(); layout.SuspendLayout(); fields.SuspendLayout(); buttons.SuspendLayout();
        layout.Name = "layout"; layout.Dock = DockStyle.Fill; layout.Padding = new Padding(18); layout.ColumnCount = 1; layout.RowCount = 2;
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
        fields.Name = "fields"; fields.Dock = DockStyle.Fill; fields.ColumnCount = 2; fields.RowCount = 4;
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 185F)); fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        dateLabel.Name = "dateLabel"; dateLabel.Text = "Date"; dateLabel.AutoSize = true; dateLabel.Padding = new Padding(0, 6, 0, 0); dateLabel.UseMnemonic = false;
        dateInput.Name = "dateInput"; dateInput.Dock = DockStyle.Fill; dateInput.TabIndex = 0;
        dateInput.Format = DateTimePickerFormat.Custom;
        dateInput.CustomFormat = "dd MMM yyyy";
        dateInput.MinDate = new DateTime(1900, 1, 1);
        fields.Controls.Add(dateLabel, 0, 0); fields.Controls.Add(dateInput, 1, 0);
        descriptionLabel.Name = "descriptionLabel"; descriptionLabel.Text = "Description"; descriptionLabel.AutoSize = true; descriptionLabel.Padding = new Padding(0, 6, 0, 0); descriptionLabel.UseMnemonic = false;
        descriptionInput.Name = "descriptionInput"; descriptionInput.Dock = DockStyle.Fill; descriptionInput.TabIndex = 1;
        descriptionInput.MaxLength = 500;
        fields.Controls.Add(descriptionLabel, 0, 1); fields.Controls.Add(descriptionInput, 1, 1);
        referenceLabel.Name = "referenceLabel"; referenceLabel.Text = "Reference"; referenceLabel.AutoSize = true; referenceLabel.Padding = new Padding(0, 6, 0, 0); referenceLabel.UseMnemonic = false;
        referenceInput.Name = "referenceInput"; referenceInput.Dock = DockStyle.Fill; referenceInput.TabIndex = 2;
        referenceInput.MaxLength = 150;
        fields.Controls.Add(referenceLabel, 0, 2); fields.Controls.Add(referenceInput, 1, 2);
        amountLabel.Name = "amountLabel"; amountLabel.Text = "Signed amount (+ in / - out)"; amountLabel.AutoSize = true; amountLabel.Padding = new Padding(0, 6, 0, 0); amountLabel.UseMnemonic = false;
        amountInput.Name = "amountInput"; amountInput.Dock = DockStyle.Fill; amountInput.TabIndex = 3;
        amountInput.Minimum = -1000000000M;
        amountInput.Maximum = 1000000000M;
        amountInput.DecimalPlaces = 2;
        amountInput.ThousandsSeparator = true;
        fields.Controls.Add(amountLabel, 0, 3); fields.Controls.Add(amountInput, 1, 3);
        buttons.Name = "buttons"; buttons.Dock = DockStyle.Fill; buttons.FlowDirection = FlowDirection.RightToLeft;
        saveButton.Name = "saveButton"; saveButton.Text = "Save"; saveButton.AutoSize = true; saveButton.Click += SaveClicked;
        cancelButton.Name = "cancelButton"; cancelButton.Text = "Cancel"; cancelButton.AutoSize = true; cancelButton.DialogResult = DialogResult.Cancel;
        buttons.Controls.Add(cancelButton); buttons.Controls.Add(saveButton); layout.Controls.Add(fields, 0, 0); layout.Controls.Add(buttons, 0, 1); Controls.Add(layout);
        Name = "StatementLineForm"; Text = "Add bank / cashbook statement line"; ClientSize = new Size(760, 380); MinimumSize = new Size(650, 400); StartPosition = FormStartPosition.CenterParent; CancelButton = cancelButton;
        AutoScaleDimensions = new SizeF(7F, 15F); AutoScaleMode = AutoScaleMode.Font;
        ((System.ComponentModel.ISupportInitialize)amountInput).EndInit();
        buttons.ResumeLayout(false); buttons.PerformLayout(); fields.ResumeLayout(false); fields.PerformLayout(); layout.ResumeLayout(false); ResumeLayout(false);
    }
}
