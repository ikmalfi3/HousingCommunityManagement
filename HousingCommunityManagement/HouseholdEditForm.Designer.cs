namespace HousingCommunityManagement;
partial class HouseholdEditForm
{
    private System.ComponentModel.IContainer components = null;
    private TableLayoutPanel table;
    private FlowLayoutPanel buttons;
    private Button save;
    private Button cancel;
    private TextBox house;
    private Label houseLabel;
    private ComboBox race;
    private Label raceLabel;
    private CheckBox owned;
    private Label ownedLabel;
    private TextBox owner;
    private Label ownerLabel;
    private TextBox contact;
    private Label contactLabel;
    private CheckedListBox flags;
    private TableLayoutPanel flagPanel;
    private TableLayoutPanel flagEntry;
    private TextBox newFlag;
    private Button addFlag;
    private Button deleteFlag;
    private Label flagsLabel;
    private TextBox derived;
    private Label derivedLabel;
    private CheckBox active;
    private Label activeLabel;
    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }
    private void InitializeComponent()
    {
        race = new ComboBox();
        raceLabel = new Label();
        table = new TableLayoutPanel();
        houseLabel = new Label();
        house = new TextBox();
        ownedLabel = new Label();
        owned = new CheckBox();
        ownerLabel = new Label();
        owner = new TextBox();
        contactLabel = new Label();
        contact = new TextBox();
        flagsLabel = new Label();
        flagPanel = new TableLayoutPanel();
        flagEntry = new TableLayoutPanel();
        newFlag = new TextBox();
        addFlag = new Button();
        deleteFlag = new Button();
        flags = new CheckedListBox();
        derivedLabel = new Label();
        derived = new TextBox();
        activeLabel = new Label();
        active = new CheckBox();
        buttons = new FlowLayoutPanel();
        cancel = new Button();
        save = new Button();
        table.SuspendLayout();
        flagPanel.SuspendLayout();
        flagEntry.SuspendLayout();
        buttons.SuspendLayout();
        SuspendLayout();
        // 
        // race
        // 
        race.Dock = DockStyle.Fill;
        race.DropDownStyle = ComboBoxStyle.DropDownList;
        race.Items.AddRange(new object[] { "", "Malay", "Chinese", "Indian", "Others" });
        race.Location = new Point(203, 95);
        race.Name = "race";
        race.Size = new Size(414, 23);
        race.TabIndex = 2;
        // 
        // raceLabel
        // 
        raceLabel.AutoSize = true;
        raceLabel.Location = new Point(23, 92);
        raceLabel.Name = "raceLabel";
        raceLabel.Size = new Size(32, 15);
        raceLabel.TabIndex = 8;
        raceLabel.Text = "Race";
        // 
        // table
        // 
        table.ColumnCount = 2;
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180F));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        table.Controls.Add(houseLabel, 0, 0);
        table.Controls.Add(house, 1, 0);
        table.Controls.Add(ownedLabel, 0, 1);
        table.Controls.Add(owned, 1, 1);
        table.Controls.Add(ownerLabel, 0, 3);
        table.Controls.Add(owner, 1, 3);
        table.Controls.Add(contactLabel, 0, 4);
        table.Controls.Add(contact, 1, 4);
        table.Controls.Add(flagsLabel, 0, 5);
        table.Controls.Add(flagPanel, 1, 5);
        table.Controls.Add(derivedLabel, 0, 6);
        table.Controls.Add(derived, 1, 6);
        table.Controls.Add(activeLabel, 0, 7);
        table.Controls.Add(active, 1, 7);
        table.Controls.Add(buttons, 0, 8);
        table.Controls.Add(raceLabel, 0, 2);
        table.Controls.Add(race, 1, 2);
        table.Dock = DockStyle.Fill;
        table.Location = new Point(0, 0);
        table.Name = "table";
        table.Padding = new Padding(20);
        table.RowCount = 9;
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 85F));
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 85F));
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 130F));
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 110F));
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
        table.RowStyles.Add(new RowStyle());
        table.Size = new Size(640, 706);
        table.TabIndex = 0;
        // 
        // houseLabel
        // 
        houseLabel.AutoSize = true;
        houseLabel.Location = new Point(23, 20);
        houseLabel.Name = "houseLabel";
        houseLabel.Size = new Size(86, 15);
        houseLabel.TabIndex = 0;
        houseLabel.Text = "House number";
        // 
        // house
        // 
        house.Dock = DockStyle.Fill;
        house.Location = new Point(203, 23);
        house.MaxLength = 50;
        house.Name = "house";
        house.Size = new Size(414, 23);
        house.TabIndex = 0;
        // 
        // ownedLabel
        // 
        ownedLabel.AutoSize = true;
        ownedLabel.Location = new Point(23, 56);
        ownedLabel.Name = "ownedLabel";
        ownedLabel.Size = new Size(64, 15);
        ownedLabel.TabIndex = 1;
        ownedLabel.Text = "Ownership";
        // 
        // owned
        // 
        owned.Dock = DockStyle.Fill;
        owned.Location = new Point(203, 59);
        owned.Name = "owned";
        owned.Size = new Size(414, 30);
        owned.TabIndex = 1;
        owned.Text = "Owned";
        // 
        // ownerLabel
        // 
        ownerLabel.AutoSize = true;
        ownerLabel.Location = new Point(23, 128);
        ownerLabel.Name = "ownerLabel";
        ownerLabel.Size = new Size(120, 15);
        ownerLabel.TabIndex = 2;
        ownerLabel.Text = "Owners (one per line)";
        // 
        // owner
        // 
        owner.AcceptsReturn = true;
        owner.Dock = DockStyle.Fill;
        owner.Location = new Point(203, 131);
        owner.Multiline = true;
        owner.Name = "owner";
        owner.ScrollBars = ScrollBars.Vertical;
        owner.Size = new Size(414, 79);
        owner.TabIndex = 2;
        // 
        // contactLabel
        // 
        contactLabel.AutoSize = true;
        contactLabel.Location = new Point(23, 213);
        contactLabel.Name = "contactLabel";
        contactLabel.Size = new Size(127, 15);
        contactLabel.TabIndex = 3;
        contactLabel.Text = "Contacts (one per line)";
        // 
        // contact
        // 
        contact.AcceptsReturn = true;
        contact.Dock = DockStyle.Fill;
        contact.Location = new Point(203, 216);
        contact.Multiline = true;
        contact.Name = "contact";
        contact.ScrollBars = ScrollBars.Vertical;
        contact.Size = new Size(414, 79);
        contact.TabIndex = 3;
        // 
        // flagsLabel
        // 
        flagsLabel.AutoSize = true;
        flagsLabel.Location = new Point(23, 298);
        flagsLabel.Name = "flagsLabel";
        flagsLabel.Size = new Size(75, 15);
        flagsLabel.TabIndex = 4;
        flagsLabel.Text = "Manual flags";
        // 
        // flagPanel
        // 
        flagPanel.ColumnCount = 1;
        flagPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        flagPanel.Controls.Add(flagEntry, 0, 0);
        flagPanel.Controls.Add(flags, 0, 1);
        flagPanel.Dock = DockStyle.Fill;
        flagPanel.Location = new Point(200, 298);
        flagPanel.Margin = new Padding(0);
        flagPanel.Name = "flagPanel";
        flagPanel.RowCount = 2;
        flagPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
        flagPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        flagPanel.Size = new Size(420, 130);
        flagPanel.TabIndex = 5;
        // 
        // flagEntry
        // 
        flagEntry.ColumnCount = 3;
        flagEntry.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        flagEntry.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 65F));
        flagEntry.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 65F));
        flagEntry.Controls.Add(newFlag, 0, 0);
        flagEntry.Controls.Add(addFlag, 1, 0);
        flagEntry.Controls.Add(deleteFlag, 2, 0);
        flagEntry.Dock = DockStyle.Fill;
        flagEntry.Location = new Point(0, 0);
        flagEntry.Margin = new Padding(0);
        flagEntry.Name = "flagEntry";
        flagEntry.RowCount = 1;
        flagEntry.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        flagEntry.Size = new Size(420, 32);
        flagEntry.TabIndex = 0;
        // 
        // newFlag
        // 
        newFlag.Dock = DockStyle.Fill;
        newFlag.Location = new Point(3, 3);
        newFlag.MaxLength = 80;
        newFlag.Name = "newFlag";
        newFlag.PlaceholderText = "New flag name";
        newFlag.Size = new Size(284, 23);
        newFlag.TabIndex = 0;
        newFlag.KeyDown += NewFlagKeyDown;
        // 
        // addFlag
        // 
        addFlag.Dock = DockStyle.Fill;
        addFlag.Location = new Point(293, 3);
        addFlag.Name = "addFlag";
        addFlag.Size = new Size(59, 26);
        addFlag.TabIndex = 1;
        addFlag.Text = "Add";
        addFlag.Click += AddFlagClicked;
        // 
        // deleteFlag
        // 
        deleteFlag.Dock = DockStyle.Fill;
        deleteFlag.Location = new Point(358, 3);
        deleteFlag.Name = "deleteFlag";
        deleteFlag.Size = new Size(59, 26);
        deleteFlag.TabIndex = 2;
        deleteFlag.Text = "Delete";
        deleteFlag.Click += DeleteFlagClicked;
        // 
        // flags
        // 
        flags.CheckOnClick = true;
        flags.Dock = DockStyle.Fill;
        flags.Location = new Point(3, 35);
        flags.Name = "flags";
        flags.Size = new Size(414, 92);
        flags.TabIndex = 1;
        // 
        // derivedLabel
        // 
        derivedLabel.AutoSize = true;
        derivedLabel.Location = new Point(23, 428);
        derivedLabel.Name = "derivedLabel";
        derivedLabel.Size = new Size(135, 15);
        derivedLabel.TabIndex = 5;
        derivedLabel.Text = "Derived flags (read only)";
        // 
        // derived
        // 
        derived.Dock = DockStyle.Fill;
        derived.Location = new Point(203, 431);
        derived.Multiline = true;
        derived.Name = "derived";
        derived.ReadOnly = true;
        derived.ScrollBars = ScrollBars.Vertical;
        derived.Size = new Size(414, 104);
        derived.TabIndex = 5;
        // 
        // activeLabel
        // 
        activeLabel.AutoSize = true;
        activeLabel.Location = new Point(23, 538);
        activeLabel.Name = "activeLabel";
        activeLabel.Size = new Size(78, 15);
        activeLabel.TabIndex = 6;
        activeLabel.Text = "Record status";
        // 
        // active
        // 
        active.Checked = true;
        active.CheckState = CheckState.Checked;
        active.Dock = DockStyle.Fill;
        active.Location = new Point(203, 541);
        active.Name = "active";
        active.Size = new Size(414, 30);
        active.TabIndex = 6;
        active.Text = "Active house";
        // 
        // buttons
        // 
        buttons.AutoSize = true;
        table.SetColumnSpan(buttons, 2);
        buttons.Controls.Add(cancel);
        buttons.Controls.Add(save);
        buttons.Dock = DockStyle.Fill;
        buttons.FlowDirection = FlowDirection.RightToLeft;
        buttons.Location = new Point(23, 577);
        buttons.Name = "buttons";
        buttons.Size = new Size(594, 106);
        buttons.TabIndex = 7;
        // 
        // cancel
        // 
        cancel.AutoSize = true;
        cancel.DialogResult = DialogResult.Cancel;
        cancel.Location = new Point(516, 3);
        cancel.Name = "cancel";
        cancel.Size = new Size(75, 25);
        cancel.TabIndex = 0;
        cancel.Text = "Cancel";
        // 
        // save
        // 
        save.AutoSize = true;
        save.Location = new Point(435, 3);
        save.Name = "save";
        save.Size = new Size(75, 25);
        save.TabIndex = 1;
        save.Text = "Save";
        save.Click += SaveClicked;
        // 
        // HouseholdEditForm
        // 
        AcceptButton = save;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = cancel;
        ClientSize = new Size(640, 706);
        Controls.Add(table);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "HouseholdEditForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Edit house";
        table.ResumeLayout(false);
        table.PerformLayout();
        flagPanel.ResumeLayout(false);
        flagEntry.ResumeLayout(false);
        flagEntry.PerformLayout();
        buttons.ResumeLayout(false);
        buttons.PerformLayout();
        ResumeLayout(false);
    }
}
