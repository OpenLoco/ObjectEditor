namespace ObjectService.Identity;

/// <summary>
/// Well-known permission values stored as claims of type "permission"
/// on ASP.NET Identity roles.  Admin users automatically have every
/// permission without needing an explicit claim.
/// </summary>
public static class LocoPermissions
{
	/// <summary>Claim type used for permission claims.</summary>
	public const string ClaimType = "permission";

	/// <summary>Allowed to create object packs (and scenario packs).</summary>
	public const string ObjectPacksCreate = "objectpacks:create";

	/// <summary>Allowed to modify/delete object packs.</summary>
	public const string ObjectPacksModify = "objectpacks:modify";

	/// <summary>Allowed to modify/delete scenario packs.</summary>
	public const string SC5FilePacksModify = "sc5filepacks:modify";

	/// <summary>Allowed to add/remove tags on any entity.</summary>
	public const string TagsManage = "tags:manage";

	/// <summary>Allowed to set the licence on any entity.</summary>
	public const string LicenceManage = "licence:manage";

	/// <summary>Allowed to set the author on any entity.</summary>
	public const string AuthorManage = "author:manage";

	/// <summary>Allowed to change own display name.</summary>
	public const string DisplayNameChange = "displayname:change";
}