namespace DriveMatch.Application.Features.Instructors.UpdateProfile;

public sealed class InstructorProfileNotFoundException : Exception
{
    public InstructorProfileNotFoundException(Guid userId)
        : base($"Perfil de instrutor não encontrado para o usuário '{userId}'.")
    {
    }
}
