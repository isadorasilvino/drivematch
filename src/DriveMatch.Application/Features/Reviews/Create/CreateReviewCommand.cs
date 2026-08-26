namespace DriveMatch.Application.Features.Reviews.Create;

public sealed record CreateReviewCommand(
    Guid LessonId,
    int Rating,
    string? Comment);
