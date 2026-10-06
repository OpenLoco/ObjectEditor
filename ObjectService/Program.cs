using Definitions.Database;
using Definitions.ObjectModels.Graphics;
using Definitions.Web;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using ObjectService;
using ObjectService.Frontend;
using ObjectService.Identity;
using ObjectService.Services;
using ObjectService.RouteHandlers;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var connectionString = builder.Configuration.GetConnectionString("SQLiteConnection");

builder.Services.AddObjectServiceOpenApi();
builder.Services.AddHealthChecks()
	.AddCheck<ObjectServiceHealthCheck>("object-service");
builder.Services.AddProblemDetails();
builder.Services.AddRazorPages();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDbContext<LocoDbContext>(options =>
{
	_ = options.UseSqlite(connectionString);
	if (builder.Environment.IsDevelopment())
	{
		// EnableSensitiveDataLogging exposes parameter values in logs, which can
		// leak PII / secrets. Restrict to the Development environment.
		_ = options.EnableDetailedErrors();
		_ = options.EnableSensitiveDataLogging();
	}
});

builder.Services.AddScoped<FrontendApiClient>(); builder.Services.AddScoped<ObjectExplorerService>(); builder.Services.AddObjectEditorServices();

// Rendered object images are content-addressed by the source DAT's xxHash3 and cached in two tiers: an
// in-process memory cache (L1, bounded by a byte size limit) in front of a disk cache under
// GameData/Cache/images (L2, survives restarts). The image cache itself is registered by
// AddGameDataFolderServices; here we only configure the memory tier's byte limit (default 256 MB).
var imageCacheBytes = builder.Configuration.GetValue<long?>("ObjectService:ImageCache:MemoryCacheSizeBytes") ?? 256L * 1024 * 1024;
builder.Services.AddMemoryCache(options => options.SizeLimit = imageCacheBytes);

// Output caching for the small, immutable image responses. Combined with the long-lived ETag/Cache-Control
// headers the route handlers set, this means an object image is decoded at most once per expiry window
// regardless of how many clients ask for it. Bounded so it can never grow without limit.
builder.Services.AddOutputCache(options =>
{
	options.SizeLimit = 64L * 1024 * 1024;
	options.AddPolicy("ObjectImages", policy => policy
		.Expire(TimeSpan.FromHours(24))
		.SetVaryByRouteValue("id", "imageId"));
});

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// this breaks the client side, even if the same converter is added...
//builder.Services.Configure<JsonOptions>(options => options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

var objRoot = builder.Configuration["ObjectService:RootFolder"];
var paletteMapFile = builder.Configuration["ObjectService:PaletteMapFile"];
ArgumentNullException.ThrowIfNull(objRoot);
ArgumentNullException.ThrowIfNull(paletteMapFile);

var serverFolderManager = await ServerFolderManager.CreateAsync(objRoot);
var paletteMap = new PaletteMap(paletteMapFile);

builder.Services.AddSingleton(serverFolderManager);
builder.Services.AddSingleton(paletteMap);

// The system admin identity (email/username/password) is resolved once for the whole process so the
// startup bootstrap (DatabaseInitializer) and the development-only quick-login always agree on the
// same account.
builder.Services.AddSingleton<AdminUserProvider>();

// The GameData folder services are shared by the startup synchronisation and the file watchers, and
// are always registered so the files on disk and the database are reconciled whenever the server
// starts - even when the file watcher is turned off.
builder.Services.AddGameDataFolderServices();

// Watches the whole GameData folder tree for files dropped in at runtime. A single service owns
// one watcher per category folder; DAT files are indexed into objectIndex.json and the database
// and scenarios are added to the database so changes appear on the live service without a restart.
if (builder.Configuration.GetValue("ObjectService:EnableFileWatcher", true))
{
	builder.Services.AddGameDataFileWatchers();
}

//var server = new Server(new ServerSettings(objRoot, paletteMapFile));
//builder.Services.AddSingleton(server);

