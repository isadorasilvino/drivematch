namespace DriveMatch.Application.Features.Instructors.CreateProfile;

public sealed class InvalidUserRoleException : Exception
{
    public InvalidUserRoleException()
        : base("Somente usuários do tipo Instructor podem possuir perfil de instrutor.")
    {
    }
}
