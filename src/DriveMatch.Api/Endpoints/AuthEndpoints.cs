using DriveMatch.Application.Features.Auth.Login;

namespace DriveMatch.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/api/auth")
            .WithTags("Auth");

        group.MapPost("/login", LoginAsync)
            .WithName("Login")
            .AllowAnonymous()
            .Produces<LoginResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);

        return endpoints;
    }

    private static async Task<IResult> LoginAsync(
        LoginRequest request,
        LoginHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await handler.HandleAsync(
                new LoginCommand(
                    request.Email,
                    request.Password),
                cancellationToken);

            return Results.Ok(result);
        }
        catch (InvalidCredentialsException)
        {
            return Results.Unauthorized();
        }
        catch (UserInactiveException exception)
        {
            return Results.Json(
                new { error = exception.Message },
                statusCode: StatusCodes.Status403Forbidden);
        }
    }

    public sealed record LoginRequest(
        string Email,
        string Password);
}