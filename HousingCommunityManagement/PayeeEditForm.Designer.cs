#nullable enable
namespace HousingCommunityManagement;
partial class PayeeEditForm
{
    private TextBox recordName = null !;
    private ComboBox choice = null !;
    private TextBox contact = null !;
    private CheckBox active = null !;
    private System.ComponentModel.IContainer? components = null;
    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }
    private void InitializeComponent()
    {
        SuspendLayout();
        layout = new TableLayoutPanel();
        layout.Dock = DockStyle.Fill;
        layout.Padding = new Padding(20);
        layout.ColumnCount = 2;
        layout.RowCount = 5;
        layout.Name = "layout";
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 125));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        recordName = new TextBox();
        recordName.Name = "recordName";
        recordName.Dock = DockStyle.Fill;
        recordName.MaxLength = 150;
        recordName.TabIndex = 0;
        recordName.Name = "recordName";
        choice = new ComboBox();
        choice.Name = "choice";
        choice.Items.AddRange(new object[] { "Supplier", "Claimant" });
        choice.Dock = DockStyle.Fill;
        choice.DropDownStyle = ComboBoxStyle.DropDownList;
        choice.TabIndex = 1;
        choice.Name = "choice";
        active = new CheckBox();
        active.Text = "Active";
        active.AutoSize = true;
        active.TabIndex = 3;
        active.Name = "active";
        label = new Label();
        label.Text = "Name";
        label.AutoSize = true;
        label.Padding = new Padding(0, 5, 0, 0);
        label.Name = "label";
        layout.Controls.Add(label, 0, 0);
        layout.Controls.Add(recordName, 1, 0);
        label1 = new Label();
        label1.Text = "Type";
        label1.AutoSize = true;
        label1.Padding = new Padding(0, 5, 0, 0);
        label1.Name = "label1";
        layout.Controls.Add(label1, 0, 1);
        layout.Controls.Add(choice, 1, 1);
        contact = new TextBox();
        contact.Name = "contact";
        contact.Dock = DockStyle.Fill;
        contact.MaxLength = 200;
        contact.TabIndex = 2;
        contact.Name = "contact";
        label2 = new Label();
        label2.Text = "Contact";
        label2.AutoSize = true;
        label2.Padding = new Padding(0, 5, 0, 0);
        label2.Name = "label2";
        layout.Controls.Add(label2, 0, 2);
        layout.Controls.Add(contact, 1, 2);
        layout.Controls.Add(active, 1, 3);
        buttons = new FlowLayoutPanel();
        buttons.Dock = DockStyle.Fill;
        buttons.FlowDirection = FlowDirection.RightToLeft;
        buttons.Name = "buttons";
        cancel = new Button();
        cancel.Text = "Cancel";
        cancel.DialogResult = DialogResult.Cancel;
        cancel.AutoSize = true;
        cancel.Name = "cancel";
        save = new Button();
        save.Text = "Save payee";
        save.AutoSize = true;
        save.Name = "save";
        save.Click += SaveClicked;
        buttons.Controls.Add(cancel);
        buttons.Controls.Add(save);
        layout.Controls.Add(buttons, 0, 4);
        layout.SetColumnSpan(buttons, 2);
        Controls.Add(layout);
        AcceptButton = save;
        CancelButton = cancel;
        ClientSize = new Size(580, 270);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "PayeeEditForm";
        AutoScaleDimensions = new SizeF(7, 15);
        AutoScaleMode = AutoScaleMode.Font;
        ResumeLayout(false);
        PerformLayout();
    }

    private TableLayoutPanel layout = null !;
    private Label label = null !;
    private Label label1 = null !;
    private Label label2 = null !;
    private FlowLayoutPanel buttons = null !;
    private Button cancel = null !;
    private Button save = null !;
}
