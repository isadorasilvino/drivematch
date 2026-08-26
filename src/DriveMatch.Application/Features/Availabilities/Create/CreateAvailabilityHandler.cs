using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Domain.Entities;

namespace DriveMatch.Application.Features.Availabilities.Create;

public sealed class CreateAvailabilityHandler
{
    private readonly IInstructorProfileRepository _instructorProfileRepository;
    private readonly IAvailabilityRepository _availabilityRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAvailabilityHandler(
        IInstructorProfileRepository instructorProfileRepository,
        IAvailabilityRepository availabilityRepository,
        IUnitOfWork unitOfWork)
    {
        _instructorProfileRepository = instructorProfileRepository;
        _availabilityRepository = availabilityRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateAvailabilityResult> HandleAsync(
        CreateAvailabilityCommand command,
        CancellationToken cancellationToken = default)
    {
        var instructorProfile =
            await _instructorProfileRepository.GetByUserIdAsync(
                command.UserId,
                cancellationToken);

        if (instructorProfile is null)
            throw new InstructorProfileNotFoundException(command.UserId);

        var availability = new Availability(
            Guid.NewGuid(),
            instructorProfile.Id,
            command.DayOfWeek,
            command.StartTime,
            command.EndTime);

        await _availabilityRepository.AddAsync(
            availability,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateAvailabilityResult(
            availability.Id,
            availability.InstructorProfileId,
            availability.DayOfWeek,
            availability.StartTime,
            availability.EndTime);
    }
}
