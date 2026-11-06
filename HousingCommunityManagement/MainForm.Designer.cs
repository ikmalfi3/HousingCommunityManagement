namespace HousingCommunityManagement;
partial class MainForm
{
    private System.ComponentModel.IContainer components = null;
    private TableLayoutPanel shell;
    private TableLayoutPanel sidebar;
    private Label brand;
    private Button dashboardButton;
    private Button housesButton;
    private Button billingButton;
    private Button renovationsButton;
    private Button expensesButton;
    private Button accountingButton;
    private ProgressBar collectionProgress;
    private Button settingsButton;
    private Label offlineLabel;
    private Panel contentPanel;
    private TableLayoutPanel dashboardPanel;
    private Label dateLabel;
    private Label heading;
    private TableLayoutPanel cards;
    private TableLayoutPanel houseCard;
    private Label houseCardTitle;
    private Label totalValue;
    private Label houseDetails;
    private TableLayoutPanel feeCard;
    private Label feeCardTitle;
    private Label monthlyFeeValue;
    private Label feeDetails;
    private TableLayoutPanel collectedCard;
    private Label collectedCardTitle;
    private Label collectedValue;
    private Label collectedDetails;
    private TableLayoutPanel claimsCard;
    private Label claimsCardTitle;
    private Label claimsValue;
    private Label claimsDetails;
    private Label collectionHeading;
    private Label collectionNote;
    private Label recentHeading;
    private Label recentEmpty;
    private Label footerNote;
    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }
    private void InitializeComponent()
    {
        billingButton = new Button();
        renovationsButton = new Button();
        expensesButton = new Button();
        accountingButton = new Button();
        collectionProgress = new ProgressBar();
        shell = new TableLayoutPanel();
        sidebar = new TableLayoutPanel();
        brand = new Label();
        dashboardButton = new Button();
        housesButton = new Button();
        settingsButton = new Button();
        offlineLabel = new Label();
        contentPanel = new Panel();
        dashboardPanel = new TableLayoutPanel();
        dateLabel = new Label();
        heading = new Label();
        cards = new TableLayoutPanel();
        houseCard = new TableLayoutPanel();
        houseCardTitle = new Label();
        totalValue = new Label();
        houseDetails = new Label();
        feeCard = new TableLayoutPanel();
        feeCardTitle = new Label();
        monthlyFeeValue = new Label();
        feeDetails = new Label();
        collectedCard = new TableLayoutPanel();
        collectedCardTitle = new Label();
        collectedValue = new Label();
        collectedDetails = new Label();
        claimsCard = new TableLayoutPanel();
        claimsCardTitle = new Label();
        claimsValue = new Label();
        claimsDetails = new Label();
        collectionHeading = new Label();
        collectionNote = new Label();
        recentHeading = new Label();
        recentEmpty = new Label();
        footerNote = new Label();
        shell.SuspendLayout();
        sidebar.SuspendLayout();
        contentPanel.SuspendLayout();
        dashboardPanel.SuspendLayout();
        cards.SuspendLayout();
        houseCard.SuspendLayout();
        feeCard.SuspendLayout();
        collectedCard.SuspendLayout();
        claimsCard.SuspendLayout();
        collectionHeading.SuspendLayout();
        SuspendLayout();
        // 
        // billingButton
        // 
        billingButton.BackColor = Color.FromArgb(25, 39, 57);
        billingButton.Dock = DockStyle.Top;
        billingButton.FlatAppearance.BorderSize = 0;
        billingButton.FlatStyle = FlatStyle.Flat;
        billingButton.Font = new Font("Segoe UI", 11F);
        billingButton.ForeColor = Color.White;
        billingButton.Location = new Point(15, 231);
        billingButton.Name = "billingButton";
        billingButton.Size = new Size(185, 48);
        billingButton.TabIndex = 3;
        billingButton.Text = "▤  Billing && payments";
        billingButton.TextAlign = ContentAlignment.MiddleLeft;
        billingButton.UseVisualStyleBackColor = false;
        billingButton.Click += BillingClicked;
        renovationsButton.Text = "⚒  Renovations";
        renovationsButton.Dock = DockStyle.Top;
        renovationsButton.Height = 48;
        renovationsButton.FlatStyle = FlatStyle.Flat;
        renovationsButton.FlatAppearance.BorderSize = 0;
        renovationsButton.Font = new Font("Segoe UI", 11F);
        renovationsButton.ForeColor = Color.White;
        renovationsButton.BackColor = Color.FromArgb(25, 39, 57);
        renovationsButton.TextAlign = ContentAlignment.MiddleLeft;
        renovationsButton.Click += RenovationsClicked;
        expensesButton.Name = "expensesButton";
        expensesButton.Text = "↗  Expenses && payments";
        expensesButton.Dock = DockStyle.Top;
        expensesButton.Height = 48;
        expensesButton.FlatStyle = FlatStyle.Flat;
        expensesButton.FlatAppearance.BorderSize = 0;
        expensesButton.Font = new Font("Segoe UI", 11F);
        expensesButton.ForeColor = Color.White;
        expensesButton.BackColor = Color.FromArgb(25, 39, 57);
        expensesButton.TextAlign = ContentAlignment.MiddleLeft;
        expensesButton.Click += ExpensesClicked;
        accountingButton.Name = "accountingButton";
        accountingButton.Text = "▥  Accounting";
        accountingButton.Dock = DockStyle.Top;
        accountingButton.Height = 48;
        accountingButton.FlatStyle = FlatStyle.Flat;
        accountingButton.FlatAppearance.BorderSize = 0;
        accountingButton.Font = new Font("Segoe UI", 11F);
        accountingButton.ForeColor = Color.White;
        accountingButton.BackColor = Color.FromArgb(25, 39, 57);
        accountingButton.TextAlign = ContentAlignment.MiddleLeft;
        accountingButton.Click += AccountingClicked;
        // 
        // collectionProgress
        // 
        collectionProgress.Dock = DockStyle.Bottom;
        collectionProgress.Location = new Point(0, 32);
        collectionProgress.Name = "collectionProgress";
        collectionProgress.Size = new Size(1003, 10);
        collectionProgress.TabIndex = 0;
        // 
        // shell
        // 
        shell.ColumnCount = 2;
        shell.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 215F));
        shell.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        shell.Controls.Add(sidebar, 0, 0);
        shell.Controls.Add(contentPanel, 1, 0);
        shell.Dock = DockStyle.Fill;
        shell.Location = new Point(0, 0);
        shell.Margin = new Padding(0);
        shell.Name = "shell";
        shell.RowCount = 1;
        shell.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        shell.Size = new Size(1280, 760);
        shell.TabIndex = 0;
        // 
        // sidebar
        // 
        sidebar.BackColor = Color.FromArgb(25, 39, 57);
        sidebar.ColumnCount = 1;
        sidebar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        sidebar.Controls.Add(brand, 0, 0);
        sidebar.Controls.Add(dashboardButton, 0, 1);
        sidebar.Controls.Add(housesButton, 0, 2);
        sidebar.Controls.Add(billingButton, 0, 3);
        sidebar.Controls.Add(renovationsButton, 0, 4);
        sidebar.Controls.Add(expensesButton, 0, 5);
        sidebar.Controls.Add(accountingButton, 0, 6);
        sidebar.Controls.Add(settingsButton, 0, 7);
        sidebar.Controls.Add(offlineLabel, 0, 8);
        sidebar.Dock = DockStyle.Fill;
        sidebar.Location = new Point(0, 0);
        sidebar.Margin = new Padding(0);
        sidebar.Name = "sidebar";
        sidebar.Padding = new Padding(12, 24, 12, 12);
        sidebar.RowCount = 9;
        sidebar.RowStyles.Add(new RowStyle(SizeType.Absolute, 100F));
        sidebar.RowStyles.Add(new RowStyle(SizeType.Absolute, 52F));
        sidebar.RowStyles.Add(new RowStyle(SizeType.Absolute, 52F));
        sidebar.RowStyles.Add(new RowStyle(SizeType.Absolute, 52F));
        sidebar.RowStyles.Add(new RowStyle(SizeType.Absolute, 52F));
        sidebar.RowStyles.Add(new RowStyle(SizeType.Absolute, 52F));
        sidebar.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        sidebar.RowStyles.Add(new RowStyle(SizeType.Absolute, 52F));
        sidebar.RowStyles.Add(new RowStyle(SizeType.Absolute, 45F));
        sidebar.Size = new Size(215, 760);
        sidebar.TabIndex = 0;
        // 
        // brand
        // 
        brand.Dock = DockStyle.Fill;
        brand.Font = new Font("Segoe UI", 17F, FontStyle.Bold);
        brand.ForeColor = Color.White;
        brand.Location = new Point(15, 24);
        brand.Name = "brand";
        brand.Size = new Size(185, 100);
        brand.TabIndex = 0;
        brand.Text = "COMMUNITY\nFINANCE";
        // 
        // dashboardButton
        // 
        dashboardButton.BackColor = Color.FromArgb(25, 39, 57);
        dashboardButton.Dock = DockStyle.Fill;
        dashboardButton.FlatAppearance.BorderSize = 0;
        dashboardButton.FlatStyle = FlatStyle.Flat;
        dashboardButton.Font = new Font("Segoe UI", 11F);
        dashboardButton.ForeColor = Color.White;
        dashboardButton.Location = new Point(15, 127);
        dashboardButton.Name = "dashboardButton";
        dashboardButton.Size = new Size(185, 46);
        dashboardButton.TabIndex = 1;
        dashboardButton.Text = "▦  Dashboard";
        dashboardButton.TextAlign = ContentAlignment.MiddleLeft;
        dashboardButton.UseVisualStyleBackColor = false;
        dashboardButton.Click += DashboardClicked;
        // 
        // housesButton
        // 
        housesButton.BackColor = Color.FromArgb(25, 39, 57);
        housesButton.Dock = DockStyle.Fill;
        housesButton.FlatAppearance.BorderSize = 0;
        housesButton.FlatStyle = FlatStyle.Flat;
        housesButton.Font = new Font("Segoe UI", 11F);
        housesButton.ForeColor = Color.White;
        housesButton.Location = new Point(15, 179);
        housesButton.Name = "housesButton";
        housesButton.Size = new Size(185, 46);
        housesButton.TabIndex = 2;
        housesButton.Text = "⌂  Households";
        housesButton.TextAlign = ContentAlignment.MiddleLeft;
        housesButton.UseVisualStyleBackColor = false;
        housesButton.Click += HousesClicked;
        // 
        // settingsButton
        // 
        settingsButton.BackColor = Color.FromArgb(25, 39, 57);
        settingsButton.Dock = DockStyle.Fill;
        settingsButton.FlatAppearance.BorderSize = 0;
        settingsButton.FlatStyle = FlatStyle.Flat;
        settingsButton.Font = new Font("Segoe UI", 11F);
        settingsButton.ForeColor = Color.White;
        settingsButton.Location = new Point(15, 654);
        settingsButton.Name = "settingsButton";
        settingsButton.Size = new Size(185, 46);
        settingsButton.TabIndex = 4;
        settingsButton.Text = "⚙  Settings";
        settingsButton.TextAlign = ContentAlignment.MiddleLeft;
        settingsButton.UseVisualStyleBackColor = false;
        settingsButton.Click += SettingsClicked;
        // 
        // offlineLabel
        // 
        offlineLabel.Dock = DockStyle.Fill;
        offlineLabel.ForeColor = Color.FromArgb(172, 190, 210);
        offlineLabel.Location = new Point(15, 703);
        offlineLabel.Name = "offlineLabel";
        offlineLabel.Size = new Size(185, 45);
        offlineLabel.TabIndex = 5;
        offlineLabel.Text = "Offline desktop app";
        offlineLabel.TextAlign = ContentAlignment.BottomLeft;
        // 
        // contentPanel
        // 
        contentPanel.BackColor = Color.White;
        contentPanel.Controls.Add(dashboardPanel);
        contentPanel.Dock = DockStyle.Fill;
        contentPanel.Location = new Point(215, 0);
        contentPanel.Margin = new Padding(0);
        contentPanel.Name = "contentPanel";
        contentPanel.Size = new Size(1065, 760);
        contentPanel.TabIndex = 1;
        // 
        // dashboardPanel
        // 
        dashboardPanel.ColumnCount = 1;
        dashboardPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        dashboardPanel.Controls.Add(dateLabel, 0, 0);
        dashboardPanel.Controls.Add(heading, 0, 1);
        dashboardPanel.Controls.Add(cards, 0, 2);
        dashboardPanel.Controls.Add(collectionHeading, 0, 3);
        dashboardPanel.Controls.Add(collectionNote, 0, 4);
        dashboardPanel.Controls.Add(recentHeading, 0, 5);
        dashboardPanel.Controls.Add(recentEmpty, 0, 6);
        dashboardPanel.Controls.Add(footerNote, 0, 7);
        dashboardPanel.Dock = DockStyle.Fill;
        dashboardPanel.Location = new Point(0, 0);
        dashboardPanel.Name = "dashboardPanel";
        dashboardPanel.Padding = new Padding(28);
        dashboardPanel.RowCount = 8;
        dashboardPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
        dashboardPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 55F));
        dashboardPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 200F));
        dashboardPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
        dashboardPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
        dashboardPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 45F));
        dashboardPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        dashboardPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        dashboardPanel.Size = new Size(1065, 760);
        dashboardPanel.TabIndex = 0;
        // 
        // dateLabel
        // 
        dateLabel.Dock = DockStyle.Fill;
        dateLabel.ForeColor = Color.DimGray;
        dateLabel.Location = new Point(31, 28);
        dateLabel.Name = "dateLabel";
        dateLabel.Size = new Size(1003, 25);
        dateLabel.TabIndex = 0;
        dateLabel.Text = "COMMUNITY FINANCE";
        // 
        // heading
        // 
        heading.Dock = DockStyle.Fill;
        heading.Font = new Font("Segoe UI", 23F, FontStyle.Bold);
        heading.Location = new Point(31, 53);
        heading.Name = "heading";
        heading.Size = new Size(1003, 55);
        heading.TabIndex = 1;
        heading.Text = "Management Dashboard";
        // 
        // cards
        // 
        cards.ColumnCount = 2;
        cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        cards.Controls.Add(houseCard, 0, 0);
        cards.Controls.Add(feeCard, 1, 0);
        cards.Controls.Add(collectedCard, 0, 1);
        cards.Controls.Add(claimsCard, 1, 1);
        cards.Dock = DockStyle.Fill;
        cards.Location = new Point(31, 111);
        cards.Name = "cards";
        cards.RowCount = 2;
        cards.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        cards.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        cards.Size = new Size(1003, 194);
        cards.TabIndex = 2;
        // 
        // houseCard
        // 
        houseCard.BackColor = Color.FromArgb(245, 246, 248);
        houseCard.ColumnCount = 1;
        houseCard.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        houseCard.Controls.Add(houseCardTitle, 0, 0);
        houseCard.Controls.Add(totalValue, 0, 1);
        houseCard.Controls.Add(houseDetails, 0, 2);
        houseCard.Dock = DockStyle.Fill;
        houseCard.Location = new Point(3, 3);
        houseCard.Name = "houseCard";
        houseCard.Padding = new Padding(14, 8, 14, 8);
        houseCard.RowCount = 3;
        houseCard.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        houseCard.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        houseCard.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        houseCard.Size = new Size(495, 91);
        houseCard.TabIndex = 0;
        // 
        // houseCardTitle
        // 
        houseCardTitle.Dock = DockStyle.Fill;
        houseCardTitle.ForeColor = Color.DimGray;
        houseCardTitle.Location = new Point(17, 8);
        houseCardTitle.Name = "houseCardTitle";
        houseCardTitle.Size = new Size(461, 20);
        houseCardTitle.TabIndex = 0;
        houseCardTitle.Text = "Total houses";
        // 
        // totalValue
        // 
        totalValue.Dock = DockStyle.Fill;
        totalValue.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
        totalValue.Location = new Point(17, 28);
        totalValue.Name = "totalValue";
        totalValue.Size = new Size(461, 35);
        totalValue.TabIndex = 1;
        totalValue.Text = "0";
        // 
        // houseDetails
        // 
        houseDetails.Dock = DockStyle.Fill;
        houseDetails.ForeColor = Color.DimGray;
        houseDetails.Location = new Point(17, 63);
        houseDetails.Name = "houseDetails";
        houseDetails.Size = new Size(461, 20);
        houseDetails.TabIndex = 2;
        houseDetails.Text = "0 active / 0 owned";
        // 
        // feeCard
        // 
        feeCard.BackColor = Color.FromArgb(245, 246, 248);
        feeCard.ColumnCount = 1;
        feeCard.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        feeCard.Controls.Add(feeCardTitle, 0, 0);
        feeCard.Controls.Add(monthlyFeeValue, 0, 1);
        feeCard.Controls.Add(feeDetails, 0, 2);
        feeCard.Dock = DockStyle.Fill;
        feeCard.Location = new Point(504, 3);
        feeCard.Name = "feeCard";
        feeCard.Padding = new Padding(14, 8, 14, 8);
        feeCard.RowCount = 3;
        feeCard.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        feeCard.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        feeCard.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        feeCard.Size = new Size(496, 91);
        feeCard.TabIndex = 1;
        // 
        // feeCardTitle
        // 
        feeCardTitle.Dock = DockStyle.Fill;
        feeCardTitle.ForeColor = Color.DimGray;
        feeCardTitle.Location = new Point(17, 8);
        feeCardTitle.Name = "feeCardTitle";
        feeCardTitle.Size = new Size(462, 20);
        feeCardTitle.TabIndex = 0;
        feeCardTitle.Text = "Outstanding this month";
        // 
        // monthlyFeeValue
        // 
        monthlyFeeValue.Dock = DockStyle.Fill;
        monthlyFeeValue.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
        monthlyFeeValue.Location = new Point(17, 28);
        monthlyFeeValue.Name = "monthlyFeeValue";
        monthlyFeeValue.Size = new Size(462, 35);
        monthlyFeeValue.TabIndex = 1;
        monthlyFeeValue.Text = "RM 0.00";
        // 
        // feeDetails
        // 
        feeDetails.Dock = DockStyle.Fill;
        feeDetails.ForeColor = Color.DimGray;
        feeDetails.Location = new Point(17, 63);
        feeDetails.Name = "feeDetails";
        feeDetails.Size = new Size(462, 20);
        feeDetails.TabIndex = 2;
        feeDetails.Text = "Applies to all houses";
        // 
        // collectedCard
        // 
        collectedCard.BackColor = Color.FromArgb(245, 246, 248);
        collectedCard.ColumnCount = 1;
        collectedCard.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        collectedCard.Controls.Add(collectedCardTitle, 0, 0);
        collectedCard.Controls.Add(collectedValue, 0, 1);
        collectedCard.Controls.Add(collectedDetails, 0, 2);
        collectedCard.Dock = DockStyle.Fill;
        collectedCard.Location = new Point(3, 100);
        collectedCard.Name = "collectedCard";
        collectedCard.Padding = new Padding(14, 8, 14, 8);
        collectedCard.RowCount = 3;
        collectedCard.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        collectedCard.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        collectedCard.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        collectedCard.Size = new Size(495, 91);
        collectedCard.TabIndex = 2;
        // 
        // collectedCardTitle
        // 
        collectedCardTitle.Dock = DockStyle.Fill;
        collectedCardTitle.ForeColor = Color.DimGray;
        collectedCardTitle.Location = new Point(17, 8);
        collectedCardTitle.Name = "collectedCardTitle";
        collectedCardTitle.Size = new Size(461, 20);
        collectedCardTitle.TabIndex = 0;
        collectedCardTitle.Text = "Collected this month";
        // 
        // collectedValue
        // 
        collectedValue.Dock = DockStyle.Fill;
        collectedValue.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
        collectedValue.Location = new Point(17, 28);
        collectedValue.Name = "collectedValue";
        collectedValue.Size = new Size(461, 35);
        collectedValue.TabIndex = 1;
        collectedValue.Text = "Unavailable";
        // 
        // collectedDetails
        // 
        collectedDetails.Dock = DockStyle.Fill;
        collectedDetails.ForeColor = Color.DimGray;
        collectedDetails.Location = new Point(17, 63);
        collectedDetails.Name = "collectedDetails";
        collectedDetails.Size = new Size(461, 20);
        collectedDetails.TabIndex = 2;
        collectedDetails.Text = "Payments module not yet available";
        // 
        // claimsCard
        // 
        claimsCard.BackColor = Color.FromArgb(245, 246, 248);
        claimsCard.ColumnCount = 1;
        claimsCard.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        claimsCard.Controls.Add(claimsCardTitle, 0, 0);
        claimsCard.Controls.Add(claimsValue, 0, 1);
        claimsCard.Controls.Add(claimsDetails, 0, 2);
        claimsCard.Dock = DockStyle.Fill;
        claimsCard.Location = new Point(504, 100);
        claimsCard.Name = "claimsCard";
        claimsCard.Padding = new Padding(14, 8, 14, 8);
        claimsCard.RowCount = 3;
        claimsCard.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        claimsCard.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        claimsCard.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        claimsCard.Size = new Size(496, 91);
        claimsCard.TabIndex = 3;
        // 
        // claimsCardTitle
        // 
        claimsCardTitle.Dock = DockStyle.Fill;
        claimsCardTitle.ForeColor = Color.DimGray;
        claimsCardTitle.Location = new Point(17, 8);
        claimsCardTitle.Name = "claimsCardTitle";
        claimsCardTitle.Size = new Size(462, 20);
        claimsCardTitle.TabIndex = 0;
        claimsCardTitle.Text = "Overdue balance";
        // 
        // claimsValue
        // 
        claimsValue.Dock = DockStyle.Fill;
        claimsValue.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
        claimsValue.Location = new Point(17, 28);
        claimsValue.Name = "claimsValue";
        claimsValue.Size = new Size(462, 35);
        claimsValue.TabIndex = 1;
        claimsValue.Text = "Unavailable";
        // 
        // claimsDetails
        // 
        claimsDetails.Dock = DockStyle.Fill;
        claimsDetails.ForeColor = Color.DimGray;
        claimsDetails.Location = new Point(17, 63);
        claimsDetails.Name = "claimsDetails";
        claimsDetails.Size = new Size(462, 20);
        claimsDetails.TabIndex = 2;
        claimsDetails.Text = "Claims module not yet available";
        // 
        // collectionHeading
        // 
        collectionHeading.Controls.Add(collectionProgress);
        collectionHeading.Dock = DockStyle.Fill;
        collectionHeading.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        collectionHeading.Location = new Point(31, 308);
        collectionHeading.Name = "collectionHeading";
        collectionHeading.Size = new Size(1003, 42);
        collectionHeading.TabIndex = 3;
        collectionHeading.Text = "Maintenance fee collection";
        collectionHeading.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // collectionNote
        // 
        collectionNote.Dock = DockStyle.Fill;
        collectionNote.ForeColor = Color.DimGray;
        collectionNote.Location = new Point(31, 350);
        collectionNote.Name = "collectionNote";
        collectionNote.Size = new Size(1003, 48);
        collectionNote.TabIndex = 4;
        collectionNote.Text = "Collection rate and outstanding balances will appear once billing and payments are available.";
        collectionNote.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // recentHeading
        // 
        recentHeading.Dock = DockStyle.Fill;
        recentHeading.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        recentHeading.Location = new Point(31, 398);
        recentHeading.Name = "recentHeading";
        recentHeading.Size = new Size(1003, 45);
        recentHeading.TabIndex = 5;
        recentHeading.Text = "Recent payments";
        recentHeading.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // recentEmpty
        // 
        recentEmpty.BackColor = Color.FromArgb(250, 250, 251);
        recentEmpty.Dock = DockStyle.Fill;
        recentEmpty.ForeColor = Color.DimGray;
        recentEmpty.Location = new Point(31, 443);
        recentEmpty.Name = "recentEmpty";
        recentEmpty.Padding = new Padding(16);
        recentEmpty.Size = new Size(1003, 259);
        recentEmpty.TabIndex = 6;
        recentEmpty.Text = "Payment history will appear here when payment recording is available.";
        // 
        // footerNote
        // 
        footerNote.Dock = DockStyle.Fill;
        footerNote.ForeColor = Color.DimGray;
        footerNote.Location = new Point(31, 702);
        footerNote.Name = "footerNote";
        footerNote.Size = new Size(1003, 30);
        footerNote.TabIndex = 7;
        footerNote.Text = "House counts and maintenance fee are from your saved community data.";
        footerNote.TextAlign = ContentAlignment.BottomLeft;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1280, 760);
        Controls.Add(shell);
        MinimumSize = new Size(1100, 700);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Community Finance";
        FormClosing += MainClosing;
        Shown += MainShown;
        shell.ResumeLayout(false);
        sidebar.ResumeLayout(false);
        contentPanel.ResumeLayout(false);
        dashboardPanel.ResumeLayout(false);
        cards.ResumeLayout(false);
        houseCard.ResumeLayout(false);
        feeCard.ResumeLayout(false);
        collectedCard.ResumeLayout(false);
        claimsCard.ResumeLayout(false);
        collectionHeading.ResumeLayout(false);
        ResumeLayout(false);
    }
}
