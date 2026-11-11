using HousingCommunityManagement.Core;
using HousingCommunityManagement.Data;
using Microsoft.Data.Sqlite;
var folder = Path.Combine(Path.GetTempPath(), "HouseholdChecks-" + Guid.NewGuid());
Directory.CreateDirectory(folder);
try
{
    var historical = new AccountingService(new HouseholdRepository(Path.Combine(folder, "historical-opening.db")));
    var historicalBalances = historical.GetAccounts().Where(a => a.IsActive && a.IsCashBank).ToDictionary(a => a.Id, a => 123m);
    var openingDate = new DateTime(2023, 1, 1);
    Reject(() => historical.Start(historicalBalances, DateTime.Today.AddDays(1)));
    Require(historical.GetSettings().StartedOn is null && historical.GetJournal().Count == 0, "Rejected opening date leaves accounting unstarted");
    historical.Start(historicalBalances, openingDate);
    var historicalReopened = new AccountingService(new HouseholdRepository(Path.Combine(folder, "historical-opening.db")));
    Require(historicalReopened.GetSettings().StartedOn == openingDate && historicalReopened.GetJournal().All(e => e.Date == openingDate), "Historical opening date and journal dates persist");
    Reject(() => historicalReopened.Start(historicalBalances, openingDate.AddDays(1)));
    var datedRepo = new HouseholdRepository(Path.Combine(folder, "dated-charges.db"));
    var datedHouses = new HouseholdService(datedRepo);
    datedHouses.CreateRange("D-", 1, 1, 1);
    var datedAccounting = new AccountingService(datedRepo);
    datedAccounting.Start(datedAccounting.GetAccounts().Where(a => a.IsActive && a.IsCashBank).ToDictionary(a => a.Id, a => 0m), new DateTime(2023, 1, 1));
    var datedBilling = new MonthlyBillingService(datedRepo);
    var datedChargeDate = new DateTime(2023, 2, 1);
    Reject(() => datedBilling.GenerateMonths(2023, [1], 50m, chargeDate: new DateTime(2022, 12, 31)));
    Reject(() => datedBilling.GenerateMonths(2023, [1], 50m, chargeDate: DateTime.Today.AddDays(1)));
    Require(datedBilling.GetCharges().Count == 0 && datedAccounting.GetJournal().Count == 0, "Invalid charge dates leave charges and ledger unchanged");
    Require(datedBilling.GenerateMonths(2023, [2, 3], 50m, chargeDate: datedChargeDate) == 2, "Historical run creates selected months");
    var datedReopened = new HouseholdRepository(Path.Combine(folder, "dated-charges.db"));
    Require(new MonthlyBillingService(datedReopened).GetCharges().All(c => c.ChargeDate == datedChargeDate), "Charge dates persist on reopening");
    Require(new AccountingService(datedReopened).GetJournal().All(e => e.Date == datedChargeDate), "Charge journals use selected date");
    Require(datedBilling.GenerateMonths(2023, [2, 3], 75m, chargeDate: datedChargeDate.AddDays(1)) == 0 && datedBilling.GetCharges().All(c => c.ChargeDate == datedChargeDate && c.Amount == 50m), "Repeated run preserves original dates and amounts");
    datedBilling.GenerateMonths(2023, [4], 50m, chargeDate: datedChargeDate.AddDays(10));
    var datedCharges = datedBilling.GetCharges().OrderBy(c => c.Month).ToList();
    Reject(() => datedBilling.RecordPayment(datedCharges[0].Id, 10m, datedChargeDate.AddDays(-1), "Too early"));
    Require(datedBilling.GetCharges().All(c => c.Payments.Count == 0), "Early single payment saves nothing");
    var datedBatch = new[] { new MaintenancePaymentRequest(datedCharges[0].Id, 10m, 50m, "Older charge"), new MaintenancePaymentRequest(datedCharges[2].Id, 10m, 50m, "Later charge") };
    var datedJournalCount = datedAccounting.GetJournal().Count;
    Reject(() => datedBilling.RecordPayments(datedBatch, datedChargeDate.AddDays(9)));
    Require(datedBilling.GetCharges().All(c => c.Payments.Count == 0) && datedAccounting.GetJournal().Count == datedJournalCount, "Batch before latest charge date rejects every payment and journal");
    Require(datedBilling.RecordPayments(datedBatch, datedChargeDate.AddDays(10)) == 2, "Payment on latest selected charge date is accepted");
    datedBilling.RecordPayment(datedCharges[1].Id, 10m, datedChargeDate, "Same date");
    MonthlyBillingService.ValidatePaymentChargeDates([new MonthlyCharge { ChargeDate = null }], new DateTime(2020, 1, 1));
    var carryRepo = new HouseholdRepository(Path.Combine(folder, "historical-carry.db"));
    var carryHouses = new HouseholdService(carryRepo); carryHouses.CreateRange("H", 1, 1, 1);
    var carryHouse = carryHouses.GetAll().Single(); carryHouse.IsOwned = true; carryHouses.Save(carryHouse);
    var carryBilling = new MonthlyBillingService(carryRepo); var carryAccounting = new AccountingService(carryRepo);
    var carryRenovations = new RenovationService(carryRepo);
    var carryBank = carryAccounting.GetAccounts().Single(a => a.Code == "1000");
    var boundary = new DateTime(2023, 1, 1);
    carryBilling.GenerateMonths(2022, [12], 100m, chargeDate: boundary.AddDays(-10));
    var carryCharge = carryBilling.GetCharges().Single();
    carryBilling.RecordPayment(carryCharge.Id, 20m, boundary.AddDays(-1), "Before", carryBank.Id);
    carryBilling.RecordPayment(carryCharge.Id, 80m, boundary.AddDays(5), "After", carryBank.Id);
    carryBilling.GenerateMonths(2023, [1], 50m, chargeDate: boundary);
    var boundaryCharge = carryBilling.GetCharges().Single(c => c.Year == 2023);
    carryBilling.RecordPayment(boundaryCharge.Id, 50m, boundary, "Boundary", carryBank.Id);
    Renovation CarryWork(string name) => new() { HouseholdId = carryHouse.Id, Description = name, StartDate = boundary.AddDays(-20), DurationDays = 30, DepositAmount = 200m };
    var carryWork = CarryWork("Held at opening"); carryRenovations.Add(carryWork);
    carryRenovations.RecordAction(carryWork.Id, "Collect", boundary.AddDays(-5), carryBank.Id);
    carryRenovations.RecordAction(carryWork.Id, "Complete", boundary.AddDays(1));
    carryRenovations.RecordAction(carryWork.Id, "Return", boundary.AddDays(2), carryBank.Id);
    var newWork = CarryWork("Collected after opening"); carryRenovations.Add(newWork);
    carryRenovations.RecordAction(newWork.Id, "Collect", boundary.AddDays(3), carryBank.Id);
    carryAccounting.Start(carryAccounting.GetAccounts().Where(a => a.IsActive && a.IsCashBank).ToDictionary(a => a.Id, a => a.Id == carryBank.Id ? 1000m : 0m), boundary);
    var carryJournal = carryAccounting.GetJournal();
    Require(carryJournal.Single(e => e.Source == "OpeningCharge").Lines.Sum(l => l.Debit) == 80m, "Opening receivable ignores payments after opening");
    Require(carryJournal.Count(e => e.Source == "MaintenancePayment") == 2 && carryJournal.Single(e => e.Source == "MonthlyCharge").Date == boundary, "Same-day and later transactions post separately from brought-forward balances");
    Require(carryJournal.Single(e => e.Source == "OpeningDeposit").SourceId == carryWork.Id && carryJournal.Single(e => e.Source == "RenovationCollection").SourceId == newWork.Id && carryJournal.Single(e => e.Source == "RenovationReturn").Date == boundary.AddDays(2), "Historical held deposits and later collections/refunds reconstruct correctly");
    Require(carryJournal.SelectMany(e => e.Lines).Where(l => l.AccountId == carryBank.Id).Sum(l => l.Debit - l.Credit) == 1130m, "Opening cash plus later receipts and deposits reconciles");
    Require(carryJournal.SelectMany(e => e.Lines).Where(l => l.AccountId == carryAccounting.GetSettings().ReceivableAccountId).Sum(l => l.Debit - l.Credit) == 0m, "Historical opening receivable clears after later receipts");
    var carryExpenses = new ExpenseService(carryRepo); var carrySupplier = new Payee { Name = "Historical supplier" }; carryExpenses.SavePayee(carrySupplier);
    var carryBill = new ExpenseBill { PayeeId = carrySupplier.Id, Description = "Past expense", DocumentDate = boundary.AddDays(-2), AccountingDate = boundary.AddDays(4), Lines = [new() { CategoryId = carryExpenses.GetCategories().First().Id, Amount = 30m }] };
    var carryBillId = carryExpenses.Save(carryBill); carryExpenses.Post(carryBillId);
    Reject(() => carryExpenses.Pay(carryBillId, 30m, boundary.AddDays(3), carryBank.Id, "Too early"));
    carryExpenses.Pay(carryBillId, 30m, boundary.AddDays(4), carryBank.Id, "Historical payment");
    var carryPayment = carryExpenses.Get(carryBillId).Payments.Single();
    Reject(() => carryExpenses.ReversePayment(carryPayment.Id, boundary.AddDays(3), "Too early"));
    carryExpenses.ReversePayment(carryPayment.Id, boundary.AddDays(5), "Historical reversal");
    Reject(() => carryExpenses.Cancel(carryBillId, boundary.AddDays(4), "Too early"));
    carryExpenses.Cancel(carryBillId, boundary.AddDays(5), "Historical cancellation");
    Require(carryExpenses.Get(carryBillId).CancelledDate == boundary.AddDays(5), "Historical expense posting, payment, reversal and cancellation retain actual dates");
    var beforeCarry = carryAccounting.GetJournal().Count;
    Reject(() => carryRenovations.RecordAction(newWork.Id, "Complete", boundary.AddDays(2)));
    carryRenovations.RecordAction(newWork.Id, "Complete", boundary.AddDays(4));
    Reject(() => carryRenovations.RecordAction(newWork.Id, "Return", boundary.AddDays(3), carryBank.Id));
    Require(carryAccounting.GetJournal().Count == beforeCarry, "Invalid historical deposit ordering writes no journals");
    carryRenovations.RecordAction(newWork.Id, "Return", boundary.AddDays(4), carryBank.Id);
    Require(new AccountingService(new HouseholdRepository(Path.Combine(folder, "historical-carry.db"))).GetJournal().All(e => e.Date >= boundary), "Historical ledger persists with no postings before opening");
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
    Reject(() => billing.GenerateMonths(2026, new[] { 1 }, 0m));
    Reject(() => billing.GenerateMonths(2026, new[] { 1 }, -1m));
    Reject(() => billing.GenerateMonths(2026, new[] { 1 }, 1.001m));
    Reject(() => billing.GenerateMonths(2026, new[] { 1 }, 1000000.01m));
    Require(billing.GenerateMonths(2026, new[] { 1, 2, 3 }, 123.45m, chargeDate: new DateTime(2026, 1, 1)) == 33, "Selected months charge active houses at monthly rate");
    Require(billing.GenerateMonths(2026, new[] { 2, 3, 4, 4 }, 123.45m) == 11, "Overlapping and duplicate selections skip existing months");
    Require(billing.GetCharges().All(c => c.Amount == 123.45m), "Monthly rate not multiplied");
    Reject(() => billing.GenerateMonths(2026, Array.Empty<int>(), 50m));
    Reject(() => billing.GenerateMonths(2026, new[] { 0, 13 }, 50m));
    var bill = billing.GetCharges().First(c => c.Month == 1);
    Require(bill.DueDate == new DateTime(2026, 1, 6) && bill.StatusOn(new DateTime(2026, 1, 6)) == "Payment due" && bill.StatusOn(new DateTime(2026, 1, 7)) == "Overdue", "Monthly due date boundary");
    settings = reopened.GetSettings(); settings.MonthlyMaintenanceFee = 50; reopened.SaveSettings(settings);
    Require(billing.GenerateMonths(2026, new[] { 5, 6 }, 50m, new DateTime(2026, 5, 6)) == 22, "Multiple months may share a collection deadline");
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
    Require(billing.GenerateMonths(2026, new[] { 9, 10 }, 50m, householdIds: eligibleHouses) == 4, "Only explicitly selected houses and months are charged");
    Require(billing.GetCharges().Where(c => c.Month is 9 or 10).All(c => eligibleHouses.Contains(c.HouseholdId)), "Unselected houses are excluded");
    Require(billing.GenerateMonths(2026, new[] { 9, 10 }, 80m, householdIds: eligibleHouses) == 0, "Changed input amount still skips existing monthly charges");
    Require(billing.GetCharges().Where(c => c.Month is 9 or 10).All(c => c.Amount == 50m), "Repeated charging with a different amount never changes saved charges");
    Reject(() => billing.GenerateMonths(2026, new[] { 11 }, 50m, householdIds: Array.Empty<int>()));
    Reject(() => billing.GenerateMonths(2026, new[] { 11 }, 50m, householdIds: new[] { int.MaxValue }));
    Reject(() => billing.GenerateMonths(2026, new[] { 11 }, 50m, householdIds: new[] { house.Id }));
    Require(!billing.GetCharges().Any(c => c.Month == 11), "Empty, unknown and inactive house selections create no charges");
    settings = reopened.GetSettings(); settings.MonthlyMaintenanceFee = 0; reopened.SaveSettings(settings);
    var manualHouse = reopened.GetAll().First(h => h.IsActive);
    Require(billing.GenerateMonths(2026, new[] { 7 }, 17.25m, householdIds: new[] { manualHouse.Id }) == 1, "An explicit amount works with no saved global rate");
    Require(billing.GetCharges().Single(c => c.Month == 7).Amount == 17.25m && reopened.GetSettings().MonthlyMaintenanceFee == 0, "Charging uses the entered amount without storing it as a global default");
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
    Require(new MonthlyBillingService(new HouseholdRepository(legacyPath)).GenerateMonths(2026, new[] { 1 }, 100m) == 2, "Legacy database gains billing tables");
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
    Require(new MonthlyBillingService(new HouseholdRepository(quarterlyPath)).GenerateMonths(2026, new[] {1,2,3}, 100m) == 0, "Migrated charges prevent rebilling");
    var renovationPath = Path.Combine(folder, "renovations.db");
    var renovationRepo = new HouseholdRepository(renovationPath);
    var renovationHouses = new HouseholdService(renovationRepo);
    renovationHouses.Save(new Household { HouseNumber = "R-1", IsOwned = true, Owners = ["Owner"] });
    var renovationHouse = renovationHouses.GetAll().Single();
    var renovationService = new RenovationService(renovationRepo);
    Renovation NewRenovation() => new() { HouseholdId = renovationHouse.Id, Description = "Kitchen", StartDate = DateTime.Today.AddDays(-10), DurationDays = 5, DepositAmount = 1234.56m };
    var renovation = NewRenovation(); renovationService.Add(renovation);
    var savedRenovation = new RenovationService(new HouseholdRepository(renovationPath)).GetAll().Single();
    Require(savedRenovation.DepositAmount == 1234.56m && savedRenovation.PlannedEndDate == renovation.StartDate.AddDays(4), "One-off deposit and inclusive duration persist");
    Require(savedRenovation.StatusOn(DateTime.Today) == "Overdue", "Unfinished renovation becomes overdue");
    Reject(() => renovationService.RecordAction(renovation.Id, "Return", DateTime.Today));
    Reject(() => renovationService.RecordAction(renovation.Id, "Complete", DateTime.Today));
    Reject(() => renovationService.RecordAction(renovation.Id, "Collect", DateTime.Today.AddDays(1)));
    renovationService.RecordAction(renovation.Id, "Collect", DateTime.Today.AddDays(-9));
    Reject(() => renovationService.RecordAction(renovation.Id, "Collect", DateTime.Today));
    Reject(() => renovationService.RecordAction(renovation.Id, "Complete", DateTime.Today.AddDays(-11)));
    renovationService.RecordAction(renovation.Id, "Complete", DateTime.Today.AddDays(-1));
    Require(renovationService.GetAll().Single().DepositStatus == "Awaiting return", "Completed deposit awaits actual return");
    Reject(() => renovationService.RecordAction(renovation.Id, "Return", DateTime.Today.AddDays(-2)));
    renovationService.RecordAction(renovation.Id, "Return", DateTime.Today);
    Reject(() => renovationService.RecordAction(renovation.Id, "Return", DateTime.Today));
    Reject(() => renovationService.RecordAction(renovation.Id, "Complete", DateTime.Today));
    Require(new RenovationService(new HouseholdRepository(renovationPath)).GetAll().Single().DepositStatus == "Returned", "Deposit return survives reopening");
    var invalidRenovation = NewRenovation(); invalidRenovation.DurationDays = 0; Reject(() => renovationService.Add(invalidRenovation));
    invalidRenovation = NewRenovation(); invalidRenovation.DepositAmount = -1; Reject(() => renovationService.Add(invalidRenovation));
    invalidRenovation.DepositAmount = 1.001m; Reject(() => renovationService.Add(invalidRenovation));
    invalidRenovation = NewRenovation(); invalidRenovation.HouseholdId = -1; Reject(() => renovationService.Add(invalidRenovation));
    var noDeposit = NewRenovation(); noDeposit.DepositAmount = 0; renovationService.Add(noDeposit);
    renovationService.RecordAction(noDeposit.Id, "Complete", DateTime.Today);
    Require(renovationService.GetAll().Single(r => r.Id == noDeposit.Id).DepositStatus == "No deposit", "Zero-deposit renovations complete without collection");
    var filterDate = new DateTime(2026, 6, 15);
    var filterRecords = new List<Renovation>
    {
        new() { Id = 101, StartDate = new DateTime(2026, 7, 1), DurationDays = 5 },
        new() { Id = 102, StartDate = filterDate, DurationDays = 1, DepositAmount = 100, DepositCollectedDate = filterDate },
        new() { Id = 103, StartDate = new DateTime(2025, 12, 30), DurationDays = 200, DepositAmount = 100 },
        new() { Id = 104, StartDate = filterDate.AddDays(-10), DurationDays = 2, DepositAmount = 100, DepositCollectedDate = filterDate.AddDays(-10), CompletedDate = filterDate },
        new() { Id = 105, StartDate = filterDate.AddDays(-10), DurationDays = 2 }
    };
    Require(RenovationService.Filter(filterRecords, filterDate).Count == 5, "All filters show every renovation");
    Require(RenovationService.Filter(filterRecords, filterDate, 2026, "In progress", "Held").Single().Id == 102, "Year and both statuses combine, including end-date boundary");
    Require(RenovationService.Filter(filterRecords, filterDate, 2025, "In progress").Single().Id == 103, "Year filter uses start year for work spanning years");
    Require(RenovationService.Filter(filterRecords, filterDate, status: "Planned").Single().Id == 101, "Planned status filter");
    Require(RenovationService.Filter(filterRecords, filterDate, status: "Overdue").Single().Id == 105, "Overdue status filter");
    Require(RenovationService.Filter(filterRecords, filterDate, 2026, "Completed", "Awaiting return").Single().Id == 104, "Completed and awaiting-return filters combine");
    Require(RenovationService.Filter(filterRecords, filterDate, 2024).Count == 0, "No matches returns empty list");
    renovationService.Delete(renovation.Id);
    Require(new RenovationService(new HouseholdRepository(renovationPath)).GetAll().All(r => r.Id != renovation.Id), "A renovation with deposit history is hidden from the active listing");
    Require(renovationService.GetAll().Single().Id == noDeposit.Id && renovationHouses.GetAll().Count == 1, "Archiving preserves other renovations and house");
    Reject(() => renovationService.Delete(renovation.Id));
    using (var c = new SqliteConnection($"Data Source={renovationPath}"))
    {
        c.Open(); using var cmd = c.CreateCommand(); cmd.CommandText = "DROP TABLE Renovations"; cmd.ExecuteNonQuery();
    }
    Require(new RenovationService(new HouseholdRepository(renovationPath)).GetAll().Count == 0 && renovationHouses.GetAll().Count == 1, "Existing databases gain renovations without losing houses");
    CheckAccounting(folder);
    CheckExpenses(folder);
    CheckDataReset(folder);
    CheckDerivedFlags(folder);
    CheckMonthDueDates(folder);
    CheckBatchPayments(folder);
    Console.WriteLine("PASS: renovations, deposits, returns and database upgrade.");
    Console.WriteLine("PASS: household checks, monthly generation, overlapping selections, due dates, shared deadlines, partial payments, rate snapshots and quarterly migration.");
}
finally { SqliteConnection.ClearAllPools(); Directory.Delete(folder, true); }
static void Require(bool value, string label) { if (!value) throw new Exception(label); }
static void Reject(Action action) { try { action(); } catch (ArgumentException) { return; } throw new Exception("Expected rejection"); }

