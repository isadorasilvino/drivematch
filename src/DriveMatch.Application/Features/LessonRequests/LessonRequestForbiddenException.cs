namespace DriveMatch.Application.Features.LessonRequests;

public sealed class LessonRequestForbiddenException : Exception
{
    public LessonRequestForbiddenException()
        : base("O usuário autenticado não possui permissão para alterar esta solicitação.")
    {
    }
}