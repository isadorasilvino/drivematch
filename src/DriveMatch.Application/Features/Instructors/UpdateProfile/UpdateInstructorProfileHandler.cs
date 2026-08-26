using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Domain.ValueObjects;

namespace DriveMatch.Application.Features.Instructors.UpdateProfile;

public sealed class UpdateInstructorProfileHandler
{
    private readonly IInstructorProfileRepository _instructorProfileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateInstructorProfileHandler(
        IInstructorProfileRepository instructorProfileRepository,
        IUnitOfWork unitOfWork)
    {
        _instructorProfileRepository = instructorProfileRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateInstructorProfileResult> HandleAsync(
        UpdateInstructorProfileCommand command,
        CancellationToken cancellationToken = default)
    {
        var profile = await _instructorProfileRepository.GetByUserIdAsync(
            command.UserId,
            cancellationToken);

        if (profile is null)
            throw new InstructorProfileNotFoundException(command.UserId);

        profile.UpdateDescription(command.Description);
        profile.UpdateExperienceYears(command.ExperienceYears);
        profile.UpdateLocation(command.City, command.State);
        profile.UpdatePrice(new Money(command.PricePerLesson));

        profile.UpdateServicePreferences(
            command.AcceptsBeginners,
            command.AcceptsExperiencedStudents,
            command.AcceptsStudentVehicle);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UpdateInstructorProfileResult(
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
            profile.Status,
            profile.UpdatedAt);
    }
}
