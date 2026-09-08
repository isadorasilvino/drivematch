namespace DriveMatch.Application.Abstractions.Persistence.Models;

public sealed record LessonScheduleItem(
    TimeOnly StartTime,
    TimeOnly EndTime);