static void CheckAccounting(string folder)
{
    var path = Path.Combine(folder, "accounting.db");
    var repo = new HouseholdRepository(path);
    var houses = new HouseholdService(repo);
    houses.Save(new Household { HouseNumber = "AC-1", IsOwned = true, Owners = ["Owner"] });
    var house = houses.GetAll().Single();
    houses.SaveSettings(new CommunitySettings { MonthlyMaintenanceFee = 100 });
    var billing = new MonthlyBillingService(repo);
    var renovations = new RenovationService(repo);
    var accounting = new AccountingService(repo);
    var accounts = accounting.GetAccounts();
    var bank = accounts.Single(a => a.Code == "1000"); var cash = accounts.Single(a => a.Code == "1010");
    var settings = accounting.GetSettings();
    Require(!settings.StartedOn.HasValue && accounts.Count == 11, "Accounting setup is seeded without starting or posting");
    billing.GenerateMonths(2026, [1], 100m, householdIds: [house.Id], chargeDate: DateTime.Today.AddDays(-1));
    var original = billing.GetCharges().Single();
    billing.RecordPayment(original.Id, 25, DateTime.Today.AddDays(-1), "Before accounting", cash.Id);
    Renovation NewDeposit(decimal amount) => new() { HouseholdId = house.Id, StartDate = DateTime.Today.AddDays(-1), DurationDays = 10, Description = "Accounting deposit", DepositAmount = amount };
    var held = NewDeposit(200); renovations.Add(held);
    renovations.RecordAction(held.Id, "Collect", DateTime.Today.AddDays(-1), bank.Id);
    renovations.Delete(held.Id);
    Require(renovations.GetAll(true).Single().IsArchived && renovations.GetAll().Count == 0, "Collected deposit archives instead of deleting");
    Require(renovations.GetAll(true).Single().DepositStatus == "Awaiting return", "Archived unfinished deposit still awaits return");
    Require(accounting.GetJournal().Count == 0, "Transactions before start do not fabricate historical ledger entries");
    Reject(() => accounting.Start(new Dictionary<int, decimal> { [bank.Id] = 1000 }));
    Reject(() => accounting.Start(new Dictionary<int, decimal> { [bank.Id] = -1, [cash.Id] = 50 }));
    accounting.Start(new Dictionary<int, decimal> { [bank.Id] = 1000, [cash.Id] = 50 });
    Require(accounting.GetJournal().Count == 4, "Opening balances include cash, bank, outstanding charge and archived held deposit exactly once");
    Require(accounting.GetJournal().Single(e => e.Source == "OpeningCharge").Lines.Sum(l => l.Debit) == 75, "Opening receivable is unpaid balance, not total charge");
    Reject(() => accounting.Start(new Dictionary<int, decimal> { [bank.Id] = 1000, [cash.Id] = 50 }));
    billing.RecordPayment(original.Id, 75, DateTime.Today, "Settle old charge", cash.Id);
    renovations.RecordAction(held.Id, "Return", DateTime.Today, cash.Id);
    Require(renovations.GetAll(true).Single().DepositStatus == "Returned", "Archived deposit can be refunded without completion");
    var before = accounting.GetJournal().Count;
    Reject(() => renovations.RecordAction(held.Id, "Return", DateTime.Today, cash.Id));
    Require(accounting.GetJournal().Count == before, "Duplicate refund adds no ledger entry");
    billing.GenerateMonths(2026, [2, 3], 100m, householdIds: [house.Id]);
    var feb = billing.GetCharges().Single(c => c.Month == 2);
    var march = billing.GetCharges().Single(c => c.Month == 3);
    var newReceivable = new Account { Code = "1110", Name = "New receivable", Type = "Asset" };
    var newIncome = new Account { Code = "4010", Name = "New maintenance income", Type = "Income" };
    var newLiability = new Account { Code = "2010", Name = "New deposit liability", Type = "Liability" };
    accounting.SaveAccount(newReceivable); accounting.SaveAccount(newIncome); accounting.SaveAccount(newLiability);
    var oldReceivable = settings.ReceivableAccountId; var oldDeposit = settings.DepositAccountId;
    settings.ReceivableAccountId = newReceivable.Id; settings.IncomeAccountId = newIncome.Id; settings.DepositAccountId = newLiability.Id; settings.ReceiptAccountId = cash.Id;
    accounting.SaveSettings(settings);
    billing.RecordPayment(feb.Id, 100, DateTime.Today, "Original account", bank.Id);
    var payment = billing.GetCharges().Single(c => c.Id == feb.Id).Payments.Single();
    var postedPayment = accounting.GetJournal().Single(e => e.Source == "MaintenancePayment" && e.SourceId == payment.Id);
    Require(postedPayment.Lines.Any(l => l.AccountId == oldReceivable && l.Credit == 100) && postedPayment.Lines.Any(l => l.AccountId == bank.Id && l.Debit == 100), "Payment keeps original receivable after defaults change and respects bank override");
    billing.GenerateMonths(2026, [4], 100m, householdIds: [house.Id]);
    var april = billing.GetCharges().Single(c => c.Month == 4);
    var newChargeEntry = accounting.GetJournal().Single(e => e.Source == "MonthlyCharge" && e.SourceId == april.Id);
    Require(newChargeEntry.Lines.Any(l => l.AccountId == newReceivable.Id && l.Debit == 100) && newChargeEntry.Lines.Any(l => l.AccountId == newIncome.Id && l.Credit == 100), "Changed defaults apply to new charges only");
    before = accounting.GetJournal().Count;
    Require(billing.GenerateMonths(2026, [2, 3, 4], 100m, householdIds: [house.Id]) == 0 && accounting.GetJournal().Count == before, "Repeated charge generation cannot duplicate postings");
    var deposit = NewDeposit(300); renovations.Add(deposit);
    renovations.RecordAction(deposit.Id, "Collect", DateTime.Today);
    var collection = accounting.GetJournal().Single(e => e.Source == "RenovationCollection" && e.SourceId == deposit.Id);
    Require(collection.Lines.Any(l => l.AccountId == cash.Id && l.Debit == 300), "Receipt uses updated default cash account");
    settings.DepositAccountId = oldDeposit; accounting.SaveSettings(settings);
    renovations.RecordAction(deposit.Id, "Complete", DateTime.Today);
    renovations.RecordAction(deposit.Id, "Return", DateTime.Today, bank.Id);
    var refund = accounting.GetJournal().Single(e => e.Source == "RenovationReturn" && e.SourceId == deposit.Id);
    Require(refund.Lines.Any(l => l.AccountId == newLiability.Id && l.Debit == 300) && refund.Lines.Any(l => l.AccountId == bank.Id && l.Credit == 300), "Refund clears original deposit liability after defaults change");
    renovations.Delete(deposit.Id);
    Require(accounting.GetJournal().Count(e => e.SourceId == deposit.Id && e.Source.StartsWith("Renovation")) == 2, "Archiving preserves collection and refund entries");
    Reject(() => billing.RecordPayment(march.Id, 10, DateTime.Today.AddDays(-1), "Backdated", bank.Id));
    Reject(() => billing.RecordPayment(march.Id, 10, DateTime.Today, "Wrong account", newIncome.Id));
    Require(billing.GetCharges().Single(c => c.Id == march.Id).Paid == 0, "Invalid posting date/account leaves source payment unchanged");
    Reject(() => accounting.SaveAccount(new Account { Code = "1000", Name = "Duplicate", Type = "Asset" }));
    Reject(() => accounting.SaveAccount(new Account { Code = "9999", Name = "Invalid", Type = "Income", IsCashBank = true }));
    bank.IsActive = false; Reject(() => accounting.SaveAccount(bank)); bank.IsActive = true;
    bank.Type = "Expense"; bank.IsCashBank = false; Reject(() => accounting.SaveAccount(bank)); bank.Type = "Asset"; bank.IsCashBank = true;
    // Force failure on the journal save after the source payment has been inserted, to verify transaction rollback.
    using (var c = new SqliteConnection($"Data Source={path}"))
    {
        c.Open(); using var cmd = c.CreateCommand();
        cmd.CommandText = "CREATE TRIGGER RejectTestPosting BEFORE INSERT ON JournalEntries WHEN NEW.Source='MaintenancePayment' BEGIN SELECT RAISE(ABORT, 'Test journal failure'); END"; cmd.ExecuteNonQuery();
    }
    var failed = false;
    try { billing.RecordPayment(march.Id, 10, DateTime.Today, "Rollback", bank.Id); }
    catch (Microsoft.EntityFrameworkCore.DbUpdateException) { failed = true; }
    Require(failed && billing.GetCharges().Single(c => c.Id == march.Id).Paid == 0, "Source and ledger roll back together if journal save fails");
    var reopened = new AccountingService(new HouseholdRepository(path));
    Require(reopened.GetSettings().StartedOn == DateTime.Today, "Accounting start persists and initialization is idempotent");
    var journal = reopened.GetJournal();
    Require(journal.All(e => e.Lines.Sum(l => l.Debit) == e.Lines.Sum(l => l.Credit)) && journal.Sum(e => e.Lines.Sum(l => l.Debit - l.Credit)) == 0, "Every journal entry and trial balance balance exactly");
    var allLines = journal.SelectMany(e => e.Lines).ToList();
    Require(allLines.Where(l => l.AccountId == oldDeposit || l.AccountId == newLiability.Id).Sum(l => l.Credit - l.Debit) == 0, "All returned deposits clear their liability accounts");
    Require(allLines.Where(l => l.AccountId == oldReceivable || l.AccountId == newReceivable.Id).Sum(l => l.Debit - l.Credit) == billing.GetCharges().Sum(c => c.Balance), "Receivable ledger reconciles to billing balances across changed mappings");
    Console.WriteLine("PASS: accounting setup, opening balances, automatic postings, cash/bank overrides, mapping changes, archive preservation and atomic rollback.");
}

