using DriveMatch.Application.Abstractions.Persistence;

namespace DriveMatch.Application.Features.Lessons.MarkAsNotAttended;

public sealed class MarkLessonAsNotAttendedHandler
{
    private readonly ILessonRepository _lessonRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MarkLessonAsNotAttendedHandler(
        ILessonRepository lessonRepository,
        IUnitOfWork unitOfWork)
    {
        _lessonRepository = lessonRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<MarkLessonAsNotAttendedResult> HandleAsync(
        MarkLessonAsNotAttendedCommand command,
        CancellationToken cancellationToken = default)
    {
        var lesson = await _lessonRepository.GetByIdAsync(
            command.LessonId,
            cancellationToken);

        if (lesson is null)
            throw new LessonNotFoundException(command.LessonId);

        lesson.MarkAsNotAttended();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new MarkLessonAsNotAttendedResult(
            lesson.Id,
            lesson.Status);
    }
}
