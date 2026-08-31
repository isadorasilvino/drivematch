using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Features.Lessons;

namespace DriveMatch.Application.Features.Lessons.ConfirmCheckIn;

public sealed class ConfirmLessonCheckInHandler
{
    private readonly ILessonRepository _lessonRepository;
    private readonly IStudentProfileRepository _studentProfileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ConfirmLessonCheckInHandler(
        ILessonRepository lessonRepository,
        IStudentProfileRepository studentProfileRepository,
        IUnitOfWork unitOfWork)
    {
        _lessonRepository = lessonRepository;
        _studentProfileRepository = studentProfileRepository;
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

        var studentProfile =
            await _studentProfileRepository.GetByUserIdAsync(
                command.UserId,
                cancellationToken);

        if (studentProfile is null ||
            studentProfile.Id != lesson.StudentId)
        {
            throw new LessonForbiddenException();
        }

        lesson.ConfirmCheckIn(command.CheckInToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ConfirmLessonCheckInResult(
            lesson.Id,
            lesson.Status,
            lesson.CheckInAt,
            lesson.StartedAt);
    }
}