static void CheckExpenses(string folder)
{
    var path = Path.Combine(folder, "expenses.db");
    var repository = new HouseholdRepository(path);
    var expenses = new ExpenseService(repository);
    var accounting = new AccountingService(repository);
    var supplier = new Payee { Name = "Security Company", Contact = "0123456" };
    var claimant = new Payee { Name = "Committee member", Kind = "Claimant" };
    expenses.SavePayee(supplier); expenses.SavePayee(claimant);
    Reject(() => expenses.SavePayee(new Payee { Name = "security COMPANY" }));
    var categories = expenses.GetCategories();
    var security = categories.Single(c => c.Name.Contains("Security"));
    var cleaning = categories.Single(c => c.Name.Contains("Cleaning"));
    ExpenseBill Bill(string reference = "") => new() { PayeeId = supplier.Id, Description = "Security and cleaning", Reference = reference, DueDate = DateTime.Today, Lines = [new() { CategoryId = security.Id, Description = "Guards", Amount = 4000 }, new() { CategoryId = cleaning.Id, Amount = 1000 }] };
    var draft = Bill("INV-1");
    draft.Attachments.Add(new ExpenseAttachment { FileName = "receipt.txt", Content = [65, 66, 67] });
    var id = expenses.Save(draft);
    Require(expenses.Get(id).Status == "Draft" && accounting.GetJournal().Count == 0, "Drafts have no accounting entries");
    Require(expenses.Get(id).Attachments.Single().Content.SequenceEqual(new byte[] { 65, 66, 67 }), "Attachment content persists in database");
    Reject(() => expenses.Post(id));
    Reject(() => expenses.Save(Bill("inv-1")));
    var stale = expenses.Get(id); var changed = expenses.Get(id); changed.Description = "Edited draft";
    expenses.Save(changed); Reject(() => expenses.Save(stale));
    Require(expenses.Get(id).Description == "Edited draft" && expenses.Get(id).Lines.Count == 2, "Draft version prevents overwriting newer edits and replaces lines");
    var wrongPayee = Bill(); wrongPayee.Kind = "Reimbursement claim"; Reject(() => expenses.Save(wrongPayee));
    var invalid = Bill(); invalid.Lines[0].Amount = -1; Reject(() => expenses.Save(invalid));
    invalid = Bill(); invalid.AccountingDate = DateTime.Today.AddDays(1); Reject(() => expenses.Save(invalid));
    invalid = Bill(); invalid.Attachments.Add(new() { FileName = "bad.exe", Content = [1] }); Reject(() => expenses.Save(invalid));
    var balances = accounting.GetAccounts().Where(a => a.IsActive && a.IsCashBank).ToDictionary(a => a.Id, a => 10000m);
    accounting.Start(balances);
    var bank = accounting.GetAccounts().Single(a => a.Code == "1000");
    var cash = accounting.GetAccounts().Single(a => a.Code == "1010");
    var payable = accounting.GetSettings().PayableAccountId;
    expenses.Post(id);
    Require(expenses.Get(id).Status == "Unpaid" && expenses.Get(id).Balance == 5000, "Posting creates an unpaid bill");
    var posting = accounting.GetJournal().Single(e => e.Source == "ExpenseBill" && e.SourceId == id);
    Require(posting.Lines.Count == 3 && posting.Lines.Single(l => l.AccountId == payable).Credit == 5000, "Multi-line expense posts against payable");
    Reject(() => expenses.Post(id)); Reject(() => expenses.DeleteDraft(id)); Reject(() => expenses.Save(expenses.Get(id)));
    expenses.Pay(id, 3000, DateTime.Today, bank.Id, "Bank transfer");
    Require(expenses.Get(id).Status == "Partially paid" && expenses.Get(id).Balance == 2000, "Partial payment reduces outstanding amount");
    Reject(() => expenses.Pay(id, 2000.01m, DateTime.Today, bank.Id, "Overpayment"));
    Reject(() => expenses.Pay(id, 1, DateTime.Today.AddDays(-1), bank.Id, "Before accounting"));
    Reject(() => expenses.Cancel(id, DateTime.Today, "Has actual payment"));
    // Defaults change, but both bill settlement and reversal retain the original accounts.
    var newPayable = new Account { Code = "2110", Name = "Other payable", Type = "Liability" }; accounting.SaveAccount(newPayable);
    var settings = accounting.GetSettings(); settings.PayableAccountId = newPayable.Id; settings.PaymentAccountId = cash.Id; accounting.SaveSettings(settings);
    expenses.Pay(id, 2000, DateTime.Today, null, "Cash settlement");
    Require(expenses.Get(id).Status == "Paid", "Final payment settles bill");
    var payments = expenses.Get(id).Payments.OrderBy(p => p.Id).ToList();
    var settlement = accounting.GetJournal().Single(e => e.Source == "ExpensePayment" && e.SourceId == payments[1].Id);
    Require(settlement.Lines.Any(l => l.AccountId == payable && l.Debit == 2000) && settlement.Lines.Any(l => l.AccountId == cash.Id && l.Credit == 2000), "Payment uses original payable and new default cash account");
    Reject(() => expenses.ReversePayment(payments[0].Id, DateTime.Today, ""));
    expenses.ReversePayment(payments[0].Id, DateTime.Today, "Incorrect payment entry");
    Require(expenses.Get(id).Balance == 3000 && expenses.Get(id).Payments.Count == 2, "Payment reversal reopens balance and preserves payment history");
    Reject(() => expenses.ReversePayment(payments[0].Id, DateTime.Today, "Duplicate reversal"));
    expenses.ReversePayment(payments[1].Id, DateTime.Today, "Money returned");
    expenses.Cancel(id, DateTime.Today, "Duplicate supplier invoice");
    Require(expenses.Get(id).Status == "Cancelled" && expenses.Get(id).Balance == 0 && expenses.Get(id).Attachments.Count == 1, "Cancellation preserves bill and attachment history");
    Reject(() => expenses.Cancel(id, DateTime.Today, "Again"));
    Reject(() => expenses.Pay(id, 1, DateTime.Today, bank.Id, "Cancelled bill"));
    Require(accounting.GetJournal().Where(e => (e.Source == "ExpenseBill" || e.Source == "ExpenseBillCancellation") && e.SourceId == id).SelectMany(e => e.Lines).GroupBy(l => l.AccountId).All(g => g.Sum(l => l.Debit - l.Credit) == 0), "Bill cancellation exactly reverses original accounts");
    var corrected = Bill("INV-1"); var correctedId = expenses.Save(corrected); expenses.Post(correctedId);
    Require(accounting.GetJournal().Single(e => e.Source == "ExpenseBill" && e.SourceId == correctedId).Lines.Any(l => l.AccountId == newPayable.Id && l.Credit == 5000), "Corrected bill can reuse cancelled reference and uses new default payable");
    // Category remapping affects drafts/future entries, never posted expense lines.
    security.AccountId = cleaning.AccountId; expenses.SaveCategory(security);
    Require(expenses.Get(correctedId).Lines.Any(l => l.AccountId != cleaning.AccountId), "Category changes preserve posted account snapshots");
    var claim = Bill("CLAIM-1"); claim.Kind = "Reimbursement claim"; claim.PayeeId = claimant.Id; claim.Lines = [new() { CategoryId = cleaning.Id, Amount = 123.45m }];
    var claimId = expenses.Save(claim, true, bank.Id, DateTime.Today, "Claim paid");
    Require(expenses.Get(claimId).Status == "Paid" && expenses.Get(claimId).Paid == 123.45m, "Claim can post and pay atomically");
    var direct = Bill("DIRECT-1"); direct.Kind = "Direct expense"; direct.Lines = [new() { CategoryId = cleaning.Id, Amount = 20 }];
    var directId = expenses.Save(direct, true, cash.Id);
    Require(expenses.Get(directId).Status == "Paid", "Direct expense posts and pays immediately");
    var removeId = expenses.Save(Bill("DRAFT-DELETE")); expenses.DeleteDraft(removeId); Reject(() => expenses.Get(removeId));
    claimant.IsActive = false; expenses.SavePayee(claimant); claim.Reference = "CLAIM-2"; Reject(() => expenses.Save(claim));
    cleaning.IsActive = false; expenses.SaveCategory(cleaning); Reject(() => expenses.Save(Bill("INACTIVE-CATEGORY"))); cleaning.IsActive = true; expenses.SaveCategory(cleaning);
    // Force payment journal failure after inserting the source payment to verify rollback.
    using (var c = new SqliteConnection($"Data Source={path}"))
    {
        c.Open(); using var cmd = c.CreateCommand(); cmd.CommandText = "CREATE TRIGGER FailExpensePayment BEFORE INSERT ON JournalEntries WHEN NEW.Source='ExpensePayment' BEGIN SELECT RAISE(ABORT,'Test failure'); END"; cmd.ExecuteNonQuery();
    }
    var failed = false;
    try { expenses.Pay(correctedId, 10, DateTime.Today, bank.Id, "Rollback"); } catch (Microsoft.EntityFrameworkCore.DbUpdateException) { failed = true; }
    Require(failed && expenses.Get(correctedId).Paid == 0, "Payment source and journal rollback together");
    var count = expenses.GetAll().Count; failed = false;
    try { expenses.Save(Bill("ROLLBACK-IMMEDIATE"), true, bank.Id); } catch (Microsoft.EntityFrameworkCore.DbUpdateException) { failed = true; }
    Require(failed && expenses.GetAll().Count == count, "Save/post/pay rolls back the entire new expense on journal failure");
    var reopened = new ExpenseService(new HouseholdRepository(path));
    Require(reopened.Get(id).CancellationReason == "Duplicate supplier invoice" && reopened.Get(id).Payments.All(p => p.ReversedDate.HasValue), "Cancellation and reversal reasons persist after reopening");
    var ledger = accounting.GetJournal(); var allLines = ledger.SelectMany(e => e.Lines).ToList();
    Require(ledger.All(e => e.Lines.Sum(l => l.Debit) == e.Lines.Sum(l => l.Credit)), "Every expense and reversal entry balances to the sen");
    Require(allLines.Where(l => l.AccountId == payable || l.AccountId == newPayable.Id).Sum(l => l.Credit - l.Debit) == expenses.GetAll().Sum(b => b.Balance), "Payable ledger reconciles to outstanding expense balances");
    Require(new ExpenseBill { IsPosted = true, DueDate = DateTime.Today.AddDays(-1), Lines = [new() { Amount = 1 }] }.IsOverdueOn(DateTime.Today), "Overdue status follows due date and unpaid balance");
    // Simulate the previous accounting-enabled schema, before expenses or a payable mapping existed.
    var upgradePath = Path.Combine(folder, "expense-upgrade.db");
    var upgradeRepo = new HouseholdRepository(upgradePath); var upgradeAccounting = new AccountingService(upgradeRepo);
    upgradeAccounting.Start(upgradeAccounting.GetAccounts().Where(a => a.IsActive && a.IsCashBank).ToDictionary(a => a.Id, a => 100m));
    var previousPayable = upgradeAccounting.GetSettings().PayableAccountId;
    var previousJournal = upgradeAccounting.GetJournal().Count;
    using (var c = new SqliteConnection($"Data Source={upgradePath}"))
    {
        c.Open(); using var cmd = c.CreateCommand();
        cmd.CommandText = $"DROP TABLE ExpenseAttachments; DROP TABLE ExpensePayments; DROP TABLE ExpenseLines; DROP TABLE ExpenseBills; DROP TABLE ExpenseCategories; DROP TABLE Payees; ALTER TABLE AccountingSetup DROP COLUMN PayableAccountId; DELETE FROM Accounts WHERE Id={previousPayable};";
        cmd.ExecuteNonQuery();
    }
    var upgraded = new HouseholdRepository(upgradePath);
    Require(upgraded.GetAccountingSettings().StartedOn == DateTime.Today && upgraded.GetAccountingSettings().PayableAccountId > 0 && upgraded.GetJournal().Count == previousJournal && upgraded.GetExpenseCategories().Count == 4, "Previous accounting-enabled database upgrades without altering its ledger or opening balances");
    Console.WriteLine("PASS: expense drafts, attachments, posting, partial payments, claims, direct expenses, corrections, payable reconciliation and atomic rollback.");
}

