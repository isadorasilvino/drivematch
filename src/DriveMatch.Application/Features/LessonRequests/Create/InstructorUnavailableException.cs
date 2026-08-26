namespace DriveMatch.Application.Features.LessonRequests.Create;

public sealed class InstructorUnavailableException : Exception
{
    public InstructorUnavailableException()
        : base("O instrutor não possui disponibilidade para o horário solicitado.")
    {
    }
}
