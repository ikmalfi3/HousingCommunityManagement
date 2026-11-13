#nullable enable
namespace HousingCommunityManagement;
partial class CashReversalForm
{
    private DateTimePicker date = null !;
    private TextBox reason = null !;
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
        layout.ColumnCount = 1;
        layout.RowCount = 5;
        layout.Name = "layout";
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        date = new DateTimePicker();
        date.Format = DateTimePickerFormat.Custom;
        date.CustomFormat = "dd MMM yyyy";
        date.Dock = DockStyle.Fill;
        date.Name = "date";
        reason = new TextBox();
        reason.Multiline = true;
        reason.MaxLength = 500;
        reason.Dock = DockStyle.Fill;
        reason.Name = "reason";
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
        save.Text = "Reverse transaction";
        save.AutoSize = true;
        save.Name = "save";
        save.Click += SaveClicked;
        buttons.Controls.AddRange(new Control[] { cancel, save });
        label = new Label();
        label.Text = "Reversal date";
        label.AutoSize = true;
        label.Name = "label";
        layout.Controls.Add(label, 0, 0);
        layout.Controls.Add(date, 0, 1);
        label1 = new Label();
        label1.Text = "Reason (required)";
        label1.AutoSize = true;
        label1.Name = "label1";
        layout.Controls.Add(label1, 0, 2);
        layout.Controls.Add(reason, 0, 3);
        layout.Controls.Add(buttons, 0, 4);
        Controls.Add(layout);
        CancelButton = cancel;
        Text = "Reverse money transaction";
        ClientSize = new Size(520, 300);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "CashReversalForm";
        AutoScaleDimensions = new SizeF(7, 15);
        AutoScaleMode = AutoScaleMode.Font;
        ResumeLayout(false);
        PerformLayout();
    }

    private TableLayoutPanel layout = null !;
    private FlowLayoutPanel buttons = null !;
    private Button cancel = null !;
    private Button save = null !;
    private Label label = null !;
    private Label label1 = null !;
}
