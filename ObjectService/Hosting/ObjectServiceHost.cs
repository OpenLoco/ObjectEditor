using Definitions.Database;
using Definitions.ObjectModels;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using ObjectService.Frontend;
using ObjectService.RouteHandlers;
using ObjectService.Services;
using Scalar.AspNetCore;
using System.Text;
using System.Threading.RateLimiting;
namespace ObjectService.Hosting;

// Reusable composition of the OpenLoco Object Service web application.
// Program.cs (standalone server) and the GUI's EmbeddedObjectServiceHost both go through
// this class so behavior stays in sync across the two hosting modes.
public static class ObjectServiceHost
{
	public const string TokenPolicy = "token";

	// Layer the embedded options onto the configuration before any services are registered.
	// This keeps the existing IConfiguration-based wiring untouched.
	public static void ApplyOptionsToConfiguration(WebApplicationBuilder builder, ObjectServiceHostOptions options)
	{
		var overrides = new Dictionary<string, string?>
		{
			["ConnectionStrings:SQLiteConnection"] = $"Data Source={options.DatabaseFile}",
			["ObjectService:RootFolder"] = options.RootFolder,
			["ObjectService:PaletteMapFile"] = options.PaletteMapFile,
			["ObjectService:ShowScalar"] = options.ShowScalar ? "true" : "false",
			["JwtSettings:Key"] = options.JwtKey,
			["JwtSettings:Issuer"] = options.JwtIssuer,
			["JwtSettings:Audience"] = options.JwtAudience,
			["JwtSettings:DurationInMinutes"] = options.JwtDurationInMinutes.ToString(),
		};

		// Kestrel endpoints: replace any inherited config so the embedded host doesn't
		// accidentally bind to the standalone server's ports.
		overrides["Kestrel:Endpoints:Http:Url"] = options.HttpUrl;
		if (!string.IsNullOrWhiteSpace(options.HttpsUrl))
		{
			overrides["Kestrel:Endpoints:HttpsDefaultCert:Url"] = options.HttpsUrl;
		}

		_ = builder.Configuration.AddInMemoryCollection(overrides);
	}

	// Creates the on-disk artifacts the server expects: the objects-root folder tree, the
	// parent directory of the SQLite database file, and ensures the appropriate Loco
	// schema is migrated. For server hosts (options.IsServer = true) the IdentityContext
	// schema is migrated too; client hosts skip Identity so their DBs stay free of the
	// server-only AspNet* tables. Idempotent and safe to call on every startup.
	public static async Task BootstrapAsync(ObjectServiceHostOptions options, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(options);

		ServerFolderManager.EnsureFolderStructure(options.RootFolder);

		var dbDirectory = Path.GetDirectoryName(Path.GetFullPath(options.DatabaseFile));
		if (!string.IsNullOrEmpty(dbDirectory))
		{
			_ = Directory.CreateDirectory(dbDirectory);
		}

		var connectionString = $"Data Source={options.DatabaseFile}";

		if (options.IsServer)
		{
			// Server uses LocoDbContext (which is the concrete type the existing
			// /Definitions/Migrations/Loco snapshot is tied to). ServerLocoDbContext
			// exists as a future hook for server-only schema additions.
			var locoOptions = new DbContextOptionsBuilder<LocoDbContext>()
				.UseSqlite(connectionString)
				.Options;
			await using var locoDb = new LocoDbContext(locoOptions);
			await locoDb.Database.MigrateAsync(cancellationToken);

			var identityOptions = new DbContextOptionsBuilder<IdentityContext>()
				.UseSqlite(connectionString, sql => sql.MigrationsHistoryTable(IdentityContext.MigrationsHistoryTableName))
				.Options;
			await using var identityDb = new IdentityContext(identityOptions);
			await identityDb.Database.MigrateAsync(cancellationToken);
		}
		else
		{
			// Client uses ClientLocoDbContext with EnsureCreated. Client DBs are local
			// caches; there is no migration history table for this context. If the
			// schema evolves the client DB is regenerated rather than migrated.
			var clientLocoOptions = new DbContextOptionsBuilder<ClientLocoDbContext>()
				.UseSqlite(connectionString)
				.Options;
			await using var clientLocoDb = new ClientLocoDbContext(clientLocoOptions);
			_ = await clientLocoDb.Database.EnsureCreatedAsync(cancellationToken);
		}
	}

