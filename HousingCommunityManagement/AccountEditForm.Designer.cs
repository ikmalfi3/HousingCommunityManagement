#nullable enable
namespace HousingCommunityManagement;
partial class AccountEditForm
{
    private TextBox code = null !;
    private TextBox accountName = null !;
    private ComboBox type = null !;
    private CheckBox cashBank = null !;
    private CheckBox active = null !;
    private Label usageNote = null !;
    private System.ComponentModel.IContainer? components = null;
    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }
    private void InitializeComponent()
    {
        layout = new TableLayoutPanel();
        label = new Label();
        code = new TextBox();
        label1 = new Label();
        accountName = new TextBox();
        label2 = new Label();
        type = new ComboBox();
        cashBank = new CheckBox();
        active = new CheckBox();
        usageNote = new Label();
        buttons = new FlowLayoutPanel();
        cancel = new Button();
        save = new Button();
        layout.SuspendLayout();
        buttons.SuspendLayout();
        SuspendLayout();
        // 
        // layout
        // 
        layout.ColumnCount = 2;
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        layout.Controls.Add(label, 0, 0);
        layout.Controls.Add(code, 1, 0);
        layout.Controls.Add(label1, 0, 1);
        layout.Controls.Add(accountName, 1, 1);
        layout.Controls.Add(label2, 0, 2);
        layout.Controls.Add(type, 1, 2);
        layout.Controls.Add(cashBank, 1, 3);
        layout.Controls.Add(active, 1, 4);
        layout.Controls.Add(usageNote, 0, 5);
        layout.Controls.Add(buttons, 0, 6);
        layout.Dock = DockStyle.Fill;
        layout.Location = new Point(0, 0);
        layout.Name = "layout";
        layout.Padding = new Padding(20);
        layout.RowCount = 7;
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 45F));
        layout.Size = new Size(520, 345);
        layout.TabIndex = 0;
        // 
        // label
        // 
        label.AutoSize = true;
        label.Location = new Point(23, 20);
        label.Name = "label";
        label.Padding = new Padding(0, 5, 0, 0);
        label.Size = new Size(35, 20);
        label.TabIndex = 0;
        label.Text = "Code";
        // 
        // code
        // 
        code.Dock = DockStyle.Fill;
        code.Location = new Point(133, 23);
        code.MaxLength = 20;
        code.Name = "code";
        code.Size = new Size(364, 23);
        code.TabIndex = 0;
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(23, 58);
        label1.Name = "label1";
        label1.Padding = new Padding(0, 5, 0, 0);
        label1.Size = new Size(39, 20);
        label1.TabIndex = 1;
        label1.Text = "Name";
        // 
        // accountName
        // 
        accountName.Dock = DockStyle.Fill;
        accountName.Location = new Point(133, 61);
        accountName.MaxLength = 100;
        accountName.Name = "accountName";
        accountName.Size = new Size(364, 23);
        accountName.TabIndex = 1;
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Location = new Point(23, 96);
        label2.Name = "label2";
        label2.Padding = new Padding(0, 5, 0, 0);
        label2.Size = new Size(78, 20);
        label2.TabIndex = 2;
        label2.Text = "Account type";
        // 
        // type
        // 
        type.Dock = DockStyle.Fill;
        type.DropDownStyle = ComboBoxStyle.DropDownList;
        type.Items.AddRange(new object[] { "Asset", "Liability", "Equity", "Income", "Expense" });
        type.Location = new Point(133, 99);
        type.Name = "type";
        type.Size = new Size(364, 23);
        type.TabIndex = 2;
        // 
        // cashBank
        // 
        cashBank.AutoSize = true;
        cashBank.Location = new Point(133, 137);
        cashBank.Name = "cashBank";
        cashBank.Size = new Size(135, 19);
        cashBank.TabIndex = 3;
        cashBank.Text = "Cash / bank account";
        // 
        // active
        // 
        active.AutoSize = true;
        active.Location = new Point(133, 175);
        active.Name = "active";
        active.Size = new Size(59, 19);
        active.TabIndex = 4;
        active.Text = "Active";
        // 
        // usageNote
        // 
        layout.SetColumnSpan(usageNote, 2);
        usageNote.Dock = DockStyle.Fill;
        usageNote.Location = new Point(23, 210);
        usageNote.Name = "usageNote";
        usageNote.Size = new Size(474, 70);
        usageNote.TabIndex = 5;
        usageNote.Text = "Account type and cash/bank designation are locked because this account has posted entries.";
        // 
        // buttons
        // 
        layout.SetColumnSpan(buttons, 2);
        buttons.Controls.Add(cancel);
        buttons.Controls.Add(save);
        buttons.Dock = DockStyle.Fill;
        buttons.FlowDirection = FlowDirection.RightToLeft;
        buttons.Location = new Point(23, 283);
        buttons.Name = "buttons";
        buttons.Size = new Size(474, 39);
        buttons.TabIndex = 6;
        // 
        // cancel
        // 
        cancel.AutoSize = true;
        cancel.DialogResult = DialogResult.Cancel;
        cancel.Location = new Point(396, 3);
        cancel.Name = "cancel";
        cancel.Size = new Size(75, 25);
        cancel.TabIndex = 0;
        cancel.Text = "Cancel";
        // 
        // save
        // 
        save.AutoSize = true;
        save.Location = new Point(303, 3);
        save.Name = "save";
        save.Size = new Size(87, 25);
        save.TabIndex = 1;
        save.Text = "Save account";
        save.Click += SaveClicked;
        // 
        // AccountEditForm
        // 
        AcceptButton = save;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = cancel;
        ClientSize = new Size(520, 345);
        Controls.Add(layout);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "AccountEditForm";
        StartPosition = FormStartPosition.CenterParent;
        layout.ResumeLayout(false);
        layout.PerformLayout();
        buttons.ResumeLayout(false);
        buttons.PerformLayout();
        ResumeLayout(false);
    }

    private TableLayoutPanel layout = null !;
    private Label label = null !;
    private Label label1 = null !;
    private Label label2 = null !;
    private FlowLayoutPanel buttons = null !;
    private Button cancel = null !;
    private Button save = null !;
}
