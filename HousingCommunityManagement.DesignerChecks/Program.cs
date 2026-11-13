using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using HousingCommunityManagement;
internal static class Program
{
    [STAThread]
    private static int Main()
    {
        AppContext.SetSwitch("HousingCommunityManagement.ThrowOnUiError", true);
        ApplicationConfiguration.Initialize(); Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
        var repo = FindRepo(); var directory = Path.Combine(repo, "HousingCommunityManagement"); var count = 0;
        foreach (var path in Directory.GetFiles(directory, "*.Designer.cs").Order())
        {
            var name = Path.GetFileName(path).Replace(".Designer.cs", "");
            var root = CSharpSyntaxTree.ParseText(File.ReadAllText(path)).GetCompilationUnitRoot();
            if (root.GetDiagnostics().Any(d => d.Severity == DiagnosticSeverity.Error)) throw new Exception($"Invalid designer syntax: {path}");
            var classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();
            Require(classes.Count == 1 && classes[0].Identifier.Text == name, $"{name}: one matching designable class per file");
            var method = classes[0].Members.OfType<MethodDeclarationSyntax>().Single(m => m.Identifier.Text == "InitializeComponent");
            Require(!method.DescendantNodes().OfType<AssignmentExpressionSyntax>().Any(a => a.Right is AssignmentExpressionSyntax), $"{name}: chained assignments cannot be read reliably by the WinForms Designer; assign each property separately");
            foreach (var invalid in method.DescendantNodes().Where(n => n is LocalDeclarationStatementSyntax local && local.Declaration.Type.IsVar || n is ForStatementSyntax or ForEachStatementSyntax or WhileStatementSyntax or IfStatementSyntax or LambdaExpressionSyntax or AnonymousMethodExpressionSyntax || n.IsKind(SyntaxKind.ObjectInitializerExpression))) Console.WriteLine($"Unsupported {name}: {invalid.Kind()} {invalid}");
            Require(!method.DescendantNodes().Any(n => n is LocalDeclarationStatementSyntax local && local.Declaration.Type.IsVar || n is ForStatementSyntax or ForEachStatementSyntax or WhileStatementSyntax or IfStatementSyntax or LambdaExpressionSyntax or AnonymousMethodExpressionSyntax || n.IsKind(SyntaxKind.ObjectInitializerExpression)), $"{name}: designer initialization must use explicit assignments, not factories, loops or lambdas");
            Require(!method.ToString().Contains("AccountingUi.") && !method.ToString().Contains("DateTime.Today"), $"{name}: runtime helpers/date defaults must stay outside the designer");
            var sourcePath = Path.Combine(directory, name + ".cs"); Require(File.Exists(sourcePath), $"{name}: matching source file exists");
            var source = CSharpSyntaxTree.ParseText(File.ReadAllText(sourcePath)).GetCompilationUnitRoot();
            Require(source.DescendantNodes().OfType<ClassDeclarationSyntax>().First().Identifier.Text == name, $"{name}: designable class must be first in its source file");
            var type = typeof(MainForm).Assembly.GetType("HousingCommunityManagement." + name) ?? throw new Exception($"Missing type {name}");
            Require(type.GetConstructor(Type.EmptyTypes) is not null, $"{name}: public parameterless constructor required");
            using var control = (Control)Activator.CreateInstance(type)!;
            control.CreateControl(); control.PerformLayout();
            type.GetMethod("RefreshData", Type.EmptyTypes)?.Invoke(control, null);
            if (control is DashboardView dashboard) dashboard.Reload();
            if (control is CashTransactionsView or FinanceRegistersView or BankCsvImportForm)
            {
                foreach (var combo in AllControls(control).OfType<ComboBox>().Where(c => c.Items.Count > 1)) combo.SelectedIndex = (combo.SelectedIndex + 1) % combo.Items.Count;
                foreach (var date in AllControls(control).OfType<DateTimePicker>()) if (date.Value.Date > date.MinDate.Date) date.Value = date.Value.AddDays(-1);
            }
            if (control is HouseRangeForm)
            {
                var prefix = AllControls(control).OfType<TextBox>().Single(c => c.Name == "prefix"); prefix.Text = "D-";
                Require(AllControls(control).OfType<Label>().Single(c => c.Name == "preview").Text.StartsWith("D-"), "Designer-owned range events must update the preview");
            }
            if (control is AccountingForm) foreach (var tabs in AllControls(control).OfType<TabControl>()) for (var i = 0; i < tabs.TabCount; i++) tabs.SelectedIndex = i;
            using var bitmap = new Bitmap(Math.Max(1, Math.Min(control.Width, 1400)), Math.Max(1, Math.Min(control.Height, 1100)));
            control.DrawToBitmap(bitmap, new Rectangle(Point.Empty, bitmap.Size));
            Require(type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(f => f.DeclaringType?.Assembly == type.Assembly).Select(f => f.GetValue(control)).OfType<Control>().All(c => !string.IsNullOrWhiteSpace(c.Name)), $"{name}: every fixed control has a designer name");
            if (control is AccountingForm) Require(AllControls(control).OfType<TabPage>().Count() >= 6, "Accounting tabs must be present without runtime services");
            if (control is ExpensesForm) Require(AllControls(control).OfType<CashTransactionsView>().Any(), "Income/expenditure cash view must exist in the designer");
            count++; Console.WriteLine($"PASS: {name}");
        }
        Console.WriteLine($"PASS: {count} designer layouts parse, instantiate and render without application services."); return 0;
    }
    private static IEnumerable<Control> AllControls(Control control) => control.Controls.Cast<Control>().SelectMany(c => new[] { c }.Concat(AllControls(c)));
    private static void Require(bool valid, string message) { if (!valid) throw new Exception(message); }
    private static string FindRepo()
    {
        var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (directory is not null) { if (File.Exists(Path.Combine(directory.FullName, "HousingCommunityManagement.sln"))) return directory.FullName; directory = directory.Parent; }
        throw new Exception("Run designer checks from the repository.");
    }
}
