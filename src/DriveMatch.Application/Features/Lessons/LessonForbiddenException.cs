namespace DriveMatch.Application.Features.Lessons;

public sealed class LessonForbiddenException : Exception
{
    public LessonForbiddenException()
        : base("O usuário autenticado não possui permissão para alterar esta aula.")
    {
    }
}