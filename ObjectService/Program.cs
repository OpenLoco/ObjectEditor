using Definitions.Database;
using Definitions.ObjectModels.Graphics;
using Definitions.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using ObjectService;
using ObjectService.Frontend;
using ObjectService.Identity;
using ObjectService.Services;
using ObjectService.RouteHandlers;
using Scalar.AspNetCore;
using System.Security.Cryptography;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var connectionString = builder.Configuration.GetConnectionString("SQLiteConnection");

builder.Services.AddOpenApi(options =>
{
	_ = options.AddDocumentTransformer((document, context, cancellationToken) =>
	{
		document.Info.Title = "OpenLoco Object Service";
		document.Info.Version = "2.0";
		document.Info.Contact = new OpenApiContact
		{
			Name = "Left of Zen",
			Email = "leftofzen@openloco.io"
		};

		document.Servers?.Clear();
		document.Servers?.Add(new OpenApiServer() { Url = "https://openloco.leftofzen.dev" });

		return Task.CompletedTask;
	});
});

// (options => _ = options.AddDocumentTransformer<BearerSecuritySchemeTransformer>());
builder.Services.AddEndpointsApiExplorer();
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

var serverFolderManager = new ServerFolderManager(objRoot);
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

const string tokenPolicy = "token";

var rateLimiterSection = builder.Configuration.GetSection("ObjectService:RateLimiter");
ArgumentNullException.ThrowIfNull(rateLimiterSection);
var rateLimiter = new RateLimitOptions();
rateLimiterSection.Bind(rateLimiter);

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
	options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

builder.Services.AddRateLimiter(rlOptions => rlOptions
	.AddTokenBucketLimiter(policyName: tokenPolicy, options =>
	{
		options.TokenLimit = rateLimiter.TokenLimit;
		options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
		options.QueueLimit = rateLimiter.QueueLimit;
		options.ReplenishmentPeriod = TimeSpan.FromSeconds(rateLimiter.ReplenishmentPeriod);
		options.TokensPerPeriod = rateLimiter.TokensReplenishedPerPeriod;
		options.AutoReplenishment = rateLimiter.AutoReplenishment;
		rlOptions.OnRejected = (context, cancellationToken) =>
		{
			if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
			{
				context.HttpContext.Response.Headers.RetryAfter = retryAfter.TotalSeconds.ToString();
			}

			context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
			_ = context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", cancellationToken);

			return new ValueTask();
		};
	}));

builder.Services
	.AddIdentityApiEndpoints<TblUser>()
	.AddRoles<TblUserRole>()
	.AddEntityFrameworkStores<LocoDbContext>();

// Relax default password rules for development.
// Override via appsettings or user secrets in production.
builder.Services.Configure<IdentityOptions>(options =>
{
	options.Password.RequireDigit = true;
	options.Password.RequireLowercase = true;
	options.Password.RequireUppercase = true;
	options.Password.RequireNonAlphanumeric = true;
	options.Password.RequiredLength = 12;
});

// Configure bearer token expiration from settings
builder.Services.Configure<BearerTokenOptions>(IdentityConstants.BearerScheme, options =>
{
	var durationInMinutes = builder.Configuration.GetValue<int?>("JwtSettings:DurationInMinutes") ?? 60;
	options.BearerTokenExpiration = TimeSpan.FromMinutes(durationInMinutes);
});

// Dev-mode authentication bypass: when enabled, API requests are authenticated as a local admin.
// Set "ObjectService:DisableAuthentication": true in appsettings.Development.json.
// Outside Development the bypass is never honoured, so a production deployment cannot disable
// authentication even if the setting leaks into configuration.
var disableAuthRequested = builder.Configuration.GetValue<bool?>("ObjectService:DisableAuthentication") ?? false;
var disableAuth = disableAuthRequested && builder.Environment.IsDevelopment();
if (disableAuthRequested && !builder.Environment.IsDevelopment())
{
	Console.Error.WriteLine("ObjectService:DisableAuthentication is set but the environment is not Development; ignoring it. Authentication cannot be disabled outside Development.");
}

// The schemes used by the API authorization policies. The dev scheme is added last so that real
// credentials (cookies / bearer tokens) still take precedence when they are supplied.
var apiAuthenticationSchemes = new List<string>
{
	IdentityConstants.ApplicationScheme,
	IdentityConstants.BearerScheme,
	JwtBearerDefaults.AuthenticationScheme,
};

if (disableAuth)
{
	apiAuthenticationSchemes.Add(DevAuthenticationHandler.SchemeName);
}

// Secrets are never committed to appsettings*.json. JwtSettings:Key must be supplied via user-secrets
// (development) or an environment variable (deployment). In Development a random key is generated so
// the app still starts with no setup; it changes on restart, which is harmless because no issued token
// depends on it (clients authenticate with Identity bearer tokens, not JWTs).
var jwtKey = builder.Configuration["JwtSettings:Key"];
if (string.IsNullOrWhiteSpace(jwtKey))
{
	if (!builder.Environment.IsDevelopment())
	{
		throw new InvalidOperationException("JwtSettings:Key is not configured. Provide it via user-secrets or an environment variable.");
	}

	jwtKey = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
}

var authenticationBuilder = builder.Services.AddAuthentication()
.AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
		ValidAudience = builder.Configuration["JwtSettings:Audience"],
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
	};
});