static void CheckDataReset(string folder)
{
    static HouseholdRepository Seed(string path)
    {
        var repo = new HouseholdRepository(path);
        var households = new HouseholdService(repo);
        households.SaveSettings(new CommunitySettings { MonthlyMaintenanceFee = 100, FlagNames = ["Trial flag"] });
        households.Save(new Household { HouseNumber = "RESET-1", IsOwned = true, Owners = ["Test owner"], Contacts = ["0123"], Flags = [new() { Name = "Trial flag" }] });
        var accounting = new AccountingService(repo);
        accounting.Start(accounting.GetAccounts().Where(a => a.IsActive && a.IsCashBank).ToDictionary(a => a.Id, a => 2000m));
        var billing = new MonthlyBillingService(repo); billing.GenerateMonths(2026, [1], 100m);
        billing.RecordPayment(billing.GetCharges().Single().Id, 25, DateTime.Today, "Trial payment");
        var renovations = new RenovationService(repo);
        var renovation = new Renovation { HouseholdId = households.GetAll().Single().Id, Description = "Trial renovation", StartDate = DateTime.Today, DurationDays = 10, DepositAmount = 200 };
        renovations.Add(renovation); renovations.RecordAction(renovation.Id, "Collect", DateTime.Today); renovations.Delete(renovation.Id);
        var expenses = new ExpenseService(repo); var payee = new Payee { Name = "Trial supplier" }; expenses.SavePayee(payee);
        var expenseId = expenses.Save(new ExpenseBill { PayeeId = payee.Id, Description = "Trial expense", Reference = "TRIAL-1", Lines = [new() { CategoryId = expenses.GetCategories().First().Id, Amount = 500 }], Attachments = [new() { FileName = "receipt.txt", Content = [65] }] });
        expenses.Post(expenseId); expenses.Pay(expenseId, 100, DateTime.Today, null, "Trial outgoing payment");
        using var c = new SqliteConnection($"Data Source={path}"); c.Open(); using var command = c.CreateCommand();
        command.CommandText = "CREATE TABLE QuarterlyCharges(Id INTEGER PRIMARY KEY, HouseholdId INTEGER REFERENCES Households(Id)); CREATE TABLE MaintenancePayments(Id INTEGER PRIMARY KEY, QuarterlyChargeId INTEGER REFERENCES QuarterlyCharges(Id)); CREATE TABLE LegacyHouseholdFees(HouseholdId INTEGER REFERENCES Households(Id), MonthlyFeeSen INTEGER); INSERT INTO QuarterlyCharges VALUES(1,1); INSERT INTO MaintenancePayments VALUES(1,1); INSERT INTO LegacyHouseholdFees VALUES(1,10000);"; command.ExecuteNonQuery();
        return repo;
    }
    Reject(() => DataResetService.IncludeDependencies((DataResetCategory)1024));
    foreach (var category in new[] { DataResetCategory.Households, DataResetCategory.Maintenance, DataResetCategory.Renovations, DataResetCategory.Expenses, DataResetCategory.Payees, DataResetCategory.ExpenseCategories, DataResetCategory.Accounting, DataResetCategory.CommunitySettings, DataResetCategory.All })
    {
        var path = Path.Combine(folder, $"reset-{category}.db"); var repo = Seed(path); var reset = new DataResetService(repo);
        Reject(() => reset.Prepare(DataResetCategory.None));
        var confirmation = reset.Prepare(category); var selected = confirmation.Categories;
        Require(confirmation.Code.Length == 6 && confirmation.Code.All(c => c is >= 'a' and <= 'z' or >= '0' and <= '9') && confirmation.Code.Any(char.IsLower) && confirmation.Code.Any(char.IsDigit), "Confirmation contains lowercase letters and numbers");
        var journalsBefore = repo.GetJournal().Count;
        Reject(() => reset.Delete(confirmation, "")); Reject(() => reset.Delete(confirmation, confirmation.Code.ToUpperInvariant()));
        Require(repo.GetAll().Count == 1 && repo.GetCharges().Count == 1 && repo.GetJournal().Count == journalsBefore, "Wrong or uppercase captcha deletes nothing");
        reset.Delete(confirmation, confirmation.Code);
        Reject(() => reset.Delete(confirmation, confirmation.Code));
        Require(repo.GetAll().Count == (selected.HasFlag(DataResetCategory.Households) ? 0 : 1), "Only selected households are deleted");
        Require(repo.GetCharges().Count == (selected.HasFlag(DataResetCategory.Maintenance) ? 0 : 1), "Only selected billing records are deleted");
        Require(repo.GetRenovations(true).Count == (selected.HasFlag(DataResetCategory.Renovations) ? 0 : 1), "Renovation deletion includes archived deposits");
        Require(repo.GetExpenses().Count == (selected.HasFlag(DataResetCategory.Expenses) ? 0 : 1), "Only selected expenses are deleted");
        Require(repo.GetPayees().Count == (selected.HasFlag(DataResetCategory.Payees) ? 0 : 1), "Only selected payees are deleted");
        Require(repo.GetExpenseCategories().Count == 4 && repo.GetAccounts().Count == 11, "Default categories/accounts remain usable after reset");
        var settings = repo.GetSettings();
        Require(settings.MonthlyMaintenanceFee == (selected.HasFlag(DataResetCategory.CommunitySettings) ? 0 : 100), "Community settings reset only when selected");
        if (selected.HasFlag(DataResetCategory.CommunitySettings)) Require(settings.FlagNames.Count == 0 && repo.GetAll().All(h => h.Flags.All(DerivedHouseFlags.IsManaged)), "Saved flag definitions/assignments clear; live automatic flags reflect remaining records");
        else Require(settings.FlagNames.Contains("Trial flag"), "Unselected flags remain");
        var journal = repo.GetJournal();
        if (selected.HasFlag(DataResetCategory.Accounting)) Require(journal.Count == 0 && !repo.GetAccountingSettings().StartedOn.HasValue, "Accounting reset clears opening balances, journals and activation");
        else Require(repo.GetAccountingSettings().StartedOn.HasValue && journal.Count(e => e.Source == "OpeningCash") == 2, "Partial deletion preserves accounting setup and opening cash");
        Require(!selected.HasFlag(DataResetCategory.Maintenance) || journal.All(e => e.Source is not ("MonthlyCharge" or "MaintenancePayment" or "OpeningCharge")), "Deleted billing leaves no associated journals");
        Require(!selected.HasFlag(DataResetCategory.Renovations) || journal.All(e => e.Source is not ("RenovationCollection" or "RenovationReturn" or "OpeningDeposit")), "Deleted deposits leave no associated journals");
        Require(!selected.HasFlag(DataResetCategory.Expenses) || journal.All(e => !e.Source.StartsWith("Expense")), "Deleted expenses leave no associated journals");
        using (var db = new HouseholdDbContext(path))
        {
            Require(!selected.HasFlag(DataResetCategory.Expenses) || (!db.ExpensePayments.Any() && !db.ExpenseAttachments.Any() && !db.ExpenseLines.Any()), "Expense child records and receipt blobs are removed");
            Require(db.JournalLines.All(l => db.JournalEntries.Any(e => e.Id == l.JournalEntryId) && db.Accounts.Any(a => a.Id == l.AccountId)), "No orphan journal lines after deletion");
        }
        var reopened = new HouseholdRepository(path);
        Require(reopened.GetCharges().Count == repo.GetCharges().Count && reopened.GetJournal().Count == journal.Count, "Reopening does not resurrect deleted data or legacy charges");
        if (selected.HasFlag(DataResetCategory.Accounting))
        {
            var accounting = new AccountingService(reopened); accounting.Start(accounting.GetAccounts().Where(a => a.IsCashBank && a.IsActive).ToDictionary(a => a.Id, a => 0m));
            Require(accounting.GetSettings().StartedOn.HasValue, "Accounting can be started again after reset");
        }
    }
    var rollbackPath = Path.Combine(folder, "reset-rollback.db"); var rollbackRepo = Seed(rollbackPath); var rollbackReset = new DataResetService(rollbackRepo);
    var countBefore = rollbackRepo.GetJournal().Count;
    using (var c = new SqliteConnection($"Data Source={rollbackPath}"))
    {
        c.Open(); using var command = c.CreateCommand(); command.CommandText = "CREATE TRIGGER RejectReset BEFORE DELETE ON ExpenseBills BEGIN SELECT RAISE(ABORT,'Test reset failure'); END"; command.ExecuteNonQuery();
    }
    var token = rollbackReset.Prepare(DataResetCategory.All); var failed = false;
    try { rollbackReset.Delete(token, token.Code); } catch (SqliteException) { failed = true; }
    Require(failed && rollbackRepo.GetAll().Count == 1 && rollbackRepo.GetExpenses().Count == 1 && rollbackRepo.GetExpense(1).Payments.Count == 1 && rollbackRepo.GetExpense(1).Attachments.Count == 1 && rollbackRepo.GetJournal().Count == countBefore, "Any reset failure rolls back source records, attachments and journals together");
    Console.WriteLine("PASS: category deletion, dependency expansion, captcha validation, complete reset, legacy cleanup, restart and atomic rollback.");
}

