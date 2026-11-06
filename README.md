# Housing Community Management

Offline Windows Forms application with SQLite storage. Houses exist independently of owners. Create a numeric range with an optional prefix and zero-padding (for example A-001 through A-100); existing numbers are skipped. Each house supports an owned/unowned status, multiple owner names and contact numbers, multiple flags, and an active/inactive record status. House numbers are unique ignoring ASCII letter case.

Use **Community settings** to set the monthly maintenance fee for all houses. Owners and contacts are entered one per line. In the house editor, type a flag name above the checkboxes and click **Add** (or press Enter). Adding a flag immediately remembers its name for all houses and checks it for the current house; click Save to apply the selection to that house. Reopening the editor lists remembered names and all flags used on other houses. Flag names are trimmed and duplicates are matched without case. There are no predefined flags; unused original starter suggestions are removed on opening an older database. Flags store their source so future modules can supply derived flags; automatic flag calculation is not implemented yet. Derived flags are read-only in the editor, and manual edits preserve them. Unowned houses can be saved without owners or contacts. Ownership, occupancy flags, and active status are independent.

## Run

The app opens to `MainForm`, a dashboard with sidebar navigation to Dashboard, Households, Billing & payments and Settings. Household management and billing open inside the main window. Returning to Dashboard refreshes house counts, current-month collection and outstanding amounts, all-month overdue amounts and recent payments. All forms have Designer-editable layouts.

## Monthly charges and flexible collection

Billing & payments has two tabs: **Charges & payments** retains the monthly filter, charging dialog and payment recording; **Summary** has an independent year spinner and one row per house (including inactive houses), owners, contacts and January-December columns. House details stay frozen while scrolling horizontally. Month cells show remaining balances, or paid totals for settled charges. Blue means charged but not overdue, red means a remaining balance after its saved deadline, green means fully paid, and grey means no charge. Partial payments retain the outstanding status until fully settled. Tooltips show charged, paid, remaining and due-date details. The summary refreshes when opened, when its year changes, or when Refresh is clicked.

Open **Billing & payments** and choose the year and months to display. The **All** checkbox selects/deselects all months and shows an indeterminate state for a partial selection. With no months selected, no charge records are shown. The current month is selected on opening.

Click **Charge houses...** to open a separate dialog. Its year and month checkboxes are independent of the table filters, with their own **All** checkbox. Select houses individually from the active-house listing or use **Select all houses**. **Only show owned houses** filters the listing; hidden unowned selections are cleared, and Select all applies only to visible houses. The selected count is shown below the list. At least one month and one house must be selected. Cancelling creates no charges.

Click **Charge selected months** in the dialog to create one charge per selected month and selected house at the saved monthly rate. Previously charged house/year/month combinations are skipped. Unknown or inactive house selections are rejected. The popup retains the optional shared due-date setting.

By default each month is due on its own 6th. For grouped collection, check **Use one due date** and choose a shared date before generating charges (for example, select January-March with a shared 6 January deadline). Any unpaid balance becomes overdue the following day. Generation never changes existing charges or their deadlines, and later rate changes only affect newly generated charges.

Select a monthly charge and click **Record payment** to enter a full or partial payment, date and reference. Payments reduce that month's balance; overpayments are rejected. Record amounts spanning multiple months separately against each month. No monthly-payer or quarterly-payer categories are needed.

Existing quarterly charges are converted transactionally into three monthly charges on first opening. Their total amounts and original due dates are preserved. Any indivisible sen are assigned to the earlier months. Old payments are allocated oldest month first, preserving payment dates, references and original payment IDs; original quarterly tables remain as an archive. Migration runs once and migrated months cannot be charged again.

Financial status is calculated independently of manually assigned house flags. Dashboard collection means payments allocated to the current month (not all cash received during that month). Overdue balance covers all generated months past their due dates. Claims, payment reversals, credits, automatic house flags and financial reports are not implemented yet.

To delete a flag option, select it in the house editor and click **Delete**. Deletion is immediate and allowed when the flag is unused or its only use is on the currently opened house. The flag is removed from that house and the options together, even if you subsequently cancel other edits. If another house uses it (including inactive houses and derived flags), deletion is blocked until those uses are removed and saved.

Open `HousingCommunityManagement.sln` in Visual Studio with the .NET desktop development workload and .NET 9 SDK. Set `HousingCommunityManagement` as the startup project and press F5. NuGet restores Entity Framework Core SQLite automatically; no SQLite server or manual database setup is needed.

The database is created at `Data/community.db` beside the executable. Run the application from a writable folder. Debug and Release output folders have separate databases. Keep the database with the app when moving it, and back it up with the app closed. Automatic backup and restore are not implemented yet.

## Structure

Drag the houses table's column-header dividers to resize columns. Widths are saved on closing the main window to `Data/house-table.json` beside the database and restored on the next run. Owners and contacts appear one per line, wrap within their columns, and automatically increase row height to display all entries.

- `HousingCommunityManagement`: WinForms screens.
- `HousingCommunityManagement.Core`: household model, validation and repository interface.
- `HousingCommunityManagement.Data`: EF Core SQLite persistence.
- `HousingCommunityManagement.Checks`: integration checks using a temporary database.

Run checks with `dotnet run --project HousingCommunityManagement.Checks`.

The database is created on first use. The original single-owner schema is upgraded transactionally on opening: house IDs, active status, owner and contact details are preserved, and nonempty owners imply owned status. The initial global fee uses the highest original per-house fee (zero for an empty database); review this in Community settings. Original amounts remain in the `LegacyHouseholdFees` table for reference. Existing databases gain the monthly charge and payment tables on opening. Future schema changes need explicit migrations.

All forms keep their controls in `.Designer.cs` files for visual editing in Visual Studio. The checks cover range generation and overlaps, ownership, multiple owners and contacts, flag source persistence, community settings, validation and upgrades from the original database.
