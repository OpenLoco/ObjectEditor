using Definitions.Database;
using Microsoft.AspNetCore.Authorization;
using Definitions.ObjectModels.Types;
using Microsoft.EntityFrameworkCore;

namespace ObjectService.Identity;

/// <summary>
/// Requirement that the authenticated user must own the target object (or be an admin).
/// </summary>
public class ObjectOwnershipRequirement : IAuthorizationRequirement
{
}

/// <summary>
/// Handles <see cref="ObjectOwnershipRequirement"/> by checking if the current user owns the object
/// identified by the {id} route value, is linked to an author the object credits, or is in the
/// "Admin" role. Vanilla game objects (LocomotionSteam / LocomotionGoG) are never editable by anyone.
/// </summary>
public class ObjectOwnershipHandler : AuthorizationHandler<ObjectOwnershipRequirement>
{
	private readonly LocoDbContext _db;

	public ObjectOwnershipHandler(LocoDbContext db)
	{
		_db = db;
	}

	protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ObjectOwnershipRequirement requirement)
	{
		if (context.User.IsInRole("Admin"))
		{
			context.Succeed(requirement);
			return;
		}

		if (context.Resource is HttpContext httpContext)
		{
			var routeId = httpContext.Request.RouteValues["id"]?.ToString();
			if (string.IsNullOrEmpty(routeId) || !ulong.TryParse(routeId, out var objectId))
			{
				// Cannot determine the object — fall through to default (deny).
				return;
			}

			var userIdClaim = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(userIdClaim) || !ulong.TryParse(userIdClaim, out var userId))
			{
				return;
			}

			// Check if this object is owned by the current user, or credits an author the user is linked to
			// (an ASP.NET user may be associated with one author, which grants edit rights over every
			// object crediting that author).
			var obj = await _db.Objects
				.AsNoTracking()
				.Where(x => x.Id == objectId)
				.Select(x => new
				{
					x.OwnerUserId,
					x.ObjectSource,
					AuthorIds = x.Authors.Select(a => a.Id).ToList(),
				})
				.SingleOrDefaultAsync();

			if (obj == null)
			{
				return;
			}

			// Vanilla game objects (original Locomotion assets) can not be edited
			if (obj.ObjectSource is ObjectSource.LocomotionSteam or ObjectSource.LocomotionGoG)
			{
				return;
			}

			if (obj.OwnerUserId == userId)
			{
				context.Succeed(requirement);
				return;
			}

			if (obj.AuthorIds.Count == 0)
			{
				return;
			}

			var associatedAuthorId = await _db.Users
				.AsNoTracking()
				.Where(u => u.Id == userId)
				.Select(u => u.AssociatedAuthorId)
				.SingleOrDefaultAsync();

			if (associatedAuthorId.HasValue && obj.AuthorIds.Contains(associatedAuthorId.Value))
			{
				context.Succeed(requirement);
			}
		}
	}
}