static void CheckDerivedFlags(string folder)
{
    var today = new DateTime(2026, 6, 15);
    var house = new Household { Id = 1, HouseNumber = "FLAGS-1", IsActive = false, Flags = [new() { Name = "Manual example" }, new() { Name = "Other module", Source = "Other" }] };
    var charge = new MonthlyCharge { HouseholdId = 1, Amount = 100, DueDate = today };
    var upcoming = new Renovation { Id = 1, HouseholdId = 1, StartDate = today.AddDays(1), DurationDays = 3, DepositAmount = 100 };
    var current = new Renovation { Id = 2, HouseholdId = 1, StartDate = today, DurationDays = 1, DepositAmount = 0 };
    var late = new Renovation { Id = 3, HouseholdId = 1, StartDate = today.AddDays(-5), DurationDays = 2, DepositAmount = 100, DepositCollectedDate = today.AddDays(-5) };
    var completed = new Renovation { Id = 4, HouseholdId = 1, StartDate = today.AddDays(-10), DurationDays = 2, DepositAmount = 100, DepositCollectedDate = today.AddDays(-10), CompletedDate = today };
    bool Has(string name) => house.Flags.Any(f => f.Name == name && f.Source != "Manual");
    DerivedHouseFlags.Apply([house], [charge], [upcoming, current, late, completed], today);
    Require(Has(DerivedHouseFlags.MaintenanceOutstanding) && !Has(DerivedHouseFlags.MaintenanceOverdue), "Unpaid maintenance is outstanding but not overdue on due date");
    Require(new[] { DerivedHouseFlags.UpcomingRenovation, DerivedHouseFlags.CurrentRenovation, DerivedHouseFlags.OverdueRenovation, DerivedHouseFlags.DepositCollection, DerivedHouseFlags.DepositHeld, DerivedHouseFlags.DepositReturn }.All(Has), "All renovation flags aggregate across multiple records, including inactive houses");
    var count = house.Flags.Count;
    DerivedHouseFlags.Apply([house], [charge], [upcoming, current, late, completed], today);
    Require(house.Flags.Count == count && house.Flags.GroupBy(f => (f.Name, f.Source)).All(g => g.Count() == 1), "Repeated calculation never duplicates flags");
    DerivedHouseFlags.Apply([house], [charge], [upcoming, current, late, completed], today.AddDays(1));
    Require(Has(DerivedHouseFlags.MaintenanceOverdue) && !Has(DerivedHouseFlags.UpcomingRenovation), "Due dates and start dates roll over without any source record edits");
    DerivedHouseFlags.Apply([house], [], [current], today);
    Require(Has(DerivedHouseFlags.CurrentRenovation) && !Has(DerivedHouseFlags.OverdueRenovation) && !Has(DerivedHouseFlags.DepositHeld) && !Has(DerivedHouseFlags.DepositCollection), "Inclusive planned end is not overdue and zero deposit has no financial flags");
    current.CompletedDate = today;
    DerivedHouseFlags.Apply([house], [], [current], today);
    Require(house.Flags.Count == 2, "Completion clears active work; manual and unknown-source flags remain");
    late.IsArchived = true;
    DerivedHouseFlags.Apply([house], [], [late], today);
    Require(!Has(DerivedHouseFlags.CurrentRenovation) && !Has(DerivedHouseFlags.OverdueRenovation) && Has(DerivedHouseFlags.DepositHeld) && Has(DerivedHouseFlags.DepositReturn), "Archived renovation only retains unreturned deposit flags");
    late.DepositReturnedDate = today;
    DerivedHouseFlags.Apply([house], [], [late], today);
    Require(house.Flags.Count == 2, "Returning archived deposit clears both financial flags");
    charge.Payments.Add(new MaintenancePayment { Amount = 25 });
    DerivedHouseFlags.Apply([house], [charge], [], today.AddDays(1));
    Require(Has(DerivedHouseFlags.MaintenanceOutstanding) && Has(DerivedHouseFlags.MaintenanceOverdue), "Partial payments retain outstanding and overdue flags");
    charge.Payments.Add(new MaintenancePayment { Amount = 75 });
    DerivedHouseFlags.Apply([house], [charge], [], today.AddDays(1));
    Require(house.Flags.Count == 2, "Full payment clears maintenance flags");
    var path = Path.Combine(folder, "derived-flags.db"); var repo = new HouseholdRepository(path); var households = new HouseholdService(repo);
    households.Save(new Household { HouseNumber = "AUTO-1", IsOwned = true, Owners = ["Owner"], Flags = [new() { Name = "Custom" }] });
    var savedHouse = households.GetAll().Single();
    households.SaveSettings(new CommunitySettings { MonthlyMaintenanceFee = 100, FlagNames = ["Custom"] });
    var billing = new MonthlyBillingService(repo); billing.GenerateMonths(2026, [1], 100m, DateTime.Today.AddDays(-1));
    var renovations = new RenovationService(repo);
    var r = new Renovation { HouseholdId = savedHouse.Id, StartDate = DateTime.Today.AddDays(-3), DurationDays = 1, Description = "Auto flags", DepositAmount = 200 };
    renovations.Add(r);
    var live = households.GetAll().Single();
    Require(live.Flags.Any(f => f.Name == DerivedHouseFlags.MaintenanceOverdue) && live.Flags.Any(f => f.Name == DerivedHouseFlags.DepositCollection), "Repository reads derive live flags from actual billing and renovations");
    Require(!households.GetAvailableFlags().Any(DerivedHouseFlags.IsAutomaticName), "Automatic flags never enter the manual options checklist");
    Reject(() => households.AddFlag(DerivedHouseFlags.CurrentRenovation)); Reject(() => households.DeleteFlag(DerivedHouseFlags.CurrentRenovation, live.Id));
    Reject(() => households.SaveSettings(new CommunitySettings { FlagNames = [DerivedHouseFlags.MaintenanceOutstanding] }));
    var fabricated = new Household { HouseNumber = "Fake", Flags = [new() { Name = DerivedHouseFlags.CurrentRenovation }] }; Reject(() => households.Save(fabricated));
    live.Owners = ["Updated owner"]; households.Save(live);
    using (var db = new HouseholdDbContext(path)) Require(!db.Households.Single().Flags.Any(DerivedHouseFlags.IsManaged), "Saving a house never stores stale computed flags");
    billing.RecordPayment(billing.GetCharges().Single().Id, 100, DateTime.Today, "Paid");
    renovations.RecordAction(r.Id, "Collect", DateTime.Today);
    live = households.GetAll().Single();
    Require(!live.Flags.Any(f => f.Source == "Billing") && live.Flags.Any(f => f.Name == DerivedHouseFlags.DepositHeld) && !live.Flags.Any(f => f.Name == DerivedHouseFlags.DepositCollection), "Recording payment and collecting deposit immediately changes flags on read");
    renovations.RecordAction(r.Id, "Complete", DateTime.Today);
    live = households.GetAll().Single();
    Require(!live.Flags.Any(f => f.Name == DerivedHouseFlags.CurrentRenovation) && live.Flags.Any(f => f.Name == DerivedHouseFlags.DepositReturn), "Completion clears work flags and adds awaiting return");
    renovations.RecordAction(r.Id, "Return", DateTime.Today);
    Require(households.GetAll().Single().Flags.Single().Name == "Custom", "Refund clears final automatic flags without changing manual flags");
    var future = new Renovation { HouseholdId = savedHouse.Id, StartDate = DateTime.Today.AddDays(2), DurationDays = 1, Description = "Future", DepositAmount = 0 }; renovations.Add(future);
    Require(households.GetAll().Single().Flags.Any(f => f.Name == DerivedHouseFlags.UpcomingRenovation), "Future renovation automatically flags its house");
    renovations.Delete(future.Id);
    Require(new HouseholdService(new HouseholdRepository(path)).GetAll().Single().Flags.Single().Name == "Custom", "Deleting source records and reopening does not leave stale automatic flags");
    Console.WriteLine("PASS: all eight derived flags, date boundaries, partial payments, archived refunds, multiple renovations, read-only guards and live nonpersistent calculation.");
}

