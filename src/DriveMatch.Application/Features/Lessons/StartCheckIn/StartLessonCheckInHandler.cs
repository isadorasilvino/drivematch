using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Features.Lessons;

namespace DriveMatch.Application.Features.Lessons.StartCheckIn;

public sealed class StartLessonCheckInHandler
{
    private readonly ILessonRepository _lessonRepository;
    private readonly IInstructorProfileRepository _instructorProfileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public StartLessonCheckInHandler(
        ILessonRepository lessonRepository,
        IInstructorProfileRepository instructorProfileRepository,
        IUnitOfWork unitOfWork)
    {
        _lessonRepository = lessonRepository;
        _instructorProfileRepository = instructorProfileRepository;
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

        var instructorProfile =
            await _instructorProfileRepository.GetByUserIdAsync(
                command.UserId,
                cancellationToken);

        if (instructorProfile is null ||
            instructorProfile.Id != lesson.InstructorId)
        {
            throw new LessonForbiddenException();
        }

        lesson.StartCheckIn();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new StartLessonCheckInResult(
            lesson.Id,
            lesson.Status);
    }
}