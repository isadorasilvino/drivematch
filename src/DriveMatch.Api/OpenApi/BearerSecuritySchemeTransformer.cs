using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace DriveMatch.Api.OpenApi;

internal sealed class BearerSecuritySchemeTransformer(
    IAuthenticationSchemeProvider authenticationSchemeProvider)
    : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        var authenticationSchemes =
            await authenticationSchemeProvider.GetAllSchemesAsync();

        if (authenticationSchemes.All(
                scheme => scheme.Name != "Bearer"))
        {
            return;
        }

        document.Components ??= new OpenApiComponents();

        document.Components.SecuritySchemes =
            new Dictionary<string, IOpenApiSecurityScheme>
            {
                ["Bearer"] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    In = ParameterLocation.Header,
                    BearerFormat = "JWT"
                }
            };

        foreach (var operation in document.Paths.Values
             .Where(path => path.Operations is not null)
             .SelectMany(path => path.Operations!))
        {
            operation.Value.Security ??= [];

            operation.Value.Security.Add(
                new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference(
                        "Bearer",
                        document)] = []
                });
        }
    }
}