if (disableAuth)
{
	_ = authenticationBuilder.AddScheme<AuthenticationSchemeOptions, DevAuthenticationHandler>(DevAuthenticationHandler.SchemeName, null);
}

builder.Services.AddAuthorization(options =>
{
	// Configure the default policy to accept Identity cookies, Identity Bearer tokens, and JWT tokens.
	// This allows both page-based cookie auth (from SignInManager) and API bearer token auth.
	options.DefaultPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
		.AddAuthenticationSchemes([.. apiAuthenticationSchemes])
		.RequireAuthenticatedUser()
		.Build();

	// Policy: user must own the object (id from route) or be an Admin
	options.AddPolicy("CanEditObject", policy =>
		policy.AddAuthenticationSchemes([.. apiAuthenticationSchemes])
			.RequireAuthenticatedUser()
			.AddRequirements(new ObjectOwnershipRequirement()));

	// Admin-only policy (for user/role management). The dev bypass scheme is intentionally excluded so
	// identity management always requires a real admin login.
	options.AddPolicy("AdminOnly", policy =>
		policy.AddAuthenticationSchemes(IdentityConstants.ApplicationScheme, IdentityConstants.BearerScheme, JwtBearerDefaults.AuthenticationScheme)
			.RequireRole("Admin"));
	// Curator policy – any user with at least one curator permission (or Admin)
	options.AddPolicy("Curator", policy =>
		policy.AddAuthenticationSchemes([.. apiAuthenticationSchemes])
			.RequireAuthenticatedUser()
			.RequireAssertion(context =>
				context.User.IsInRole("Admin") ||
				context.User.HasClaim(LocoPermissions.ClaimType, LocoPermissions.TagsManage) ||
				context.User.HasClaim(LocoPermissions.ClaimType, LocoPermissions.LicenceManage) ||
				context.User.HasClaim(LocoPermissions.ClaimType, LocoPermissions.AuthorManage)));

	// Individual permission policies for fine-grained control when needed
	options.AddPolicy("CanManageTags", policy =>
		policy.AddAuthenticationSchemes(IdentityConstants.ApplicationScheme, IdentityConstants.BearerScheme, JwtBearerDefaults.AuthenticationScheme)
			.RequireAuthenticatedUser()
			.AddRequirements(new PermissionRequirement(LocoPermissions.TagsManage)));

	options.AddPolicy("CanManageLicences", policy =>
		policy.AddAuthenticationSchemes(IdentityConstants.ApplicationScheme, IdentityConstants.BearerScheme, JwtBearerDefaults.AuthenticationScheme)
			.RequireAuthenticatedUser()
			.AddRequirements(new PermissionRequirement(LocoPermissions.LicenceManage)));

	options.AddPolicy("CanManageAuthors", policy =>
		policy.AddAuthenticationSchemes(IdentityConstants.ApplicationScheme, IdentityConstants.BearerScheme, JwtBearerDefaults.AuthenticationScheme)
			.RequireAuthenticatedUser()
			.AddRequirements(new PermissionRequirement(LocoPermissions.AuthorManage)));

	// Pack management policies. Pack writes require an explicit permission claim; Admin users satisfy
	// every permission implicitly via PermissionHandler.
	options.AddPolicy("CanCreateObjectPacks", policy =>
		policy.AddAuthenticationSchemes([.. apiAuthenticationSchemes])
			.RequireAuthenticatedUser()
			.AddRequirements(new PermissionRequirement(LocoPermissions.ObjectPacksCreate)));

	options.AddPolicy("CanModifyObjectPacks", policy =>
		policy.AddAuthenticationSchemes([.. apiAuthenticationSchemes])
			.RequireAuthenticatedUser()
			.AddRequirements(new PermissionRequirement(LocoPermissions.ObjectPacksModify)));

	options.AddPolicy("CanModifyScenarioPacks", policy =>
		policy.AddAuthenticationSchemes([.. apiAuthenticationSchemes])
			.RequireAuthenticatedUser()
			.AddRequirements(new PermissionRequirement(LocoPermissions.ScenarioPacksModify)));
});

// Register the ownership authorization handler
builder.Services.AddScoped<IAuthorizationHandler, ObjectOwnershipHandler>();
// Register the permission authorization handler
builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();

// Used for the Identity stuff to send emails to users
// disabling this line effectively disables all email sending, as a default NoOpEmailSender is used in place
// builder.Services.AddTransient<IEmailSender, EmailSender>();

var app = builder.Build();

// Make the active environment obvious at startup: appsettings.Development.json and user-secrets only
// load when this is "Development".
app.Logger.LogInformation(
	"Object Service starting in the '{Environment}' environment (content root: {ContentRoot})",
	app.Environment.EnvironmentName, app.Environment.ContentRootPath);

app.UseForwardedHeaders();

app.UseHttpLogging();
app.UseRateLimiter();
app.UseStaticFiles();
app.UseOutputCache();

app.UseAuthentication();
app.UseAuthorization();

// ASP.NET Identity's built-in endpoints, mounted under /v2/identity so the whole API is versioned.
var identityEndpoints = app.MapGroup($"{Routes.Prefix}{Routes.Identity}");
_ = identityEndpoints.MapIdentityApi<TblUser>();

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
