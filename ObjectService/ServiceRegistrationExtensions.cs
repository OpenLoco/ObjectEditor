using Definitions.Database;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using ObjectService.Identity;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.RateLimiting;

namespace ObjectService;

/// <summary>
/// Static registrations that keep <c>Program.cs</c> a small composition root. Each method is a
/// cohesive slice of the app's service configuration.
/// </summary>
public static class ServiceRegistrationExtensions
{
	/// <summary>Name of the single (global) token-bucket rate-limit policy applied to the API.</summary>
	public const string RateLimitPolicyName = "token";

	/// <summary>Registers the OpenAPI document (with the Bearer scheme declared) and endpoint explorer.</summary>
	public static IServiceCollection AddObjectServiceOpenApi(this IServiceCollection services)
	{
		_ = services.AddOpenApi(options =>
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

			// Declare the "Bearer" security scheme so it appears in the generated document, and mark the
			// operations that actually require authorization (group-level RequireAuthorization included) with
			// that scheme, so the Scalar API reference matches the real routes.
			_ = options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
			_ = options.AddOperationTransformer<BearerOperationTransformer>();
		});

		_ = services.AddEndpointsApiExplorer();

		return services;
	}

	/// <summary>
	/// Configures forwarded-header handling for the reverse proxy (cloudflared runs on the same host and
	/// connects over loopback, so only loopback proxies are trusted).
	/// </summary>
	public static IServiceCollection AddObjectServiceForwardedHeaders(this IServiceCollection services)
	{
		_ = services.Configure<ForwardedHeadersOptions>(options =>
		{
			options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
			options.KnownProxies.Add(IPAddress.Loopback);
			options.KnownProxies.Add(IPAddress.IPv6Loopback);
		});

		return services;
	}

	/// <summary>Registers the global token-bucket rate limiter (config-bound, with a 429 rejection handler).</summary>
	public static IServiceCollection AddObjectServiceRateLimiting(this IServiceCollection services, IConfiguration configuration)
	{
		var rateLimiterSection = configuration.GetSection("ObjectService:RateLimiter");
		ArgumentNullException.ThrowIfNull(rateLimiterSection);

		var rateLimiter = new RateLimitOptions();
		rateLimiterSection.Bind(rateLimiter);

		_ = services.AddRateLimiter(rlOptions => rlOptions
			.AddTokenBucketLimiter(policyName: RateLimitPolicyName, options =>
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

		return services;
	}

	/// <summary>
	/// Registers ASP.NET Identity, the JWT bearer scheme, the API authorization policies and the
	/// ownership/permission handlers. The development-only authentication bypass is only honoured when
	/// <paramref name="environment"/> is Development.
	/// </summary>
	public static IServiceCollection AddObjectServiceIdentity(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
	{
		_ = services
			.AddIdentityApiEndpoints<TblUser>()
			.AddRoles<TblUserRole>()
			.AddEntityFrameworkStores<LocoDbContext>();

		// Relax default password rules for development.
		// Override via appsettings or user secrets in production.
		_ = services.Configure<IdentityOptions>(options =>
		{
			options.Password.RequireDigit = true;
			options.Password.RequireLowercase = true;
			options.Password.RequireUppercase = true;
			options.Password.RequireNonAlphanumeric = true;
			options.Password.RequiredLength = 12;
		});

		// Configure bearer token expiration from settings
		_ = services.Configure<BearerTokenOptions>(IdentityConstants.BearerScheme, options =>
		{
			var durationInMinutes = configuration.GetValue<int?>("JwtSettings:DurationInMinutes") ?? 60;
			options.BearerTokenExpiration = TimeSpan.FromMinutes(durationInMinutes);
		});

		// Dev-mode authentication bypass: when enabled, API requests are authenticated as a local admin.
		// Set "ObjectService:DisableAuthentication": true in appsettings.Development.json.
		// Outside Development the bypass is never honoured, so a production deployment cannot disable
		// authentication even if the setting leaks into configuration.
		var disableAuthRequested = configuration.GetValue<bool?>("ObjectService:DisableAuthentication") ?? false;
		var disableAuth = disableAuthRequested && environment.IsDevelopment();
		if (disableAuthRequested && !environment.IsDevelopment())
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
		var jwtKey = configuration["JwtSettings:Key"];
		if (string.IsNullOrWhiteSpace(jwtKey))
		{
			if (!environment.IsDevelopment())
			{
				throw new InvalidOperationException("JwtSettings:Key is not configured. Provide it via user-secrets or an environment variable.");
			}

			jwtKey = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
		}

		var authenticationBuilder = services.AddAuthentication()
			.AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = configuration["JwtSettings:Issuer"],
					ValidAudience = configuration["JwtSettings:Audience"],
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
				};
			});

		if (disableAuth)
		{
			_ = authenticationBuilder.AddScheme<AuthenticationSchemeOptions, DevAuthenticationHandler>(DevAuthenticationHandler.SchemeName, null);
		}

		AddAuthorizationPolicies(services, apiAuthenticationSchemes);

		// Register the ownership and permission authorization handlers
		_ = services.AddScoped<IAuthorizationHandler, ObjectOwnershipHandler>();
		_ = services.AddScoped<IAuthorizationHandler, PermissionHandler>();

		return services;
	}

	private static void AddAuthorizationPolicies(IServiceCollection services, IReadOnlyList<string> apiAuthenticationSchemes)
	{
		_ = services.AddAuthorization(options =>
		{
			// Configure the default policy to accept Identity cookies, Identity Bearer tokens, and JWT tokens.
			// This allows both page-based cookie auth (from SignInManager) and API bearer token auth.
			options.DefaultPolicy = new AuthorizationPolicyBuilder()
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
			// Curator policy - any user with at least one curator permission (or Admin)
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
	}
}
