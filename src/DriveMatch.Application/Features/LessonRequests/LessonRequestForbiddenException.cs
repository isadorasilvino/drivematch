namespace DriveMatch.Application.Features.LessonRequests;

public sealed class LessonRequestForbiddenException : Exception
{
    public LessonRequestForbiddenException()
        : base("O instrutor autenticado não possui permissão para alterar esta solicitação.")
    {
    }
}