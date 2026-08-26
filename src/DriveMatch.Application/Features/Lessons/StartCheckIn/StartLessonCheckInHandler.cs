using DriveMatch.Application.Abstractions.Persistence;

namespace DriveMatch.Application.Features.Lessons.StartCheckIn;

public sealed class StartLessonCheckInHandler
{
    private readonly ILessonRepository _lessonRepository;
    private readonly IUnitOfWork _unitOfWork;

    public StartLessonCheckInHandler(
        ILessonRepository lessonRepository,
        IUnitOfWork unitOfWork)
    {
        _lessonRepository = lessonRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<StartLessonCheckInResult> HandleAsync(
        StartLessonCheckInCommand command,
        CancellationToken cancellationToken = default)
    {
        var lesson = await _lessonRepository.GetByIdAsync(
            command.LessonId,
            cancellationToken);

        if (lesson is null)
            throw new LessonNotFoundException(command.LessonId);

        lesson.StartCheckIn();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new StartLessonCheckInResult(
            lesson.Id,
            lesson.Status);
    }
}
