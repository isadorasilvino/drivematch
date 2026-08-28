using DriveMatch.Application.Features.Reviews.Create;
using DriveMatch.Application.Features.Reviews;
using DriveMatch.Domain.Exceptions;
using DriveMatch.Api.Extensions;

using System.Security.Claims;

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
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict);

        return endpoints;
    }

    private static async Task<IResult> CreateAsync(
        CreateReviewRequest request,
        CreateReviewHandler handler,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = user.GetUserId();

            var result = await handler.HandleAsync(
                new CreateReviewCommand(
                    request.LessonId,
                    userId,
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
        catch (ReviewForbiddenException exception)
        {
            return Results.Json(
                new { error = exception.Message },
                statusCode: StatusCodes.Status403Forbidden);
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