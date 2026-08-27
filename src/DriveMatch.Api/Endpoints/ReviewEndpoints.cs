using DriveMatch.Application.Features.Reviews.Create;
using DriveMatch.Domain.Exceptions;

namespace DriveMatch.Api.Endpoints;

public static class ReviewEndpoints
{
    public static IEndpointRouteBuilder MapReviewEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/api/reviews")
            .WithTags("Reviews")
            .RequireAuthorization(policy =>
                policy.RequireRole("Student"));

        group.MapPost("/", CreateAsync)
            .WithName("CreateReview")
            .Produces<CreateReviewResult>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict);

        return endpoints;
    }

    private static async Task<IResult> CreateAsync(
        CreateReviewRequest request,
        CreateReviewHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await handler.HandleAsync(
                new CreateReviewCommand(
                    request.LessonId,
                    request.Rating,
                    request.Comment),
                cancellationToken);

            return Results.Created(
                $"/api/reviews/{result.ReviewId}",
                result);
        }
        catch (LessonNotFoundException exception)
        {
            return Results.NotFound(new { error = exception.Message });
        }
        catch (LessonNotCompletedException exception)
        {
            return Results.BadRequest(new { error = exception.Message });
        }
        catch (ReviewAlreadyExistsException exception)
        {
            return Results.Conflict(new { error = exception.Message });
        }
        catch (DomainException exception)
        {
            return Results.BadRequest(new { error = exception.Message });
        }
    }

    public sealed record CreateReviewRequest(
        Guid LessonId,
        int Rating,
        string? Comment);
}