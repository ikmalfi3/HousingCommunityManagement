namespace HousingCommunityManagement;
partial class ChargeHousesForm
{
    private System.ComponentModel.IContainer components = null;
    private TableLayoutPanel layout;
    private Label heading;
    private FlowLayoutPanel toolbar;
    private Label yearLabel;
    private NumericUpDown year;
    private FlowLayoutPanel actions;
    private CheckBox useSharedDue;
    private DateTimePicker sharedDue;
    private CheckBox january;
    private CheckBox february;
    private CheckBox march;
    private CheckBox april;
    private CheckBox may;
    private CheckBox june;
    private CheckBox july;
    private CheckBox august;
    private CheckBox september;
    private CheckBox october;
    private CheckBox november;
    private CheckBox december;
    private Button generate;
    private Label note;
    private CheckedListBox houses;
    private FlowLayoutPanel houseToolbar;
    private CheckBox onlyOwned;
    private CheckBox allHouses;
    private CheckBox allMonths;
    private Button cancel;
    private TableLayoutPanel accountDefaults;
    private Label receivableAccountLabel;
    private TextBox receivableAccount;
    private Label incomeAccountLabel;
    private TextBox incomeAccount;
    private Label accountNote;
    private FlowLayoutPanel amountPanel;
    private Label amountLabel;
    private NumericUpDown chargeAmount;
    private Label chargeDateLabel;
    private DateTimePicker chargeDate;
    private CheckBox useMonthlyDue;
    private GroupBox monthlyDueGroup;
    private TableLayoutPanel monthlyDueGrid;
    private FlowLayoutPanel januaryDuePanel;
    private Label januaryDueLabel;
    private DateTimePicker januaryDue;
    private FlowLayoutPanel februaryDuePanel;
    private Label februaryDueLabel;
    private DateTimePicker februaryDue;
    private FlowLayoutPanel marchDuePanel;
    private Label marchDueLabel;
    private DateTimePicker marchDue;
    private FlowLayoutPanel aprilDuePanel;
    private Label aprilDueLabel;
    private DateTimePicker aprilDue;
    private FlowLayoutPanel mayDuePanel;
    private Label mayDueLabel;
    private DateTimePicker mayDue;
    private FlowLayoutPanel juneDuePanel;
    private Label juneDueLabel;
    private DateTimePicker juneDue;
    private FlowLayoutPanel julyDuePanel;
    private Label julyDueLabel;
    private DateTimePicker julyDue;
    private FlowLayoutPanel augustDuePanel;
    private Label augustDueLabel;
    private DateTimePicker augustDue;
    private FlowLayoutPanel septemberDuePanel;
    private Label septemberDueLabel;
    private DateTimePicker septemberDue;
    private FlowLayoutPanel octoberDuePanel;
    private Label octoberDueLabel;
    private DateTimePicker octoberDue;
    private FlowLayoutPanel novemberDuePanel;
    private Label novemberDueLabel;
    private DateTimePicker novemberDue;
    private FlowLayoutPanel decemberDuePanel;
    private Label decemberDueLabel;
    private DateTimePicker decemberDue;
    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }
    private void InitializeComponent()
    {
        layout = new TableLayoutPanel();
        heading = new Label();
        toolbar = new FlowLayoutPanel();
        yearLabel = new Label();
        year = new NumericUpDown();
        allMonths = new CheckBox();
        january = new CheckBox();
        february = new CheckBox();
        march = new CheckBox();
        april = new CheckBox();
        may = new CheckBox();
        june = new CheckBox();
        july = new CheckBox();
        august = new CheckBox();
        september = new CheckBox();
        october = new CheckBox();
        november = new CheckBox();
        december = new CheckBox();
        amountPanel = new FlowLayoutPanel();
        amountLabel = new Label();
        chargeAmount = new NumericUpDown();
        chargeDateLabel = new Label();
        chargeDate = new DateTimePicker();
        accountDefaults = new TableLayoutPanel();
        receivableAccountLabel = new Label();
        receivableAccount = new TextBox();
        incomeAccountLabel = new Label();
        incomeAccount = new TextBox();
        accountNote = new Label();
        actions = new FlowLayoutPanel();
        generate = new Button();
        cancel = new Button();
        useSharedDue = new CheckBox();
        sharedDue = new DateTimePicker();
        useMonthlyDue = new CheckBox();
        monthlyDueGroup = new GroupBox();
        monthlyDueGrid = new TableLayoutPanel();
        januaryDuePanel = new FlowLayoutPanel();
        januaryDueLabel = new Label();
        januaryDue = new DateTimePicker();
        februaryDuePanel = new FlowLayoutPanel();
        februaryDueLabel = new Label();
        februaryDue = new DateTimePicker();
        marchDuePanel = new FlowLayoutPanel();
        marchDueLabel = new Label();
        marchDue = new DateTimePicker();
        aprilDuePanel = new FlowLayoutPanel();
        aprilDueLabel = new Label();
        aprilDue = new DateTimePicker();
        mayDuePanel = new FlowLayoutPanel();
        mayDueLabel = new Label();
        mayDue = new DateTimePicker();
        juneDuePanel = new FlowLayoutPanel();
        juneDueLabel = new Label();
        juneDue = new DateTimePicker();
        julyDuePanel = new FlowLayoutPanel();
        julyDueLabel = new Label();
        julyDue = new DateTimePicker();
        augustDuePanel = new FlowLayoutPanel();
        augustDueLabel = new Label();
        augustDue = new DateTimePicker();
        septemberDuePanel = new FlowLayoutPanel();
        septemberDueLabel = new Label();
        septemberDue = new DateTimePicker();
        octoberDuePanel = new FlowLayoutPanel();
        octoberDueLabel = new Label();
        octoberDue = new DateTimePicker();
        novemberDuePanel = new FlowLayoutPanel();
        novemberDueLabel = new Label();
        novemberDue = new DateTimePicker();
        decemberDuePanel = new FlowLayoutPanel();
        decemberDueLabel = new Label();
        decemberDue = new DateTimePicker();
        houseToolbar = new FlowLayoutPanel();
        allHouses = new CheckBox();
        onlyOwned = new CheckBox();
        houses = new CheckedListBox();
        note = new Label();
        layout.SuspendLayout();
        toolbar.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)year).BeginInit();
        amountPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)chargeAmount).BeginInit();
        accountDefaults.SuspendLayout();
        actions.SuspendLayout();
        monthlyDueGroup.SuspendLayout();
        monthlyDueGrid.SuspendLayout();
        januaryDuePanel.SuspendLayout();
        februaryDuePanel.SuspendLayout();
        marchDuePanel.SuspendLayout();
        aprilDuePanel.SuspendLayout();
        mayDuePanel.SuspendLayout();
        juneDuePanel.SuspendLayout();
        julyDuePanel.SuspendLayout();
        augustDuePanel.SuspendLayout();
        septemberDuePanel.SuspendLayout();
        octoberDuePanel.SuspendLayout();
        novemberDuePanel.SuspendLayout();
        decemberDuePanel.SuspendLayout();
        houseToolbar.SuspendLayout();
        SuspendLayout();
        // 
        // layout
        // 
        layout.ColumnCount = 1;
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        layout.Controls.Add(heading, 0, 0);
        layout.Controls.Add(toolbar, 0, 1);
        layout.Controls.Add(amountPanel, 0, 2);
        layout.Controls.Add(accountDefaults, 0, 3);
        layout.Controls.Add(actions, 0, 4);
        layout.Controls.Add(monthlyDueGroup, 0, 5);
        layout.Controls.Add(houseToolbar, 0, 6);
        layout.Controls.Add(houses, 0, 7);
        layout.Controls.Add(note, 0, 8);
        layout.Dock = DockStyle.Fill;
        layout.Location = new Point(0, 0);
        layout.Name = "layout";
        layout.Padding = new Padding(20);
        layout.RowCount = 9;
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 55F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 72F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 0F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 65F));
        layout.Size = new Size(1050, 680);
        layout.TabIndex = 0;
        // 
        // heading
        // 
        heading.Dock = DockStyle.Fill;
        heading.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
        heading.Location = new Point(23, 20);
        heading.Name = "heading";
        heading.Size = new Size(1004, 55);
        heading.TabIndex = 0;
        heading.Text = "Charge houses";
        // 
        // toolbar
        // 
        toolbar.AutoScroll = true;
        toolbar.Controls.Add(yearLabel);
        toolbar.Controls.Add(year);
        toolbar.Controls.Add(allMonths);
        toolbar.Controls.Add(january);
        toolbar.Controls.Add(february);
        toolbar.Controls.Add(march);
        toolbar.Controls.Add(april);
        toolbar.Controls.Add(may);
        toolbar.Controls.Add(june);
        toolbar.Controls.Add(july);
        toolbar.Controls.Add(august);
        toolbar.Controls.Add(september);
        toolbar.Controls.Add(october);
        toolbar.Controls.Add(november);
        toolbar.Controls.Add(december);
        toolbar.Dock = DockStyle.Fill;
        toolbar.Location = new Point(23, 78);
        toolbar.Name = "toolbar";
        toolbar.Size = new Size(1004, 42);
        toolbar.TabIndex = 1;
        toolbar.WrapContents = false;
        // 
        // yearLabel
        // 
        yearLabel.AutoSize = true;
        yearLabel.Location = new Point(3, 0);
        yearLabel.Name = "yearLabel";
        yearLabel.Size = new Size(29, 15);
        yearLabel.TabIndex = 0;
        yearLabel.Text = "Year";
        // 
        // year
        // 
        year.Location = new Point(38, 3);
        year.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
        year.Minimum = new decimal(new int[] { 1900, 0, 0, 0 });
        year.Name = "year";
        year.Size = new Size(85, 23);
        year.TabIndex = 1;
        year.Value = new decimal(new int[] { 2026, 0, 0, 0 });
        year.ValueChanged += RefreshClicked;
        // 
        // allMonths
        // 
        allMonths.AutoCheck = false;
        allMonths.AutoSize = true;
        allMonths.Location = new Point(129, 3);
        allMonths.Name = "allMonths";
        allMonths.Size = new Size(40, 19);
        allMonths.TabIndex = 2;
        allMonths.Text = "All";
        allMonths.ThreeState = true;
        allMonths.Click += AllMonthsClicked;
        // 
        // january
        // 
        january.AccessibleName = "January";
        january.AutoSize = true;
        january.Location = new Point(175, 3);
        january.Name = "january";
        january.Size = new Size(30, 19);
        january.TabIndex = 3;
        january.Text = "J";
        january.CheckedChanged += RefreshClicked;
        // 
        // february
        // 
        february.AccessibleName = "February";
        february.AutoSize = true;
        february.Location = new Point(211, 3);
        february.Name = "february";
        february.Size = new Size(32, 19);
        february.TabIndex = 4;
        february.Text = "F";
        february.CheckedChanged += RefreshClicked;
        // 
        // march
        // 
        march.AccessibleName = "March";
        march.AutoSize = true;
        march.Location = new Point(249, 3);
        march.Name = "march";
        march.Size = new Size(37, 19);
        march.TabIndex = 5;
        march.Text = "M";
        march.CheckedChanged += RefreshClicked;
        // 
        // april
        // 
        april.AccessibleName = "April";
        april.AutoSize = true;
        april.Location = new Point(292, 3);
        april.Name = "april";
        april.Size = new Size(34, 19);
        april.TabIndex = 6;
        april.Text = "A";
        april.CheckedChanged += RefreshClicked;
        // 
        // may
        // 
        may.AccessibleName = "May";
        may.AutoSize = true;
        may.Location = new Point(332, 3);
        may.Name = "may";
        may.Size = new Size(37, 19);
        may.TabIndex = 7;
        may.Text = "M";
        may.CheckedChanged += RefreshClicked;
        // 
        // june
        // 
        june.AccessibleName = "June";
        june.AutoSize = true;
        june.Location = new Point(375, 3);
        june.Name = "june";
        june.Size = new Size(30, 19);
        june.TabIndex = 8;
        june.Text = "J";
        june.CheckedChanged += RefreshClicked;
        // 
        // july
        // 
        july.AccessibleName = "July";
        july.AutoSize = true;
        july.Location = new Point(411, 3);
        july.Name = "july";
        july.Size = new Size(30, 19);
        july.TabIndex = 9;
        july.Text = "J";
        july.CheckedChanged += RefreshClicked;
        // 
        // august
        // 
        august.AccessibleName = "August";
        august.AutoSize = true;
        august.Location = new Point(447, 3);
        august.Name = "august";
        august.Size = new Size(34, 19);
        august.TabIndex = 10;
        august.Text = "A";
        august.CheckedChanged += RefreshClicked;
        // 
        // september
        // 
        september.AccessibleName = "September";
        september.AutoSize = true;
        september.Location = new Point(487, 3);
        september.Name = "september";
        september.Size = new Size(32, 19);
        september.TabIndex = 11;
        september.Text = "S";
        september.CheckedChanged += RefreshClicked;
        // 
        // october
        // 
        october.AccessibleName = "October";
        october.AutoSize = true;
        october.Location = new Point(525, 3);
        october.Name = "october";
        october.Size = new Size(35, 19);
        october.TabIndex = 12;
        october.Text = "O";
        october.CheckedChanged += RefreshClicked;
        // 
        // november
        // 
        november.AccessibleName = "November";
        november.AutoSize = true;
        november.Location = new Point(566, 3);
        november.Name = "november";
        november.Size = new Size(35, 19);
        november.TabIndex = 13;
        november.Text = "N";
        november.CheckedChanged += RefreshClicked;
        // 
        // december
        // 
        december.AccessibleName = "December";
        december.AutoSize = true;
        december.Location = new Point(607, 3);
        december.Name = "december";
        december.Size = new Size(34, 19);
        december.TabIndex = 14;
        december.Text = "D";
        december.CheckedChanged += RefreshClicked;
        // 
        // amountPanel
        // 
        amountPanel.Controls.Add(amountLabel);
        amountPanel.Controls.Add(chargeAmount);
        chargeDateLabel.Text = "Charge date";
        chargeDateLabel.AutoSize = true;
        chargeDateLabel.Margin = new Padding(24, 0, 6, 0);
        chargeDateLabel.Padding = new Padding(0, 5, 0, 0);
        chargeDate.Name = "chargeDate";
        chargeDate.AccessibleName = "Charge date";
        chargeDate.Format = DateTimePickerFormat.Custom;
        chargeDate.CustomFormat = "dd MMM yyyy";
        chargeDate.MinDate = new DateTime(1900, 1, 1);
        chargeDate.Width = 160;
        chargeDate.TabIndex = 2;
        amountPanel.Controls.Add(chargeDateLabel);
        amountPanel.Controls.Add(chargeDate);
        amountPanel.Dock = DockStyle.Fill;
        amountPanel.Location = new Point(23, 126);
        amountPanel.Name = "amountPanel";
        amountPanel.Size = new Size(1004, 36);
        amountPanel.TabIndex = 2;
        amountPanel.WrapContents = false;
        // 
        // amountLabel
        // 
        amountLabel.AutoSize = true;
        amountLabel.Location = new Point(3, 0);
        amountLabel.Name = "amountLabel";
        amountLabel.Padding = new Padding(0, 5, 8, 0);
        amountLabel.Size = new Size(202, 20);
        amountLabel.TabIndex = 0;
        amountLabel.Text = "Amount per house per month (RM)";
        // 
        // chargeAmount
        // 
        chargeAmount.AccessibleName = "Maintenance amount per house per month in RM";
        chargeAmount.DecimalPlaces = 2;
        chargeAmount.Location = new Point(211, 3);
        chargeAmount.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
        chargeAmount.Name = "chargeAmount";
        chargeAmount.Size = new Size(150, 23);
        chargeAmount.TabIndex = 1;
        chargeAmount.ThousandsSeparator = true;
        chargeAmount.ValueChanged += AmountChanged;
        // 
        // accountDefaults
        // 
        accountDefaults.ColumnCount = 4;
        accountDefaults.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 155F));
        accountDefaults.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        accountDefaults.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 155F));
        accountDefaults.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        accountDefaults.Controls.Add(receivableAccountLabel, 0, 0);
        accountDefaults.Controls.Add(receivableAccount, 1, 0);
        accountDefaults.Controls.Add(incomeAccountLabel, 2, 0);
        accountDefaults.Controls.Add(incomeAccount, 3, 0);
        accountDefaults.Controls.Add(accountNote, 0, 1);
        accountDefaults.Dock = DockStyle.Fill;
        accountDefaults.Location = new Point(23, 168);
        accountDefaults.Name = "accountDefaults";
        accountDefaults.RowCount = 2;
        accountDefaults.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
        accountDefaults.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        accountDefaults.Size = new Size(1004, 66);
        accountDefaults.TabIndex = 3;
        // 
        // receivableAccountLabel
        // 
        receivableAccountLabel.Dock = DockStyle.Fill;
        receivableAccountLabel.Location = new Point(3, 0);
        receivableAccountLabel.Name = "receivableAccountLabel";
        receivableAccountLabel.Size = new Size(149, 32);
        receivableAccountLabel.TabIndex = 0;
        receivableAccountLabel.Text = "Receivable account (debit)";
        receivableAccountLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // receivableAccount
        // 
        receivableAccount.Dock = DockStyle.Fill;
        receivableAccount.Location = new Point(158, 3);
        receivableAccount.Name = "receivableAccount";
        receivableAccount.ReadOnly = true;
        receivableAccount.Size = new Size(341, 23);
        receivableAccount.TabIndex = 1;
        receivableAccount.TabStop = false;
        receivableAccount.Text = "Not configured";
        // 
        // incomeAccountLabel
        // 
        incomeAccountLabel.Dock = DockStyle.Fill;
        incomeAccountLabel.Location = new Point(505, 0);
        incomeAccountLabel.Name = "incomeAccountLabel";
        incomeAccountLabel.Size = new Size(149, 32);
        incomeAccountLabel.TabIndex = 2;
        incomeAccountLabel.Text = "Maintenance income (credit)";
        incomeAccountLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // incomeAccount
        // 
        incomeAccount.Dock = DockStyle.Fill;
        incomeAccount.Location = new Point(660, 3);
        incomeAccount.Name = "incomeAccount";
        incomeAccount.ReadOnly = true;
        incomeAccount.Size = new Size(341, 23);
        incomeAccount.TabIndex = 3;
        incomeAccount.TabStop = false;
        incomeAccount.Text = "Not configured";
        // 
        // accountNote
        // 
        accountDefaults.SetColumnSpan(accountNote, 4);
        accountNote.Dock = DockStyle.Fill;
        accountNote.ForeColor = SystemColors.GrayText;
        accountNote.Location = new Point(3, 32);
        accountNote.Name = "accountNote";
        accountNote.Size = new Size(998, 34);
        accountNote.TabIndex = 4;
        accountNote.Text = "Default accounts from Settings > Accounting.";
        // 
        // actions
        // 
        actions.AutoScroll = true;
        actions.Controls.Add(generate);
        actions.Controls.Add(cancel);
        actions.Controls.Add(useSharedDue);
        actions.Controls.Add(sharedDue);
        actions.Controls.Add(useMonthlyDue);
        actions.Dock = DockStyle.Fill;
        actions.Location = new Point(23, 240);
        actions.Name = "actions";
        actions.Size = new Size(1004, 42);
        actions.TabIndex = 4;
        actions.WrapContents = false;
        // 
        // generate
        // 
        generate.AutoSize = true;
        generate.Location = new Point(3, 3);
        generate.Name = "generate";
        generate.Size = new Size(145, 25);
        generate.TabIndex = 0;
        generate.Text = "Charge selected months";
        generate.Click += GenerateClicked;
        // 
        // cancel
        // 
        cancel.DialogResult = DialogResult.Cancel;
        cancel.Location = new Point(154, 3);
        cancel.Name = "cancel";
        cancel.Size = new Size(75, 23);
        cancel.TabIndex = 1;
        cancel.Text = "Cancel";
        // 
        // useSharedDue
        // 
        useSharedDue.AutoSize = true;
        useSharedDue.Location = new Point(235, 3);
        useSharedDue.Name = "useSharedDue";
        useSharedDue.Size = new Size(117, 19);
        useSharedDue.TabIndex = 2;
        useSharedDue.Text = "Use one due date";
        useSharedDue.CheckedChanged += SharedDueChanged;
        // 
        // sharedDue
        // 
        sharedDue.Enabled = false;
        sharedDue.Format = DateTimePickerFormat.Short;
        sharedDue.Location = new Point(358, 3);
        sharedDue.Name = "sharedDue";
        sharedDue.Size = new Size(120, 23);
        sharedDue.TabIndex = 3;
        // 
        // useMonthlyDue
        // 
        useMonthlyDue.AutoSize = true;
        useMonthlyDue.Location = new Point(484, 3);
        useMonthlyDue.Name = "useMonthlyDue";
        useMonthlyDue.Size = new Size(171, 19);
        useMonthlyDue.TabIndex = 4;
        useMonthlyDue.Text = "Use multi-month due dates";
        useMonthlyDue.CheckedChanged += MonthlyDueChanged;
        // 
        // monthlyDueGroup
        // 
        monthlyDueGroup.Controls.Add(monthlyDueGrid);
        monthlyDueGroup.Dock = DockStyle.Fill;
        monthlyDueGroup.Location = new Point(23, 288);
        monthlyDueGroup.Name = "monthlyDueGroup";
        monthlyDueGroup.Padding = new Padding(8);
        monthlyDueGroup.Size = new Size(1004, 1);
        monthlyDueGroup.TabIndex = 5;
        monthlyDueGroup.TabStop = false;
        monthlyDueGroup.Text = "Due dates for selected months";
        monthlyDueGroup.Visible = false;
        // 
        // monthlyDueGrid
        // 
        monthlyDueGrid.ColumnCount = 4;
        monthlyDueGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        monthlyDueGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        monthlyDueGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        monthlyDueGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        monthlyDueGrid.Controls.Add(januaryDuePanel, 0, 0);
        monthlyDueGrid.Controls.Add(februaryDuePanel, 1, 0);
        monthlyDueGrid.Controls.Add(marchDuePanel, 2, 0);
        monthlyDueGrid.Controls.Add(aprilDuePanel, 3, 0);
        monthlyDueGrid.Controls.Add(mayDuePanel, 0, 1);
        monthlyDueGrid.Controls.Add(juneDuePanel, 1, 1);
        monthlyDueGrid.Controls.Add(julyDuePanel, 2, 1);
        monthlyDueGrid.Controls.Add(augustDuePanel, 3, 1);
        monthlyDueGrid.Controls.Add(septemberDuePanel, 0, 2);
        monthlyDueGrid.Controls.Add(octoberDuePanel, 1, 2);
        monthlyDueGrid.Controls.Add(novemberDuePanel, 2, 2);
        monthlyDueGrid.Controls.Add(decemberDuePanel, 3, 2);
        monthlyDueGrid.Dock = DockStyle.Fill;
        monthlyDueGrid.Location = new Point(8, 24);
        monthlyDueGrid.Name = "monthlyDueGrid";
        monthlyDueGrid.RowCount = 3;
        monthlyDueGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
        monthlyDueGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
        monthlyDueGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
        monthlyDueGrid.Size = new Size(988, 0);
        monthlyDueGrid.TabIndex = 0;
        // 
        // januaryDuePanel
        // 
        januaryDuePanel.Controls.Add(januaryDueLabel);
        januaryDuePanel.Controls.Add(januaryDue);
        januaryDuePanel.Dock = DockStyle.Fill;
        januaryDuePanel.Location = new Point(0, 0);
        januaryDuePanel.Margin = new Padding(0);
        januaryDuePanel.Name = "januaryDuePanel";
        januaryDuePanel.Size = new Size(247, 36);
        januaryDuePanel.TabIndex = 0;
        januaryDuePanel.WrapContents = false;
        // 
        // januaryDueLabel
        // 
        januaryDueLabel.Location = new Point(3, 0);
        januaryDueLabel.Name = "januaryDueLabel";
        januaryDueLabel.Padding = new Padding(0, 4, 0, 0);
        januaryDueLabel.Size = new Size(70, 23);
        januaryDueLabel.TabIndex = 0;
        januaryDueLabel.Text = "January";
        // 
        // januaryDue
        // 
        januaryDue.CustomFormat = "dd MMM yyyy";
        januaryDue.Format = DateTimePickerFormat.Custom;
        januaryDue.Location = new Point(79, 3);
        januaryDue.Name = "januaryDue";
        januaryDue.ShowUpDown = true;
        januaryDue.Size = new Size(125, 23);
        januaryDue.TabIndex = 1;
        januaryDue.Value = new DateTime(2026, 1, 6, 0, 0, 0, 0);
        // 
        // februaryDuePanel
        // 
        februaryDuePanel.Controls.Add(februaryDueLabel);
        februaryDuePanel.Controls.Add(februaryDue);
        februaryDuePanel.Dock = DockStyle.Fill;
        februaryDuePanel.Location = new Point(247, 0);
        februaryDuePanel.Margin = new Padding(0);
        februaryDuePanel.Name = "februaryDuePanel";
        februaryDuePanel.Size = new Size(247, 36);
        februaryDuePanel.TabIndex = 1;
        februaryDuePanel.WrapContents = false;
        // 
        // februaryDueLabel
        // 
        februaryDueLabel.Location = new Point(3, 0);
        februaryDueLabel.Name = "februaryDueLabel";
        februaryDueLabel.Padding = new Padding(0, 4, 0, 0);
        februaryDueLabel.Size = new Size(70, 23);
        februaryDueLabel.TabIndex = 0;
        februaryDueLabel.Text = "February";
        // 
        // februaryDue
        // 
        februaryDue.CustomFormat = "dd MMM yyyy";
        februaryDue.Format = DateTimePickerFormat.Custom;
        februaryDue.Location = new Point(79, 3);
        februaryDue.Name = "februaryDue";
        februaryDue.ShowUpDown = true;
        februaryDue.Size = new Size(125, 23);
        februaryDue.TabIndex = 1;
        februaryDue.Value = new DateTime(2026, 2, 6, 0, 0, 0, 0);
        // 
        // marchDuePanel
        // 
        marchDuePanel.Controls.Add(marchDueLabel);
        marchDuePanel.Controls.Add(marchDue);
        marchDuePanel.Dock = DockStyle.Fill;
        marchDuePanel.Location = new Point(494, 0);
        marchDuePanel.Margin = new Padding(0);
        marchDuePanel.Name = "marchDuePanel";
        marchDuePanel.Size = new Size(247, 36);
        marchDuePanel.TabIndex = 2;
        marchDuePanel.WrapContents = false;
        // 
        // marchDueLabel
        // 
        marchDueLabel.Location = new Point(3, 0);
        marchDueLabel.Name = "marchDueLabel";
        marchDueLabel.Padding = new Padding(0, 4, 0, 0);
        marchDueLabel.Size = new Size(70, 23);
        marchDueLabel.TabIndex = 0;
        marchDueLabel.Text = "March";
        // 
        // marchDue
        // 
        marchDue.CustomFormat = "dd MMM yyyy";
        marchDue.Format = DateTimePickerFormat.Custom;
        marchDue.Location = new Point(79, 3);
        marchDue.Name = "marchDue";
        marchDue.ShowUpDown = true;
        marchDue.Size = new Size(125, 23);
        marchDue.TabIndex = 1;
        marchDue.Value = new DateTime(2026, 3, 6, 0, 0, 0, 0);
        // 
        // aprilDuePanel
        // 
        aprilDuePanel.Controls.Add(aprilDueLabel);
        aprilDuePanel.Controls.Add(aprilDue);
        aprilDuePanel.Dock = DockStyle.Fill;
        aprilDuePanel.Location = new Point(741, 0);
        aprilDuePanel.Margin = new Padding(0);
        aprilDuePanel.Name = "aprilDuePanel";
        aprilDuePanel.Size = new Size(247, 36);
        aprilDuePanel.TabIndex = 3;
        aprilDuePanel.WrapContents = false;
        // 
        // aprilDueLabel
        // 
        aprilDueLabel.Location = new Point(3, 0);
        aprilDueLabel.Name = "aprilDueLabel";
        aprilDueLabel.Padding = new Padding(0, 4, 0, 0);
        aprilDueLabel.Size = new Size(70, 23);
        aprilDueLabel.TabIndex = 0;
        aprilDueLabel.Text = "April";
        // 
        // aprilDue
        // 
        aprilDue.CustomFormat = "dd MMM yyyy";
        aprilDue.Format = DateTimePickerFormat.Custom;
        aprilDue.Location = new Point(79, 3);
        aprilDue.Name = "aprilDue";
        aprilDue.ShowUpDown = true;
        aprilDue.Size = new Size(125, 23);
        aprilDue.TabIndex = 1;
        aprilDue.Value = new DateTime(2026, 4, 6, 0, 0, 0, 0);
        // 
        // mayDuePanel
        // 
        mayDuePanel.Controls.Add(mayDueLabel);
        mayDuePanel.Controls.Add(mayDue);
        mayDuePanel.Dock = DockStyle.Fill;
        mayDuePanel.Location = new Point(0, 36);
        mayDuePanel.Margin = new Padding(0);
        mayDuePanel.Name = "mayDuePanel";
        mayDuePanel.Size = new Size(247, 36);
        mayDuePanel.TabIndex = 4;
        mayDuePanel.WrapContents = false;
        // 
        // mayDueLabel
        // 
        mayDueLabel.Location = new Point(3, 0);
        mayDueLabel.Name = "mayDueLabel";
        mayDueLabel.Padding = new Padding(0, 4, 0, 0);
        mayDueLabel.Size = new Size(70, 23);
        mayDueLabel.TabIndex = 0;
        mayDueLabel.Text = "May";
        // 
        // mayDue
        // 
        mayDue.CustomFormat = "dd MMM yyyy";
        mayDue.Format = DateTimePickerFormat.Custom;
        mayDue.Location = new Point(79, 3);
        mayDue.Name = "mayDue";
        mayDue.ShowUpDown = true;
        mayDue.Size = new Size(125, 23);
        mayDue.TabIndex = 1;
        mayDue.Value = new DateTime(2026, 5, 6, 0, 0, 0, 0);
        // 
        // juneDuePanel
        // 
        juneDuePanel.Controls.Add(juneDueLabel);
        juneDuePanel.Controls.Add(juneDue);
        juneDuePanel.Dock = DockStyle.Fill;
        juneDuePanel.Location = new Point(247, 36);
        juneDuePanel.Margin = new Padding(0);
        juneDuePanel.Name = "juneDuePanel";
        juneDuePanel.Size = new Size(247, 36);
        juneDuePanel.TabIndex = 5;
        juneDuePanel.WrapContents = false;
        // 
        // juneDueLabel
        // 
        juneDueLabel.Location = new Point(3, 0);
        juneDueLabel.Name = "juneDueLabel";
        juneDueLabel.Padding = new Padding(0, 4, 0, 0);
        juneDueLabel.Size = new Size(70, 23);
        juneDueLabel.TabIndex = 0;
        juneDueLabel.Text = "June";
        // 
        // juneDue
        // 
        juneDue.CustomFormat = "dd MMM yyyy";
        juneDue.Format = DateTimePickerFormat.Custom;
        juneDue.Location = new Point(79, 3);
        juneDue.Name = "juneDue";
        juneDue.ShowUpDown = true;
        juneDue.Size = new Size(125, 23);
        juneDue.TabIndex = 1;
        juneDue.Value = new DateTime(2026, 6, 6, 0, 0, 0, 0);
        // 
        // julyDuePanel
        // 
        julyDuePanel.Controls.Add(julyDueLabel);
        julyDuePanel.Controls.Add(julyDue);
        julyDuePanel.Dock = DockStyle.Fill;
        julyDuePanel.Location = new Point(494, 36);
        julyDuePanel.Margin = new Padding(0);
        julyDuePanel.Name = "julyDuePanel";
        julyDuePanel.Size = new Size(247, 36);
        julyDuePanel.TabIndex = 6;
        julyDuePanel.WrapContents = false;
        // 
        // julyDueLabel
        // 
        julyDueLabel.Location = new Point(3, 0);
        julyDueLabel.Name = "julyDueLabel";
        julyDueLabel.Padding = new Padding(0, 4, 0, 0);
        julyDueLabel.Size = new Size(70, 23);
        julyDueLabel.TabIndex = 0;
        julyDueLabel.Text = "July";
        // 
        // julyDue
        // 
        julyDue.CustomFormat = "dd MMM yyyy";
        julyDue.Format = DateTimePickerFormat.Custom;
        julyDue.Location = new Point(79, 3);
        julyDue.Name = "julyDue";
        julyDue.ShowUpDown = true;
        julyDue.Size = new Size(125, 23);
        julyDue.TabIndex = 1;
        julyDue.Value = new DateTime(2026, 7, 6, 0, 0, 0, 0);
        // 
        // augustDuePanel
        // 
        augustDuePanel.Controls.Add(augustDueLabel);
        augustDuePanel.Controls.Add(augustDue);
        augustDuePanel.Dock = DockStyle.Fill;
        augustDuePanel.Location = new Point(741, 36);
        augustDuePanel.Margin = new Padding(0);
        augustDuePanel.Name = "augustDuePanel";
        augustDuePanel.Size = new Size(247, 36);
        augustDuePanel.TabIndex = 7;
        augustDuePanel.WrapContents = false;
        // 
        // augustDueLabel
        // 
        augustDueLabel.Location = new Point(3, 0);
        augustDueLabel.Name = "augustDueLabel";
        augustDueLabel.Padding = new Padding(0, 4, 0, 0);
        augustDueLabel.Size = new Size(70, 23);
        augustDueLabel.TabIndex = 0;
        augustDueLabel.Text = "August";
        // 
        // augustDue
        // 
        augustDue.CustomFormat = "dd MMM yyyy";
        augustDue.Format = DateTimePickerFormat.Custom;
        augustDue.Location = new Point(79, 3);
        augustDue.Name = "augustDue";
        augustDue.ShowUpDown = true;
        augustDue.Size = new Size(125, 23);
        augustDue.TabIndex = 1;
        augustDue.Value = new DateTime(2026, 8, 6, 0, 0, 0, 0);
        // 
        // septemberDuePanel
        // 
        septemberDuePanel.Controls.Add(septemberDueLabel);
        septemberDuePanel.Controls.Add(septemberDue);
        septemberDuePanel.Dock = DockStyle.Fill;
        septemberDuePanel.Location = new Point(0, 72);
        septemberDuePanel.Margin = new Padding(0);
        septemberDuePanel.Name = "septemberDuePanel";
        septemberDuePanel.Size = new Size(247, 36);
        septemberDuePanel.TabIndex = 8;
        septemberDuePanel.WrapContents = false;
        // 
        // septemberDueLabel
        // 
        septemberDueLabel.Location = new Point(3, 0);
        septemberDueLabel.Name = "septemberDueLabel";
        septemberDueLabel.Padding = new Padding(0, 4, 0, 0);
        septemberDueLabel.Size = new Size(70, 23);
        septemberDueLabel.TabIndex = 0;
        septemberDueLabel.Text = "September";
        // 
        // septemberDue
        // 
        septemberDue.CustomFormat = "dd MMM yyyy";
        septemberDue.Format = DateTimePickerFormat.Custom;
        septemberDue.Location = new Point(79, 3);
        septemberDue.Name = "septemberDue";
        septemberDue.ShowUpDown = true;
        septemberDue.Size = new Size(125, 23);
        septemberDue.TabIndex = 1;
        septemberDue.Value = new DateTime(2026, 9, 6, 0, 0, 0, 0);
        // 
        // octoberDuePanel
        // 
        octoberDuePanel.Controls.Add(octoberDueLabel);
        octoberDuePanel.Controls.Add(octoberDue);
        octoberDuePanel.Dock = DockStyle.Fill;
        octoberDuePanel.Location = new Point(247, 72);
        octoberDuePanel.Margin = new Padding(0);
        octoberDuePanel.Name = "octoberDuePanel";
        octoberDuePanel.Size = new Size(247, 36);
        octoberDuePanel.TabIndex = 9;
        octoberDuePanel.WrapContents = false;
        // 
        // octoberDueLabel
        // 
        octoberDueLabel.Location = new Point(3, 0);
        octoberDueLabel.Name = "octoberDueLabel";
        octoberDueLabel.Padding = new Padding(0, 4, 0, 0);
        octoberDueLabel.Size = new Size(70, 23);
        octoberDueLabel.TabIndex = 0;
        octoberDueLabel.Text = "October";
        // 
        // octoberDue
        // 
        octoberDue.CustomFormat = "dd MMM yyyy";
        octoberDue.Format = DateTimePickerFormat.Custom;
        octoberDue.Location = new Point(79, 3);
        octoberDue.Name = "octoberDue";
        octoberDue.ShowUpDown = true;
        octoberDue.Size = new Size(125, 23);
        octoberDue.TabIndex = 1;
        octoberDue.Value = new DateTime(2026, 10, 6, 0, 0, 0, 0);
        // 
        // novemberDuePanel
        // 
        novemberDuePanel.Controls.Add(novemberDueLabel);
        novemberDuePanel.Controls.Add(novemberDue);
        novemberDuePanel.Dock = DockStyle.Fill;
        novemberDuePanel.Location = new Point(494, 72);
        novemberDuePanel.Margin = new Padding(0);
        novemberDuePanel.Name = "novemberDuePanel";
        novemberDuePanel.Size = new Size(247, 36);
        novemberDuePanel.TabIndex = 10;
        novemberDuePanel.WrapContents = false;
        // 
        // novemberDueLabel
        // 
        novemberDueLabel.Location = new Point(3, 0);
        novemberDueLabel.Name = "novemberDueLabel";
        novemberDueLabel.Padding = new Padding(0, 4, 0, 0);
        novemberDueLabel.Size = new Size(70, 23);
        novemberDueLabel.TabIndex = 0;
        novemberDueLabel.Text = "November";
        // 
        // novemberDue
        // 
        novemberDue.CustomFormat = "dd MMM yyyy";
        novemberDue.Format = DateTimePickerFormat.Custom;
        novemberDue.Location = new Point(79, 3);
        novemberDue.Name = "novemberDue";
        novemberDue.ShowUpDown = true;
        novemberDue.Size = new Size(125, 23);
        novemberDue.TabIndex = 1;
        novemberDue.Value = new DateTime(2026, 11, 6, 0, 0, 0, 0);
        // 
        // decemberDuePanel
        // 
        decemberDuePanel.Controls.Add(decemberDueLabel);
        decemberDuePanel.Controls.Add(decemberDue);
        decemberDuePanel.Dock = DockStyle.Fill;
        decemberDuePanel.Location = new Point(741, 72);
        decemberDuePanel.Margin = new Padding(0);
        decemberDuePanel.Name = "decemberDuePanel";
        decemberDuePanel.Size = new Size(247, 36);
        decemberDuePanel.TabIndex = 11;
        decemberDuePanel.WrapContents = false;
        // 
        // decemberDueLabel
        // 
        decemberDueLabel.Location = new Point(3, 0);
        decemberDueLabel.Name = "decemberDueLabel";
        decemberDueLabel.Padding = new Padding(0, 4, 0, 0);
        decemberDueLabel.Size = new Size(70, 23);
        decemberDueLabel.TabIndex = 0;
        decemberDueLabel.Text = "December";
        // 
        // decemberDue
        // 
        decemberDue.CustomFormat = "dd MMM yyyy";
        decemberDue.Format = DateTimePickerFormat.Custom;
        decemberDue.Location = new Point(79, 3);
        decemberDue.Name = "decemberDue";
        decemberDue.ShowUpDown = true;
        decemberDue.Size = new Size(125, 23);
        decemberDue.TabIndex = 1;
        decemberDue.Value = new DateTime(2026, 12, 6, 0, 0, 0, 0);
        // 
        // houseToolbar
        // 
        houseToolbar.Controls.Add(allHouses);
        houseToolbar.Controls.Add(onlyOwned);
        houseToolbar.Dock = DockStyle.Fill;
        houseToolbar.Location = new Point(23, 288);
        houseToolbar.Name = "houseToolbar";
        houseToolbar.Size = new Size(1004, 30);
        houseToolbar.TabIndex = 5;
        // 
        // allHouses
        // 
        allHouses.AutoCheck = false;
        allHouses.AutoSize = true;
        allHouses.Location = new Point(3, 3);
        allHouses.Name = "allHouses";
        allHouses.Size = new Size(112, 19);
        allHouses.TabIndex = 0;
        allHouses.Text = "Select all houses";
        allHouses.ThreeState = true;
        allHouses.Click += AllHousesClicked;
        // 
        // onlyOwned
        // 
        onlyOwned.AutoSize = true;
        onlyOwned.Location = new Point(121, 3);
        onlyOwned.Name = "onlyOwned";
        onlyOwned.Size = new Size(161, 19);
        onlyOwned.TabIndex = 1;
        onlyOwned.Text = "Only show owned houses";
        onlyOwned.CheckedChanged += OwnedFilterChanged;
        // 
        // houses
        // 
        houses.CheckOnClick = true;
        houses.Dock = DockStyle.Fill;
        houses.Location = new Point(23, 324);
        houses.Name = "houses";
        houses.Size = new Size(1004, 268);
        houses.TabIndex = 6;
        houses.ItemCheck += HouseItemCheck;
        // 
        // note
        // 
        note.Dock = DockStyle.Fill;
        note.Location = new Point(23, 595);
        note.Name = "note";
        note.Padding = new Padding(0, 10, 0, 0);
        note.Size = new Size(1004, 65);
        note.TabIndex = 7;
        note.Text = "Select months to charge at the monthly rate. Existing charges are skipped.";
        // 
        // ChargeHousesForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = cancel;
        ClientSize = new Size(1050, 680);
        Controls.Add(layout);
        MinimumSize = new Size(950, 560);
        Name = "ChargeHousesForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Charge houses";
        layout.ResumeLayout(false);
        toolbar.ResumeLayout(false);
        toolbar.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)year).EndInit();
        amountPanel.ResumeLayout(false);
        amountPanel.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)chargeAmount).EndInit();
        accountDefaults.ResumeLayout(false);
        accountDefaults.PerformLayout();
        actions.ResumeLayout(false);
        actions.PerformLayout();
        monthlyDueGroup.ResumeLayout(false);
        monthlyDueGrid.ResumeLayout(false);
        januaryDuePanel.ResumeLayout(false);
        februaryDuePanel.ResumeLayout(false);
        marchDuePanel.ResumeLayout(false);
        aprilDuePanel.ResumeLayout(false);
        mayDuePanel.ResumeLayout(false);
        juneDuePanel.ResumeLayout(false);
        julyDuePanel.ResumeLayout(false);
        augustDuePanel.ResumeLayout(false);
        septemberDuePanel.ResumeLayout(false);
        octoberDuePanel.ResumeLayout(false);
        novemberDuePanel.ResumeLayout(false);
        decemberDuePanel.ResumeLayout(false);
        houseToolbar.ResumeLayout(false);
        houseToolbar.PerformLayout();
        ResumeLayout(false);
    }
}
