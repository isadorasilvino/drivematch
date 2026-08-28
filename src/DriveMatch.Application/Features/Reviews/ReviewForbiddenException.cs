namespace DriveMatch.Application.Features.Reviews;

public sealed class ReviewForbiddenException : Exception
{
    public ReviewForbiddenException()
        : base("O aluno autenticado não possui permissão para avaliar esta aula.")
    {
    }
}