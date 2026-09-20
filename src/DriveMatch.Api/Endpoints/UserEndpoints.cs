using System.Security.Claims;
using DriveMatch.Api.Extensions;
using DriveMatch.Application.Features.Users.Account.GetMyAccount;
using DriveMatch.Application.Features.Users.Account.UpdateMyAccount;
using DriveMatch.Application.Features.Users.Account.ChangePassword;
using DriveMatch.Application.Features.Users.Account;
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

        group.MapGet("/me", GetMyAccountAsync)
            .WithName("GetMyAccount")
            .RequireAuthorization()
            .Produces<GetMyAccountResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPut("/me", UpdateMyAccountAsync)
            .WithName("UpdateMyAccount")
            .RequireAuthorization()
            .Produces<UpdateMyAccountResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPut("/me/password", ChangePasswordAsync)
            .WithName("ChangePassword")
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized);

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

    private static async Task<IResult> GetMyAccountAsync(
        ClaimsPrincipal user,
        GetMyAccountHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = user.GetUserId();

            var query = new GetMyAccountQuery(userId);

            var result = await handler.HandleAsync(
                query,
                cancellationToken);

            return Results.Ok(result);
        }
        catch (AccountNotFoundException exception)
        {
            return Results.NotFound(new
            {
                error = exception.Message
            });
        }
    }

    private static async Task<IResult> UpdateMyAccountAsync(
        ClaimsPrincipal user,
        UpdateMyAccountRequest request,
        UpdateMyAccountHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = user.GetUserId();

            var command = new UpdateMyAccountCommand(
                userId,
                request.Name,
                request.Email);

            var result = await handler.HandleAsync(
                command,
                cancellationToken);

            return Results.Ok(result);
        }
        catch (AccountNotFoundException exception)
        {
            return Results.NotFound(new
            {
                error = exception.Message
            });
        }
        catch (AccountEmailAlreadyInUseException exception)
        {
            return Results.Conflict(new
            {
                error = exception.Message
            });
        }
    }

    private static async Task<IResult> ChangePasswordAsync(
        ClaimsPrincipal user,
        ChangePasswordRequest request,
        ChangePasswordHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = user.GetUserId();

            var command = new ChangePasswordCommand(
                userId,
                request.CurrentPassword,
                request.NewPassword);

            await handler.HandleAsync(
                command,
                cancellationToken);

            return Results.NoContent();
        }
        catch (AccountNotFoundException exception)
        {
            return Results.NotFound(new
            {
                error = exception.Message
            });
        }
        catch (CurrentPasswordIncorrectException exception)
        {
            return Results.BadRequest(new
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

    public sealed record UpdateMyAccountRequest(
        string Name,
        string Email);

    public sealed record ChangePasswordRequest(
        string CurrentPassword,
        string NewPassword);
}