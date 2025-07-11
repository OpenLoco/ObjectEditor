using Microsoft.AspNetCore.Authorization;

namespace Definitions.Database;

public class AdminPolicy
{
	public const string Name = "modification-requires-admin";

	public static void Build(AuthorizationPolicyBuilder policyBuilder)
		=> _ = policyBuilder.RequireRole("Admin");
}
