namespace DriveMatch.Application.Features.Lessons;

public sealed class LessonForbiddenException : Exception
{
    public LessonForbiddenException()
        : base("O instrutor autenticado não possui permissão para alterar esta aula.")
    {
    }
}