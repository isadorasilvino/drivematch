using DriveMatch.Application.Abstractions.Persistence.Models;

namespace DriveMatch.Application.Features.Lessons;

public sealed record LessonListResult(
    IReadOnlyCollection<LessonListItem> Lessons);
