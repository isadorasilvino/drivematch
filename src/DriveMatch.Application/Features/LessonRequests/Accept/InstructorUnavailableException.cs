namespace DriveMatch.Application.Features.LessonRequests.Accept;

public sealed class InstructorUnavailableException : Exception
{
    public InstructorUnavailableException()
        : base("O instrutor não possui mais disponibilidade para o horário solicitado.")
    {
    }
}
