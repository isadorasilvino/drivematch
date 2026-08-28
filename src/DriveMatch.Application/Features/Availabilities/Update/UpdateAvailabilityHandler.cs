using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Features.Availabilities;

namespace DriveMatch.Application.Features.Availabilities.Update;

public sealed class UpdateAvailabilityHandler
{
    private readonly IAvailabilityRepository _availabilityRepository;
    private readonly IInstructorProfileRepository _instructorProfileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAvailabilityHandler(
        IAvailabilityRepository availabilityRepository,
        IInstructorProfileRepository instructorProfileRepository,
        IUnitOfWork unitOfWork)
    {
        _availabilityRepository = availabilityRepository;
        _instructorProfileRepository = instructorProfileRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateAvailabilityResult> HandleAsync(
        UpdateAvailabilityCommand command,
        CancellationToken cancellationToken = default)
    {
        var availability = await _availabilityRepository.GetByIdAsync(
            command.AvailabilityId,
            cancellationToken);

        if (availability is null)
            throw new AvailabilityNotFoundException(command.AvailabilityId);

        var instructorProfile =
            await _instructorProfileRepository.GetByUserIdAsync(
                command.UserId,
                cancellationToken);

        if (instructorProfile is null ||
            instructorProfile.Id != availability.InstructorProfileId)
        {
            throw new AvailabilityForbiddenException();
        }

        availability.Update(
            command.DayOfWeek,
            command.StartTime,
            command.EndTime);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UpdateAvailabilityResult(
            availability.Id,
            availability.InstructorProfileId,
            availability.DayOfWeek,
            availability.StartTime,
            availability.EndTime,
            availability.IsActive);
    }
}