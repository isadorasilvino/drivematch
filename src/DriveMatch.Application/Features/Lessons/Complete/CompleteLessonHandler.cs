using DriveMatch.Application.Abstractions.Persistence;

namespace DriveMatch.Application.Features.Lessons.Complete;

public sealed class CompleteLessonHandler
{
    private readonly ILessonRepository _lessonRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CompleteLessonHandler(
        ILessonRepository lessonRepository,
        IUnitOfWork unitOfWork)
    {
        _lessonRepository = lessonRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CompleteLessonResult> HandleAsync(
        CompleteLessonCommand command,
        CancellationToken cancellationToken = default)
    {
        var lesson = await _lessonRepository.GetByIdAsync(
            command.LessonId,
            cancellationToken);

        if (lesson is null)
            throw new LessonNotFoundException(command.LessonId);

        lesson.Complete();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CompleteLessonResult(
            lesson.Id,
            lesson.Status,
            lesson.CompletedAt);
    }
}
