using DriveMatch.Application.Abstractions.Persistence;

namespace DriveMatch.Application.Features.Lessons.ConfirmCheckIn;

public sealed class ConfirmLessonCheckInHandler
{
    private readonly ILessonRepository _lessonRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ConfirmLessonCheckInHandler(
        ILessonRepository lessonRepository,
        IUnitOfWork unitOfWork)
    {
        _lessonRepository = lessonRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ConfirmLessonCheckInResult> HandleAsync(
        ConfirmLessonCheckInCommand command,
        CancellationToken cancellationToken = default)
    {
        var lesson = await _lessonRepository.GetByIdAsync(
            command.LessonId,
            cancellationToken);

        if (lesson is null)
            throw new LessonNotFoundException(command.LessonId);

        lesson.ConfirmCheckIn();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ConfirmLessonCheckInResult(
            lesson.Id,
            lesson.Status,
            lesson.CheckInAt,
            lesson.StartedAt);
    }
}
