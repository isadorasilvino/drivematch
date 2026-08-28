using DriveMatch.Api.Extensions;
using DriveMatch.Application.Features.LessonRequests;
using DriveMatch.Application.Features.LessonRequests.Accept;
using DriveMatch.Application.Features.LessonRequests.Create;
using DriveMatch.Application.Features.LessonRequests.Reject;
using System.Security.Claims;

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
            .WithTags("Lessons");

        group.MapPost("/", CreateAsync)
            .WithName("CreateLessonRequest")
            .Produces<CreateLessonRequestResult>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy =>
                policy.RequireRole("Student"));

        group.MapPatch("/{lessonRequestId:guid}/accept", AcceptAsync)
            .WithName("AcceptLessonRequest")
            .Produces<AcceptLessonRequestResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .RequireAuthorization(policy =>
                policy.RequireRole("Instructor"));

        group.MapPatch("/{lessonRequestId:guid}/reject", RejectAsync)
            .WithName("RejectLessonRequest")
            .Produces<RejectLessonRequestResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy =>
                policy.RequireRole("Instructor"));

        return endpoints;
    }

    private static async Task<IResult> CreateAsync(
    ClaimsPrincipal user,
    CreateLessonRequestRequest request,
    CreateLessonRequestHandler handler,
    CancellationToken cancellationToken)
    {
        try
        {
            var userId = user.GetUserId();

            var result = await handler.HandleAsync(
                new CreateLessonRequestCommand(
                    userId,
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
            return Results.NotFound(new
            {
                error = exception.Message
            });
        }
        catch (InstructorProfileNotFoundException exception)
        {
            return Results.NotFound(new
            {
                error = exception.Message
            });
        }
        catch (InstructorNotActiveException exception)
        {
            return Results.BadRequest(new
            {
                error = exception.Message
            });
        }
        catch (
            DriveMatch.Application.Features.LessonRequests.Create
                .InstructorUnavailableException exception)
        {
            return Results.BadRequest(new
            {
                error = exception.Message
            });
        }
        catch (StudentVehicleNotAcceptedException exception)
        {
            return Results.BadRequest(new
            {
                error = exception.Message
            });
        }
    }

    private static async Task<IResult> AcceptAsync(
        ClaimsPrincipal user,
        Guid lessonRequestId,
        AcceptLessonRequestHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = user.GetUserId();

            var result = await handler.HandleAsync(
                new AcceptLessonRequestCommand(
                    lessonRequestId,
                    userId),
                cancellationToken);

            return Results.Ok(result);
        }
        catch (AcceptNotFoundException exception)
        {
            return Results.NotFound(new
            {
                error = exception.Message
            });
        }
        catch (
            DriveMatch.Application.Features.LessonRequests.Accept
                .InstructorUnavailableException exception)
        {
            return Results.BadRequest(new
            {
                error = exception.Message
            });
        }
        catch (LessonScheduleConflictException exception)
        {
            return Results.Conflict(new
            {
                error = exception.Message
            });
        }
    }

    private static async Task<IResult> RejectAsync(
        ClaimsPrincipal user,
        Guid lessonRequestId,
        RejectLessonRequestHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = user.GetUserId();

            var result = await handler.HandleAsync(
                new RejectLessonRequestCommand(
                    lessonRequestId,
                    userId),
                cancellationToken);

            return Results.Ok(result);
        }
        catch (RejectNotFoundException exception)
        {
            return Results.NotFound(new
            {
                error = exception.Message
            });
        }
        catch (LessonRequestForbiddenException exception)
        {
            return Results.Json(
                new
                {
                    error = exception.Message
                },
                statusCode: StatusCodes.Status403Forbidden);
        }
    }

    public sealed record CreateLessonRequestRequest(
        Guid InstructorProfileId,
        DateOnly RequestedDate,
        TimeOnly StartTime,
        TimeOnly EndTime,
        bool UsesStudentVehicle,
        string? StudentMessage);
}