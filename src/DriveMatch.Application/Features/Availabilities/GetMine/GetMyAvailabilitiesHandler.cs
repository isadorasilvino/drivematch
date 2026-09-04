using DriveMatch.Application.Abstractions.Persistence;

namespace DriveMatch.Application.Features.Availabilities.GetMine;

public sealed class GetMyAvailabilitiesHandler
{
    private readonly IInstructorProfileRepository _instructorProfileRepository;
    private readonly IAvailabilityRepository _availabilityRepository;

    public GetMyAvailabilitiesHandler(
        IInstructorProfileRepository instructorProfileRepository,
        IAvailabilityRepository availabilityRepository)
    {
        _instructorProfileRepository = instructorProfileRepository;
        _availabilityRepository = availabilityRepository;
    }

    public async Task<IReadOnlyCollection<GetMyAvailabilitiesResult>> HandleAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var instructorProfile =
            await _instructorProfileRepository.GetByUserIdAsync(
                userId,
                cancellationToken);

        if (instructorProfile is null)
            throw new InstructorProfileNotFoundException(userId);

        var availabilities =
            await _availabilityRepository.GetByInstructorProfileIdAsync(
                instructorProfile.Id,
                cancellationToken);

        return availabilities
            .Select(availability => new GetMyAvailabilitiesResult(
                availability.Id,
                availability.InstructorProfileId,
                availability.DayOfWeek,
                availability.StartTime,
                availability.EndTime,
                availability.IsActive))
            .ToArray();
    }
}