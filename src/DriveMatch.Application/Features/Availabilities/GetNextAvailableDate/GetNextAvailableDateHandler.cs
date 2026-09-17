using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Abstractions.Time;
using DriveMatch.Domain.Enums;

namespace DriveMatch.Application.Features.Availabilities.GetNextAvailableDate;

public sealed class GetNextAvailableDateHandler
{
    private const int SearchWindowDays = 60;

    private readonly IInstructorProfileRepository _instructorProfileRepository;
    private readonly IAvailabilityRepository _availabilityRepository;
    private readonly ILessonRepository _lessonRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public GetNextAvailableDateHandler(
        IInstructorProfileRepository instructorProfileRepository,
        IAvailabilityRepository availabilityRepository,
        ILessonRepository lessonRepository,
        IDateTimeProvider dateTimeProvider)
    {
        _instructorProfileRepository = instructorProfileRepository;
        _availabilityRepository = availabilityRepository;
        _lessonRepository = lessonRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<GetNextAvailableDateResult?> HandleAsync(
        GetNextAvailableDateQuery query,
        CancellationToken cancellationToken = default)
    {
        var instructorProfile =
            await _instructorProfileRepository.GetByIdAsync(
                query.InstructorProfileId,
                cancellationToken);

        if (instructorProfile is null)
            return null;

        if (instructorProfile.Status != InstructorProfileStatus.Active)
            return null;

        var availabilities =
            await _availabilityRepository.GetByInstructorProfileIdAsync(
                instructorProfile.Id,
                cancellationToken);

        var activeAvailabilities =
            availabilities
                .Where(availability => availability.IsActive)
                .ToArray();

        if (activeAvailabilities.Length == 0)
            return null;

        var now = _dateTimeProvider.LocalNow;
        var today = DateOnly.FromDateTime(now);
        var currentTime = TimeOnly.FromDateTime(now);

        for (var offset = 0; offset <= SearchWindowDays; offset++)
        {
            var date = today.AddDays(offset);

            var availabilitiesForDay =
                activeAvailabilities
                    .Where(availability =>
                        availability.DayOfWeek == date.DayOfWeek)
                    .ToArray();

            if (availabilitiesForDay.Length == 0)
                continue;

            var blockingSchedule =
                await _lessonRepository.GetBlockingScheduleAsync(
                    instructorProfile.Id,
                    date,
                    cancellationToken);

            var hasAvailableSlot =
                availabilitiesForDay
                    .SelectMany(availability =>
                        availability.GetSlots())
                    .Where(slot =>
                        date > today ||
                        slot.StartTime > currentTime)
                    .Distinct()
                    .Any(slot =>
                        !blockingSchedule.Any(lesson =>
                            slot.StartTime < lesson.EndTime &&
                            slot.EndTime > lesson.StartTime));

            if (hasAvailableSlot)
                return new GetNextAvailableDateResult(date);
        }

        return null;
    }
}
