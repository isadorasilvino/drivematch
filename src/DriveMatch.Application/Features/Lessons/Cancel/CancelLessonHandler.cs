using DriveMatch.Application.Abstractions.Persistence;

namespace DriveMatch.Application.Features.Lessons.Cancel;

public sealed class CancelLessonHandler
{
    private readonly ILessonRepository _lessonRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelLessonHandler(
        ILessonRepository lessonRepository,
        IUnitOfWork unitOfWork)
    {
        _lessonRepository = lessonRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CancelLessonResult> HandleAsync(
        CancelLessonCommand command,
        CancellationToken cancellationToken = default)
    {
        var lesson = await _lessonRepository.GetByIdAsync(
            command.LessonId,
            cancellationToken);

        if (lesson is null)
            throw new LessonNotFoundException(command.LessonId);

        lesson.Cancel();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CancelLessonResult(
            lesson.Id,
            lesson.Status,
            lesson.CancelledAt);
    }
}
