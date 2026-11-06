namespace HousingCommunityManagement;
partial class CommunitySettingsForm
{
    private System.ComponentModel.IContainer components = null;
    private TableLayoutPanel table;
    private FlowLayoutPanel buttons;
    private Button save;
    private Button cancel;
    private NumericUpDown fee;
    private Label feeLabel;
    private TextBox flagNames;
    private Label flagNamesLabel;
    private Label note;
    private Label noteLabel;
    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }
    private void InitializeComponent()
    {
        table = new TableLayoutPanel();
        feeLabel = new Label();
        fee = new NumericUpDown();
        flagNamesLabel = new Label();
        flagNames = new TextBox();
        noteLabel = new Label();
        note = new Label();
        buttons = new FlowLayoutPanel();
        cancel = new Button();
        save = new Button();
        table.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)fee).BeginInit();
        buttons.SuspendLayout();
        SuspendLayout();
        // 
        // table
        // 
        table.ColumnCount = 2;
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180F));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        table.Controls.Add(feeLabel, 0, 0);
        table.Controls.Add(fee, 1, 0);
        table.Controls.Add(flagNamesLabel, 0, 1);
        table.Controls.Add(flagNames, 1, 1);
        table.Controls.Add(noteLabel, 0, 2);
        table.Controls.Add(note, 1, 2);
        table.Controls.Add(buttons, 0, 3);
        table.Dock = DockStyle.Fill;
        table.Location = new Point(0, 0);
        table.Name = "table";
        table.Padding = new Padding(20);
        table.RowCount = 4;
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 240F));
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 65F));
        table.RowStyles.Add(new RowStyle());
        table.Size = new Size(640, 440);
        table.TabIndex = 0;
        // 
        // feeLabel
        // 
        feeLabel.AutoSize = true;
        feeLabel.Location = new Point(23, 20);
        feeLabel.Name = "feeLabel";
        feeLabel.Size = new Size(150, 30);
        feeLabel.TabIndex = 0;
        feeLabel.Text = "Monthly maintenance rate (RM)";
        // 
        // fee
        // 
        fee.DecimalPlaces = 2;
        fee.Dock = DockStyle.Fill;
        fee.Location = new Point(203, 23);
        fee.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
        fee.Name = "fee";
        fee.Size = new Size(414, 23);
        fee.TabIndex = 0;
        fee.ThousandsSeparator = true;
        // 
        // flagNamesLabel
        // 
        flagNamesLabel.AutoSize = true;
        flagNamesLabel.Location = new Point(23, 70);
        flagNamesLabel.Name = "flagNamesLabel";
        flagNamesLabel.Size = new Size(156, 15);
        flagNamesLabel.TabIndex = 1;
        flagNamesLabel.Text = "Available flags (one per line)";
        // 
        // flagNames
        // 
        flagNames.Dock = DockStyle.Fill;
        flagNames.Location = new Point(203, 73);
        flagNames.Multiline = true;
        flagNames.Name = "flagNames";
        flagNames.ScrollBars = ScrollBars.Vertical;
        flagNames.Size = new Size(414, 234);
        flagNames.TabIndex = 1;
        // 
        // noteLabel
        // 
        noteLabel.AutoSize = true;
        noteLabel.Location = new Point(23, 310);
        noteLabel.Name = "noteLabel";
        noteLabel.Size = new Size(34, 15);
        noteLabel.TabIndex = 2;
        noteLabel.Text = "Flags";
        // 
        // note
        // 
        note.Dock = DockStyle.Fill;
        note.Location = new Point(203, 310);
        note.Name = "note";
        note.Size = new Size(414, 65);
        note.TabIndex = 2;
        note.Text = "Add custom flags here, then select them when editing a house. Removing a flag here does not remove it from existing houses.";
        // 
        // buttons
        // 
        buttons.AutoSize = true;
        table.SetColumnSpan(buttons, 2);
        buttons.Controls.Add(cancel);
        buttons.Controls.Add(save);
        buttons.Dock = DockStyle.Fill;
        buttons.FlowDirection = FlowDirection.RightToLeft;
        buttons.Location = new Point(23, 378);
        buttons.Name = "buttons";
        buttons.Size = new Size(594, 39);
        buttons.TabIndex = 3;
        // 
        // cancel
        // 
        cancel.AutoSize = true;
        cancel.DialogResult = DialogResult.Cancel;
        cancel.Location = new Point(516, 3);
        cancel.Name = "cancel";
        cancel.Size = new Size(75, 25);
        cancel.TabIndex = 0;
        cancel.Text = "Cancel";
        // 
        // save
        // 
        save.AutoSize = true;
        save.Location = new Point(435, 3);
        save.Name = "save";
        save.Size = new Size(75, 25);
        save.TabIndex = 1;
        save.Text = "Save";
        save.Click += SaveClicked;
        // 
        // CommunitySettingsForm
        // 
        AcceptButton = save;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = cancel;
        ClientSize = new Size(640, 440);
        Controls.Add(table);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "CommunitySettingsForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Community settings";
        table.ResumeLayout(false);
        table.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)fee).EndInit();
        buttons.ResumeLayout(false);
        buttons.PerformLayout();
        ResumeLayout(false);
    }
}
