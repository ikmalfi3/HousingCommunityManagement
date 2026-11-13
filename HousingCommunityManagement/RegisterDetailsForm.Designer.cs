#nullable enable
namespace HousingCommunityManagement;
partial class RegisterDetailsForm
{
    private TableLayoutPanel layout = null!;
    private TableLayoutPanel fields = null!;
    private FlowLayoutPanel buttons = null!;
    private Button saveButton = null!;
    private Button cancelButton = null!;
    private Label nameLabel = null!;
    private TextBox nameInput = null!;
    private Label partyLabel = null!;
    private TextBox partyInput = null!;
    private Label locationLabel = null!;
    private TextBox locationInput = null!;
    private Label notesLabel = null!;
    private TextBox notesInput = null!;
    private Label depreciationLabel = null!;
    private ComboBox depreciationInput = null!;
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
        nameLabel = new Label(); nameInput = new TextBox();
        partyLabel = new Label(); partyInput = new TextBox();
        locationLabel = new Label(); locationInput = new TextBox();
        notesLabel = new Label(); notesInput = new TextBox();
        depreciationLabel = new Label(); depreciationInput = new ComboBox();

        SuspendLayout(); layout.SuspendLayout(); fields.SuspendLayout(); buttons.SuspendLayout();
        layout.Name = "layout"; layout.Dock = DockStyle.Fill; layout.Padding = new Padding(18); layout.ColumnCount = 1; layout.RowCount = 2;
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
        fields.Name = "fields"; fields.Dock = DockStyle.Fill; fields.ColumnCount = 2; fields.RowCount = 5;
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 185F)); fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        nameLabel.Name = "nameLabel"; nameLabel.Text = "Name / item"; nameLabel.AutoSize = true; nameLabel.Padding = new Padding(0, 6, 0, 0); nameLabel.UseMnemonic = false;
        nameInput.Name = "nameInput"; nameInput.Dock = DockStyle.Fill; nameInput.TabIndex = 0;
        nameInput.MaxLength = 200;
        fields.Controls.Add(nameLabel, 0, 0); fields.Controls.Add(nameInput, 1, 0);
        partyLabel.Name = "partyLabel"; partyLabel.Text = "Supplier / party"; partyLabel.AutoSize = true; partyLabel.Padding = new Padding(0, 6, 0, 0); partyLabel.UseMnemonic = false;
        partyInput.Name = "partyInput"; partyInput.Dock = DockStyle.Fill; partyInput.TabIndex = 1;
        partyInput.MaxLength = 150;
        fields.Controls.Add(partyLabel, 0, 1); fields.Controls.Add(partyInput, 1, 1);
        locationLabel.Name = "locationLabel"; locationLabel.Text = "Location / serial"; locationLabel.AutoSize = true; locationLabel.Padding = new Padding(0, 6, 0, 0); locationLabel.UseMnemonic = false;
        locationInput.Name = "locationInput"; locationInput.Dock = DockStyle.Fill; locationInput.TabIndex = 2;
        locationInput.MaxLength = 200;
        fields.Controls.Add(locationLabel, 0, 2); fields.Controls.Add(locationInput, 1, 2);
        notesLabel.Name = "notesLabel"; notesLabel.Text = "Notes"; notesLabel.AutoSize = true; notesLabel.Padding = new Padding(0, 6, 0, 0); notesLabel.UseMnemonic = false;
        notesInput.Name = "notesInput"; notesInput.Dock = DockStyle.Fill; notesInput.TabIndex = 3;
        notesInput.MaxLength = 1000;
        fields.Controls.Add(notesLabel, 0, 3); fields.Controls.Add(notesInput, 1, 3);
        depreciationLabel.Name = "depreciationLabel"; depreciationLabel.Text = "Accum. depreciation account"; depreciationLabel.AutoSize = true; depreciationLabel.Padding = new Padding(0, 6, 0, 0); depreciationLabel.UseMnemonic = false;
        depreciationInput.Name = "depreciationInput"; depreciationInput.Dock = DockStyle.Fill; depreciationInput.TabIndex = 4;
        depreciationInput.DropDownStyle = ComboBoxStyle.DropDownList;
        fields.Controls.Add(depreciationLabel, 0, 4); fields.Controls.Add(depreciationInput, 1, 4);
        buttons.Name = "buttons"; buttons.Dock = DockStyle.Fill; buttons.FlowDirection = FlowDirection.RightToLeft;
        saveButton.Name = "saveButton"; saveButton.Text = "Save"; saveButton.AutoSize = true; saveButton.Click += SaveClicked;
        cancelButton.Name = "cancelButton"; cancelButton.Text = "Cancel"; cancelButton.AutoSize = true; cancelButton.DialogResult = DialogResult.Cancel;
        buttons.Controls.Add(cancelButton); buttons.Controls.Add(saveButton); layout.Controls.Add(fields, 0, 0); layout.Controls.Add(buttons, 0, 1); Controls.Add(layout);
        Name = "RegisterDetailsForm"; Text = "Edit register details"; ClientSize = new Size(760, 380); MinimumSize = new Size(650, 400); StartPosition = FormStartPosition.CenterParent; CancelButton = cancelButton;
        AutoScaleDimensions = new SizeF(7F, 15F); AutoScaleMode = AutoScaleMode.Font;

        buttons.ResumeLayout(false); buttons.PerformLayout(); fields.ResumeLayout(false); fields.PerformLayout(); layout.ResumeLayout(false); ResumeLayout(false);
    }
}
