namespace Definitions.DTO.Identity;

public record DtoRegisterRequest(string Email, string UserName, string Password);
public record DtoLoginRequest(string Email, string Password);

public record DtoInfoResponse(string Email, bool EmailIsConfirmed);

public record DtoRoleEntry(UniqueObjectId Id, string Name) : IHasId;
//public record DtoRoleDescriptor(UniqueObjectId Id, string Name) : DtoWithDbKey(Id), IHasId { }

public record DtoUserEntry(UniqueObjectId Id, string UserName) : IHasId;
//public record DtoUserDescriptor(UniqueObjectId Id, string Username, string Password) : DtoWithDbKey(Id), IHasId { }

/// <summary>Row shape for the user management list view.</summary>
public record DtoUserListEntry(
	UniqueObjectId Id,
	string UserName,
	string Email,
	ICollection<string> Roles) : IHasId;

/// <summary>Full user detail for the user management edit view.</summary>
public record DtoUserDetailDescriptor(
	UniqueObjectId Id,
	string UserName,
	string Email,
	bool EmailConfirmed,
	bool IsLockedOut,
	ICollection<string> Roles,
	ICollection<string> PermissionClaims,
	UniqueObjectId? AssociatedAuthorId,
	string? AssociatedAuthorName) : IHasId;

/// <summary>Request body for toggling a user's membership of a role.</summary>
public record DtoUserRoleRequest(string Role);

/// <summary>Request body for toggling a user's permission claim.</summary>
public record DtoUserClaimRequest(string Claim);

/// <summary>Request body for setting a user's display name (username).</summary>
public record DtoUserDisplayNameRequest(string DisplayName);

/// <summary>Response body containing a generated password-reset token.</summary>
public record DtoPasswordResetTokenResponse(string Token);
