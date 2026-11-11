namespace HousingCommunityManagement;
partial class AccountingOpeningForm
{
    private DateTimePicker openingDate = null!;
    private DataGridView balances = null!;
    private void InitializeComponent()
    {
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(18), ColumnCount = 1, RowCount = 4 };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 110));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        var note = new Label { Dock = DockStyle.Fill, Text = "Set your accounting starting point\nChoose the opening date and enter cash/bank balances brought forward into that date, including held renovation deposits. Balances are brought forward before this date. Existing activity on or after it posts at its original dates. Older charges with no charge date are brought forward. Setup is locked after starting." };
        var dateArea = new FlowLayoutPanel { Dock = DockStyle.Fill };
        dateArea.Controls.Add(new Label { Text = "Opening date", AutoSize = true, Margin = new Padding(0, 7, 18, 0) });
        openingDate = new DateTimePicker { Format = DateTimePickerFormat.Custom, CustomFormat = "dd MMM yyyy", MinDate = new DateTime(1900, 1, 1), Width = 180 };
        dateArea.Controls.Add(openingDate);
        balances = new DataGridView { Dock = DockStyle.Fill, AllowUserToAddRows = false, AllowUserToDeleteRows = false, RowHeadersVisible = false, BackgroundColor = Color.White, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
        balances.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Cash / bank account", ReadOnly = true, FillWeight = 65 });
        balances.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Opening balance (RM)", FillWeight = 35 });
        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(0, 8, 0, 0) };
        var cancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, AutoSize = true };
        var start = new Button { Text = "Start accounting", AutoSize = true, BackColor = Color.FromArgb(43, 65, 88), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
        start.Click += StartClicked;
        buttons.Controls.Add(cancel); buttons.Controls.Add(start);
        layout.Controls.Add(note, 0, 0); layout.Controls.Add(dateArea, 0, 1); layout.Controls.Add(balances, 0, 2); layout.Controls.Add(buttons, 0, 3);
        Controls.Add(layout);
        CancelButton = cancel;
        Text = "Start accounting - opening balances";
        ClientSize = new Size(760, 480);
        MinimumSize = new Size(700, 450);
        StartPosition = FormStartPosition.CenterParent;
        MaximizeBox = false;
        MinimizeBox = false;
        AutoScaleDimensions = new SizeF(7, 15);
        AutoScaleMode = AutoScaleMode.Font;
    }
}
