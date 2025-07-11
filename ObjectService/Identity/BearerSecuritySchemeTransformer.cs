//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc.ApiExplorer; // For IApiDescriptionGroupCollectionProvider and ApiDescription
//using Microsoft.AspNetCore.OpenApi; // Provides IOpenApiDocumentTransformer and OpenApiDocumentTransformerContext
//using Microsoft.OpenApi.Models;

//namespace ObjectService
//{
//	public class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
//	{
//		readonly IAuthenticationSchemeProvider _authenticationSchemeProvider;
//		readonly IApiDescriptionGroupCollectionProvider _apiDescriptionGroupCollectionProvider;

//		public BearerSecuritySchemeTransformer(
//			IAuthenticationSchemeProvider authenticationSchemeProvider,
//			IApiDescriptionGroupCollectionProvider apiDescriptionGroupCollectionProvider)
//		{
//			_authenticationSchemeProvider = authenticationSchemeProvider;
//			_apiDescriptionGroupCollectionProvider = apiDescriptionGroupCollectionProvider;
//		}

//		public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
//		{
//			var authenticationSchemes = await _authenticationSchemeProvider.GetAllSchemesAsync();

//			if (authenticationSchemes.Any(authScheme => authScheme.Name == "Bearer"))
//			{
//				document.Components ??= new OpenApiComponents();
//				if (document.Components.SecuritySchemes == null)
//				{
//					document.Components.SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>();
//				}

//				if (!document.Components.SecuritySchemes.ContainsKey("Bearer"))
//				{
//					document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
//					{
//						Type = SecuritySchemeType.Http,
//						Scheme = "bearer",
//						In = ParameterLocation.Header,
//						BearerFormat = "Json Web Token",
//						Description = "Please enter into field the JWT token (e.g. 'Bearer eyJ...')"
//					};
//				}

//				var apiDescriptions = _apiDescriptionGroupCollectionProvider
//					.ApiDescriptionGroups
//					.Items
//					.SelectMany(group => group.Items)
//					.ToList();

//				foreach (var pathItem in document.Paths)
//				{
//					foreach (var operationKvp in pathItem.Value.Operations)
//					{
//						var httpMethod = operationKvp.Key.ToString().ToLowerInvariant(); // e.g., "get", "post"
//						var operation = operationKvp.Value;
//						var path = pathItem.Key; // e.g., "/authorized-endpoint"

//						var apiDescription = apiDescriptions.FirstOrDefault(ad =>
//							ad.RelativePath?.Equals(path.TrimStart('/'), StringComparison.OrdinalIgnoreCase) == true && // Compare path
//							ad.HttpMethod?.Equals(httpMethod, StringComparison.OrdinalIgnoreCase) == true // Compare method
//						);

//						if (apiDescription != null)
//						{
//							var endpointMetadata = apiDescription.ActionDescriptor.EndpointMetadata;

//							var requiresAuthorization = endpointMetadata
//								.Any(m => m is AuthorizeAttribute || m.GetType().Name == "RequireAuthorizationAttribute");

//							var allowsAnonymous = endpointMetadata
//								.Any(m => m is AllowAnonymousAttribute);

//							if (requiresAuthorization && !allowsAnonymous)
//							{
//								operation.Security ??= [];

//								if (!operation.Security.Any(s => s.Keys.Any(k => k.Reference?.Id == "Bearer")))
//								{
//									operation.Security.Add(new OpenApiSecurityRequirement
//									{
//										{
//											new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
//											Array.Empty<string>()
//										}
//									});
//								}
//							}
//						}
//					}
//				}
//			}
//		}
//	}
//}
