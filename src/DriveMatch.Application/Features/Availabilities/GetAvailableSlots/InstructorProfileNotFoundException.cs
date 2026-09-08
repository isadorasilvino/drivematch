namespace DriveMatch.Application.Features.Availabilities.GetAvailableSlots;

public sealed class InstructorProfileNotFoundException : Exception
{
    public InstructorProfileNotFoundException(Guid instructorProfileId)
        : base($"Perfil de instrutor '{instructorProfileId}' não encontrado.")
    {
    }
}