static void CheckMonthDueDates(string folder)
{
    var path = Path.Combine(folder, "month-due-dates.db"); var repo = new HouseholdRepository(path);
    var households = new HouseholdService(repo); households.Save(new Household { HouseNumber = "DUE-1" });
    var house = households.GetAll().Single(); var billing = new MonthlyBillingService(repo);
    var accounting = new AccountingService(repo); accounting.Start(accounting.GetAccounts().Where(a => a.IsActive && a.IsCashBank).ToDictionary(a => a.Id, a => 0m));
    var dates = new Dictionary<int, DateTime> { [1] = new(2028, 1, 10, 12, 30, 0), [2] = new(2028, 2, 29), [3] = new(2028, 4, 2) };
    Require(billing.GenerateMonths(2028, [1, 2, 3], 75.50m, householdIds: [house.Id], monthDueDates: dates) == 3, "One batch saves independently selected due dates per month");
    var charges = billing.GetCharges().OrderBy(c => c.Month).ToList();
    Require(charges.All(c => c.DueDate == dates[c.Month].Date && c.Amount == 75.50m), "Per-month due dates are stored as dates and keep the selected amount");
    Require(charges[1].StatusOn(new DateTime(2028, 2, 29)) == "Payment due" && charges[1].StatusOn(new DateTime(2028, 3, 1)) == "Overdue", "Leap-day custom due date controls overdue boundary");
    var replacements = new Dictionary<int, DateTime> { [2] = new(2028, 2, 15), [3] = new(2028, 3, 20), [4] = new(2028, 4, 21) };
    Require(billing.GenerateMonths(2028, [2, 3, 4], 80m, householdIds: [house.Id], monthDueDates: replacements) == 1, "Existing months are skipped in per-month due mode");
    charges = billing.GetCharges();
    Require(charges.Single(c => c.Month == 2).DueDate == dates[2] && charges.Single(c => c.Month == 3).DueDate == dates[3] && charges.Single(c => c.Month == 4).DueDate == replacements[4], "Existing deadlines remain unchanged while a new month uses its own date");
    households.Save(new Household { HouseNumber = "DUE-2" });
    Require(billing.GenerateMonths(2028, [5], 90m, monthDueDates: new Dictionary<int, DateTime> { [5] = new(2028, 5, 8) }) == 2, "Monthly deadline applies to every selected house");
    Require(billing.GetCharges().Where(c => c.Month == 5).All(c => c.DueDate == new DateTime(2028, 5, 8)), "Both houses keep the same selected monthly due date");
    var before = billing.GetCharges().Count; var postedBefore = accounting.GetJournal().Count;
    Reject(() => billing.GenerateMonths(2028, [9, 10], 100m, monthDueDates: new Dictionary<int, DateTime> { [9] = new(2028, 9, 6) }));
    Reject(() => billing.GenerateMonths(2028, [9], 100m, monthDueDates: new Dictionary<int, DateTime> { [9] = new(2028, 9, 6), [10] = new(2028, 10, 6) }));
    Reject(() => billing.GenerateMonths(2028, [9], 100m, new DateTime(2028, 9, 6), monthDueDates: new Dictionary<int, DateTime> { [9] = new(2028, 9, 10) }));
    Reject(() => repo.GenerateMonths(2028, [9], 100m, null, [house.Id], new Dictionary<int, DateTime> { [10] = new(2028, 10, 6) }));
    Require(billing.GetCharges().Count == before && accounting.GetJournal().Count == postedBefore, "Invalid or conflicting monthly dates leave charges and ledger unchanged");
    billing.GenerateMonths(2028, [6], 100m, householdIds: [house.Id]);
    billing.GenerateMonths(2028, [7, 8], 100m, new DateTime(2028, 7, 10), [house.Id]);
    Require(billing.GetCharges().Single(c => c.Month == 6).DueDate == new DateTime(2028, 6, 6), "Normal mode still defaults to the month's 6th");
    Require(billing.GetCharges().Where(c => c.Month is 7 or 8).All(c => c.DueDate == new DateTime(2028, 7, 10)), "Single shared date mode remains available");
    Require(new MonthlyBillingService(new HouseholdRepository(path)).GetCharges().Single(c => c.Month == 2).DueDate == new DateTime(2028, 2, 29), "Custom monthly due dates survive reopening");
    Require(accounting.GetJournal().Where(e => e.Source == "MonthlyCharge").Count() == billing.GetCharges().Count && accounting.GetJournal().All(e => e.Lines.Sum(l => l.Debit) == e.Lines.Sum(l => l.Credit)), "Per-month dates preserve one balanced posting per newly created charge");
    Console.WriteLine("PASS: per-month due dates, leap days, selected houses, unchanged existing deadlines, validation, shared/default modes and journal consistency.");
}

