#nullable enable
namespace HousingCommunityManagement;
partial class ExpenseCategoryEditForm
{
    private TextBox recordName = null !;
    private ComboBox choice = null !;
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
        recordName.MaxLength = 100;
        recordName.TabIndex = 0;
        recordName.Name = "recordName";
        choice = new ComboBox();
        choice.Name = "choice";
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
        label1.Text = "Expense account";
        label1.AutoSize = true;
        label1.Padding = new Padding(0, 5, 0, 0);
        label1.Name = "label1";
        layout.Controls.Add(label1, 0, 1);
        layout.Controls.Add(choice, 1, 1);
        layout.Controls.Add(active, 1, 2);
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
        save.Text = "Save category";
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
        Name = "ExpenseCategoryEditForm";
        AutoScaleDimensions = new SizeF(7, 15);
        AutoScaleMode = AutoScaleMode.Font;
        ResumeLayout(false);
        PerformLayout();
    }

    private TableLayoutPanel layout = null !;
    private Label label = null !;
    private Label label1 = null !;
    private FlowLayoutPanel buttons = null !;
    private Button cancel = null !;
    private Button save = null !;
}
