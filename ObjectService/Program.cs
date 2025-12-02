using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using ObjectService.RouteHandlers;
using Definitions.Database;
using ObjectService;
using Scalar.AspNetCore;
using System.Threading.RateLimiting;
using Definitions.ObjectModels;
using Microsoft.OpenApi;

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
		document.Servers.Clear();
		document.Servers.Add(new OpenApiServer() { Url = "https://openloco.leftofzen.dev" });

		return Task.CompletedTask;
	});
});

// (options => _ = options.AddDocumentTransformer<BearerSecuritySchemeTransformer>());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHealthChecks();
builder.Services.AddProblemDetails();
builder.Services.AddDbContext<LocoDbContext>(options =>
{
	_ = options.UseSqlite(connectionString);
	_ = options.EnableDetailedErrors();
	_ = options.EnableSensitiveDataLogging();
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

//var server = new Server(new ServerSettings(objRoot, paletteMapFile));
//builder.Services.AddSingleton(server);

builder.Services.AddHttpLogging(logging =>
{
	// these are marked [redacted] in the logs unless specified here
	logging.RequestHeaders.Add("Cdn-Loop");
	logging.RequestHeaders.Add("Cf-Connecting-Ip");
	logging.RequestHeaders.Add("Cf-Ipcountry");
	logging.RequestHeaders.Add("Cf-Ray");
	logging.RequestHeaders.Add("Cf-Visitor");
	logging.RequestHeaders.Add("Cf-Warp-Tag-Id");
	logging.RequestHeaders.Add("X-Forwarded-For");
	logging.RequestHeaders.Add("X-Forwarded-Proto");

	logging.LoggingFields = HttpLoggingFields.All;
	//logging.LoggingFields = HttpLoggingFields.ResponsePropertiesAndHeaders | HttpLoggingFields.Duration; // this is `All` excluding `ResponseBody`
	logging.CombineLogs = true;
});

var tokenPolicy = "token";

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

//builder.Services
//.AddIdentityApiEndpoints<TblUser>()
//.AddEntityFrameworkStores<LocoDbContext>();

//builder.Services.AddAuthentication(options =>
//{
//	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//	options.TokenValidationParameters = new TokenValidationParameters
//	{
//		ValidateIssuer = true,
//		ValidateAudience = true,
//		ValidateLifetime = true,
//		ValidateIssuerSigningKey = true,
//		ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
//		ValidAudience = builder.Configuration["JwtSettings:Audience"],
//		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"])),
//	};
//});

//builder.Services.AddAuthorization();
//builder.Services
//	.AddAuthorizationBuilder()
//	.AddPolicy(AdminPolicy.Name, AdminPolicy.Build);

// Used for the Identity stuff to send emails to users
// disabling this line effectively disables all email sending, as a default NoOpEmailSender is used in place
//builder.Services.AddTransient<IEmailSender, EmailSender>();

var app = builder.Build();

app.UseForwardedHeaders();
app.UseHttpLogging();
app.UseRateLimiter();
//app.MapLocoIdentityApi<TblUser>();

// defining routes here, after MapLocoIdentityApi, will overwrite them, allowing us to customise them
//app.MapPost("/register", () => Results.Ok());

_ = app
	.MapHealthChecks("/health")
	.RequireRateLimiting(tokenPolicy);

_ = app.MapV2Routes()
	.RequireRateLimiting(tokenPolicy);

_ = app.MapV1Routes()
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
			.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);

		//.AddPreferredSecuritySchemes("Bearer");
	});
}

//app.UseAuthentication();
//app.UseAuthorization();

app.Run();

#pragma warning disable CA1050 // Declare types in namespaces

// this is to enable unit testing in a top-level statement program
public partial class Program;

#pragma warning restore CA1050 // Declare types in namespaces