static void CheckBatchPayments(string folder)
{
    var path = Path.Combine(folder, "batch-payments.db"); var repo = new HouseholdRepository(path);
    var houses = new HouseholdService(repo); houses.CreateRange("BATCH-", 1, 2, 1);
    var first = houses.GetAll()[0]; var second = houses.GetAll()[1];
    var billing = new MonthlyBillingService(repo); var accounting = new AccountingService(repo);
    var bank = accounting.GetAccounts().Single(a => a.Code == "1000"); var cash = accounting.GetAccounts().Single(a => a.Code == "1010");
    billing.GenerateMonths(2026, [1, 2], 100m);
    MonthlyCharge Charge(int houseId, int month) => billing.GetCharges().Single(c => c.HouseholdId == houseId && c.Month == month);
    var jan1 = Charge(first.Id, 1); var jan2 = Charge(second.Id, 1); var feb1 = Charge(first.Id, 2); var feb2 = Charge(second.Id, 2);
    Require(billing.RecordPayments([new(jan1.Id, 25, 100, " First receipt "), new(jan2.Id, 50, 100, "Second receipt")], DateTime.Today, bank.Id) == 2, "Batch payments can be recorded before accounting starts");
    Require(Charge(first.Id, 1).Balance == 75 && Charge(second.Id, 1).Balance == 50 && accounting.GetJournal().Count == 0, "Pre-start batch reduces charges without inventing historical postings");
    Require(Charge(first.Id, 1).Payments.Single().Reference == "First receipt", "Batch references are trimmed");
    accounting.Start(accounting.GetAccounts().Where(a => a.IsActive && a.IsCashBank).ToDictionary(a => a.Id, a => 1000m));
    var oldReceivable = accounting.GetSettings().ReceivableAccountId;
    var newReceivable = new Account { Code = "1110", Name = "New maintenance receivable", Type = "Asset" }; accounting.SaveAccount(newReceivable);
    var settings = accounting.GetSettings(); settings.ReceivableAccountId = newReceivable.Id; accounting.SaveSettings(settings);
    billing.GenerateMonths(2026, [3], 100m); var march2 = Charge(second.Id, 3);
    var preBatchEntryIds = accounting.GetJournal().Select(e => e.Id).ToHashSet();
    var batch = new List<MaintenancePaymentRequest> { new(jan1.Id, 75, 75, "One owner / January"), new(feb1.Id, 40, 100, "One owner / February"), new(march2.Id, 100, 100, "Another house") };
    Require(billing.RecordPayments(batch, DateTime.Today, cash.Id) == 3, "One batch supports multiple months, multiple houses and partial payments");
    Require(Charge(first.Id, 1).Balance == 0 && Charge(first.Id, 2).Balance == 60 && Charge(second.Id, 3).Balance == 0, "Every batch row reduces only its own month");
    var posted = accounting.GetJournal().Where(e => e.Source == "MaintenancePayment" && !preBatchEntryIds.Contains(e.Id)).ToList();
    Require(posted.Count == 3 && posted.All(e => e.Lines.Any(l => l.AccountId == cash.Id && l.Debit > 0)), "Every batch payment gets one entry using shared cash account");
    Require(posted.SelectMany(e => e.Lines).Where(l => l.AccountId == oldReceivable).Sum(l => l.Credit) == 115 && posted.SelectMany(e => e.Lines).Where(l => l.AccountId == newReceivable.Id).Sum(l => l.Credit) == 100, "Batch settlement follows original receivable accounts including opening balances");
    var paymentsBefore = billing.GetCharges().Sum(c => c.Payments.Count); var journalsBefore = accounting.GetJournal().Count;
    Reject(() => billing.RecordPayments([], DateTime.Today));
    Reject(() => billing.RecordPayments([new(feb2.Id, 10, 100, ""), new(feb2.Id, 10, 100, "")], DateTime.Today));
    Reject(() => billing.RecordPayments([new(feb2.Id, 0, 100, "")], DateTime.Today));
    Reject(() => billing.RecordPayments([new(feb2.Id, 10.001m, 100, "")], DateTime.Today));
    Reject(() => billing.RecordPayments([new(feb2.Id, 101, 100, "")], DateTime.Today));
    Reject(() => billing.RecordPayments([new(feb2.Id, 10, 100, new string('x', 151))], DateTime.Today));
    Reject(() => billing.RecordPayments([new(feb2.Id, 10, 100, "")], DateTime.Today.AddDays(1)));
    Reject(() => billing.RecordPayments([new(feb2.Id, 10, 100, "")], DateTime.Today.AddDays(-1)));
    Reject(() => billing.RecordPayments([new(feb2.Id, 10, 100, "")], DateTime.Today, settings.IncomeAccountId));
    Reject(() => billing.RecordPayments([new(feb2.Id, 10, 100, ""), new(int.MaxValue, 10, 100, "Missing charge")], DateTime.Today));
    Reject(() => billing.RecordPayments([new(jan2.Id, 10, 50, "Valid row"), new(feb1.Id, 10, 100, "Stale row")], DateTime.Today));
    Reject(() => billing.RecordPayments(batch, DateTime.Today, cash.Id));
    Require(billing.GetCharges().Sum(c => c.Payments.Count) == paymentsBefore && accounting.GetJournal().Count == journalsBefore && Charge(second.Id, 1).Balance == 50, "Invalid, stale and replayed batches never partially save");
    using (var c = new SqliteConnection($"Data Source={path}"))
    {
        c.Open(); using var command = c.CreateCommand();
        command.CommandText = "CREATE TRIGGER FailBatchJournal BEFORE INSERT ON JournalEntries WHEN NEW.Source='MaintenancePayment' AND NEW.Description LIKE '%ROLLBACK-2%' BEGIN SELECT RAISE(ABORT,'Test batch journal failure'); END"; command.ExecuteNonQuery();
    }
    var failed = false;
    try { billing.RecordPayments([new(jan2.Id, 10, 50, "ROLLBACK-1"), new(feb2.Id, 20, 100, "ROLLBACK-2")], DateTime.Today, bank.Id); }
    catch (Microsoft.EntityFrameworkCore.DbUpdateException) { failed = true; }
    Require(failed && billing.GetCharges().Sum(c => c.Payments.Count) == paymentsBefore && accounting.GetJournal().Count == journalsBefore && Charge(second.Id, 1).Balance == 50 && Charge(second.Id, 2).Balance == 100, "A later journal failure rolls back every payment and earlier journal inserts in the batch");
    var reopened = new MonthlyBillingService(new HouseholdRepository(path));
    Require(reopened.GetCharges().Sum(c => c.Payments.Count) == paymentsBefore, "Committed batch payments persist after reopening");
    var lines = accounting.GetJournal().SelectMany(e => e.Lines).ToList();
    Require(accounting.GetJournal().All(e => e.Lines.Sum(l => l.Debit) == e.Lines.Sum(l => l.Credit)) && lines.Where(l => l.AccountId == oldReceivable || l.AccountId == newReceivable.Id).Sum(l => l.Debit - l.Credit) == billing.GetCharges().Sum(c => c.Balance), "Batch journals balance and receivables reconcile to monthly charges");
    Console.WriteLine("PASS: multi-house/month payment batches, partial amounts, references, original accounts, stale balances, replay prevention and atomic rollback.");
}
