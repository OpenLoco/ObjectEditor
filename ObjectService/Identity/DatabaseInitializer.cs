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
	public const string DefaultAdminUsername = "LeftofZen";
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

		// Bring the schema up to date. Databases created before migrations were adopted have no
		// __EFMigrationsHistory table, so the baseline migration is recorded as applied first; from then on
		// EF migrations own the schema and every future model change is delivered as a migration.
		await MigrationInitializer.EnsureBaselineHistoryAsync(db, logger);
		await db.Database.MigrateAsync();

		// Ensure Admin role
		if (!await roleManager.RoleExistsAsync("Admin"))
		{
			var rr = await roleManager.CreateAsync(new TblUserRole { Name = "Admin" });
			logger.LogInformation(rr.Succeeded
				? "Created Admin role"
				: "Failed to create Admin role: {Errors}", string.Join(", ", rr.Errors.Select(e => e.Description)));
		}
		// Ensure Curator role with permission claims
		var curatorRole = await roleManager.FindByNameAsync("Curator");
		if (curatorRole == null)
		{
			curatorRole = new TblUserRole { Name = "Curator" };
			var cr = await roleManager.CreateAsync(curatorRole);
			if (cr.Succeeded)
			{
				logger.LogInformation("Created Curator role");

				// Assign curator permissions as role claims
				foreach (var perm in LocoPermissions.Curator)
				{
					_ = await roleManager.AddClaimAsync(curatorRole, new System.Security.Claims.Claim(LocoPermissions.ClaimType, perm));
				}

				logger.LogInformation("Assigned curator permissions to Curator role");
			}
			else
			{
				logger.LogError("Failed to create Curator role: {Errors}", string.Join(", ", cr.Errors.Select(e => e.Description)));
			}
		}
		else
		{
			logger.LogInformation("Curator role already exists (Id={Id})", curatorRole.Id);

			// Ensure curator claims exist (idempotent)
			var existingClaims = await roleManager.GetClaimsAsync(curatorRole);
			var existingPermissionValues = existingClaims.Where(c => c.Type == LocoPermissions.ClaimType).Select(c => c.Value).ToHashSet();

			foreach (var perm in LocoPermissions.Curator)
			{
				if (!existingPermissionValues.Contains(perm))
				{
					var result = await roleManager.AddClaimAsync(curatorRole, new System.Security.Claims.Claim(LocoPermissions.ClaimType, perm));
					if (!result.Succeeded)
					{
						logger.LogWarning("Could not add claim {Permission} to Curator: {Errors}", perm, string.Join(", ", result.Errors.Select(e => e.Description)));
					}
					else
					{
						logger.LogInformation("Added claim {Permission} to Curator role", perm);
					}
				}
			}
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
			{
				obj.OwnerUserId = adminUser.Id;
			}

			await db.SaveChangesAsync();
			logger.LogInformation("Assigned {Count} unowned objects to admin", unowned.Count);
		}

		logger.LogInformation("Database initialization complete (Admin exists={Exists})", adminUser != null);
	}
}
