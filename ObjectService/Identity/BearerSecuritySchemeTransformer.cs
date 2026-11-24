using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace ObjectService.Identity;

/// <summary>
/// Declares the bearer-token security scheme (<c>Bearer</c>) in the generated OpenAPI document so API
/// reference tooling (Scalar) can offer an "authorize" input for the token issued by
/// <c>/v2/identity/login</c>. The scheme id deliberately matches the name passed to
/// <c>ScalarOptions.AddPreferredSecuritySchemes("Bearer")</c> in Program.cs.
/// </summary>
public sealed class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
	public const string SchemeName = "Bearer";

	public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
	{
		document.Components ??= new OpenApiComponents();
		document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
		document.Components.SecuritySchemes[SchemeName] = new OpenApiSecurityScheme
		{
			Type = SecuritySchemeType.Http,
			Scheme = "bearer",
			BearerFormat = "JWT",
			Description = "Identity bearer token returned by POST /v2/identity/login (useCookies=false).",
		};

		return Task.CompletedTask;
	}
}

/// <summary>
/// Marks operations whose endpoints require authorization with the <c>Bearer</c> security requirement.
/// This keeps the generated document aligned with the endpoints' actual behaviour: routes mapped with
/// <c>RequireAuthorization</c> (including group-level requirements) are marked, while public reads stay
/// unmarked.
/// </summary>
public sealed class BearerOperationTransformer : IOpenApiOperationTransformer
{
	public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
	{
		var metadata = context.Description.ActionDescriptor.EndpointMetadata;
		var requiresAuthorization = metadata.OfType<IAuthorizeData>().Any()
			&& !metadata.OfType<IAllowAnonymous>().Any();

		if (requiresAuthorization)
		{
			operation.Security ??= [];
			operation.Security.Add(new OpenApiSecurityRequirement
			{
				[new OpenApiSecuritySchemeReference(BearerSecuritySchemeTransformer.SchemeName, context.Document)] = [],
			});
		}

		return Task.CompletedTask;
	}
}