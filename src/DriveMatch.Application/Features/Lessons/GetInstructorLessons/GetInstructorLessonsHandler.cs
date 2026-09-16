using DriveMatch.Application.Abstractions.Persistence;

namespace DriveMatch.Application.Features.Lessons.GetInstructorLessons;

public sealed class GetInstructorLessonsHandler
{
    private readonly ILessonRepository _lessonRepository;

    public GetInstructorLessonsHandler(
        ILessonRepository lessonRepository)
    {
        _lessonRepository = lessonRepository;
    }

    public async Task<LessonListResult> HandleAsync(
        GetInstructorLessonsQuery query,
        CancellationToken cancellationToken = default)
    {
        var lessons = await _lessonRepository.GetByInstructorUserIdAsync(
            query.UserId,
            cancellationToken);

        return new LessonListResult(lessons);
    }
}
