using DriveMatch.Application.Abstractions.Persistence;

namespace DriveMatch.Application.Features.Availabilities.ChangeStatus;

public sealed class ChangeAvailabilityStatusHandler
{
    private readonly IAvailabilityRepository _availabilityRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ChangeAvailabilityStatusHandler(
        IAvailabilityRepository availabilityRepository,
        IUnitOfWork unitOfWork)
    {
        _availabilityRepository = availabilityRepository;
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
