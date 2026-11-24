namespace Definitions.DTO;

/// <summary>
/// Describes the current capabilities of the object service so that clients (such as the
/// Object Editor) can avoid attempting operations the server will refuse.
/// </summary>
/// <param name="FrontendReadOnly">
/// When true the HTML frontend is in read-only mode; logins and account features are disabled.
/// </param>
/// <param name="BackendReadOnly">
/// When true the API does not map any write (POST/PUT/DELETE) routes.
/// </param>
public record DtoServerStatus(
	bool FrontendReadOnly,
	bool BackendReadOnly)
{
	/// <summary>
	/// True when the server accepts no writes. A frontend read-only server also hides the login
	/// flow the editor uses to authenticate, so both flags are treated as read-only for uploads.
	/// </summary>
	public bool IsReadOnly => FrontendReadOnly || BackendReadOnly;
}
