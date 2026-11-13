#nullable enable
namespace HousingCommunityManagement;
partial class ReasonEntryForm
{
    private TableLayoutPanel layout = null!;
    private TableLayoutPanel fields = null!;
    private FlowLayoutPanel buttons = null!;
    private Button saveButton = null!;
    private Button cancelButton = null!;
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
        reasonLabel = new Label(); reasonInput = new TextBox();

        SuspendLayout(); layout.SuspendLayout(); fields.SuspendLayout(); buttons.SuspendLayout();
        layout.Name = "layout"; layout.Dock = DockStyle.Fill; layout.Padding = new Padding(18); layout.ColumnCount = 1; layout.RowCount = 2;
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
        fields.Name = "fields"; fields.Dock = DockStyle.Fill; fields.ColumnCount = 2; fields.RowCount = 1;
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 185F)); fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        reasonLabel.Name = "reasonLabel"; reasonLabel.Text = "Reason (required)"; reasonLabel.AutoSize = true; reasonLabel.Padding = new Padding(0, 6, 0, 0); reasonLabel.UseMnemonic = false;
        reasonInput.Name = "reasonInput"; reasonInput.Dock = DockStyle.Fill; reasonInput.TabIndex = 0;
        reasonInput.MaxLength = 500;
        fields.Controls.Add(reasonLabel, 0, 0); fields.Controls.Add(reasonInput, 1, 0);
        buttons.Name = "buttons"; buttons.Dock = DockStyle.Fill; buttons.FlowDirection = FlowDirection.RightToLeft;
        saveButton.Name = "saveButton"; saveButton.Text = "Save"; saveButton.AutoSize = true; saveButton.Click += SaveClicked;
        cancelButton.Name = "cancelButton"; cancelButton.Text = "Cancel"; cancelButton.AutoSize = true; cancelButton.DialogResult = DialogResult.Cancel;
        buttons.Controls.Add(cancelButton); buttons.Controls.Add(saveButton); layout.Controls.Add(fields, 0, 0); layout.Controls.Add(buttons, 0, 1); Controls.Add(layout);
        Name = "ReasonEntryForm"; Text = "Reason"; ClientSize = new Size(760, 380); MinimumSize = new Size(650, 400); StartPosition = FormStartPosition.CenterParent; CancelButton = cancelButton;
        AutoScaleDimensions = new SizeF(7F, 15F); AutoScaleMode = AutoScaleMode.Font;

        buttons.ResumeLayout(false); buttons.PerformLayout(); fields.ResumeLayout(false); fields.PerformLayout(); layout.ResumeLayout(false); ResumeLayout(false);
    }
}
