using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Features.Lessons;

namespace DriveMatch.Application.Features.Lessons.Cancel;

public sealed class CancelLessonHandler
{
    private readonly ILessonRepository _lessonRepository;
    private readonly IInstructorProfileRepository _instructorProfileRepository;
    private readonly IStudentProfileRepository _studentProfileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelLessonHandler(
        ILessonRepository lessonRepository,
        IInstructorProfileRepository instructorProfileRepository,
        IStudentProfileRepository studentProfileRepository,
        IUnitOfWork unitOfWork)
    {
        _lessonRepository = lessonRepository;
        _instructorProfileRepository = instructorProfileRepository;
        _studentProfileRepository = studentProfileRepository;
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

        var instructorProfile =
            await _instructorProfileRepository.GetByUserIdAsync(
                command.UserId,
                cancellationToken);

        var isInstructor =
            instructorProfile?.Id == lesson.InstructorId;

        if (!isInstructor)
        {
            var studentProfile =
                await _studentProfileRepository.GetByUserIdAsync(
                    command.UserId,
                    cancellationToken);

            var isStudent =
                studentProfile?.Id == lesson.StudentId;

            if (!isStudent)
                throw new LessonForbiddenException();
        }

        lesson.Cancel();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CancelLessonResult(
            lesson.Id,
            lesson.Status,
            lesson.CancelledAt);
    }
}