	public static void ConfigureBuilder(WebApplicationBuilder builder)
	{
		var connectionString = builder.Configuration.GetConnectionString("SQLiteConnection");

		_ = builder.Services.AddOpenApi(options =>
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

		_ = builder.Services.AddEndpointsApiExplorer();
		_ = builder.Services.AddHealthChecks()
			.AddCheck<ObjectServiceHealthCheck>("object-service");
		_ = builder.Services.AddProblemDetails();
		_ = builder.Services.AddRazorPages();
		_ = builder.Services.AddHttpClient();
		_ = builder.Services.AddHttpContextAccessor();
		_ = builder.Services.AddDbContext<LocoDbContext>(options =>
		{
			_ = options.UseSqlite(connectionString);
			if (builder.Environment.IsDevelopment())
			{
				_ = options.EnableDetailedErrors();
				_ = options.EnableSensitiveDataLogging();
			}
		});

		_ = builder.Services.AddDbContext<IdentityContext>(options =>
		{
			_ = options.UseSqlite(connectionString, sql => sql.MigrationsHistoryTable(IdentityContext.MigrationsHistoryTableName));
			if (builder.Environment.IsDevelopment())
			{
				_ = options.EnableDetailedErrors();
				_ = options.EnableSensitiveDataLogging();
			}
		});

		_ = builder.Services.AddScoped<ObjectExplorerService>();
		_ = builder.Services.AddDatabaseDeveloperPageExceptionFilter();

		var objRoot = builder.Configuration["ObjectService:RootFolder"];
		var paletteMapFile = builder.Configuration["ObjectService:PaletteMapFile"];
		ArgumentNullException.ThrowIfNull(objRoot);
		ArgumentNullException.ThrowIfNull(paletteMapFile);
		ArgumentNullException.ThrowIfNull(connectionString);

		var serverFolderManager = new ServerFolderManager(objRoot, connectionString.Replace("Data Source=", string.Empty));
		var paletteMap = new PaletteMap(paletteMapFile);

		_ = builder.Services.AddSingleton(serverFolderManager);
		_ = builder.Services.AddSingleton(paletteMap);

		_ = builder.Services.AddHttpLogging(logging =>
		{
			_ = logging.RequestHeaders.Add("Cdn-Loop");
			_ = logging.RequestHeaders.Add("Cf-Connecting-Ip");
			_ = logging.RequestHeaders.Add("Cf-Ipcountry");
			_ = logging.RequestHeaders.Add("Cf-Ray");
			_ = logging.RequestHeaders.Add("Cf-Visitor");
			_ = logging.RequestHeaders.Add("Cf-Warp-Tag-Id");
			_ = logging.RequestHeaders.Add("X-Forwarded-For");
			_ = logging.RequestHeaders.Add("X-Forwarded-Proto");

			logging.LoggingFields = HttpLoggingFields.All;
			logging.CombineLogs = true;
		});

		var rateLimiterSection = builder.Configuration.GetSection("ObjectService:RateLimiter");
		ArgumentNullException.ThrowIfNull(rateLimiterSection);
		var rateLimiter = new RateLimitOptions();
		rateLimiterSection.Bind(rateLimiter);

		_ = builder.Services.Configure<ForwardedHeadersOptions>(options =>
		{
			options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
		});

		_ = builder.Services.AddRateLimiter(rlOptions => rlOptions
			.AddTokenBucketLimiter(policyName: TokenPolicy, options =>
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

		_ = builder.Services
			.AddIdentityApiEndpoints<TblUser>()
			.AddEntityFrameworkStores<IdentityContext>();

		_ = builder.Services.Configure<BearerTokenOptions>(IdentityConstants.BearerScheme, options =>
		{
			var durationInMinutes = builder.Configuration.GetValue<int?>("JwtSettings:DurationInMinutes") ?? 60;
			options.BearerTokenExpiration = TimeSpan.FromMinutes(durationInMinutes);
		});

		_ = builder.Services.AddAuthentication()
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

		_ = builder.Services.AddAuthorization(options =>
		{
			options.DefaultPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
				.AddAuthenticationSchemes(IdentityConstants.BearerScheme, JwtBearerDefaults.AuthenticationScheme)
				.RequireAuthenticatedUser()
				.Build();
		});
	}

	public static void Configure(WebApplication app)
	{
		app.UseForwardedHeaders();
		app.UseHttpLogging();
		app.UseRateLimiter();
		app.UseStaticFiles();
		app.UseAuthentication();
		app.UseAuthorization();

		app.MapIdentityApi<TblUser>();

		_ = app
			.MapHealthChecks("/health")
			.RequireRateLimiting(TokenPolicy);

		_ = app.MapRazorPages();

		_ = app.MapV2Routes()
			.RequireRateLimiting(TokenPolicy);

		_ = app.MapV1Routes()
			.RequireRateLimiting(TokenPolicy);

		var showScalar = app.Configuration.GetValue<bool?>("ObjectService:ShowScalar");

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
	}
}
