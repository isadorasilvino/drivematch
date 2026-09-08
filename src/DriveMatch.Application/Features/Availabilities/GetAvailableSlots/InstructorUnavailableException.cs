namespace DriveMatch.Application.Features.Availabilities.GetAvailableSlots;

public sealed class InstructorUnavailableException : Exception
{
    public InstructorUnavailableException()
        : base("O instrutor não está disponível para consulta de horários.")
    {
    }
}