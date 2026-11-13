#nullable enable
namespace HousingCommunityManagement;
partial class AccountingAttachments
{
    private ListBox list = null !;
    private Button add = null !, remove = null !;
    private System.ComponentModel.IContainer? components = null;
    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }
    private void InitializeComponent()
    {
        SuspendLayout();
        list = new ListBox();
        list.Dock = DockStyle.Fill;
        list.Name = "list";
        buttons = new FlowLayoutPanel();
        buttons.Dock = DockStyle.Right;
        buttons.Width = 200;
        buttons.Name = "buttons";
        add = new Button();
        add.Text = "Attach...";
        add.AutoSize = true;
        add.Name = "add";
        add.Click += AddClicked;
        buttons.Controls.Add(add);
        remove = new Button();
        remove.Text = "Remove";
        remove.AutoSize = true;
        remove.Name = "remove";
        remove.Click += RemoveClicked;
        buttons.Controls.Add(remove);
        actionButton = new Button();
        actionButton.Text = "Save attachment...";
        actionButton.AutoSize = true;
        actionButton.Name = "actionButton";
        actionButton.Click += ExportClicked;
        buttons.Controls.Add(actionButton);
        Controls.Add(list);
        Controls.Add(buttons);
        ResumeLayout(false);
        PerformLayout();
    }

    private FlowLayoutPanel buttons = null !;
    private Button actionButton = null !;
}
