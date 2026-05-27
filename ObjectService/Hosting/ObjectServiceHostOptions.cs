namespace ObjectService.Hosting;

// Options consumed by ObjectServiceHost when an embedding application (eg. the GUI)
// wants to spin up the web server in-process rather than relying on appsettings.json.
// Each property maps onto a configuration key that the existing Program/Host code already reads,
// so any value set here is layered onto the builder configuration via an in-memory provider.
public sealed record ObjectServiceHostOptions
{
	// Filesystem root containing the .dat objects to serve.
	public required string RootFolder { get; init; }

	// Path to the SQLite database file backing the Loco object schema (and, for the
	// standalone server, IdentityContext as well).
	public required string DatabaseFile { get; init; }

	// Path to the palette PNG used to colourise rendered object previews.
	public required string PaletteMapFile { get; init; }

	// HTTP listen URL. Defaults to a loopback ephemeral-style port for embedded scenarios.
	public string HttpUrl { get; init; } = "http://127.0.0.1:0";

	// Optional HTTPS listen URL. Leave null to disable the HTTPS endpoint entirely
	// (embedded GUI does not need TLS to itself).
	public string? HttpsUrl { get; init; }

	// Whether to expose the Scalar API reference at /api.
	public bool ShowScalar { get; init; }

	// JWT signing key. Required by the existing auth pipeline; embedded callers
	// should generate a per-session random key.
	public required string JwtKey { get; init; }
	public string JwtIssuer { get; init; } = "ObjectEditor.Local";
	public string JwtAudience { get; init; } = "ObjectEditor.Local";
	public int JwtDurationInMinutes { get; init; } = 60;

	// True (default) for the standalone server: applies migrations for both the Loco
	// schema and the ASP.NET Identity schema, materialises the full set of AspNet*
	// tables, and uses ServerLocoDbContext for the object schema.
	// False for the embedded GUI host: uses ClientLocoDbContext and skips Identity
	// migration entirely, so client databases never get the server-only Identity tables.
	public bool IsServer { get; init; } = true;
}
