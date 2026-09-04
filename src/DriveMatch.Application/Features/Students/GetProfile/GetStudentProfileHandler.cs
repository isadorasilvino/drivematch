using DriveMatch.Application.Abstractions.Persistence;

namespace DriveMatch.Application.Features.Students.GetProfile;

public sealed class GetStudentProfileHandler
{
    private readonly IStudentProfileRepository _studentProfileRepository;

    public GetStudentProfileHandler(
        IStudentProfileRepository studentProfileRepository)
    {
        _studentProfileRepository = studentProfileRepository;
    }

    public async Task<GetStudentProfileResult> HandleAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var profile = await _studentProfileRepository.GetByUserIdAsync(
            userId,
            cancellationToken);

        if (profile is null)
            throw new StudentProfileNotFoundException(userId);

        return new GetStudentProfileResult(
            profile.Id,
            profile.UserId,
            profile.City,
            profile.State,
            profile.ExperienceLevel,
            profile.OwnsVehicle,
            profile.HasOwnVehicleForLessons);
    }
}