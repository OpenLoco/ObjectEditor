using Microsoft.AspNetCore.Authorization;

namespace ObjectService.Identity;

/// <summary>
/// Requires the authenticated user to hold a specific permission claim
/// (or be a member of the Admin role).
/// </summary>
public class PermissionRequirement(string permission) : IAuthorizationRequirement
{
	public string Permission { get; } = permission;
}