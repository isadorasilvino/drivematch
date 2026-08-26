using DriveMatch.Application.Features.Users.Register;
using DriveMatch.Domain.Enums;

namespace DriveMatch.Api.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/api/users")
            .WithTags("Users");

        group.MapPost("/", RegisterUserAsync)
            .WithName("RegisterUser")
            .Produces<RegisterUserResult>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status409Conflict);

        return endpoints;
    }

    private static async Task<IResult> RegisterUserAsync(
        RegisterUserRequest request,
        RegisterUserHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new RegisterUserCommand(
                request.Name,
                request.Email,
                request.Password,
                request.Role);

            var result = await handler.HandleAsync(
                command,
                cancellationToken);

            return Results.Created(
                $"/api/users/{result.UserId}",
                result);
        }
        catch (UserAlreadyExistsException exception)
        {
            return Results.Conflict(new
            {
                error = exception.Message
            });
        }
    }

    public sealed record RegisterUserRequest(
        string Name,
        string Email,
        string Password,
        UserRole Role);
}
