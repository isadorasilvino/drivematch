namespace DriveMatch.Application.Features.LessonRequests.Accept;

public sealed class LessonScheduleConflictException : Exception
{
    public LessonScheduleConflictException()
        : base("O instrutor já possui uma aula confirmada nesse intervalo.")
    {
    }
}
