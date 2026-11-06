namespace HousingCommunityManagement
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            try
            {
                var path = Path.Combine(AppContext.BaseDirectory, "Data", "community.db");
                var repository = new Data.HouseholdRepository(path);
                var service = new Core.HouseholdService(repository);
                var billing = new Core.MonthlyBillingService(repository);
                Application.Run(new MainForm(service, billing, path));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"The application could not start. Check that the application folder is writable.\n\n{ex.Message}",
                    "Startup failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
