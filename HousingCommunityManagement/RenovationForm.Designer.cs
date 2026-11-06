#nullable enable
namespace HousingCommunityManagement;
partial class RenovationForm
{
    private System.ComponentModel.IContainer? components = null;
    private ComboBox house = null!;
    private TextBox description = null!;
    private DateTimePicker start = null!;
    private NumericUpDown duration = null!;
    private NumericUpDown deposit = null!;
    private Label preview = null!;
    private Button save = null!;
    protected override void Dispose(bool disposing) { if (disposing) components?.Dispose();
        base.Dispose(disposing); }
    private TableLayoutPanel layout = null!;
    private Label houseLabel = null!;
    private Label descriptionLabel = null!;
    private Label startLabel = null!;
    private Label durationLabel = null!;
    private Label depositLabel = null!;
    private FlowLayoutPanel buttons = null!;
    private Button cancel = null!;
    private void InitializeComponent()
    {
        layout = new TableLayoutPanel();
        houseLabel = new Label();
        house = new ComboBox();
        descriptionLabel = new Label();
        description = new TextBox();
        startLabel = new Label();
        start = new DateTimePicker();
        durationLabel = new Label();
        duration = new NumericUpDown();
        depositLabel = new Label();
        deposit = new NumericUpDown();
        preview = new Label();
        buttons = new FlowLayoutPanel();
        cancel = new Button();
        save = new Button();
        layout.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)duration).BeginInit();
        ((System.ComponentModel.ISupportInitialize)deposit).BeginInit();
        buttons.SuspendLayout();
        SuspendLayout();
        // 
        // layout
        // 
        layout.ColumnCount = 2;
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 155F));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        layout.Controls.Add(houseLabel, 0, 0);
        layout.Controls.Add(house, 1, 0);
        layout.Controls.Add(descriptionLabel, 0, 1);
        layout.Controls.Add(description, 1, 1);
        layout.Controls.Add(startLabel, 0, 2);
        layout.Controls.Add(start, 1, 2);
        layout.Controls.Add(durationLabel, 0, 3);
        layout.Controls.Add(duration, 1, 3);
        layout.Controls.Add(depositLabel, 0, 4);
        layout.Controls.Add(deposit, 1, 4);
        layout.Controls.Add(preview, 0, 5);
        layout.Controls.Add(buttons, 0, 6);
        layout.Dock = DockStyle.Fill;
        layout.Location = new Point(0, 0);
        layout.Name = "layout";
        layout.Padding = new Padding(20);
        layout.RowCount = 7;
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 45F));
        layout.Size = new Size(580, 480);
        layout.TabIndex = 0;
        // 
        // houseLabel
        // 
        houseLabel.Dock = DockStyle.Fill;
        houseLabel.Location = new Point(23, 20);
        houseLabel.Name = "houseLabel";
        houseLabel.Size = new Size(149, 42);
        houseLabel.TabIndex = 0;
        houseLabel.Text = "House";
        houseLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // house
        // 
        house.Dock = DockStyle.Fill;
        house.DropDownStyle = ComboBoxStyle.DropDownList;
        house.Location = new Point(178, 23);
        house.Name = "house";
        house.Size = new Size(379, 23);
        house.TabIndex = 1;
        // 
        // descriptionLabel
        // 
        descriptionLabel.Dock = DockStyle.Fill;
        descriptionLabel.Location = new Point(23, 62);
        descriptionLabel.Name = "descriptionLabel";
        descriptionLabel.Size = new Size(149, 100);
        descriptionLabel.TabIndex = 2;
        descriptionLabel.Text = "Description";
        descriptionLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // description
        // 
        description.Dock = DockStyle.Fill;
        description.Location = new Point(178, 65);
        description.MaxLength = 500;
        description.Multiline = true;
        description.Name = "description";
        description.Size = new Size(379, 94);
        description.TabIndex = 3;
        // 
        // startLabel
        // 
        startLabel.Dock = DockStyle.Fill;
        startLabel.Location = new Point(23, 162);
        startLabel.Name = "startLabel";
        startLabel.Size = new Size(149, 42);
        startLabel.TabIndex = 4;
        startLabel.Text = "Start date";
        startLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // start
        // 
        start.Dock = DockStyle.Fill;
        start.Format = DateTimePickerFormat.Short;
        start.Location = new Point(178, 165);
        start.Name = "start";
        start.Size = new Size(379, 23);
        start.TabIndex = 5;
        start.ValueChanged += UpdatePreview;
        // 
        // durationLabel
        // 
        durationLabel.Dock = DockStyle.Fill;
        durationLabel.Location = new Point(23, 204);
        durationLabel.Name = "durationLabel";
        durationLabel.Size = new Size(149, 42);
        durationLabel.TabIndex = 6;
        durationLabel.Text = "Duration (days)";
        durationLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // duration
        // 
        duration.Dock = DockStyle.Fill;
        duration.Location = new Point(178, 207);
        duration.Maximum = new decimal(new int[] { 3650, 0, 0, 0 });
        duration.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        duration.Name = "duration";
        duration.Size = new Size(379, 23);
        duration.TabIndex = 7;
        duration.Value = new decimal(new int[] { 30, 0, 0, 0 });
        duration.ValueChanged += UpdatePreview;
        // 
        // depositLabel
        // 
        depositLabel.Dock = DockStyle.Fill;
        depositLabel.Location = new Point(23, 246);
        depositLabel.Name = "depositLabel";
        depositLabel.Size = new Size(149, 42);
        depositLabel.TabIndex = 8;
        depositLabel.Text = "One-off deposit (RM)";
        depositLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // deposit
        // 
        deposit.DecimalPlaces = 2;
        deposit.Dock = DockStyle.Fill;
        deposit.Location = new Point(178, 249);
        deposit.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
        deposit.Name = "deposit";
        deposit.Size = new Size(379, 23);
        deposit.TabIndex = 9;
        deposit.ThousandsSeparator = true;
        // 
        // preview
        // 
        layout.SetColumnSpan(preview, 2);
        preview.Dock = DockStyle.Fill;
        preview.Location = new Point(23, 288);
        preview.Name = "preview";
        preview.Size = new Size(534, 60);
        preview.TabIndex = 10;
        preview.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // buttons
        // 
        layout.SetColumnSpan(buttons, 2);
        buttons.Controls.Add(cancel);
        buttons.Controls.Add(save);
        buttons.Dock = DockStyle.Fill;
        buttons.FlowDirection = FlowDirection.RightToLeft;
        buttons.Location = new Point(23, 351);
        buttons.Name = "buttons";
        buttons.Size = new Size(534, 106);
        buttons.TabIndex = 11;
        // 
        // cancel
        // 
        cancel.AutoSize = true;
        cancel.DialogResult = DialogResult.Cancel;
        cancel.Location = new Point(456, 3);
        cancel.Name = "cancel";
        cancel.Size = new Size(75, 25);
        cancel.TabIndex = 0;
        cancel.Text = "Cancel";
        // 
        // save
        // 
        save.AutoSize = true;
        save.Location = new Point(375, 3);
        save.Name = "save";
        save.Size = new Size(75, 25);
        save.TabIndex = 1;
        save.Text = "Save";
        save.Click += SaveClicked;
        // 
        // RenovationForm
        // 
        AcceptButton = save;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = cancel;
        ClientSize = new Size(580, 480);
        Controls.Add(layout);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "RenovationForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "New renovation";
        layout.ResumeLayout(false);
        layout.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)duration).EndInit();
        ((System.ComponentModel.ISupportInitialize)deposit).EndInit();
        buttons.ResumeLayout(false);
        buttons.PerformLayout();
        ResumeLayout(false);
    }
}
