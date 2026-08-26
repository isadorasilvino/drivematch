namespace DriveMatch.Application.Features.LessonRequests.Create;

public sealed class StudentVehicleNotAcceptedException : Exception
{
    public StudentVehicleNotAcceptedException(Guid instructorProfileId)
        : base($"O instrutor '{instructorProfileId}' não aceita aulas com veículo do aluno.")
    {
    }
}
