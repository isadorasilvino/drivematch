namespace DriveMatch.Application.Features.Instructors.CreateProfile;

public sealed class InstructorProfileAlreadyExistsException : Exception
{
    public InstructorProfileAlreadyExistsException(Guid userId)
        : base($"O usuário '{userId}' já possui um perfil de instrutor.")
    {
    }
}
