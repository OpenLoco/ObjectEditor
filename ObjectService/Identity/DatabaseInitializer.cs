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

	public static async Task InitializeAsync(WebApplication app)
	{
		using var scope = app.Services.CreateScope();
		var db = scope.ServiceProvider.GetRequiredService<LocoDbContext>();
		var userManager = scope.ServiceProvider.GetRequiredService<UserManager<TblUser>>();
		var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<TblUserRole>>();
		var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseInitializer");

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

		// Ensure the system admin user. The account is resolved once for the whole process by
		// AdminUserProvider: a configured password always wins, and in Development a random throwaway
		// password is generated when none is configured so the Dev Login button works with no setup.
		var adminProvider = scope.ServiceProvider.GetRequiredService<AdminUserProvider>();
		var adminSettings = adminProvider.Settings;
		TblUser? adminUser = null;

		if (adminSettings is null)
		{
			logger.LogError(
				"AdminUser:Password is not configured; the system admin account was NOT bootstrapped. " +
				"Configure AdminUser:Password via user-secrets or environment variables.");
		}
		else
		{
			if (adminProvider.UsesGeneratedPassword)
			{
				logger.LogInformation(
					"No AdminUser:Password configured; bootstrapped the Development system admin {Username} with a " +
					"generated throwaway password. Use the Dev Login button to sign in, or set AdminUser:Password " +
					"(user-secrets) to use a specific password.",
					adminSettings.UserName);
			}

			logger.LogInformation("Ensuring admin user: {Username} / {Email}", adminSettings.UserName, adminSettings.Email);

			// Look up the admin by email first, then by username. The username fallback keeps this
			// idempotent when the stored email differs from the configured one, which would otherwise
			// make CreateAsync below fail with a duplicate-user-name error.
			adminUser = await userManager.FindByEmailAsync(adminSettings.Email)
				?? await userManager.FindByNameAsync(adminSettings.UserName);

			if (adminUser == null)
			{
				adminUser = new TblUser
				{
					UserName = adminSettings.UserName,
					Email = adminSettings.Email,
					EmailConfirmed = true,
				};

				var cr = await userManager.CreateAsync(adminUser, adminSettings.Password);
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

				logger.LogInformation("Created system admin user {Username}", adminSettings.UserName);
			}
			else
			{
				logger.LogInformation("Admin user {Username} already exists (Id={Id})", adminUser.UserName, adminUser.Id);

				// Keep the login username config-driven: the Identity /login endpoint authenticates by
				// username, so the dev quick-login signs in with adminSettings.UserName.
				if (!string.Equals(adminUser.UserName, adminSettings.UserName, StringComparison.Ordinal))
				{
					var renamed = await userManager.SetUserNameAsync(adminUser, adminSettings.UserName);
					if (renamed.Succeeded)
					{
						logger.LogInformation("Set the admin username to the configured value {Username}", adminSettings.UserName);
					}
					else
					{
						logger.LogError(
							"Failed to set the admin username to {Username}: {Errors}",
							adminSettings.UserName, string.Join(", ", renamed.Errors.Select(e => e.Description)));
					}
				}

				// The resolved password is the source of truth for the system admin, so keep the stored
				// hash in sync. This is what lets a freshly configured AdminUser:Password user-secret (or
				// a regenerated Development fallback) actually sign in against an existing database.
				if (!await userManager.CheckPasswordAsync(adminUser, adminSettings.Password))
				{
					var token = await userManager.GeneratePasswordResetTokenAsync(adminUser);
					var reset = await userManager.ResetPasswordAsync(adminUser, token, adminSettings.Password);
					if (reset.Succeeded)
					{
						logger.LogInformation(
							"Reset the stored password for {Username} to match the configured AdminUser:Password",
							adminSettings.UserName);
					}
					else
					{
						logger.LogError(
							"Failed to reset the password for {Username}: {Errors}. Check AdminUser:Password meets the password policy.",
							adminSettings.UserName, string.Join(", ", reset.Errors.Select(e => e.Description)));
					}
				}

				// A locked-out admin cannot sign in even with the right password; clear it.
				if (await userManager.IsLockedOutAsync(adminUser))
				{
					_ = await userManager.SetLockoutEndDateAsync(adminUser, null);
					_ = await userManager.ResetAccessFailedCountAsync(adminUser);
					logger.LogInformation("Cleared lockout for {Username}", adminSettings.UserName);
				}
			}

			// Ensure admin role assignment
			if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
			{
				await userManager.AddToRoleAsync(adminUser, "Admin");
				logger.LogInformation("Assigned Admin role to {Username}", adminSettings.UserName);
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
		}

		logger.LogInformation("Database initialization complete (Admin bootstrapped={Bootstrapped})", adminUser != null);
	}
}