builder.Services.AddHttpLogging(logging =>
{
	// these are marked [redacted] in the logs unless specified here
	_ = logging.RequestHeaders.Add("Cdn-Loop");
	_ = logging.RequestHeaders.Add("Cf-Connecting-Ip");
	_ = logging.RequestHeaders.Add("Cf-Ipcountry");
	_ = logging.RequestHeaders.Add("Cf-Ray");
	_ = logging.RequestHeaders.Add("Cf-Visitor");
	_ = logging.RequestHeaders.Add("Cf-Warp-Tag-Id");
	_ = logging.RequestHeaders.Add("X-Forwarded-For");
	_ = logging.RequestHeaders.Add("X-Forwarded-Proto");

	logging.LoggingFields = HttpLoggingFields.All;
	//logging.LoggingFields = HttpLoggingFields.ResponsePropertiesAndHeaders | HttpLoggingFields.Duration; // this is `All` excluding `ResponseBody`
	logging.CombineLogs = true;
});

const string tokenPolicy = ServiceRegistrationExtensions.RateLimitPolicyName;

builder.Services.AddObjectServiceForwardedHeaders();
builder.Services.AddObjectServiceRateLimiting(builder.Configuration);

builder.Services.AddObjectServiceIdentity(builder.Configuration, builder.Environment);

// Used for the Identity stuff to send emails to users
// disabling this line effectively disables all email sending, as a default NoOpEmailSender is used in place
// builder.Services.AddTransient<IEmailSender, EmailSender>();

var app = builder.Build();

// Make the active environment obvious at startup: appsettings.Development.json and user-secrets only
// load when this is "Development".
app.Logger.LogInformation(
	"Object Service starting in the '{Environment}' environment (content root: {ContentRoot})",
	app.Environment.EnvironmentName, app.Environment.ContentRootPath);

// Turn any unhandled exception into an RFC-7807 ProblemDetails response (AddProblemDetails is
// registered above) instead of a bare 500. Placed first so it wraps the whole pipeline.
app.UseExceptionHandler();

app.UseForwardedHeaders();

app.UseHttpLogging();
app.UseRateLimiter();
app.UseStaticFiles();
app.UseOutputCache();

app.UseAuthentication();
app.UseAuthorization();

// ASP.NET Identity's built-in endpoints, mounted under /v2/identity so the whole API is versioned.
// Tagged explicitly so the API reference groups them under "Identity" rather than a generated
// fallback name.
var identityEndpoints = app.MapGroup($"{Routes.Prefix}{Routes.Identity}").WithTags("Identity");
_ = identityEndpoints.MapLocoIdentityApi<TblUser>();
// Self-service account routes (display name, delete own account) live next to Identity's own manage
// endpoints rather than under the admin-only /v2/users record area.
IdentityManageRouteHandler.MapRoutes(identityEndpoints);

_ = app
	.MapHealthChecks("/health")
	.RequireRateLimiting(tokenPolicy);

_ = app.MapRazorPages();

_ = app.MapApiRoutes()
	.RequireRateLimiting(tokenPolicy);

var showScalar = builder.Configuration.GetValue<bool?>("ObjectService:ShowScalar");
ArgumentNullException.ThrowIfNull(showScalar);

_ = app.MapOpenApi();

if (showScalar == true)
{
	_ = app.MapScalarApiReference("/api", options =>
	{
		_ = options
			.WithTitle("OpenLoco Object Service")
			.WithTheme(ScalarTheme.Solarized)
			.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
			.AddPreferredSecuritySchemes("Bearer");
	});
}

// Run database initialization (creates admin user, assigns ownership, etc.)
await DatabaseInitializer.InitializeAsync(app);

app.Run();

#pragma warning disable CA1050 // Declare types in namespaces

// this is to enable unit testing in a top-level statement program
public partial class Program;

#pragma warning restore CA1050 // Declare types in namespaces