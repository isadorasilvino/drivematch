using DriveMatch.Application.Features.Lessons.Cancel;
using DriveMatch.Application.Features.Lessons.Complete;
using DriveMatch.Application.Features.Lessons.ConfirmCheckIn;
using DriveMatch.Application.Features.Lessons.MarkAsNotAttended;
using DriveMatch.Application.Features.Lessons.StartCheckIn;
using DriveMatch.Domain.Exceptions;

using CancelLessonNotFoundException =
    DriveMatch.Application.Features.Lessons.Cancel.LessonNotFoundException;

using CompleteLessonNotFoundException =
    DriveMatch.Application.Features.Lessons.Complete.LessonNotFoundException;

using ConfirmCheckInLessonNotFoundException =
    DriveMatch.Application.Features.Lessons.ConfirmCheckIn.LessonNotFoundException;

using NotAttendedLessonNotFoundException =
    DriveMatch.Application.Features.Lessons.MarkAsNotAttended.LessonNotFoundException;

using StartCheckInLessonNotFoundException =
    DriveMatch.Application.Features.Lessons.StartCheckIn.LessonNotFoundException;

namespace DriveMatch.Api.Endpoints;

public static class LessonEndpoints
{
    public static IEndpointRouteBuilder MapLessonEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/api/lessons")
            .WithTags("Lessons")
            .RequireAuthorization(policy =>
                policy.RequireRole("Instructor"));

        group.MapPatch("/{lessonId:guid}/check-in/start", StartCheckInAsync)
            .WithName("StartLessonCheckIn")
            .Produces<StartLessonCheckInResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPatch("/{lessonId:guid}/check-in/confirm", ConfirmCheckInAsync)
            .WithName("ConfirmLessonCheckIn")
            .Produces<ConfirmLessonCheckInResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPatch("/{lessonId:guid}/complete", CompleteAsync)
            .WithName("CompleteLesson")
            .Produces<CompleteLessonResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPatch("/{lessonId:guid}/cancel", CancelAsync)
            .WithName("CancelLesson")
            .Produces<CancelLessonResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPatch("/{lessonId:guid}/not-attended", MarkAsNotAttendedAsync)
            .WithName("MarkLessonAsNotAttended")
            .Produces<MarkLessonAsNotAttendedResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        return endpoints;
    }

    private static async Task<IResult> StartCheckInAsync(
        Guid lessonId,
        StartLessonCheckInHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await handler.HandleAsync(
                new StartLessonCheckInCommand(lessonId),
                cancellationToken);

            return Results.Ok(result);
        }
        catch (StartCheckInLessonNotFoundException exception)
        {
            return Results.NotFound(new { error = exception.Message });
        }
        catch (DomainException exception)
        {
            return Results.BadRequest(new { error = exception.Message });
        }
    }

    private static async Task<IResult> ConfirmCheckInAsync(
        Guid lessonId,
        ConfirmLessonCheckInHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await handler.HandleAsync(
                new ConfirmLessonCheckInCommand(lessonId),
                cancellationToken);

            return Results.Ok(result);
        }
        catch (ConfirmCheckInLessonNotFoundException exception)
        {
            return Results.NotFound(new { error = exception.Message });
        }
        catch (DomainException exception)
        {
            return Results.BadRequest(new { error = exception.Message });
        }
    }

    private static async Task<IResult> CompleteAsync(
        Guid lessonId,
        CompleteLessonHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await handler.HandleAsync(
                new CompleteLessonCommand(lessonId),
                cancellationToken);

            return Results.Ok(result);
        }
        catch (CompleteLessonNotFoundException exception)
        {
            return Results.NotFound(new { error = exception.Message });
        }
        catch (DomainException exception)
        {
            return Results.BadRequest(new { error = exception.Message });
        }
    }

    private static async Task<IResult> CancelAsync(
        Guid lessonId,
        CancelLessonHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await handler.HandleAsync(
                new CancelLessonCommand(lessonId),
                cancellationToken);

            return Results.Ok(result);
        }
        catch (CancelLessonNotFoundException exception)
        {
            return Results.NotFound(new { error = exception.Message });
        }
        catch (DomainException exception)
        {
            return Results.BadRequest(new { error = exception.Message });
        }
    }

    private static async Task<IResult> MarkAsNotAttendedAsync(
        Guid lessonId,
        MarkLessonAsNotAttendedHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await handler.HandleAsync(
                new MarkLessonAsNotAttendedCommand(lessonId),
                cancellationToken);

            return Results.Ok(result);
        }
        catch (NotAttendedLessonNotFoundException exception)
        {
            return Results.NotFound(new { error = exception.Message });
        }
        catch (DomainException exception)
        {
            return Results.BadRequest(new { error = exception.Message });
        }
    }
}