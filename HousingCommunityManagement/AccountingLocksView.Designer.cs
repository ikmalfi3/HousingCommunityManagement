#nullable enable
namespace HousingCommunityManagement;
partial class AccountingLocksView
{
    private CheckBox enabled = null !;
    private DateTimePicker through = null !;
    private TextBox reason = null !;
    private DataGridView grid = null !;
    private Label status = null !;
    private System.ComponentModel.IContainer? components = null;
    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }
    private void InitializeComponent()
    {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AccountingLocksView));
        toolbar = new FlowLayoutPanel();
        enabled = new CheckBox();
        through = new DateTimePicker();
        reason = new TextBox();
        actionButton = new Button();
        actionButton1 = new Button();
        grid = new DataGridView();
        status = new Label();
        pageLayout = new TableLayoutPanel();
        headingLabel = new Label();
        noteLabel = new Label();
        toolbar.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grid).BeginInit();
        pageLayout.SuspendLayout();
        SuspendLayout();
        // 
        // toolbar
        // 
        toolbar.AutoScroll = true;
        toolbar.Controls.Add(enabled);
        toolbar.Controls.Add(through);
        toolbar.Controls.Add(reason);
        toolbar.Controls.Add(actionButton);
        toolbar.Controls.Add(actionButton1);
        toolbar.Dock = DockStyle.Fill;
        toolbar.Location = new Point(19, 115);
        toolbar.Name = "toolbar";
        toolbar.Size = new Size(2730, 56);
        toolbar.TabIndex = 2;
        // 
        // enabled
        // 
        enabled.AutoSize = true;
        enabled.Location = new Point(3, 3);
        enabled.Name = "enabled";
        enabled.Padding = new Padding(0, 5, 0, 0);
        enabled.Size = new Size(97, 24);
        enabled.TabIndex = 0;
        enabled.Text = "Lock through";
        // 
        // through
        // 
        through.CustomFormat = "dd MMM yyyy";
        through.Format = DateTimePickerFormat.Custom;
        through.Location = new Point(106, 3);
        through.MinDate = new DateTime(1900, 1, 1, 0, 0, 0, 0);
        through.Name = "through";
        through.Size = new Size(150, 23);
        through.TabIndex = 1;
        // 
        // reason
        // 
        reason.Location = new Point(262, 3);
        reason.MaxLength = 500;
        reason.Name = "reason";
        reason.PlaceholderText = "Reason required for locking or unlocking";
        reason.Size = new Size(320, 23);
        reason.TabIndex = 2;
        // 
        // actionButton
        // 
        actionButton.AutoSize = true;
        actionButton.Location = new Point(588, 3);
        actionButton.Name = "actionButton";
        actionButton.Size = new Size(75, 25);
        actionButton.TabIndex = 3;
        actionButton.Text = "Apply lock";
        actionButton.Click += ApplyClicked;
        // 
        // actionButton1
        // 
        actionButton1.AutoSize = true;
        actionButton1.Location = new Point(669, 3);
        actionButton1.Name = "actionButton1";
        actionButton1.Size = new Size(75, 25);
        actionButton1.TabIndex = 4;
        actionButton1.Text = "Refresh";
        actionButton1.Click += RefreshClicked;
        // 
        // grid
        // 
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
        grid.BackgroundColor = Color.White;
        grid.Dock = DockStyle.Fill;
        grid.Location = new Point(19, 177);
        grid.MultiSelect = false;
        grid.Name = "grid";
        grid.ReadOnly = true;
        grid.RowHeadersVisible = false;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.Size = new Size(2730, 692);
        grid.TabIndex = 3;
        // 
        // status
        // 
        status.Dock = DockStyle.Fill;
        status.Location = new Point(19, 872);
        status.Name = "status";
        status.Padding = new Padding(0, 8, 0, 0);
        status.Size = new Size(2730, 65);
        status.TabIndex = 4;
        // 
        // pageLayout
        // 
        pageLayout.ColumnCount = 1;
        pageLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        pageLayout.Controls.Add(headingLabel, 0, 0);
        pageLayout.Controls.Add(noteLabel, 0, 1);
        pageLayout.Controls.Add(toolbar, 0, 2);
        pageLayout.Controls.Add(grid, 0, 3);
        pageLayout.Controls.Add(status, 0, 4);
        pageLayout.Dock = DockStyle.Fill;
        pageLayout.Location = new Point(0, 0);
        pageLayout.Name = "pageLayout";
        pageLayout.Padding = new Padding(16);
        pageLayout.RowCount = 5;
        pageLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        pageLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 56F));
        pageLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 62F));
        pageLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        pageLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 65F));
        pageLayout.Size = new Size(2768, 953);
        pageLayout.TabIndex = 0;
        // 
        // headingLabel
        // 
        headingLabel.Dock = DockStyle.Fill;
        headingLabel.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
        headingLabel.Location = new Point(19, 16);
        headingLabel.Name = "headingLabel";
        headingLabel.Size = new Size(2730, 40);
        headingLabel.TabIndex = 0;
        headingLabel.Text = "Transaction lock & history";
        headingLabel.UseMnemonic = false;
        // 
        // noteLabel
        // 
        noteLabel.Dock = DockStyle.Fill;
        noteLabel.Location = new Point(19, 56);
        noteLabel.Name = "noteLabel";
        noteLabel.Size = new Size(2730, 56);
        noteLabel.TabIndex = 1;
        noteLabel.Text = resources.GetString("noteLabel.Text");
        noteLabel.UseMnemonic = false;
        // 
        // AccountingLocksView
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(pageLayout);
        Name = "AccountingLocksView";
        Size = new Size(2768, 953);
        toolbar.ResumeLayout(false);
        toolbar.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)grid).EndInit();
        pageLayout.ResumeLayout(false);
        ResumeLayout(false);
    }

    private FlowLayoutPanel toolbar = null !;
    private Button actionButton = null !;
    private Button actionButton1 = null !;
    private TableLayoutPanel pageLayout = null !;
    private Label headingLabel = null !;
    private Label noteLabel = null !;
}
