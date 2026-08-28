namespace DriveMatch.Application.Features.Availabilities;

public sealed class AvailabilityForbiddenException : Exception
{
    public AvailabilityForbiddenException()
        : base("O instrutor autenticado não possui permissão para alterar esta disponibilidade.")
    {
    }
}