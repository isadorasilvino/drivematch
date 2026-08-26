namespace DriveMatch.Application.Features.Reviews.Create;

public sealed record CreateReviewResult(
    Guid ReviewId,
    Guid LessonId,
    Guid StudentId,
    Guid InstructorId,
    int Rating,
    string? Comment,
    DateTime CreatedAt);
