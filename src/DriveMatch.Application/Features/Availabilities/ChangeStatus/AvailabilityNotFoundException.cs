namespace DriveMatch.Application.Features.Availabilities.ChangeStatus;

public sealed class AvailabilityNotFoundException : Exception
{
    public AvailabilityNotFoundException(Guid availabilityId)
        : base($"Disponibilidade '{availabilityId}' não encontrada.")
    {
    }
}
