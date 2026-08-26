using DriveMatch.Application.Abstractions.Persistence;

namespace DriveMatch.Application.Features.Availabilities.Update;

public sealed class UpdateAvailabilityHandler
{
    private readonly IAvailabilityRepository _availabilityRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAvailabilityHandler(
        IAvailabilityRepository availabilityRepository,
        IUnitOfWork unitOfWork)
    {
        _availabilityRepository = availabilityRepository;
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
