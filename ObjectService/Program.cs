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
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"] ?? throw new InvalidOperationException("JWT Key not configured"))),
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

app.UseForwardedHeaders();

app.UseHttpLogging();
app.UseRateLimiter();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

// ASP.NET Identity's built-in endpoints, mounted under /v2/identity so the whole API is versioned.
var identityEndpoints = app.MapGroup($"{Routes.Prefix}{Routes.Identity}");
_ = identityEndpoints.MapIdentityApi<TblUser>();

_ = app
	.MapHealthChecks("/health")
	.RequireRateLimiting(tokenPolicy);

_ = app.MapRazorPages();

// Development-only bootstrap endpoint used by the quick-login page. It ensures the
// local dev admin exists and has the Admin role, then the page signs in via the
// standard Identity /login endpoint.
if (app.Environment.IsDevelopment())
{
	_ = app.MapPost("/dev/quick-login", async (UserManager<TblUser> userManager, RoleManager<TblUserRole> roleManager, IConfiguration config) =>
	{
		// Dev credentials are configuration-driven (see appsettings.Development.json); there is no code
		// default so a deployment can never accidentally ship working dev credentials.
		var devUserEmail = config["DevAuth:Email"];
		var devPassword = config["DevAuth:Password"];
		if (string.IsNullOrWhiteSpace(devUserEmail) || string.IsNullOrWhiteSpace(devPassword))
		{
			return Results.Problem(
				"Dev quick-login is not configured. Set DevAuth:Email and DevAuth:Password (development only).",
				statusCode: StatusCodes.Status503ServiceUnavailable);
		}

		var devUserName = config["DevAuth:UserName"] ?? devUserEmail;

		var user = await userManager.FindByEmailAsync(devUserEmail);
		if (user == null)
		{
			user = new TblUser
			{
				UserName = devUserName,
				Email = devUserEmail,
				EmailConfirmed = true,
			};

			var createResult = await userManager.CreateAsync(user, devPassword);
			if (!createResult.Succeeded)
			{
				return Results.Problem(string.Join("; ", createResult.Errors.Select(e => e.Description)), statusCode: StatusCodes.Status400BadRequest);
			}
		}

		if (!await roleManager.RoleExistsAsync("Admin"))
		{
			_ = await roleManager.CreateAsync(new TblUserRole { Name = "Admin" });
		}

		if (!await userManager.IsInRoleAsync(user, "Admin"))
		{
			_ = await userManager.AddToRoleAsync(user, "Admin");
		}

		return Results.Ok();
	}).AllowAnonymous();
}

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
