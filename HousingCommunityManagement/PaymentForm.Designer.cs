namespace HousingCommunityManagement;
partial class PaymentForm
{
    private System.ComponentModel.IContainer components = null;
    private TableLayoutPanel layout;
    private Label heading;
    private Label amountLabel;
    private NumericUpDown amount;
    private Label dateLabel;
    private DateTimePicker date;
    private Label referenceLabel;
    private TextBox reference;
    private FlowLayoutPanel buttons;
    private Button save;
    private Button cancel;
    private Label cashAccountLabel;
    private ComboBox cashAccount;
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
        referenceLabel = new Label();
        reference = new TextBox();
        cashAccountLabel = new Label();
        cashAccount = new ComboBox();
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
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        layout.Controls.Add(heading, 0, 0);
        layout.Controls.Add(amountLabel, 0, 1);
        layout.Controls.Add(amount, 1, 1);
        layout.Controls.Add(dateLabel, 0, 2);
        layout.Controls.Add(date, 1, 2);
        layout.Controls.Add(referenceLabel, 0, 3);
        layout.Controls.Add(reference, 1, 3);
        layout.Controls.Add(cashAccountLabel, 0, 4);
        layout.Controls.Add(cashAccount, 1, 4);
        layout.Controls.Add(buttons, 0, 5);
        layout.Dock = DockStyle.Fill;
        layout.Location = new Point(0, 0);
        layout.Name = "layout";
        layout.Padding = new Padding(20);
        layout.RowCount = 6;
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        layout.Size = new Size(480, 330);
        layout.TabIndex = 0;
        // 
        // heading
        // 
        layout.SetColumnSpan(heading, 2);
        heading.Dock = DockStyle.Fill;
        heading.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        heading.Location = new Point(23, 20);
        heading.Name = "heading";
        heading.Size = new Size(434, 70);
        heading.TabIndex = 0;
        heading.Text = "Record payment";
        // 
        // amountLabel
        // 
        amountLabel.Location = new Point(23, 90);
        amountLabel.Name = "amountLabel";
        amountLabel.Size = new Size(100, 23);
        amountLabel.TabIndex = 1;
        amountLabel.Text = "Amount (RM)";
        // 
        // amount
        // 
        amount.DecimalPlaces = 2;
        amount.Dock = DockStyle.Fill;
        amount.Location = new Point(143, 93);
        amount.Maximum = new decimal(new int[] { 3000000, 0, 0, 0 });
        amount.Name = "amount";
        amount.Size = new Size(314, 23);
        amount.TabIndex = 2;
        // 
        // dateLabel
        // 
        dateLabel.Location = new Point(23, 130);
        dateLabel.Name = "dateLabel";
        dateLabel.Size = new Size(100, 23);
        dateLabel.TabIndex = 3;
        dateLabel.Text = "Payment date";
        // 
        // date
        // 
        date.Dock = DockStyle.Fill;
        date.Format = DateTimePickerFormat.Short;
        date.Location = new Point(143, 133);
        date.Name = "date";
        date.Size = new Size(314, 23);
        date.TabIndex = 4;
        // 
        // referenceLabel
        // 
        referenceLabel.Location = new Point(23, 170);
        referenceLabel.Name = "referenceLabel";
        referenceLabel.Size = new Size(100, 23);
        referenceLabel.TabIndex = 5;
        referenceLabel.Text = "Reference";
        // 
        // reference
        // 
        reference.Dock = DockStyle.Fill;
        reference.Location = new Point(143, 173);
        reference.MaxLength = 150;
        reference.Name = "reference";
        reference.Size = new Size(314, 23);
        reference.TabIndex = 6;
        // 
        // cashAccountLabel
        // 
        cashAccountLabel.AutoSize = true;
        cashAccountLabel.Location = new Point(23, 210);
        cashAccountLabel.Name = "cashAccountLabel";
        cashAccountLabel.Size = new Size(78, 15);
        cashAccountLabel.TabIndex = 7;
        cashAccountLabel.Text = "Received into";
        // 
        // cashAccount
        // 
        cashAccount.Dock = DockStyle.Fill;
        cashAccount.DropDownStyle = ComboBoxStyle.DropDownList;
        cashAccount.Location = new Point(143, 213);
        cashAccount.Name = "cashAccount";
        cashAccount.Size = new Size(314, 23);
        cashAccount.TabIndex = 7;
        // 
        // buttons
        // 
        layout.SetColumnSpan(buttons, 2);
        buttons.Controls.Add(cancel);
        buttons.Controls.Add(save);
        buttons.Dock = DockStyle.Fill;
        buttons.FlowDirection = FlowDirection.RightToLeft;
        buttons.Location = new Point(23, 253);
        buttons.Name = "buttons";
        buttons.Size = new Size(434, 54);
        buttons.TabIndex = 7;
        // 
        // cancel
        // 
        cancel.DialogResult = DialogResult.Cancel;
        cancel.Location = new Point(356, 3);
        cancel.Name = "cancel";
        cancel.Size = new Size(75, 23);
        cancel.TabIndex = 0;
        cancel.Text = "Cancel";
        // 
        // save
        // 
        save.Location = new Point(275, 3);
        save.Name = "save";
        save.Size = new Size(75, 23);
        save.TabIndex = 1;
        save.Text = "Record";
        save.Click += SaveClicked;
        // 
        // PaymentForm
        // 
        AcceptButton = save;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = cancel;
        ClientSize = new Size(480, 330);
        Controls.Add(layout);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "PaymentForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Record payment";
        layout.ResumeLayout(false);
        layout.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)amount).EndInit();
        buttons.ResumeLayout(false);
        ResumeLayout(false);
    }
}
