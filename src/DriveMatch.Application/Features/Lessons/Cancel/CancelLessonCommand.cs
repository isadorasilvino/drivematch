// Cancel/CancelLessonCommand.cs
namespace DriveMatch.Application.Features.Lessons.Cancel;

public sealed record CancelLessonCommand(
    Guid LessonId,
    Guid UserId);