using HousingCommunityManagement.Core;
using HousingCommunityManagement.Data;
using Microsoft.Data.Sqlite;
var folder = Path.Combine(Path.GetTempPath(), "HouseholdChecks-" + Guid.NewGuid());
Directory.CreateDirectory(folder);
try
{
    var path = Path.Combine(folder, "check.db");
    var service = new HouseholdService(new HouseholdRepository(path));
    Require(service.GetAvailableFlags().Count == 0, "No predefined flags");
    Require(service.AddFlag(" Corner lot ") == "Corner lot", "Flag name is trimmed");
    Require(service.AddFlag("corner LOT") == "Corner lot", "Case-insensitive flag reuse");
    Require(new HouseholdService(new HouseholdRepository(path)).GetAvailableFlags().SequenceEqual(new[] { "Corner lot" }), "Added flags persist before saving a house");
    Reject(() => service.AddFlag("  "));
    Reject(() => service.AddFlag(new string('x', 81)));
    service.AddFlag("Unused flag");
    service.DeleteFlag(" UNUSED FLAG ");
    Require(!new HouseholdService(new HouseholdRepository(path)).GetAvailableFlags().Contains("Unused flag"), "Unused flag deletion persists");
    Require(service.CreateRange("A-", 1, 10, 3) == 10, "Range count");
    Require(service.CreateRange("a-", 5, 12, 3) == 2, "Overlap skips existing");
    var house = service.GetAll().First();
    Require(!house.IsOwned && house.Owners.Count == 0, "Unowned defaults");
    Require(house.Race == "", "Race defaults to unspecified");
    foreach (var value in new[] { "Malay", "Chinese", "Indian", "Others", "" })
    {
        house.Race = value; service.Save(house);
        Require(new HouseholdRepository(path).GetAll().Single(h => h.Id == house.Id).Race == value, "Race selection persists");
    }
    Reject(() => service.Save(new Household { HouseNumber = "InvalidRace", Race = "Invalid" }));
    house.IsOwned = true; house.Owners = [" Owner One ", "Owner Two"]; house.Contacts = ["012", "013"];
    house.Flags = [new() { Name = "Occupied" }, new() { Name = "Payment due", Source = "Billing" }];
    service.Save(house);
    var settings = service.GetSettings(); settings.MonthlyMaintenanceFee = 123.45m; settings.FlagNames.Add("Corner lot"); service.SaveSettings(settings);
    var reopened = new HouseholdService(new HouseholdRepository(path));
    Require(reopened.GetAvailableFlags().Contains("Occupied") && reopened.GetAvailableFlags().Contains("Payment due"), "Flags used by other houses are available");
    var edited = reopened.GetAll().Single(h => h.Id == house.Id);
    Require(edited.IsOwned && edited.Owners.SequenceEqual(new[] { "Owner One", "Owner Two" }) && edited.Contacts.Count == 2 && edited.Flags[1].Source == "Billing", "Owners, contacts, flags and ownership reopen");
    Require(reopened.GetSettings().MonthlyMaintenanceFee == 123.45m && reopened.GetSettings().FlagNames.Contains("Corner lot"), "Global settings reopen");
    edited.IsActive = false; reopened.Save(edited);
    Reject(() => reopened.DeleteFlag("occupied"));
    Reject(() => reopened.DeleteFlag("Payment due"));
    Require(reopened.GetAvailableFlags().Contains("Occupied"), "Flags on inactive houses cannot be deleted");
    edited.Flags.RemoveAll(f => f.Name == "Occupied");
    reopened.Save(edited);
    reopened.DeleteFlag("Occupied");
    Require(!new HouseholdService(new HouseholdRepository(path)).GetAvailableFlags().Contains("Occupied"), "Flag can be deleted after last saved use is removed");
    reopened.AddFlag("Exclusive");
    edited = reopened.GetAll().Single(h => h.Id == house.Id);
    edited.Flags.Add(new HouseFlag { Name = "Exclusive" });
    reopened.Save(edited);
    var another = reopened.GetAll().First(h => h.Id != edited.Id);
    Reject(() => reopened.DeleteFlag("Exclusive", another.Id));
    another.Flags.Add(new HouseFlag { Name = "Exclusive" });
    reopened.Save(another);
    Reject(() => reopened.DeleteFlag("Exclusive", edited.Id));
    Require(reopened.GetAll().Count(h => h.Flags.Any(f => f.Name == "Exclusive")) == 2 && reopened.GetSettings().FlagNames.Contains("Exclusive"), "Blocked deletion changes neither assignments nor catalog");
    another.Flags.Clear(); reopened.Save(another);
    reopened.DeleteFlag("exclusive", edited.Id);
    var afterDelete = new HouseholdService(new HouseholdRepository(path));
    Require(!afterDelete.GetAvailableFlags().Contains("Exclusive") && !afterDelete.GetAll().Single(h => h.Id == edited.Id).Flags.Any(f => f.Name == "Exclusive"), "Sole current-house use and catalog entry are deleted together");
    Require(afterDelete.GetAll().Single(h => h.Id == edited.Id).Owners.Count == 2 && afterDelete.GetSettings().MonthlyMaintenanceFee == 123.45m, "Deletion preserves unrelated house details and settings");
    Reject(() => reopened.Save(new Household { HouseNumber = "a-001" }));
    Reject(() => reopened.CreateRange("", 10, 1, 1));
    Reject(() => reopened.CreateRange("", 0, 10000, 1));
    Reject(() => reopened.SaveSettings(new CommunitySettings { MonthlyMaintenanceFee = -1 }));
    Require(reopened.GetAll().Count == 12, "Rejected ranges do not change records");
    var billing = new MonthlyBillingService(new HouseholdRepository(path));
    Require(billing.GenerateMonths(2026, new[] { 1, 2, 3 }) == 33, "Selected months charge active houses at monthly rate");
    Require(billing.GenerateMonths(2026, new[] { 2, 3, 4, 4 }) == 11, "Overlapping and duplicate selections skip existing months");
    Require(billing.GetCharges().All(c => c.Amount == 123.45m), "Monthly rate not multiplied");
    Reject(() => billing.GenerateMonths(2026, Array.Empty<int>()));
    Reject(() => billing.GenerateMonths(2026, new[] { 0, 13 }));
    var bill = billing.GetCharges().First(c => c.Month == 1);
    Require(bill.DueDate == new DateTime(2026, 1, 6) && bill.StatusOn(new DateTime(2026, 1, 6)) == "Payment due" && bill.StatusOn(new DateTime(2026, 1, 7)) == "Overdue", "Monthly due date boundary");
    settings = reopened.GetSettings(); settings.MonthlyMaintenanceFee = 50; reopened.SaveSettings(settings);
    Require(billing.GenerateMonths(2026, new[] { 5, 6 }, new DateTime(2026, 5, 6)) == 22, "Multiple months may share a collection deadline");
    Require(billing.GetCharges().Where(c => c.Month == 6).All(c => c.Amount == 50m && c.DueDate == new DateTime(2026, 5, 6)), "Shared deadline and new monthly rate persist");
    Require(billing.GetCharges().First(c => c.Id == bill.Id).Amount == 123.45m, "Rate changes preserve prior charges");
    billing.RecordPayment(bill.Id, 50m, new DateTime(2026, 1, 6), "Partial");
    bill = billing.GetCharges().Single(c => c.Id == bill.Id);
    Require(bill.Balance == 73.45m && bill.StatusOn(new DateTime(2026, 1, 7)) == "Overdue", "Partial monthly payment");
    Reject(() => billing.RecordPayment(bill.Id, 100m, DateTime.Today, "Overpayment"));
    Reject(() => billing.RecordPayment(bill.Id, 0.001m, DateTime.Today, "Fractional sen"));
    Reject(() => billing.RecordPayment(bill.Id, 1m, DateTime.Today.AddDays(1), "Future"));
    billing.RecordPayment(bill.Id, 73.45m, new DateTime(2026, 3, 6), "Settled");
    Require(new MonthlyBillingService(new HouseholdRepository(path)).GetCharges().Single(c => c.Id == bill.Id).Balance == 0, "Monthly payment persists");
    var eligibleHouses = reopened.GetAll().Where(h => h.IsActive).Take(2).Select(h => h.Id).ToArray();
    Require(billing.GenerateMonths(2026, new[] { 9, 10 }, householdIds: eligibleHouses) == 4, "Only explicitly selected houses and months are charged");
    Require(billing.GetCharges().Where(c => c.Month is 9 or 10).All(c => eligibleHouses.Contains(c.HouseholdId)), "Unselected houses are excluded");
    Require(billing.GenerateMonths(2026, new[] { 9, 10 }, householdIds: eligibleHouses) == 0, "Selected-house generation still skips duplicates");
    Reject(() => billing.GenerateMonths(2026, new[] { 11 }, householdIds: Array.Empty<int>()));
    Reject(() => billing.GenerateMonths(2026, new[] { 11 }, householdIds: new[] { int.MaxValue }));
    Reject(() => billing.GenerateMonths(2026, new[] { 11 }, householdIds: new[] { house.Id }));
    Require(!billing.GetCharges().Any(c => c.Month == 11), "Empty, unknown and inactive house selections create no charges");
    var legacyPath = Path.Combine(folder, "legacy.db");
    using (var c = new SqliteConnection($"Data Source={legacyPath}"))
    {
        c.Open(); using var cmd = c.CreateCommand();
        cmd.CommandText = "CREATE TABLE Households (Id INTEGER PRIMARY KEY AUTOINCREMENT, HouseNumber TEXT COLLATE NOCASE NOT NULL, OwnerName TEXT NOT NULL, ContactNumber TEXT NOT NULL, MonthlyFee INTEGER NOT NULL, IsActive INTEGER NOT NULL); CREATE UNIQUE INDEX IX_Households_HouseNumber ON Households(HouseNumber); INSERT INTO Households VALUES(1, 'L-1', 'Legacy Owner', '0123', 5000, 1);";
        cmd.ExecuteNonQuery();
    }
    var legacy = new HouseholdService(new HouseholdRepository(legacyPath));
    Require(legacy.GetAll().Single().Owners.Single() == "Legacy Owner" && legacy.GetAll().Single().IsOwned && legacy.GetSettings().MonthlyMaintenanceFee == 50m, "Legacy upgrade preserves data");
    Require(legacy.CreateRange("L-", 1, 2, 1) == 1, "Can insert after upgrade");
    Require(new HouseholdRepository(legacyPath).GetAll().Count == 2, "Upgrade is repeatable");
    Require(new MonthlyBillingService(new HouseholdRepository(legacyPath)).GenerateMonths(2026, new[] { 1 }) == 2, "Legacy database gains billing tables");
    Require(legacy.GetAll().All(h => h.Race == ""), "Legacy houses default to unspecified race");
    using (var c = new SqliteConnection($"Data Source={legacyPath}"))
    {
        c.Open(); using var cmd = c.CreateCommand(); cmd.CommandText = "ALTER TABLE Households DROP COLUMN Race"; cmd.ExecuteNonQuery();
    }
    var raceUpgrade = new HouseholdRepository(legacyPath);
    Require(raceUpgrade.GetAll().Count == 2 && raceUpgrade.GetAll().All(h => h.Race == ""), "Previous multi-owner schema upgrades without losing houses");
    // Simulate an actual previous quarterly database, including a payment spanning months and indivisible sen.
    var quarterlyPath = Path.Combine(folder, "quarterly.db");
    var quarterHouses = new HouseholdService(new HouseholdRepository(quarterlyPath));
    quarterHouses.Save(new Household { HouseNumber = "Q-1" });
    quarterHouses.SaveSettings(new CommunitySettings { MonthlyMaintenanceFee = 50m });
    using (var c = new SqliteConnection($"Data Source={quarterlyPath}"))
    {
        c.Open(); using var cmd = c.CreateCommand();
        cmd.CommandText = "DROP TABLE MonthlyPayments; DROP TABLE MonthlyCharges; DROP TABLE BillingUpgrades; CREATE TABLE QuarterlyCharges(Id INTEGER PRIMARY KEY, HouseholdId INTEGER, Year INTEGER, Quarter INTEGER, Amount INTEGER, DueDate TEXT); CREATE TABLE MaintenancePayments(Id INTEGER PRIMARY KEY, QuarterlyChargeId INTEGER, Amount INTEGER, Date TEXT, Reference TEXT); INSERT INTO QuarterlyCharges VALUES(1,1,2026,1,10001,'2026-01-06'); INSERT INTO MaintenancePayments VALUES(1,1,5000,'2026-01-05','Original transfer'); INSERT INTO MaintenancePayments VALUES(2,1,4000,'2026-02-06','Second transfer');";
        cmd.ExecuteNonQuery();
    }
    var migrated = new MonthlyBillingService(new HouseholdRepository(quarterlyPath)).GetCharges();
    Require(migrated.Count == 3 && migrated.Sum(c => c.Amount) == 100.01m && migrated.Sum(c => c.Paid) == 90m, "Quarterly migration preserves charge and payment totals to the sen");
    Require(migrated.All(c => c.DueDate == new DateTime(2026,1,6)), "Migration preserves existing deadlines");
    Require(migrated.SelectMany(c => c.Payments).Where(p => p.LegacyPaymentId == 1).Sum(p => p.Amount) == 50m, "Split payments retain original receipt identity");
    Require(migrated.OrderBy(c => c.Month).Last().Balance == 10.01m, "Legacy payments allocated oldest month first");
    Require(new MonthlyBillingService(new HouseholdRepository(quarterlyPath)).GetCharges().Count == 3, "Migration is idempotent");
    Require(new MonthlyBillingService(new HouseholdRepository(quarterlyPath)).GenerateMonths(2026, new[] {1,2,3}) == 0, "Migrated charges prevent rebilling");
    Console.WriteLine("PASS: household checks, monthly generation, overlapping selections, due dates, shared deadlines, partial payments, rate snapshots and quarterly migration.");
}
finally { SqliteConnection.ClearAllPools(); Directory.Delete(folder, true); }
static void Require(bool value, string label) { if (!value) throw new Exception(label); }
static void Reject(Action action) { try { action(); } catch (ArgumentException) { return; } throw new Exception("Expected rejection"); }
