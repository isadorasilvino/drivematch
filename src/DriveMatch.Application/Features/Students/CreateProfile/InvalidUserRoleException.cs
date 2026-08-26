namespace DriveMatch.Application.Features.Students.CreateProfile;

public sealed class InvalidUserRoleException : Exception
{
    public InvalidUserRoleException()
        : base("Somente usuários do tipo Student podem possuir perfil de aluno.")
    {
    }
}
