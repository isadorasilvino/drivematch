using DriveMatch.Application.Abstractions.Persistence;

namespace DriveMatch.Application.Features.Instructors.Search;

public sealed class SearchInstructorsHandler
{
    private readonly IInstructorProfileRepository _instructorProfileRepository;

    public SearchInstructorsHandler(
        IInstructorProfileRepository instructorProfileRepository)
    {
        _instructorProfileRepository = instructorProfileRepository;
    }

    public async Task<IReadOnlyCollection<SearchInstructorResult>> HandleAsync(
        SearchInstructorsQuery query,
        CancellationToken cancellationToken = default)
    {
        var city = query.City.Trim();
        var state = query.State.Trim().ToUpperInvariant();

        var instructors = await _instructorProfileRepository.SearchAsync(
            city,
            state,
            query.ExperienceLevel,
            query.UsesStudentVehicle,
            query.MaxPricePerLesson,
            cancellationToken);

        return instructors
            .Select(instructor => new SearchInstructorResult(
                instructor.Id,
                instructor.UserId,
                instructor.Description,
                instructor.ExperienceYears,
                instructor.City,
                instructor.State,
                instructor.PricePerLesson.Amount,
                instructor.PricePerLesson.Currency,
                instructor.AcceptsBeginners,
                instructor.AcceptsExperiencedStudents,
                instructor.AcceptsStudentVehicle))
            .ToArray();
    }
}
