namespace HousingCommunityManagement;
partial class HouseRangeForm
{
    private System.ComponentModel.IContainer components = null;
    private TableLayoutPanel table;
    private FlowLayoutPanel buttons;
    private Button save;
    private Button cancel;
    private TextBox prefix;
    private Label prefixLabel;
    private NumericUpDown start;
    private Label startLabel;
    private NumericUpDown end;
    private Label endLabel;
    private NumericUpDown digits;
    private Label digitsLabel;
    private Label preview;
    private Label previewLabel;
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
        prefixLabel = new Label();
        prefix = new TextBox();
        startLabel = new Label();
        start = new NumericUpDown();
        endLabel = new Label();
        end = new NumericUpDown();
        digitsLabel = new Label();
        digits = new NumericUpDown();
        previewLabel = new Label();
        preview = new Label();
        noteLabel = new Label();
        note = new Label();
        buttons = new FlowLayoutPanel();
        cancel = new Button();
        save = new Button();
        table.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)start).BeginInit();
        ((System.ComponentModel.ISupportInitialize)end).BeginInit();
        ((System.ComponentModel.ISupportInitialize)digits).BeginInit();
        buttons.SuspendLayout();
        SuspendLayout();
        // 
        // table
        // 
        table.ColumnCount = 2;
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180F));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        table.Controls.Add(prefixLabel, 0, 0);
        table.Controls.Add(prefix, 1, 0);
        table.Controls.Add(startLabel, 0, 1);
        table.Controls.Add(start, 1, 1);
        table.Controls.Add(endLabel, 0, 2);
        table.Controls.Add(end, 1, 2);
        table.Controls.Add(digitsLabel, 0, 3);
        table.Controls.Add(digits, 1, 3);
        table.Controls.Add(previewLabel, 0, 4);
        table.Controls.Add(preview, 1, 4);
        table.Controls.Add(noteLabel, 0, 5);
        table.Controls.Add(note, 1, 5);
        table.Controls.Add(buttons, 0, 6);
        table.Dock = DockStyle.Fill;
        table.Location = new Point(0, 0);
        table.Name = "table";
        table.Padding = new Padding(20);
        table.RowCount = 7;
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 55F));
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
        table.RowStyles.Add(new RowStyle());
        table.Size = new Size(640, 340);
        table.TabIndex = 0;
        // 
        // prefixLabel
        // 
        prefixLabel.AutoSize = true;
        prefixLabel.Location = new Point(23, 20);
        prefixLabel.Name = "prefixLabel";
        prefixLabel.Size = new Size(82, 15);
        prefixLabel.TabIndex = 0;
        prefixLabel.Text = "Prefix (e.g. A-)";
        // 
        // prefix
        // 
        prefix.Dock = DockStyle.Fill;
        prefix.Location = new Point(203, 23);
        prefix.MaxLength = 40;
        prefix.Name = "prefix";
        prefix.Size = new Size(414, 23);
        prefix.TabIndex = 0;
        // 
        // startLabel
        // 
        startLabel.AutoSize = true;
        startLabel.Location = new Point(23, 56);
        startLabel.Name = "startLabel";
        startLabel.Size = new Size(93, 15);
        startLabel.TabIndex = 1;
        startLabel.Text = "Starting number";
        // 
        // start
        // 
        start.Dock = DockStyle.Fill;
        start.Location = new Point(203, 59);
        start.Maximum = new decimal(new int[] { 999999999, 0, 0, 0 });
        start.Name = "start";
        start.Size = new Size(414, 23);
        start.TabIndex = 1;
        start.Value = new decimal(new int[] { 1, 0, 0, 0 });
        // 
        // endLabel
        // 
        endLabel.AutoSize = true;
        endLabel.Location = new Point(23, 92);
        endLabel.Name = "endLabel";
        endLabel.Size = new Size(89, 15);
        endLabel.TabIndex = 2;
        endLabel.Text = "Ending number";
        // 
        // end
        // 
        end.Dock = DockStyle.Fill;
        end.Location = new Point(203, 95);
        end.Maximum = new decimal(new int[] { 999999999, 0, 0, 0 });
        end.Name = "end";
        end.Size = new Size(414, 23);
        end.TabIndex = 2;
        end.Value = new decimal(new int[] { 100, 0, 0, 0 });
        // 
        // digitsLabel
        // 
        digitsLabel.AutoSize = true;
        digitsLabel.Location = new Point(23, 128);
        digitsLabel.Name = "digitsLabel";
        digitsLabel.Size = new Size(137, 15);
        digitsLabel.TabIndex = 3;
        digitsLabel.Text = "Minimum number digits";
        // 
        // digits
        // 
        digits.Dock = DockStyle.Fill;
        digits.Location = new Point(203, 131);
        digits.Maximum = new decimal(new int[] { 9, 0, 0, 0 });
        digits.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        digits.Name = "digits";
        digits.Size = new Size(414, 23);
        digits.TabIndex = 3;
        digits.Value = new decimal(new int[] { 1, 0, 0, 0 });
        // 
        // previewLabel
        // 
        previewLabel.AutoSize = true;
        previewLabel.Location = new Point(23, 164);
        previewLabel.Name = "previewLabel";
        previewLabel.Size = new Size(48, 15);
        previewLabel.TabIndex = 4;
        previewLabel.Text = "Preview";
        // 
        // preview
        // 
        preview.Dock = DockStyle.Fill;
        preview.Location = new Point(203, 164);
        preview.Name = "preview";
        preview.Size = new Size(414, 55);
        preview.TabIndex = 4;
        preview.Text = "1 through 100";
        // 
        // noteLabel
        // 
        noteLabel.AutoSize = true;
        noteLabel.Location = new Point(23, 219);
        noteLabel.Name = "noteLabel";
        noteLabel.Size = new Size(87, 15);
        noteLabel.TabIndex = 5;
        noteLabel.Text = "Existing houses";
        // 
        // note
        // 
        note.Dock = DockStyle.Fill;
        note.Location = new Point(203, 219);
        note.Name = "note";
        note.Size = new Size(414, 50);
        note.TabIndex = 5;
        note.Text = "Existing numbers are skipped. New houses start unowned, with no owners, contacts or flags.";
        // 
        // buttons
        // 
        buttons.AutoSize = true;
        table.SetColumnSpan(buttons, 2);
        buttons.Controls.Add(cancel);
        buttons.Controls.Add(save);
        buttons.Dock = DockStyle.Fill;
        buttons.FlowDirection = FlowDirection.RightToLeft;
        buttons.Location = new Point(23, 272);
        buttons.Name = "buttons";
        buttons.Size = new Size(594, 45);
        buttons.TabIndex = 6;
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
        // HouseRangeForm
        // 
        AcceptButton = save;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = cancel;
        ClientSize = new Size(640, 340);
        Controls.Add(table);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "HouseRangeForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Create house range";
        table.ResumeLayout(false);
        table.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)start).EndInit();
        ((System.ComponentModel.ISupportInitialize)end).EndInit();
        ((System.ComponentModel.ISupportInitialize)digits).EndInit();
        buttons.ResumeLayout(false);
        buttons.PerformLayout();
        ResumeLayout(false);
    }
}
