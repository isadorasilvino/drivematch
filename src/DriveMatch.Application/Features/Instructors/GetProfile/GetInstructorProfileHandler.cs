using DriveMatch.Application.Abstractions.Persistence;

namespace DriveMatch.Application.Features.Instructors.GetProfile;

public sealed class GetInstructorProfileHandler
{
    private readonly IInstructorProfileRepository _instructorProfileRepository;

    public GetInstructorProfileHandler(
        IInstructorProfileRepository instructorProfileRepository)
    {
        _instructorProfileRepository = instructorProfileRepository;
    }

    public async Task<GetInstructorProfileResult> HandleAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var profile = await _instructorProfileRepository.GetByUserIdAsync(
            userId,
            cancellationToken);

        if (profile is null)
            throw new InstructorProfileNotFoundException(userId);

        return new GetInstructorProfileResult(
            profile.Id,
            profile.UserId,
            profile.Description,
            profile.ExperienceYears,
            profile.City,
            profile.State,
            profile.PricePerLesson.Amount,
            profile.PricePerLesson.Currency,
            profile.AcceptsBeginners,
            profile.AcceptsExperiencedStudents,
            profile.AcceptsStudentVehicle,
            profile.Status);
    }
}