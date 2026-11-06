#nullable enable
namespace HousingCommunityManagement;
partial class ExpenseActionForm
{
    private System.ComponentModel.IContainer? components = null;
    private TableLayoutPanel layout = null!;
    private Label heading = null!;
    private Label amountLabel = null!;
    private NumericUpDown amount = null!;
    private Label dateLabel = null!;
    private DateTimePicker date = null!;
    private Label accountLabel = null!;
    private ComboBox account = null!;
    private Label referenceLabel = null!;
    private TextBox reference = null!;
    private FlowLayoutPanel buttons = null!;
    private Button save = null!;
    private Button cancel = null!;
    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }
    private void InitializeComponent()
    {
        layout = new TableLayoutPanel();
        heading = new Label();
        amountLabel = new Label();
        amount = new NumericUpDown();
        dateLabel = new Label();
        date = new DateTimePicker();
        accountLabel = new Label();
        account = new ComboBox();
        referenceLabel = new Label();
        reference = new TextBox();
        buttons = new FlowLayoutPanel();
        cancel = new Button();
        save = new Button();
        layout.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)amount).BeginInit();
        buttons.SuspendLayout();
        SuspendLayout();
        // 
        // layout
        // 
        layout.ColumnCount = 2;
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170F));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        layout.Controls.Add(heading, 0, 0);
        layout.Controls.Add(amountLabel, 0, 1);
        layout.Controls.Add(amount, 1, 1);
        layout.Controls.Add(dateLabel, 0, 2);
        layout.Controls.Add(date, 1, 2);
        layout.Controls.Add(accountLabel, 0, 3);
        layout.Controls.Add(account, 1, 3);
        layout.Controls.Add(referenceLabel, 0, 4);
        layout.Controls.Add(reference, 1, 4);
        layout.Controls.Add(buttons, 0, 5);
        layout.Dock = DockStyle.Fill;
        layout.Location = new Point(0, 0);
        layout.Name = "layout";
        layout.Padding = new Padding(20);
        layout.RowCount = 6;
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 75F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 95F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 45F));
        layout.Size = new Size(600, 390);
        layout.TabIndex = 0;
        // 
        // heading
        // 
        layout.SetColumnSpan(heading, 2);
        heading.Dock = DockStyle.Fill;
        heading.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        heading.Location = new Point(23, 20);
        heading.Name = "heading";
        heading.Size = new Size(554, 75);
        heading.TabIndex = 0;
        // 
        // amountLabel
        // 
        amountLabel.Dock = DockStyle.Fill;
        amountLabel.Location = new Point(23, 95);
        amountLabel.Name = "amountLabel";
        amountLabel.Size = new Size(164, 40);
        amountLabel.TabIndex = 1;
        amountLabel.Text = "Amount (RM)";
        // 
        // amount
        // 
        amount.DecimalPlaces = 2;
        amount.Dock = DockStyle.Fill;
        amount.Location = new Point(193, 98);
        amount.Maximum = new decimal(new int[] { 1000000000, 0, 0, 0 });
        amount.Name = "amount";
        amount.Size = new Size(384, 23);
        amount.TabIndex = 2;
        amount.ThousandsSeparator = true;
        // 
        // dateLabel
        // 
        dateLabel.Dock = DockStyle.Fill;
        dateLabel.Location = new Point(23, 135);
        dateLabel.Name = "dateLabel";
        dateLabel.Size = new Size(164, 40);
        dateLabel.TabIndex = 3;
        dateLabel.Text = "Date";
        // 
        // date
        // 
        date.Dock = DockStyle.Fill;
        date.Format = DateTimePickerFormat.Short;
        date.Location = new Point(193, 138);
        date.Name = "date";
        date.Size = new Size(384, 23);
        date.TabIndex = 4;
        // 
        // accountLabel
        // 
        accountLabel.Dock = DockStyle.Fill;
        accountLabel.Location = new Point(23, 175);
        accountLabel.Name = "accountLabel";
        accountLabel.Size = new Size(164, 40);
        accountLabel.TabIndex = 5;
        accountLabel.Text = "Paid from";
        // 
        // account
        // 
        account.Dock = DockStyle.Fill;
        account.DropDownStyle = ComboBoxStyle.DropDownList;
        account.Location = new Point(193, 178);
        account.Name = "account";
        account.Size = new Size(384, 23);
        account.TabIndex = 6;
        // 
        // referenceLabel
        // 
        referenceLabel.Dock = DockStyle.Fill;
        referenceLabel.Location = new Point(23, 215);
        referenceLabel.Name = "referenceLabel";
        referenceLabel.Size = new Size(164, 95);
        referenceLabel.TabIndex = 7;
        referenceLabel.Text = "Reference / reason";
        // 
        // reference
        // 
        reference.Dock = DockStyle.Fill;
        reference.Location = new Point(193, 218);
        reference.MaxLength = 500;
        reference.Multiline = true;
        reference.Name = "reference";
        reference.Size = new Size(384, 89);
        reference.TabIndex = 8;
        // 
        // buttons
        // 
        layout.SetColumnSpan(buttons, 2);
        buttons.Controls.Add(cancel);
        buttons.Controls.Add(save);
        buttons.Dock = DockStyle.Fill;
        buttons.FlowDirection = FlowDirection.RightToLeft;
        buttons.Location = new Point(23, 313);
        buttons.Name = "buttons";
        buttons.Size = new Size(554, 54);
        buttons.TabIndex = 9;
        // 
        // cancel
        // 
        cancel.AutoSize = true;
        cancel.DialogResult = DialogResult.Cancel;
        cancel.Location = new Point(476, 3);
        cancel.Name = "cancel";
        cancel.Size = new Size(75, 25);
        cancel.TabIndex = 0;
        cancel.Text = "Cancel";
        // 
        // save
        // 
        save.AutoSize = true;
        save.Location = new Point(395, 3);
        save.Name = "save";
        save.Size = new Size(75, 25);
        save.TabIndex = 1;
        save.Text = "Record";
        save.Click += SaveClicked;
        // 
        // ExpenseActionForm
        // 
        AcceptButton = save;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = cancel;
        ClientSize = new Size(600, 390);
        Controls.Add(layout);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "ExpenseActionForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Expense action";
        layout.ResumeLayout(false);
        layout.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)amount).EndInit();
        buttons.ResumeLayout(false);
        buttons.PerformLayout();
        ResumeLayout(false);
    }
}
