using DriveMatch.Application.Features.Availabilities.ChangeStatus;
using DriveMatch.Application.Features.Availabilities.Create;
using DriveMatch.Application.Features.Availabilities.Update;
using DriveMatch.Application.Features.Availabilities.GetMine;
using DriveMatch.Application.Features.Availabilities;
using DriveMatch.Api.Extensions;
using System.Security.Claims;

using CreateInstructorProfileNotFoundException =
    DriveMatch.Application.Features.Availabilities.Create.InstructorProfileNotFoundException;

using GetMineInstructorProfileNotFoundException =
    DriveMatch.Application.Features.Availabilities.GetMine.InstructorProfileNotFoundException;

namespace DriveMatch.Api.Endpoints;

public static class AvailabilityEndpoints
{
    public static IEndpointRouteBuilder MapAvailabilityEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/api/availabilities")
            .WithTags("Availabilities")
            .RequireAuthorization(policy =>
                policy.RequireRole("Instructor"));

        group.MapGet("/", GetMineAsync)
            .WithName("GetMyAvailabilities")
            .Produces<IReadOnlyCollection<GetMyAvailabilitiesResult>>(
                StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", CreateAsync)
            .WithName("CreateAvailability")
            .Produces<CreateAvailabilityResult>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/{availabilityId:guid}", UpdateAsync)
            .WithName("UpdateAvailability")
            .Produces<UpdateAvailabilityResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPatch("/{availabilityId:guid}/status", ChangeStatusAsync)
            .WithName("ChangeAvailabilityStatus")
            .Produces<ChangeAvailabilityStatusResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);

        return endpoints;
    }

    private static async Task<IResult> GetMineAsync(
        ClaimsPrincipal user,
        GetMyAvailabilitiesHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = user.GetUserId();

            var result = await handler.HandleAsync(
                userId,
                cancellationToken);

            return Results.Ok(result);
        }
        catch (GetMineInstructorProfileNotFoundException exception)
        {
            return Results.NotFound(new
            {
                error = exception.Message
            });
        }
    }

    private static async Task<IResult> CreateAsync(
        ClaimsPrincipal user,
        CreateAvailabilityRequest request,
        CreateAvailabilityHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = user.GetUserId();

            var result = await handler.HandleAsync(
                new CreateAvailabilityCommand(
                    userId,
                    request.DayOfWeek,
                    request.StartTime,
                    request.EndTime,
                    request.LessonDurationMinutes,
                    request.BreakDurationMinutes),
                cancellationToken);

            return Results.Created(
                $"/api/availabilities/{result.AvailabilityId}",
                result);
        }
        catch (CreateInstructorProfileNotFoundException exception)
        {
            return Results.NotFound(new { error = exception.Message });
        }
    }

    private static async Task<IResult> UpdateAsync(
        Guid availabilityId,
        ClaimsPrincipal user,
        UpdateAvailabilityRequest request,
        UpdateAvailabilityHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = user.GetUserId();

            var result = await handler.HandleAsync(
                new UpdateAvailabilityCommand(
                    availabilityId,
                    userId,
                    request.DayOfWeek,
                    request.StartTime,
                    request.EndTime,
                    request.LessonDurationMinutes,
                    request.BreakDurationMinutes),
                cancellationToken);

            return Results.Ok(result);
        }
        catch (DriveMatch.Application.Features.Availabilities.Update.AvailabilityNotFoundException exception)
        {
            return Results.NotFound(new { error = exception.Message });
        }
        catch (AvailabilityForbiddenException exception)
        {
            return Results.Json(
                new { error = exception.Message },
                statusCode: StatusCodes.Status403Forbidden);
        }
    }

    private static async Task<IResult> ChangeStatusAsync(
        Guid availabilityId,
        ClaimsPrincipal user,
        ChangeAvailabilityStatusRequest request,
        ChangeAvailabilityStatusHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = user.GetUserId();

            var result = await handler.HandleAsync(
                new ChangeAvailabilityStatusCommand(
                    availabilityId,
                    userId,
                    request.IsActive),
                cancellationToken);

            return Results.Ok(result);
        }
        catch (DriveMatch.Application.Features.Availabilities.ChangeStatus.AvailabilityNotFoundException exception)
        {
            return Results.NotFound(new { error = exception.Message });
        }
        catch (AvailabilityForbiddenException exception)
        {
            return Results.Json(
                new { error = exception.Message },
                statusCode: StatusCodes.Status403Forbidden);
        }
    }

    public sealed record CreateAvailabilityRequest(
        DayOfWeek DayOfWeek,
        TimeOnly StartTime,
        TimeOnly EndTime,
        int LessonDurationMinutes,
        int BreakDurationMinutes);

    public sealed record UpdateAvailabilityRequest(
        DayOfWeek DayOfWeek,
        TimeOnly StartTime,
        TimeOnly EndTime,
        int LessonDurationMinutes,
        int BreakDurationMinutes);

    public sealed record ChangeAvailabilityStatusRequest(
        bool IsActive);
}