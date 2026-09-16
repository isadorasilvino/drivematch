using DriveMatch.Application.Abstractions.Persistence;

namespace DriveMatch.Application.Features.Lessons.GetMine;

public sealed class GetMineLessonsHandler
{
    private readonly ILessonRepository _lessonRepository;

    public GetMineLessonsHandler(
        ILessonRepository lessonRepository)
    {
        _lessonRepository = lessonRepository;
    }

    public async Task<LessonListResult> HandleAsync(
        GetMineLessonsQuery query,
        CancellationToken cancellationToken = default)
    {
        var lessons = await _lessonRepository.GetByStudentUserIdAsync(
            query.UserId,
            cancellationToken);

        return new LessonListResult(lessons);
    }
}
