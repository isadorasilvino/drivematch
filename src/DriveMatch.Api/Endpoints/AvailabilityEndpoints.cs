using DriveMatch.Application.Features.Availabilities.ChangeStatus;
using DriveMatch.Application.Features.Availabilities.Create;
using DriveMatch.Application.Features.Availabilities.Update;

namespace DriveMatch.Api.Endpoints;

public static class AvailabilityEndpoints
{
    public static IEndpointRouteBuilder MapAvailabilityEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/api/availabilities")
            .WithTags("Availabilities");

        group.MapPost("/", CreateAsync)
            .WithName("CreateAvailability")
            .Produces<CreateAvailabilityResult>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/{availabilityId:guid}", UpdateAsync)
            .WithName("UpdateAvailability")
            .Produces<UpdateAvailabilityResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPatch("/{availabilityId:guid}/status", ChangeStatusAsync)
            .WithName("ChangeAvailabilityStatus")
            .Produces<ChangeAvailabilityStatusResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        return endpoints;
    }

    private static async Task<IResult> CreateAsync(
        CreateAvailabilityRequest request,
        CreateAvailabilityHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await handler.HandleAsync(
                new CreateAvailabilityCommand(
                    request.UserId,
                    request.DayOfWeek,
                    request.StartTime,
                    request.EndTime),
                cancellationToken);

            return Results.Created(
                $"/api/availabilities/{result.AvailabilityId}",
                result);
        }
        catch (InstructorProfileNotFoundException exception)
        {
            return Results.NotFound(new { error = exception.Message });
        }
    }

    private static async Task<IResult> UpdateAsync(
        Guid availabilityId,
        UpdateAvailabilityRequest request,
        UpdateAvailabilityHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await handler.HandleAsync(
                new UpdateAvailabilityCommand(
                    availabilityId,
                    request.DayOfWeek,
                    request.StartTime,
                    request.EndTime),
                cancellationToken);

            return Results.Ok(result);
        }
        catch (DriveMatch.Application.Features.Availabilities.Update.AvailabilityNotFoundException exception)
        {
            return Results.NotFound(new { error = exception.Message });
        }
    }

    private static async Task<IResult> ChangeStatusAsync(
        Guid availabilityId,
        ChangeAvailabilityStatusRequest request,
        ChangeAvailabilityStatusHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await handler.HandleAsync(
                new ChangeAvailabilityStatusCommand(
                    availabilityId,
                    request.IsActive),
                cancellationToken);

            return Results.Ok(result);
        }
        catch (DriveMatch.Application.Features.Availabilities.ChangeStatus.AvailabilityNotFoundException exception)
        {
            return Results.NotFound(new { error = exception.Message });
        }
    }

    public sealed record CreateAvailabilityRequest(
        Guid UserId,
        DayOfWeek DayOfWeek,
        TimeOnly StartTime,
        TimeOnly EndTime);

    public sealed record UpdateAvailabilityRequest(
        DayOfWeek DayOfWeek,
        TimeOnly StartTime,
        TimeOnly EndTime);

    public sealed record ChangeAvailabilityStatusRequest(
        bool IsActive);
}