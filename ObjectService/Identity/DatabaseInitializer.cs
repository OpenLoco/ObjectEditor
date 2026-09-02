using Definitions.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ObjectService.Identity;

/// <summary>
/// Runs on application startup to ensure the database schema is migrated and
/// seed data (system admin user, ownership of legacy objects) is in place.
/// </summary>
public static class DatabaseInitializer
{
	private const string DefaultAdminEmail = "leftofzen@openloco.io";
	private const string DefaultAdminUsername = "LeftofZen";
	private const string DefaultAdminPassword = "3!D:Gy681%&y(HCg";

	public static async Task InitializeAsync(WebApplication app)
	{
		using var scope = app.Services.CreateScope();
		var db = scope.ServiceProvider.GetRequiredService<LocoDbContext>();
		var userManager = scope.ServiceProvider.GetRequiredService<UserManager<TblUser>>();
		var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<TblUserRole>>();
		var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
		var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseInitializer");

		// Recreate DB from scratch when the dev flag is on
		var deleteDbOnStartup = config.GetValue<bool?>("ObjectService:DeleteDatabaseOnStartup") ?? false;
		if (deleteDbOnStartup)
		{
			logger.LogWarning("ObjectService:DeleteDatabaseOnStartup is true — dropping and recreating database");
			await db.Database.EnsureDeletedAsync();
		}

		// Ensure the database file and core schema exist
		await db.Database.EnsureCreatedAsync();

		// Add OwnerUserId column to existing databases that were created before
		// DbCoreObject gained the OwnerUserId property. We omit the REFERENCES
		// clause because SQLite ALTER TABLE ADD COLUMN has limited FK support;
		// EF Core tracks the FK at the model level instead.
		foreach (var table in new[] { "Objects", "ObjectPacks", "SC5Files", "SC5FilePacks" })
		{
			try
			{
#pragma warning disable EF1002 // table names are from a hard-coded array, no injection risk
				await db.Database.ExecuteSqlRawAsync(
					$"ALTER TABLE \"{table}\" ADD COLUMN \"OwnerUserId\" INTEGER NULL");
#pragma warning restore EF1002
				logger.LogInformation("Added OwnerUserId column to {Table} table", table);
			}
			catch (Exception ex)
			{
				logger.LogWarning(ex, "Could not add OwnerUserId column to {Table} table (may already exist)", table);
			}
		}

		// Ensure Admin role
		if (!await roleManager.RoleExistsAsync("Admin"))
		{
			var rr = await roleManager.CreateAsync(new TblUserRole { Name = "Admin" });
			logger.LogInformation(rr.Succeeded
				? "Created Admin role"
				: "Failed to create Admin role: {Errors}", string.Join(", ", rr.Errors.Select(e => e.Description)));
		}

		// Ensure system admin user
		var adminEmail = config["AdminUser:Email"] ?? DefaultAdminEmail;
		var adminUsername = config["AdminUser:Username"] ?? DefaultAdminUsername;
		var adminPassword = config["AdminUser:Password"] ?? DefaultAdminPassword;

		logger.LogInformation("Ensuring admin user: {Username} / {Email}", adminUsername, adminEmail);

		var adminUser = await userManager.FindByEmailAsync(adminEmail);
		if (adminUser == null)
		{
			adminUser = new TblUser
			{
				UserName = adminUsername,
				Email = adminEmail,
				EmailConfirmed = true,
			};

			var cr = await userManager.CreateAsync(adminUser, adminPassword);
			if (!cr.Succeeded)
			{
				logger.LogError("Failed to create admin user: {Errors}", string.Join(", ", cr.Errors.Select(e => e.Description)));
				logger.LogError("Password rules — Digit:{RD} Lower:{RL} Upper:{RU} NonAlpha:{RNA} MinLen:{MinLen}",
					userManager.Options.Password.RequireDigit,
					userManager.Options.Password.RequireLowercase,
					userManager.Options.Password.RequireUppercase,
					userManager.Options.Password.RequireNonAlphanumeric,
					userManager.Options.Password.RequiredLength);
				return; // let app start; admin features won't work
			}

			logger.LogInformation("Created system admin user {Username}", adminUsername);
		}
		else
		{
			logger.LogInformation("Admin user {Username} already exists (Id={Id})", adminUsername, adminUser.Id);
		}

		// Ensure admin role assignment
		if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
		{
			await userManager.AddToRoleAsync(adminUser, "Admin");
			logger.LogInformation("Assigned Admin role to {Username}", adminUsername);
		}

		// Assign unowned objects to admin
		var unowned = await db.Objects.Where(o => o.OwnerUserId == null).ToListAsync();
		if (unowned.Count > 0)
		{
			foreach (var obj in unowned)
				obj.OwnerUserId = adminUser.Id;
			await db.SaveChangesAsync();
			logger.LogInformation("Assigned {Count} unowned objects to admin", unowned.Count);
		}

		logger.LogInformation("Database initialization complete (Admin exists={Exists})", adminUser != null);
	}
}