#nullable enable
namespace HousingCommunityManagement;
partial class FinanceRegistersView
{
    private DataGridView grid = null !, history = null !;
    private ComboBox filter = null !;
    private DateTimePicker asOf = null !;
    private Label footer = null !;
    private Button link = null !, act = null !;
    private System.ComponentModel.IContainer? components = null;
    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }
    private DataGridViewTextBoxColumn gridNumberColumn = null!;
    private DataGridViewTextBoxColumn gridKindColumn = null!;
    private DataGridViewTextBoxColumn gridNameColumn = null!;
    private DataGridViewTextBoxColumn gridSupplierColumn = null!;
    private DataGridViewTextBoxColumn gridLocationColumn = null!;
    private DataGridViewTextBoxColumn gridAcquiredColumn = null!;
    private DataGridViewTextBoxColumn gridAccountColumn = null!;
    private DataGridViewTextBoxColumn gridCostColumn = null!;
    private DataGridViewTextBoxColumn gridDepreciationColumn = null!;
    private DataGridViewTextBoxColumn gridRemainingColumn = null!;
    private DataGridViewTextBoxColumn gridStatusColumn = null!;
    private DataGridViewTextBoxColumn historyIdColumn = null!;
    private DataGridViewTextBoxColumn historyDateColumn = null!;
    private DataGridViewTextBoxColumn historyActionColumn = null!;
    private DataGridViewTextBoxColumn historyDescriptionColumn = null!;
    private DataGridViewTextBoxColumn historyCostChangeColumn = null!;
    private DataGridViewTextBoxColumn historyDepreciationChangeColumn = null!;
    private DataGridViewTextBoxColumn historyMoneyEntryColumn = null!;
    private DataGridViewTextBoxColumn historyAdjustmentColumn = null!;
    private void InitializeComponent()
    {
        toolbar = new FlowLayoutPanel();
        actionButton = new Button();
        actionButton1 = new Button();
        link = new Button();
        act = new Button();
        actionButton2 = new Button();
        actionButton3 = new Button();
        actionButton4 = new Button();
        actionButton5 = new Button();
        filter = new ComboBox();
        asOf = new DateTimePicker();
        grid = new DataGridView();
        history = new DataGridView();
        split = new SplitContainer();
        footer = new Label();
        pageLayout = new TableLayoutPanel();
        headingLabel = new Label();
        noteLabel = new Label();
        toolbar.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grid).BeginInit();
        ((System.ComponentModel.ISupportInitialize)history).BeginInit();
        ((System.ComponentModel.ISupportInitialize)split).BeginInit();
        split.Panel1.SuspendLayout();
        split.Panel2.SuspendLayout();
        split.SuspendLayout();
        pageLayout.SuspendLayout();
        SuspendLayout();
        // 
        // toolbar
        // 
        toolbar.AutoScroll = true;
        toolbar.Controls.Add(filter);
        toolbar.Controls.Add(asOf);
        toolbar.Controls.Add(actionButton);
        toolbar.Controls.Add(actionButton1);
        toolbar.Controls.Add(link);
        toolbar.Controls.Add(act);
        toolbar.Controls.Add(actionButton2);
        toolbar.Controls.Add(actionButton3);
        toolbar.Controls.Add(actionButton4);
        toolbar.Controls.Add(actionButton5);
        toolbar.Dock = DockStyle.Fill;
        toolbar.Location = new Point(19, 115);
        toolbar.Name = "toolbar";
        toolbar.Size = new Size(2755, 79);
        toolbar.TabIndex = 2;
        // 
        // actionButton
        // 
        actionButton.AutoSize = true;
        actionButton.Location = new Point(305, 3);
        actionButton.Name = "actionButton";
        actionButton.Size = new Size(75, 25);
        actionButton.TabIndex = 0;
        actionButton.Text = "Add item...";
        actionButton.Click += NewClicked;
        // 
        // actionButton1
        // 
        actionButton1.AutoSize = true;
        actionButton1.Location = new Point(386, 3);
        actionButton1.Name = "actionButton1";
        actionButton1.Size = new Size(83, 25);
        actionButton1.TabIndex = 1;
        actionButton1.Text = "Edit details...";
        actionButton1.Click += EditDetailsClicked;
        // 
        // link
        // 
        link.AutoSize = true;
        link.Location = new Point(475, 3);
        link.Name = "link";
        link.Size = new Size(145, 25);
        link.TabIndex = 2;
        link.Text = "Link purchase / refund...";
        link.Click += LinkClicked;
        // 
        // act
        // 
        act.AutoSize = true;
        act.Location = new Point(626, 3);
        act.Name = "act";
        act.Size = new Size(178, 25);
        act.TabIndex = 3;
        act.Text = "Apply / depreciate / write off...";
        act.Click += ActionClicked;
        // 
        // actionButton2
        // 
        actionButton2.AutoSize = true;
        actionButton2.Location = new Point(810, 3);
        actionButton2.Name = "actionButton2";
        actionButton2.Size = new Size(101, 25);
        actionButton2.TabIndex = 4;
        actionButton2.Text = "Balance check...";
        actionButton2.Click += CheckClicked;
        // 
        // actionButton3
        // 
        actionButton3.AutoSize = true;
        actionButton3.Location = new Point(917, 3);
        actionButton3.Name = "actionButton3";
        actionButton3.Size = new Size(127, 25);
        actionButton3.TabIndex = 5;
        actionButton3.Text = "Opening schedules...";
        actionButton3.Click += OpeningClicked;
        // 
        // actionButton4
        // 
        actionButton4.AutoSize = true;
        actionButton4.Location = new Point(1050, 3);
        actionButton4.Name = "actionButton4";
        actionButton4.Size = new Size(141, 25);
        actionButton4.TabIndex = 6;
        actionButton4.Text = "Correct bank opening...";
        actionButton4.Click += BankOpeningClicked;
        // 
        // actionButton5
        // 
        actionButton5.AutoSize = true;
        actionButton5.Location = new Point(1197, 3);
        actionButton5.Name = "actionButton5";
        actionButton5.Size = new Size(75, 25);
        actionButton5.TabIndex = 7;
        actionButton5.Text = "Refresh";
        actionButton5.Click += RefreshClicked;
        // 
        // filter
        // 
        filter.DropDownStyle = ComboBoxStyle.DropDownList;
        filter.Items.AddRange(new object[] { "All types", "Asset", "Supplier deposit", "Prepayment" });
        filter.Location = new Point(3, 3);
        filter.Name = "filter";
        filter.Size = new Size(145, 23);
        filter.TabIndex = 8;
        filter.SelectedIndexChanged += RefreshClicked;
        // 
        // asOf
        // 
        asOf.CustomFormat = "dd MMM yyyy";
        asOf.Format = DateTimePickerFormat.Custom;
        asOf.Location = new Point(154, 3);
        asOf.Name = "asOf";
        asOf.Size = new Size(145, 23);
        asOf.TabIndex = 9;
        asOf.ValueChanged += RefreshClicked;
        // 
        // grid
        // 
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
        grid.BackgroundColor = Color.White;
        grid.Dock = DockStyle.Fill;
        grid.Location = new Point(0, 0);
        grid.MultiSelect = false;
        grid.Name = "grid";
        grid.ReadOnly = true;
        grid.RowHeadersVisible = false;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.Size = new Size(2755, 347);
        grid.TabIndex = 0;
        grid.SelectionChanged += SelectedChanged;
        // 
        // history
        // 
        history.AllowUserToAddRows = false;
        history.AllowUserToDeleteRows = false;
        history.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
        history.BackgroundColor = Color.White;
        history.Dock = DockStyle.Fill;
        history.Location = new Point(0, 0);
        history.MultiSelect = false;
        history.Name = "history";
        history.ReadOnly = true;
        history.RowHeadersVisible = false;
        history.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        history.Size = new Size(2755, 318);
        history.TabIndex = 0;
        // 
        // split
        // 
        split.Dock = DockStyle.Fill;
        split.Location = new Point(19, 200);
        split.Name = "split";
        split.Orientation = Orientation.Horizontal;
        // 
        // split.Panel1
        // 
        split.Panel1.Controls.Add(grid);
        // 
        // split.Panel2
        // 
        split.Panel2.Controls.Add(history);
        split.Size = new Size(2755, 669);
        split.SplitterDistance = 347;
        split.TabIndex = 3;
        // 
        // footer
        // 
        footer.Dock = DockStyle.Fill;
        footer.Location = new Point(19, 872);
        footer.Name = "footer";
        footer.Padding = new Padding(0, 8, 0, 0);
        footer.Size = new Size(2755, 65);
        footer.TabIndex = 4;
        // 
        // pageLayout
        // 
        pageLayout.ColumnCount = 1;
        pageLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        pageLayout.Controls.Add(headingLabel, 0, 0);
        pageLayout.Controls.Add(noteLabel, 0, 1);
        pageLayout.Controls.Add(toolbar, 0, 2);
        pageLayout.Controls.Add(split, 0, 3);
        pageLayout.Controls.Add(footer, 0, 4);
        pageLayout.Dock = DockStyle.Fill;
        pageLayout.Location = new Point(0, 0);
        pageLayout.Name = "pageLayout";
        pageLayout.Padding = new Padding(16);
        pageLayout.RowCount = 5;
        pageLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        pageLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 56F));
        pageLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 85F));
        pageLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        pageLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 65F));
        pageLayout.Size = new Size(2793, 953);
        pageLayout.TabIndex = 0;
        // 
        // headingLabel
        // 
        headingLabel.Dock = DockStyle.Fill;
        headingLabel.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
        headingLabel.Location = new Point(19, 16);
        headingLabel.Name = "headingLabel";
        headingLabel.Size = new Size(2755, 40);
        headingLabel.TabIndex = 0;
        headingLabel.Text = "Assets, supplier deposits & prepayments";
        headingLabel.UseMnemonic = false;
        // 
        // noteLabel
        // 
        noteLabel.Dock = DockStyle.Fill;
        noteLabel.Location = new Point(19, 56);
        noteLabel.Name = "noteLabel";
        noteLabel.Size = new Size(2755, 56);
        noteLabel.TabIndex = 1;
        noteLabel.Text = "Track individual items against their ledger accounts. Record cash movements once in Income & expenditure, then link them here. Noncash actions update both register and ledger together.";
        noteLabel.UseMnemonic = false;
        // 
        // FinanceRegistersView
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(pageLayout);
        Name = "FinanceRegistersView";
        Size = new Size(2793, 953);
        toolbar.ResumeLayout(false);
        toolbar.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)grid).EndInit();
        ((System.ComponentModel.ISupportInitialize)history).EndInit();
        split.Panel1.ResumeLayout(false);
        split.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)split).EndInit();
        split.ResumeLayout(false);
        pageLayout.ResumeLayout(false);
        ResumeLayout(false);
    }

    private FlowLayoutPanel toolbar = null !;
    private Button actionButton = null !;
    private Button actionButton1 = null !;
    private Button actionButton2 = null !;
    private Button actionButton3 = null !;
    private Button actionButton4 = null !;
    private Button actionButton5 = null !;
    private SplitContainer split = null !;
    private TableLayoutPanel pageLayout = null !;
    private Label headingLabel = null !;
    private Label noteLabel = null !;
}
