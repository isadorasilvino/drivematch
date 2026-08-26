namespace DriveMatch.Application.Features.Availabilities.Update;

public sealed class AvailabilityNotFoundException : Exception
{
    public AvailabilityNotFoundException(Guid availabilityId)
        : base($"Disponibilidade '{availabilityId}' não encontrada.")
    {
    }
}
