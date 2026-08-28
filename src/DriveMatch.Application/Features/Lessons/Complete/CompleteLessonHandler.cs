using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Features.Lessons;

namespace DriveMatch.Application.Features.Lessons.Complete;

public sealed class CompleteLessonHandler
{
    private readonly ILessonRepository _lessonRepository;
    private readonly IInstructorProfileRepository _instructorProfileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CompleteLessonHandler(
        ILessonRepository lessonRepository,
        IInstructorProfileRepository instructorProfileRepository,
        IUnitOfWork unitOfWork)
    {
        _lessonRepository = lessonRepository;
        _instructorProfileRepository = instructorProfileRepository;
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

        var instructorProfile =
            await _instructorProfileRepository.GetByUserIdAsync(
                command.UserId,
                cancellationToken);

        if (instructorProfile is null ||
            instructorProfile.Id != lesson.InstructorId)
        {
            throw new LessonForbiddenException();
        }

        lesson.Complete();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CompleteLessonResult(
            lesson.Id,
            lesson.Status,
            lesson.CompletedAt);
    }
}