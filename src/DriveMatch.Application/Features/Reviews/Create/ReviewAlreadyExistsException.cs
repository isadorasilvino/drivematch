namespace DriveMatch.Application.Features.Reviews.Create;

public sealed class ReviewAlreadyExistsException : Exception
{
    public ReviewAlreadyExistsException(Guid lessonId)
        : base($"A aula '{lessonId}' já possui uma avaliação.")
    {
    }
}
