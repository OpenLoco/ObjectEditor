using Microsoft.AspNetCore.Authorization;

namespace ObjectService.Identity;

/// <summary>
/// Checks <see cref="PermissionRequirement"/> against the current user.
/// Admin users automatically satisfy every requirement; other users must
/// hold a role claim of type <c>"permission"</c> with the required value.
/// </summary>
public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
	protected override Task HandleRequirementAsync(
		AuthorizationHandlerContext context,
		PermissionRequirement requirement)
	{
		if (context.User.IsInRole("Admin"))
		{
			context.Succeed(requirement);
		}
		else if (context.User.HasClaim(LocoPermissions.ClaimType, requirement.Permission))
		{
			context.Succeed(requirement);
		}

		return Task.CompletedTask;
	}
}