using Definitions.Database;
using Definitions.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace ObjectService.Identity;

/// <summary>
/// Development-only authentication scheme used when <c>ObjectService:DisableAuthentication</c> is enabled.
/// API requests (<c>/v2</c>) are authenticated as a local admin, while UI pages are left untouched so the
/// normal login/logout flow still works.
/// <para>
/// Identity-management endpoints (<c>/v2/users</c> and <c>/v2/roles</c>) are deliberately excluded so that
/// anonymous requests are still challenged (and thus the real authentication/authorization pipeline is
/// exercised for those routes).
/// </para>
/// </summary>
public class DevAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
	public const string SchemeName = "DevAuthentication";

	public DevAuthenticationHandler(
		IOptionsMonitor<AuthenticationSchemeOptions> options,
		ILoggerFactory logger,
		UrlEncoder encoder)
		: base(options, logger, encoder)
	{ }

	protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
	{
		var path = Request.Path;
		if (!path.StartsWithSegments(Routes.Prefix, StringComparison.OrdinalIgnoreCase)
			|| path.StartsWithSegments($"{Routes.Prefix}{Routes.Users}", StringComparison.OrdinalIgnoreCase)
			|| path.StartsWithSegments($"{Routes.Prefix}{Routes.Roles}", StringComparison.OrdinalIgnoreCase))
		{
			return AuthenticateResult.NoResult();
		}

		// Impersonate the real system admin user (created by DatabaseInitializer) rather than a synthetic
		// id, so that write operations which record owner ids satisfy their foreign keys.
		var adminUserId = await GetAdminUserIdAsync();
		var claims = new[]
		{
			new Claim(ClaimTypes.NameIdentifier, adminUserId.ToString()),
			new Claim(ClaimTypes.Name, "devadmin"),
			new Claim(ClaimTypes.Role, "Admin"),
		};
		var identity = new ClaimsIdentity(claims, SchemeName, ClaimTypes.Name, ClaimTypes.Role);
		var principal = new ClaimsPrincipal(identity);

		return AuthenticateResult.Success(new AuthenticationTicket(principal, SchemeName));
	}

	private async Task<ulong> GetAdminUserIdAsync()
	{
		var db = Context.RequestServices.GetService<LocoDbContext>();
		if (db is null)
		{
			return 0;
		}

		var admin = await db.Users
			.Where(u => u.UserName == DatabaseInitializer.DefaultAdminUsername)
			.Select(u => (ulong?)u.Id)
			.FirstOrDefaultAsync();

		// Fall back to any existing user (e.g. when the admin username has been customised).
		admin ??= await db.Users.Select(u => (ulong?)u.Id).FirstOrDefaultAsync();

		return admin ?? 0;
	}
}
