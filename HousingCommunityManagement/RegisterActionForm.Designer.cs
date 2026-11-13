#nullable enable
namespace HousingCommunityManagement;
partial class RegisterActionForm
{
    private TableLayoutPanel layout = null!;
    private TableLayoutPanel fields = null!;
    private FlowLayoutPanel buttons = null!;
    private Button saveButton = null!;
    private Button cancelButton = null!;
    private Label actionLabel = null!;
    private ComboBox actionInput = null!;
    private Label dateLabel = null!;
    private DateTimePicker dateInput = null!;
    private Label amountLabel = null!;
    private NumericUpDown amountInput = null!;
    private Label expenseLabel = null!;
    private ComboBox expenseInput = null!;
    private Label billLabel = null!;
    private ComboBox billInput = null!;
    private Label reasonLabel = null!;
    private TextBox reasonInput = null!;
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
        actionLabel = new Label(); actionInput = new ComboBox();
        dateLabel = new Label(); dateInput = new DateTimePicker();
        amountLabel = new Label(); amountInput = new NumericUpDown();
        expenseLabel = new Label(); expenseInput = new ComboBox();
        billLabel = new Label(); billInput = new ComboBox();
        reasonLabel = new Label(); reasonInput = new TextBox();
        ((System.ComponentModel.ISupportInitialize)amountInput).BeginInit();
        SuspendLayout(); layout.SuspendLayout(); fields.SuspendLayout(); buttons.SuspendLayout();
        layout.Name = "layout"; layout.Dock = DockStyle.Fill; layout.Padding = new Padding(18); layout.ColumnCount = 1; layout.RowCount = 2;
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
        fields.Name = "fields"; fields.Dock = DockStyle.Fill; fields.ColumnCount = 2; fields.RowCount = 6;
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 185F)); fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        actionLabel.Name = "actionLabel"; actionLabel.Text = "Action"; actionLabel.AutoSize = true; actionLabel.Padding = new Padding(0, 6, 0, 0); actionLabel.UseMnemonic = false;
        actionInput.Name = "actionInput"; actionInput.Dock = DockStyle.Fill; actionInput.TabIndex = 0;
        actionInput.DropDownStyle = ComboBoxStyle.DropDownList;
        fields.Controls.Add(actionLabel, 0, 0); fields.Controls.Add(actionInput, 1, 0);
        dateLabel.Name = "dateLabel"; dateLabel.Text = "Date"; dateLabel.AutoSize = true; dateLabel.Padding = new Padding(0, 6, 0, 0); dateLabel.UseMnemonic = false;
        dateInput.Name = "dateInput"; dateInput.Dock = DockStyle.Fill; dateInput.TabIndex = 1;
        dateInput.Format = DateTimePickerFormat.Custom;
        dateInput.CustomFormat = "dd MMM yyyy";
        dateInput.MinDate = new DateTime(1900, 1, 1);
        fields.Controls.Add(dateLabel, 0, 1); fields.Controls.Add(dateInput, 1, 1);
        amountLabel.Name = "amountLabel"; amountLabel.Text = "Amount (+/- for corrections)"; amountLabel.AutoSize = true; amountLabel.Padding = new Padding(0, 6, 0, 0); amountLabel.UseMnemonic = false;
        amountInput.Name = "amountInput"; amountInput.Dock = DockStyle.Fill; amountInput.TabIndex = 2;
        amountInput.Minimum = -1000000000M;
        amountInput.Maximum = 1000000000M;
        amountInput.DecimalPlaces = 2;
        amountInput.ThousandsSeparator = true;
        fields.Controls.Add(amountLabel, 0, 2); fields.Controls.Add(amountInput, 1, 2);
        expenseLabel.Name = "expenseLabel"; expenseLabel.Text = "Expense / loss account"; expenseLabel.AutoSize = true; expenseLabel.Padding = new Padding(0, 6, 0, 0); expenseLabel.UseMnemonic = false;
        expenseInput.Name = "expenseInput"; expenseInput.Dock = DockStyle.Fill; expenseInput.TabIndex = 3;
        expenseInput.DropDownStyle = ComboBoxStyle.DropDownList;
        fields.Controls.Add(expenseLabel, 0, 3); fields.Controls.Add(expenseInput, 1, 3);
        billLabel.Name = "billLabel"; billLabel.Text = "Bill (Apply to bill only)"; billLabel.AutoSize = true; billLabel.Padding = new Padding(0, 6, 0, 0); billLabel.UseMnemonic = false;
        billInput.Name = "billInput"; billInput.Dock = DockStyle.Fill; billInput.TabIndex = 4;
        billInput.DropDownStyle = ComboBoxStyle.DropDownList;
        fields.Controls.Add(billLabel, 0, 4); fields.Controls.Add(billInput, 1, 4);
        reasonLabel.Name = "reasonLabel"; reasonLabel.Text = "Description / reason"; reasonLabel.AutoSize = true; reasonLabel.Padding = new Padding(0, 6, 0, 0); reasonLabel.UseMnemonic = false;
        reasonInput.Name = "reasonInput"; reasonInput.Dock = DockStyle.Fill; reasonInput.TabIndex = 5;
        reasonInput.MaxLength = 500;
        fields.Controls.Add(reasonLabel, 0, 5); fields.Controls.Add(reasonInput, 1, 5);
        buttons.Name = "buttons"; buttons.Dock = DockStyle.Fill; buttons.FlowDirection = FlowDirection.RightToLeft;
        saveButton.Name = "saveButton"; saveButton.Text = "Save"; saveButton.AutoSize = true; saveButton.Click += SaveClicked;
        cancelButton.Name = "cancelButton"; cancelButton.Text = "Cancel"; cancelButton.AutoSize = true; cancelButton.DialogResult = DialogResult.Cancel;
        buttons.Controls.Add(cancelButton); buttons.Controls.Add(saveButton); layout.Controls.Add(fields, 0, 0); layout.Controls.Add(buttons, 0, 1); Controls.Add(layout);
        Name = "RegisterActionForm"; Text = "Register action"; ClientSize = new Size(760, 380); MinimumSize = new Size(650, 400); StartPosition = FormStartPosition.CenterParent; CancelButton = cancelButton;
        AutoScaleDimensions = new SizeF(7F, 15F); AutoScaleMode = AutoScaleMode.Font;
        ((System.ComponentModel.ISupportInitialize)amountInput).EndInit();
        buttons.ResumeLayout(false); buttons.PerformLayout(); fields.ResumeLayout(false); fields.PerformLayout(); layout.ResumeLayout(false); ResumeLayout(false);
    }
}
