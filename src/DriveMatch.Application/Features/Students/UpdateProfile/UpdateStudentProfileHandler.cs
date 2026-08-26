using DriveMatch.Application.Abstractions.Persistence;

namespace DriveMatch.Application.Features.Students.UpdateProfile;

public sealed class UpdateStudentProfileHandler
{
    private readonly IStudentProfileRepository _studentProfileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateStudentProfileHandler(
        IStudentProfileRepository studentProfileRepository,
        IUnitOfWork unitOfWork)
    {
        _studentProfileRepository = studentProfileRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateStudentProfileResult> HandleAsync(
        UpdateStudentProfileCommand command,
        CancellationToken cancellationToken = default)
    {
        var profile = await _studentProfileRepository.GetByUserIdAsync(
            command.UserId,
            cancellationToken);

        if (profile is null)
            throw new StudentProfileNotFoundException(command.UserId);

        profile.UpdateLocation(
            command.City,
            command.State);

        profile.UpdateExperienceLevel(
            command.ExperienceLevel);

        profile.UpdateVehiclePreferences(
            command.OwnsVehicle,
            command.HasOwnVehicleForLessons);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new UpdateStudentProfileResult(
            profile.Id,
            profile.UserId,
            profile.City,
            profile.State,
            profile.ExperienceLevel,
            profile.OwnsVehicle,
            profile.HasOwnVehicleForLessons,
            profile.UpdatedAt);
    }
}
