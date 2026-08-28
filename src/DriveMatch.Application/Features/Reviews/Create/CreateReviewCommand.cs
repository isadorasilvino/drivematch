namespace DriveMatch.Application.Features.Reviews.Create;

public sealed record CreateReviewCommand(
    Guid LessonId,
    Guid UserId,
    int Rating,
    string? Comment);