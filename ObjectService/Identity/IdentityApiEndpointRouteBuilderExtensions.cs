// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// https://github.com/dotnet/aspnetcore/blob/c9027726579d953496ce3c43546d1b69f77385a5/src/Identity/Core/src/IdentityApiEndpointRouteBuilderExtensions.cs

using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using OpenLoco.Definitions.DTO.Identity;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace ObjectService.Identity
{

	/// <summary>
	/// Provides extension methods for <see cref="IEndpointRouteBuilder"/> to add identity endpoints.
	/// </summary>
	public static class IdentityApiEndpointRouteBuilderExtensions
	{
		// Validate the email address using DataAnnotations like the UserValidator does when RequireUniqueEmail = true.
		static readonly EmailAddressAttribute _emailAddressAttribute = new();

		/// <summary>
		/// Add endpoints for registering, logging in, and logging out using ASP.NET Core Identity.
		/// </summary>
		/// <typeparam name="TUser">The type describing the user. This should match the generic parameter in <see cref="UserManager{TUser}"/>.</typeparam>
		/// <param name="endpoints">
		/// The <see cref="IEndpointRouteBuilder"/> to add the identity endpoints to.
		/// Call <see cref="EndpointRouteBuilderExtensions.MapGroup(IEndpointRouteBuilder, string)"/> to add a prefix to all the endpoints.
		/// </param>
		/// <returns>An <see cref="IEndpointConventionBuilder"/> to further customize the added endpoints.</returns>
		public static IEndpointConventionBuilder MapLocoIdentityApi<TUser>(this IEndpointRouteBuilder endpoints)
			where TUser : class, new()
		{
			ArgumentNullException.ThrowIfNull(endpoints);

			var timeProvider = endpoints.ServiceProvider.GetRequiredService<TimeProvider>();
			var bearerTokenOptions = endpoints.ServiceProvider.GetRequiredService<IOptionsMonitor<BearerTokenOptions>>();
			var emailSender = endpoints.ServiceProvider.GetRequiredService<IEmailSender<TUser>>();
			var linkGenerator = endpoints.ServiceProvider.GetRequiredService<LinkGenerator>();

			// We'll figure out a unique endpoint name based on the final route pattern during endpoint generation.
			string? confirmEmailEndpointName = null;

			var routeGroup = endpoints.MapGroup("");

			// NOTE: We cannot inject UserManager<TUser> directly because the TUser generic parameter is currently unsupported by RDG.
			// https://github.com/dotnet/aspnetcore/issues/47338
			_ = routeGroup.MapPost("/register", async Task<Results<Ok, ValidationProblem>>
				([FromBody] DtoRegisterRequest registration, HttpContext context, [FromServices] IServiceProvider sp) =>
			{
				var userManager = sp.GetRequiredService<UserManager<TUser>>();

				if (!userManager.SupportsUserEmail)
				{
					throw new NotSupportedException($"{nameof(MapLocoIdentityApi)} requires a user store with email support.");
				}

				var userStore = sp.GetRequiredService<IUserStore<TUser>>();
				var emailStore = (IUserEmailStore<TUser>)userStore;
				var email = registration.Email;

				if (string.IsNullOrEmpty(email) || !_emailAddressAttribute.IsValid(email))
				{
					return CreateValidationProblem(IdentityResult.Failed(userManager.ErrorDescriber.InvalidEmail(email)));
				}

				var userName = registration.UserName;
				if (string.IsNullOrEmpty(userName))
				{
					return CreateValidationProblem(IdentityResult.Failed(userManager.ErrorDescriber.InvalidUserName(userName)));
				}

				var user = new TUser();
				await userStore.SetUserNameAsync(user, userName, CancellationToken.None);
				await emailStore.SetEmailAsync(user, email, CancellationToken.None);
				var result = await userManager.CreateAsync(user, registration.Password);

				if (!result.Succeeded)
				{
					return CreateValidationProblem(result);
				}

				await SendConfirmationEmailAsync(user, userManager, context, email);
				return TypedResults.Ok();
			}).AllowAnonymous();

			_ = routeGroup.MapPost("/login", async Task<Results<Ok<AccessTokenResponse>, EmptyHttpResult, ProblemHttpResult>>
				([FromBody] LoginRequest login, [FromQuery] bool? useCookies, [FromQuery] bool? useSessionCookies, [FromServices] IServiceProvider sp) =>
			{
				var signInManager = sp.GetRequiredService<SignInManager<TUser>>();

				var useCookieScheme = useCookies == true || useSessionCookies == true;
				var isPersistent = useCookies == true && useSessionCookies != true;
				signInManager.AuthenticationScheme = useCookieScheme ? IdentityConstants.ApplicationScheme : IdentityConstants.BearerScheme;

				var userManager = sp.GetRequiredService<UserManager<TUser>>();
				if (await userManager.FindByEmailAsync(login.Email) is not { } user)
				{
					return TypedResults.Problem();
				}

				var result = await signInManager.PasswordSignInAsync(user, login.Password, isPersistent, lockoutOnFailure: true);

				if (!result.Succeeded)
				{
					return TypedResults.Problem(result.ToString(), statusCode: StatusCodes.Status401Unauthorized);
				}

				// The signInManager already produced the needed response in the form of a cookie or bearer token.
				// HOW THE FUCK DOES IT DO THAT
				// this is where the response is created, but how the fuck is it returned here when the code clearly says return Empty
				// https://github.com/dotnet/aspnetcore/blob/ee050bd56bdf2b653d4bad75f26ba8802a4f58fa/src/Security/Authentication/BearerToken/src/BearerTokenHandler.cs#L64
				return TypedResults.Empty;
			}).AllowAnonymous();

			_ = routeGroup.MapPost("/refresh", async Task<Results<Ok<AccessTokenResponse>, UnauthorizedHttpResult, SignInHttpResult, ChallengeHttpResult>>
				([FromBody] RefreshRequest refreshRequest, [FromServices] IServiceProvider sp) =>
			{
				var signInManager = sp.GetRequiredService<SignInManager<TUser>>();
				var refreshTokenProtector = bearerTokenOptions.Get(IdentityConstants.BearerScheme).RefreshTokenProtector;
				var refreshTicket = refreshTokenProtector.Unprotect(refreshRequest.RefreshToken);

				// Reject the /refresh attempt with a 401 if the token expired or the security stamp validation fails
				if (refreshTicket?.Properties?.ExpiresUtc is not { } expiresUtc ||
					timeProvider.GetUtcNow() >= expiresUtc ||
					await signInManager.ValidateSecurityStampAsync(refreshTicket.Principal) is not TUser user)

				{
					return TypedResults.Challenge();
				}

				var newPrincipal = await signInManager.CreateUserPrincipalAsync(user);
				return TypedResults.SignIn(newPrincipal, authenticationScheme: IdentityConstants.BearerScheme);
			});

			routeGroup.MapGet("/confirmEmail", async Task<Results<ContentHttpResult, UnauthorizedHttpResult>>
				([FromQuery] string userId, [FromQuery] string code, [FromQuery] string? changedEmail, [FromServices] IServiceProvider sp) =>
			{
				var userManager = sp.GetRequiredService<UserManager<TUser>>();
				if (await userManager.FindByIdAsync(userId) is not { } user)
				{
					// We could respond with a 404 instead of a 401 like Identity UI, but that feels like unnecessary information.
					return TypedResults.Unauthorized();
				}

				try
				{
					code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
				}
				catch (FormatException)
				{
					return TypedResults.Unauthorized();
				}

				IdentityResult result;

				if (string.IsNullOrEmpty(changedEmail))
				{
					result = await userManager.ConfirmEmailAsync(user, code);
				}
				else
				{
					// As with Identity UI, email and user name are one and the same. So when we update the email,
					// we need to update the user name.
					result = await userManager.ChangeEmailAsync(user, changedEmail, code);

					if (result.Succeeded)
					{
						result = await userManager.SetUserNameAsync(user, changedEmail);
					}
				}

				if (!result.Succeeded)
				{
					return TypedResults.Unauthorized();
				}

				return TypedResults.Text("Thank you for confirming your email.");
			})
			.Add(endpointBuilder =>
			{
				var finalPattern = ((RouteEndpointBuilder)endpointBuilder).RoutePattern.RawText;
				confirmEmailEndpointName = $"{nameof(MapLocoIdentityApi)}-{finalPattern}";
				endpointBuilder.Metadata.Add(new EndpointNameMetadata(confirmEmailEndpointName));
			});

			_ = routeGroup.MapPost("/resendConfirmationEmail", async Task<Ok>
				([FromBody] ResendConfirmationEmailRequest resendRequest, HttpContext context, [FromServices] IServiceProvider sp) =>
			{
				var userManager = sp.GetRequiredService<UserManager<TUser>>();
				if (await userManager.FindByEmailAsync(resendRequest.Email) is not { } user)
				{
					return TypedResults.Ok();
				}

				await SendConfirmationEmailAsync(user, userManager, context, resendRequest.Email);
				return TypedResults.Ok();
			});

			_ = routeGroup.MapPost("/forgotPassword", async Task<Results<Ok, ValidationProblem>>
				([FromBody] ForgotPasswordRequest resetRequest, [FromServices] IServiceProvider sp) =>
			{
				var userManager = sp.GetRequiredService<UserManager<TUser>>();
				var user = await userManager.FindByEmailAsync(resetRequest.Email);

				if (user is not null && await userManager.IsEmailConfirmedAsync(user))
				{
					var code = await userManager.GeneratePasswordResetTokenAsync(user);
					code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

					await emailSender.SendPasswordResetCodeAsync(user, resetRequest.Email, HtmlEncoder.Default.Encode(code));
				}

				// Don't reveal that the user does not exist or is not confirmed, so don't return a 200 if we would have
				// returned a 400 for an invalid code given a valid user email.
				return TypedResults.Ok();
			});

			_ = routeGroup.MapPost("/resetPassword", async Task<Results<Ok, ValidationProblem>>
				([FromBody] ResetPasswordRequest resetRequest, [FromServices] IServiceProvider sp) =>
			{
				var userManager = sp.GetRequiredService<UserManager<TUser>>();

				var user = await userManager.FindByEmailAsync(resetRequest.Email);

				if (user is null || !await userManager.IsEmailConfirmedAsync(user))
				{
					// Don't reveal that the user does not exist or is not confirmed, so don't return a 200 if we would have
					// returned a 400 for an invalid code given a valid user email.
					return CreateValidationProblem(IdentityResult.Failed(userManager.ErrorDescriber.InvalidToken()));
				}

				IdentityResult result;
				try
				{
					var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(resetRequest.ResetCode));
					result = await userManager.ResetPasswordAsync(user, code, resetRequest.NewPassword);
				}
				catch (FormatException)
				{
					result = IdentityResult.Failed(userManager.ErrorDescriber.InvalidToken());
				}

				if (!result.Succeeded)
				{
					return CreateValidationProblem(result);
				}

				return TypedResults.Ok();
			});

			var accountGroup = routeGroup.MapGroup("/manage").RequireAuthorization();

			_ = accountGroup.MapGet("/info", async Task<Results<Ok<DtoInfoResponse>, ValidationProblem, NotFound>>
				(ClaimsPrincipal claimsPrincipal, [FromServices] IServiceProvider sp) =>
			{
				var userManager = sp.GetRequiredService<UserManager<TUser>>();
				if (await userManager.GetUserAsync(claimsPrincipal) is not { } user)
				{
					return TypedResults.NotFound();
				}

				return TypedResults.Ok(await CreateInfoResponseAsync(user, userManager));
			}).RequireAuthorization();

			_ = accountGroup.MapPost("/info", async Task<Results<Ok<DtoInfoResponse>, ValidationProblem, NotFound>>
				(ClaimsPrincipal claimsPrincipal, [FromBody] InfoRequest infoRequest, HttpContext context, [FromServices] IServiceProvider sp) =>
			{
				var userManager = sp.GetRequiredService<UserManager<TUser>>();
				if (await userManager.GetUserAsync(claimsPrincipal) is not { } user)
				{
					return TypedResults.NotFound();
				}

				if (!string.IsNullOrEmpty(infoRequest.NewEmail) && !_emailAddressAttribute.IsValid(infoRequest.NewEmail))
				{
					return CreateValidationProblem(IdentityResult.Failed(userManager.ErrorDescriber.InvalidEmail(infoRequest.NewEmail)));
				}

				if (!string.IsNullOrEmpty(infoRequest.NewPassword))
				{
					if (string.IsNullOrEmpty(infoRequest.OldPassword))
					{
						return CreateValidationProblem("OldPasswordRequired",
							"The old password is required to set a new password. If the old password is forgotten, use /resetPassword.");
					}

					var changePasswordResult = await userManager.ChangePasswordAsync(user, infoRequest.OldPassword, infoRequest.NewPassword);
					if (!changePasswordResult.Succeeded)
					{
						return CreateValidationProblem(changePasswordResult);
					}
				}

				if (!string.IsNullOrEmpty(infoRequest.NewEmail))
				{
					var email = await userManager.GetEmailAsync(user);

					if (email != infoRequest.NewEmail)
					{
						await SendConfirmationEmailAsync(user, userManager, context, infoRequest.NewEmail, isChange: true);
					}
				}

				return TypedResults.Ok(await CreateInfoResponseAsync(user, userManager));
			}).RequireAuthorization();

			async Task SendConfirmationEmailAsync(TUser user, UserManager<TUser> userManager, HttpContext context, string email, bool isChange = false)
			{
				if (confirmEmailEndpointName is null)
				{
					throw new NotSupportedException("No email confirmation endpoint was registered!");
				}

				var code = isChange
					? await userManager.GenerateChangeEmailTokenAsync(user, email)
					: await userManager.GenerateEmailConfirmationTokenAsync(user);
				code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

				var userId = await userManager.GetUserIdAsync(user);
				var routeValues = new RouteValueDictionary()
				{
					["userId"] = userId,
					["code"] = code,
				};

				if (isChange)
				{
					// This is validated by the /confirmEmail endpoint on change.
					routeValues.Add("changedEmail", email);
				}

				var confirmEmailUrl = linkGenerator.GetUriByName(context, confirmEmailEndpointName, routeValues)
					?? throw new NotSupportedException($"Could not find endpoint named '{confirmEmailEndpointName}'.");

				await emailSender.SendConfirmationLinkAsync(user, email, HtmlEncoder.Default.Encode(confirmEmailUrl));
			}

			return new IdentityEndpointsConventionBuilder(routeGroup);
		}

		static ValidationProblem CreateValidationProblem(string errorCode, string errorDescription) =>
			TypedResults.ValidationProblem(new Dictionary<string, string[]> {
			{ errorCode, [errorDescription] }
			});

		static ValidationProblem CreateValidationProblem(IdentityResult result)
		{
			// We expect a single error code and description in the normal case.
			// This could be golfed with GroupBy and ToDictionary, but perf! :P
			Debug.Assert(!result.Succeeded);
			var errorDictionary = new Dictionary<string, string[]>(1);

			foreach (var error in result.Errors)
			{
				string[] newDescriptions;

				if (errorDictionary.TryGetValue(error.Code, out var descriptions))
				{
					newDescriptions = new string[descriptions.Length + 1];
					Array.Copy(descriptions, newDescriptions, descriptions.Length);
					newDescriptions[descriptions.Length] = error.Description;
				}
				else
				{
					newDescriptions = [error.Description];
				}

				errorDictionary[error.Code] = newDescriptions;
			}

			return TypedResults.ValidationProblem(errorDictionary);
		}

		static async Task<DtoInfoResponse> CreateInfoResponseAsync<TUser>(TUser user, UserManager<TUser> userManager)
			where TUser : class
			=> new(
				await userManager.GetUserNameAsync(user) ?? throw new NotSupportedException("Users must have a username."),
				await userManager.GetEmailAsync(user) ?? throw new NotSupportedException("Users must have an email."),
				await userManager.IsEmailConfirmedAsync(user));

		// Wrap RouteGroupBuilder with a non-public type to avoid a potential future behavioral breaking change.
		sealed class IdentityEndpointsConventionBuilder(RouteGroupBuilder inner) : IEndpointConventionBuilder
		{
			IEndpointConventionBuilder InnerAsConventionBuilder => inner;

			public void Add(Action<EndpointBuilder> convention) => InnerAsConventionBuilder.Add(convention);
			public void Finally(Action<EndpointBuilder> finallyConvention) => InnerAsConventionBuilder.Finally(finallyConvention);
		}

		[AttributeUsage(AttributeTargets.Parameter)]
		sealed class FromBodyAttribute : Attribute, IFromBodyMetadata
		{
		}

		[AttributeUsage(AttributeTargets.Parameter)]
		sealed class FromServicesAttribute : Attribute, IFromServiceMetadata
		{
		}

		[AttributeUsage(AttributeTargets.Parameter)]
		sealed class FromQueryAttribute : Attribute, IFromQueryMetadata
		{
			public string? Name => null;
		}
	}
}
