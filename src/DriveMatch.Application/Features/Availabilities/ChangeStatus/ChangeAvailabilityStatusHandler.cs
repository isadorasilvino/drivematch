using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Features.Availabilities;

namespace DriveMatch.Application.Features.Availabilities.ChangeStatus;

public sealed class ChangeAvailabilityStatusHandler
{
    private readonly IAvailabilityRepository _availabilityRepository;
    private readonly IInstructorProfileRepository _instructorProfileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ChangeAvailabilityStatusHandler(
        IAvailabilityRepository availabilityRepository,
        IInstructorProfileRepository instructorProfileRepository,
        IUnitOfWork unitOfWork)
    {
        _availabilityRepository = availabilityRepository;
        _instructorProfileRepository = instructorProfileRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ChangeAvailabilityStatusResult> HandleAsync(
        ChangeAvailabilityStatusCommand command,
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

        if (command.IsActive)
            availability.Activate();
        else
            availability.Deactivate();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ChangeAvailabilityStatusResult(
            availability.Id,
            availability.IsActive);
    }
}