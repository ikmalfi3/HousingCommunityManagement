#nullable enable
namespace HousingCommunityManagement;
partial class ExpenseEditForm
{
    private System.ComponentModel.IContainer? components = null;
    private TableLayoutPanel layout = null!;
    private TableLayoutPanel metadata = null!;
    private Label kindLabel = null!;
    private ComboBox kind = null!;
    private Label payeeLabel = null!;
    private ComboBox payee = null!;
    private Label documentLabel = null!;
    private DateTimePicker documentDate = null!;
    private Label accountingLabel = null!;
    private DateTimePicker accountingDate = null!;
    private Label referenceLabel = null!;
    private TextBox reference = null!;
    private Label dueLabel = null!;
    private DateTimePicker dueDate = null!;
    private TextBox description = null!;
    private Label lineNote = null!;
    private DataGridView lines = null!;
    private DataGridViewComboBoxColumn categoryColumn = null!;
    private DataGridViewTextBoxColumn descriptionColumn = null!;
    private DataGridViewTextBoxColumn amountColumn = null!;
    private TableLayoutPanel attachmentsPanel = null!;
    private ListBox attachments = null!;
    private FlowLayoutPanel attachmentButtons = null!;
    private Button addAttachment = null!;
    private Button removeAttachment = null!;
    private Button setupButton = null!;
    private FlowLayoutPanel paymentPanel = null!;
    private Label cashLabel = null!;
    private ComboBox cash = null!;
    private Label paymentDateLabel = null!;
    private DateTimePicker paymentDate = null!;
    private Label paymentReferenceLabel = null!;
    private TextBox paymentReference = null!;
    private FlowLayoutPanel buttons = null!;
    private Button saveDraft = null!;
    private Button saveAndPay = null!;
    private Button cancel = null!;
    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }
    private void InitializeComponent()
    {
        DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
        layout = new TableLayoutPanel();
        metadata = new TableLayoutPanel();
        kindLabel = new Label();
        kind = new ComboBox();
        payeeLabel = new Label();
        payee = new ComboBox();
        documentLabel = new Label();
        documentDate = new DateTimePicker();
        accountingLabel = new Label();
        accountingDate = new DateTimePicker();
        referenceLabel = new Label();
        reference = new TextBox();
        dueLabel = new Label();
        dueDate = new DateTimePicker();
        description = new TextBox();
        lineNote = new Label();
        lines = new DataGridView();
        categoryColumn = new DataGridViewComboBoxColumn();
        descriptionColumn = new DataGridViewTextBoxColumn();
        amountColumn = new DataGridViewTextBoxColumn();
        attachmentsPanel = new TableLayoutPanel();
        attachments = new ListBox();
        attachmentButtons = new FlowLayoutPanel();
        addAttachment = new Button();
        removeAttachment = new Button();
        setupButton = new Button();
        paymentPanel = new FlowLayoutPanel();
        cashLabel = new Label();
        cash = new ComboBox();
        paymentDateLabel = new Label();
        paymentDate = new DateTimePicker();
        paymentReferenceLabel = new Label();
        paymentReference = new TextBox();
        buttons = new FlowLayoutPanel();
        cancel = new Button();
        saveAndPay = new Button();
        saveDraft = new Button();
        layout.SuspendLayout();
        metadata.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)lines).BeginInit();
        attachmentsPanel.SuspendLayout();
        attachmentButtons.SuspendLayout();
        paymentPanel.SuspendLayout();
        buttons.SuspendLayout();
        SuspendLayout();
        // 
        // layout
        // 
        layout.ColumnCount = 1;
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        layout.Controls.Add(metadata, 0, 0);
        layout.Controls.Add(description, 0, 1);
        layout.Controls.Add(lineNote, 0, 2);
        layout.Controls.Add(lines, 0, 3);
        layout.Controls.Add(attachmentsPanel, 0, 4);
        layout.Controls.Add(paymentPanel, 0, 5);
        layout.Controls.Add(buttons, 0, 6);
        layout.Dock = DockStyle.Fill;
        layout.Location = new Point(0, 0);
        layout.Name = "layout";
        layout.Padding = new Padding(16);
        layout.RowCount = 7;
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 160F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 45F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 45F));
        layout.Size = new Size(1000, 740);
        layout.TabIndex = 0;
        // 
        // metadata
        // 
        metadata.ColumnCount = 4;
        metadata.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
        metadata.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        metadata.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 135F));
        metadata.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        metadata.Controls.Add(kindLabel, 0, 0);
        metadata.Controls.Add(kind, 1, 0);
        metadata.Controls.Add(payeeLabel, 2, 0);
        metadata.Controls.Add(payee, 3, 0);
        metadata.Controls.Add(documentLabel, 0, 1);
        metadata.Controls.Add(documentDate, 1, 1);
        metadata.Controls.Add(accountingLabel, 2, 1);
        metadata.Controls.Add(accountingDate, 3, 1);
        metadata.Controls.Add(referenceLabel, 0, 2);
        metadata.Controls.Add(reference, 1, 2);
        metadata.Controls.Add(dueLabel, 2, 2);
        metadata.Controls.Add(dueDate, 3, 2);
        metadata.Dock = DockStyle.Fill;
        metadata.Location = new Point(19, 19);
        metadata.Name = "metadata";
        metadata.RowCount = 3;
        metadata.RowStyles.Add(new RowStyle(SizeType.Percent, 33.333F));
        metadata.RowStyles.Add(new RowStyle(SizeType.Percent, 33.333F));
        metadata.RowStyles.Add(new RowStyle(SizeType.Percent, 33.333F));
        metadata.Size = new Size(962, 154);
        metadata.TabIndex = 0;
        // 
        // kindLabel
        // 
        kindLabel.AutoSize = true;
        kindLabel.Location = new Point(3, 0);
        kindLabel.Name = "kindLabel";
        kindLabel.Size = new Size(32, 15);
        kindLabel.TabIndex = 0;
        kindLabel.Text = "Type";
        // 
        // kind
        // 
        kind.Dock = DockStyle.Fill;
        kind.DropDownStyle = ComboBoxStyle.DropDownList;
        kind.Items.AddRange(new object[] { "Supplier bill", "Reimbursement claim", "Direct expense" });
        kind.Location = new Point(123, 3);
        kind.Name = "kind";
        kind.Size = new Size(347, 23);
        kind.TabIndex = 1;
        kind.SelectedIndexChanged += KindChanged;
        // 
        // payeeLabel
        // 
        payeeLabel.AutoSize = true;
        payeeLabel.Location = new Point(476, 0);
        payeeLabel.Name = "payeeLabel";
        payeeLabel.Size = new Size(38, 15);
        payeeLabel.TabIndex = 2;
        payeeLabel.Text = "Payee";
        // 
        // payee
        // 
        payee.Dock = DockStyle.Fill;
        payee.DropDownStyle = ComboBoxStyle.DropDownList;
        payee.Location = new Point(611, 3);
        payee.Name = "payee";
        payee.Size = new Size(348, 23);
        payee.TabIndex = 3;
        // 
        // documentLabel
        // 
        documentLabel.AutoSize = true;
        documentLabel.Location = new Point(3, 51);
        documentLabel.Name = "documentLabel";
        documentLabel.Size = new Size(89, 15);
        documentLabel.TabIndex = 4;
        documentLabel.Text = "Document date";
        // 
        // documentDate
        // 
        documentDate.Dock = DockStyle.Fill;
        documentDate.Format = DateTimePickerFormat.Short;
        documentDate.Location = new Point(123, 54);
        documentDate.Name = "documentDate";
        documentDate.Size = new Size(347, 23);
        documentDate.TabIndex = 5;
        // 
        // accountingLabel
        // 
        accountingLabel.AutoSize = true;
        accountingLabel.Location = new Point(476, 51);
        accountingLabel.Name = "accountingLabel";
        accountingLabel.Size = new Size(95, 15);
        accountingLabel.TabIndex = 6;
        accountingLabel.Text = "Accounting date";
        // 
        // accountingDate
        // 
        accountingDate.Dock = DockStyle.Fill;
        accountingDate.Format = DateTimePickerFormat.Short;
        accountingDate.Location = new Point(611, 54);
        accountingDate.Name = "accountingDate";
        accountingDate.Size = new Size(348, 23);
        accountingDate.TabIndex = 7;
        // 
        // referenceLabel
        // 
        referenceLabel.AutoSize = true;
        referenceLabel.Location = new Point(3, 102);
        referenceLabel.Name = "referenceLabel";
        referenceLabel.Size = new Size(109, 15);
        referenceLabel.TabIndex = 8;
        referenceLabel.Text = "Invoice / receipt ref";
        // 
        // reference
        // 
        reference.Dock = DockStyle.Fill;
        reference.Location = new Point(123, 105);
        reference.MaxLength = 150;
        reference.Name = "reference";
        reference.Size = new Size(347, 23);
        reference.TabIndex = 9;
        // 
        // dueLabel
        // 
        dueLabel.AutoSize = true;
        dueLabel.Location = new Point(476, 102);
        dueLabel.Name = "dueLabel";
        dueLabel.Size = new Size(109, 15);
        dueLabel.TabIndex = 10;
        dueLabel.Text = "Due date (optional)";
        // 
        // dueDate
        // 
        dueDate.Checked = false;
        dueDate.Dock = DockStyle.Fill;
        dueDate.Format = DateTimePickerFormat.Short;
        dueDate.Location = new Point(611, 105);
        dueDate.Name = "dueDate";
        dueDate.ShowCheckBox = true;
        dueDate.Size = new Size(348, 23);
        dueDate.TabIndex = 11;
        // 
        // description
        // 
        description.Dock = DockStyle.Fill;
        description.Location = new Point(19, 179);
        description.MaxLength = 500;
        description.Multiline = true;
        description.Name = "description";
        description.PlaceholderText = "Expense description (required)";
        description.Size = new Size(962, 54);
        description.TabIndex = 1;
        // 
        // lineNote
        // 
        lineNote.Dock = DockStyle.Fill;
        lineNote.Location = new Point(19, 236);
        lineNote.Name = "lineNote";
        lineNote.Size = new Size(962, 28);
        lineNote.TabIndex = 2;
        lineNote.Text = "Expense lines: choose a category and amount on each row. Delete a selected row with the Delete key.";
        // 
        // lines
        // 
        lines.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
        lines.BackgroundColor = Color.White;
        lines.Columns.AddRange(new DataGridViewColumn[] { categoryColumn, descriptionColumn, amountColumn });
        lines.Dock = DockStyle.Fill;
        lines.Location = new Point(19, 267);
        lines.MultiSelect = false;
        lines.Name = "lines";
        lines.RowHeadersVisible = false;
        lines.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        lines.Size = new Size(962, 264);
        lines.TabIndex = 3;
        lines.DefaultValuesNeeded += LineDefaults;
        // 
        // categoryColumn
        // 
        categoryColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
        categoryColumn.HeaderText = "Category";
        categoryColumn.MinimumWidth = 220;
        categoryColumn.Name = "categoryColumn";
        categoryColumn.Width = 240;
        // 
        // descriptionColumn
        // 
        descriptionColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        descriptionColumn.HeaderText = "Description";
        descriptionColumn.Name = "descriptionColumn";
        // 
        // amountColumn
        // 
        dataGridViewCellStyle1.Format = "N2";
        amountColumn.DefaultCellStyle = dataGridViewCellStyle1;
        amountColumn.HeaderText = "Amount (RM)";
        amountColumn.Name = "amountColumn";
        amountColumn.Width = 105;
        // 
        // attachmentsPanel
        // 
        attachmentsPanel.ColumnCount = 2;
        attachmentsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        attachmentsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200F));
        attachmentsPanel.Controls.Add(attachments, 0, 0);
        attachmentsPanel.Controls.Add(attachmentButtons, 1, 0);
        attachmentsPanel.Dock = DockStyle.Fill;
        attachmentsPanel.Location = new Point(19, 537);
        attachmentsPanel.Name = "attachmentsPanel";
        attachmentsPanel.RowCount = 1;
        attachmentsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        attachmentsPanel.Size = new Size(962, 94);
        attachmentsPanel.TabIndex = 4;
        // 
        // attachments
        // 
        attachments.Dock = DockStyle.Fill;
        attachments.Location = new Point(3, 3);
        attachments.Name = "attachments";
        attachments.Size = new Size(756, 88);
        attachments.TabIndex = 0;
        // 
        // attachmentButtons
        // 
        attachmentButtons.Controls.Add(addAttachment);
        attachmentButtons.Controls.Add(removeAttachment);
        attachmentButtons.Controls.Add(setupButton);
        attachmentButtons.Dock = DockStyle.Fill;
        attachmentButtons.Location = new Point(765, 3);
        attachmentButtons.Name = "attachmentButtons";
        attachmentButtons.Size = new Size(194, 88);
        attachmentButtons.TabIndex = 1;
        // 
        // addAttachment
        // 
        addAttachment.AutoSize = true;
        addAttachment.Location = new Point(3, 3);
        addAttachment.Name = "addAttachment";
        addAttachment.Size = new Size(100, 25);
        addAttachment.TabIndex = 0;
        addAttachment.Text = "Attach receipt...";
        addAttachment.Click += AddAttachmentClicked;
        // 
        // removeAttachment
        // 
        removeAttachment.AutoSize = true;
        removeAttachment.Location = new Point(3, 34);
        removeAttachment.Name = "removeAttachment";
        removeAttachment.Size = new Size(124, 25);
        removeAttachment.TabIndex = 1;
        removeAttachment.Text = "Remove attachment";
        removeAttachment.Click += RemoveAttachmentClicked;
        // 
        // setupButton
        // 
        setupButton.AutoSize = true;
        setupButton.Location = new Point(3, 65);
        setupButton.Name = "setupButton";
        setupButton.Size = new Size(132, 25);
        setupButton.TabIndex = 2;
        setupButton.Text = "Payees & categories...";
        setupButton.UseMnemonic = false;
        setupButton.Click += SetupClicked;
        // 
        // paymentPanel
        // 
        paymentPanel.AutoScroll = true;
        paymentPanel.Controls.Add(cashLabel);
        paymentPanel.Controls.Add(cash);
        paymentPanel.Controls.Add(paymentDateLabel);
        paymentPanel.Controls.Add(paymentDate);
        paymentPanel.Controls.Add(paymentReferenceLabel);
        paymentPanel.Controls.Add(paymentReference);
        paymentPanel.Dock = DockStyle.Fill;
        paymentPanel.Location = new Point(19, 637);
        paymentPanel.Name = "paymentPanel";
        paymentPanel.Size = new Size(962, 39);
        paymentPanel.TabIndex = 5;
        paymentPanel.WrapContents = false;
        // 
        // cashLabel
        // 
        cashLabel.AutoSize = true;
        cashLabel.Location = new Point(3, 0);
        cashLabel.Name = "cashLabel";
        cashLabel.Size = new Size(59, 15);
        cashLabel.TabIndex = 0;
        cashLabel.Text = "Paid from";
        // 
        // cash
        // 
        cash.DropDownStyle = ComboBoxStyle.DropDownList;
        cash.Location = new Point(68, 3);
        cash.Name = "cash";
        cash.Size = new Size(235, 23);
        cash.TabIndex = 1;
        // 
        // paymentDateLabel
        // 
        paymentDateLabel.AutoSize = true;
        paymentDateLabel.Location = new Point(309, 0);
        paymentDateLabel.Name = "paymentDateLabel";
        paymentDateLabel.Size = new Size(80, 15);
        paymentDateLabel.TabIndex = 2;
        paymentDateLabel.Text = "Payment date";
        // 
        // paymentDate
        // 
        paymentDate.Format = DateTimePickerFormat.Short;
        paymentDate.Location = new Point(395, 3);
        paymentDate.Name = "paymentDate";
        paymentDate.Size = new Size(115, 23);
        paymentDate.TabIndex = 3;
        // 
        // paymentReferenceLabel
        // 
        paymentReferenceLabel.AutoSize = true;
        paymentReferenceLabel.Location = new Point(516, 0);
        paymentReferenceLabel.Name = "paymentReferenceLabel";
        paymentReferenceLabel.Size = new Size(71, 15);
        paymentReferenceLabel.TabIndex = 4;
        paymentReferenceLabel.Text = "Payment ref";
        // 
        // paymentReference
        // 
        paymentReference.Location = new Point(593, 3);
        paymentReference.MaxLength = 150;
        paymentReference.Name = "paymentReference";
        paymentReference.Size = new Size(160, 23);
        paymentReference.TabIndex = 5;
        // 
        // buttons
        // 
        buttons.Controls.Add(cancel);
        buttons.Controls.Add(saveAndPay);
        buttons.Controls.Add(saveDraft);
        buttons.Dock = DockStyle.Fill;
        buttons.FlowDirection = FlowDirection.RightToLeft;
        buttons.Location = new Point(19, 682);
        buttons.Name = "buttons";
        buttons.Size = new Size(962, 39);
        buttons.TabIndex = 6;
        // 
        // cancel
        // 
        cancel.AutoSize = true;
        cancel.DialogResult = DialogResult.Cancel;
        cancel.Location = new Point(884, 3);
        cancel.Name = "cancel";
        cancel.Size = new Size(75, 25);
        cancel.TabIndex = 0;
        cancel.Text = "Cancel";
        // 
        // saveAndPay
        // 
        saveAndPay.AutoSize = true;
        saveAndPay.Location = new Point(708, 3);
        saveAndPay.Name = "saveAndPay";
        saveAndPay.Size = new Size(170, 25);
        saveAndPay.TabIndex = 1;
        saveAndPay.Text = "Save, post & record payment";
        saveAndPay.UseMnemonic = false;
        saveAndPay.Click += SaveAndPayClicked;
        // 
        // saveDraft
        // 
        saveDraft.AutoSize = true;
        saveDraft.Location = new Point(627, 3);
        saveDraft.Name = "saveDraft";
        saveDraft.Size = new Size(75, 25);
        saveDraft.TabIndex = 2;
        saveDraft.Text = "Save draft";
        saveDraft.Click += SaveDraftClicked;
        // 
        // ExpenseEditForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = cancel;
        ClientSize = new Size(1000, 740);
        Controls.Add(layout);
        MinimumSize = new Size(930, 700);
        Name = "ExpenseEditForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Expense draft";
        layout.ResumeLayout(false);
        layout.PerformLayout();
        metadata.ResumeLayout(false);
        metadata.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)lines).EndInit();
        attachmentsPanel.ResumeLayout(false);
        attachmentButtons.ResumeLayout(false);
        attachmentButtons.PerformLayout();
        paymentPanel.ResumeLayout(false);
        paymentPanel.PerformLayout();
        buttons.ResumeLayout(false);
        buttons.PerformLayout();
        ResumeLayout(false);
    }
}
