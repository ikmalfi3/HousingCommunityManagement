#nullable enable
namespace HousingCommunityManagement;
partial class DeleteDataConfirmForm
{
    private System.ComponentModel.IContainer? components = null;
    private TableLayoutPanel layout = null!;
    private Label warning = null!;
    private TextBox summary = null!;
    private Label instructions = null!;
    private Label codeLabel = null!;
    private TextBox answer = null!;
    private Label feedback = null!;
    private FlowLayoutPanel buttons = null!;
    private Button confirm = null!;
    private Button cancel = null!;
    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }
    private void InitializeComponent()
    {
        layout = new TableLayoutPanel();
        warning = new Label();
        summary = new TextBox();
        instructions = new Label();
        codeLabel = new Label();
        answer = new TextBox();
        feedback = new Label();
        buttons = new FlowLayoutPanel();
        confirm = new Button();
        cancel = new Button();
        SuspendLayout();
        layout.Name = "layout";
        warning.Name = "warning";
        summary.Name = "summary";
        instructions.Name = "instructions";
        codeLabel.Name = "codeLabel";
        answer.Name = "answer";
        feedback.Name = "feedback";
        buttons.Name = "buttons";
        confirm.Name = "confirm";
        cancel.Name = "cancel";
        layout.Dock = DockStyle.Fill;
        layout.Padding = new Padding(20);
        layout.ColumnCount = 1;
        layout.RowCount = 7;
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 55F));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 55F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
        warning.Text = "Permanently delete the data listed below? This cannot be undone. Review every category before confirming.";
        warning.Dock = DockStyle.Fill;
        warning.ForeColor = Color.Firebrick;
        summary.Dock = DockStyle.Fill;
        summary.Multiline = true;
        summary.ReadOnly = true;
        summary.ScrollBars = ScrollBars.Vertical;
        summary.TabStop = false;
        instructions.Text = "Type the code exactly as shown (lowercase letters and numbers).";
        instructions.Dock = DockStyle.Fill;
        instructions.TextAlign = ContentAlignment.BottomLeft;
        codeLabel.Text = "a1b2c3";
        codeLabel.Font = new Font("Consolas", 23F, FontStyle.Bold);
        codeLabel.Dock = DockStyle.Fill;
        codeLabel.TextAlign = ContentAlignment.MiddleCenter;
        answer.Dock = DockStyle.Top;
        answer.MaxLength = 6;
        answer.TextAlign = HorizontalAlignment.Center;
        answer.Font = new Font("Consolas", 16F);
        answer.TextChanged += AnswerChanged;
        feedback.Text = "Complete the code to enable deletion.";
        feedback.Dock = DockStyle.Fill;
        buttons.Dock = DockStyle.Fill;
        buttons.FlowDirection = FlowDirection.RightToLeft;
        cancel.Text = "Cancel";
        cancel.AutoSize = true;
        cancel.DialogResult = DialogResult.Cancel;
        confirm.Text = "Delete permanently";
        confirm.AutoSize = true;
        confirm.ForeColor = Color.Firebrick;
        confirm.Enabled = false;
        confirm.Click += ConfirmClicked;
        buttons.Controls.Add(cancel);
        buttons.Controls.Add(confirm);
        layout.Controls.Add(warning, 0, 0);
        layout.Controls.Add(summary, 0, 1);
        layout.Controls.Add(instructions, 0, 2);
        layout.Controls.Add(codeLabel, 0, 3);
        layout.Controls.Add(answer, 0, 4);
        layout.Controls.Add(feedback, 0, 5);
        layout.Controls.Add(buttons, 0, 6);
        Controls.Add(layout);
        AcceptButton = confirm;
        CancelButton = cancel;
        ClientSize = new Size(760, 630);
        MinimumSize = new Size(740, 600);
        StartPosition = FormStartPosition.CenterParent;
        Text = "Confirm permanent deletion";
        Name = "DeleteDataConfirmForm";
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ResumeLayout(false);
    }
}
