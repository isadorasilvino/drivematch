namespace DriveMatch.Application.Features.Instructors.ChangeStatus;

public sealed class InstructorProfileCannotBeActivatedException : Exception
{
    public InstructorProfileCannotBeActivatedException()
        : base(
            "O perfil do instrutor não pode ser ativado sem pelo menos uma disponibilidade ativa.")
    {
    }
}