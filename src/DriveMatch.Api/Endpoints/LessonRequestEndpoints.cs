using DriveMatch.Application.Features.LessonRequests.Accept;
using DriveMatch.Application.Features.LessonRequests.Create;
using DriveMatch.Application.Features.LessonRequests.Reject;

using AcceptNotFoundException =
    DriveMatch.Application.Features.LessonRequests.Accept.LessonRequestNotFoundException;

using RejectNotFoundException =
    DriveMatch.Application.Features.LessonRequests.Reject.LessonRequestNotFoundException;

namespace DriveMatch.Api.Endpoints;

public static class LessonRequestEndpoints
{
    public static IEndpointRouteBuilder MapLessonRequestEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/api/lessons")
            .WithTags("Lessons")
            .RequireAuthorization(policy =>
                policy.RequireRole("Instructor"));

        group.MapPost("/", CreateAsync)
            .WithName("CreateLessonRequest")
            .Produces<CreateLessonRequestResult>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy =>
                policy.RequireRole("Student")); ;

        group.MapPatch("/{lessonRequestId:guid}/accept", AcceptAsync)
            .WithName("AcceptLessonRequest")
            .Produces<AcceptLessonRequestResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .RequireAuthorization(policy =>
                policy.RequireRole("Instructor"));
        
        group.MapPatch("/{lessonRequestId:guid}/reject", RejectAsync)
            .WithName("RejectLessonRequest")
            .Produces<RejectLessonRequestResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        return endpoints;
    }

    private static async Task<IResult> CreateAsync(
        CreateLessonRequestRequest request,
        CreateLessonRequestHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await handler.HandleAsync(
                new CreateLessonRequestCommand(
                    request.StudentProfileId,
                    request.InstructorProfileId,
                    request.RequestedDate,
                    request.StartTime,
                    request.EndTime,
                    request.UsesStudentVehicle,
                    request.StudentMessage),
                cancellationToken);

            return Results.Created(
                $"/api/lesson-requests/{result.LessonRequestId}",
                result);
        }
        catch (StudentProfileNotFoundException exception)
        {
            return Results.NotFound(new { error = exception.Message });
        }
        catch (InstructorProfileNotFoundException exception)
        {
            return Results.NotFound(new { error = exception.Message });
        }
        catch (InstructorNotActiveException exception)
        {
            return Results.BadRequest(new { error = exception.Message });
        }
        catch (DriveMatch.Application.Features.LessonRequests.Create.InstructorUnavailableException exception)
        {
            return Results.BadRequest(new { error = exception.Message });
        }
        catch (StudentVehicleNotAcceptedException exception)
        {
            return Results.BadRequest(new { error = exception.Message });
        }
    }

    private static async Task<IResult> AcceptAsync(
        Guid lessonRequestId,
        AcceptLessonRequestHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await handler.HandleAsync(
                new AcceptLessonRequestCommand(lessonRequestId),
                cancellationToken);

            return Results.Ok(result);
        }
        catch (AcceptNotFoundException exception)
        {
            return Results.NotFound(new { error = exception.Message });
        }
        catch (DriveMatch.Application.Features.LessonRequests.Accept.InstructorUnavailableException exception)
        {
            return Results.BadRequest(new { error = exception.Message });
        }
        catch (LessonScheduleConflictException exception)
        {
            return Results.Conflict(new { error = exception.Message });
        }
    }

    private static async Task<IResult> RejectAsync(
        Guid lessonRequestId,
        RejectLessonRequestHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await handler.HandleAsync(
                new RejectLessonRequestCommand(lessonRequestId),
                cancellationToken);

            return Results.Ok(result);
        }
        catch (RejectNotFoundException exception)
        {
            return Results.NotFound(new { error = exception.Message });
        }
    }

    public sealed record CreateLessonRequestRequest(
        Guid StudentProfileId,
        Guid InstructorProfileId,
        DateOnly RequestedDate,
        TimeOnly StartTime,
        TimeOnly EndTime,
        bool UsesStudentVehicle,
        string? StudentMessage);
}