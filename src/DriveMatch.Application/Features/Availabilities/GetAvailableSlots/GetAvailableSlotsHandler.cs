using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Abstractions.Time;
using DriveMatch.Domain.Enums;

namespace DriveMatch.Application.Features.Availabilities.GetAvailableSlots;

public sealed class GetAvailableSlotsHandler
{
    private readonly IInstructorProfileRepository _instructorProfileRepository;
    private readonly IAvailabilityRepository _availabilityRepository;
    private readonly ILessonRepository _lessonRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public GetAvailableSlotsHandler(
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

    public async Task<IReadOnlyCollection<AvailableSlotResult>> HandleAsync(
        GetAvailableSlotsQuery query,
        CancellationToken cancellationToken = default)
    {
        var instructorProfile =
            await _instructorProfileRepository.GetByIdAsync(
                query.InstructorProfileId,
                cancellationToken);

        if (instructorProfile is null)
            throw new InstructorProfileNotFoundException(
                query.InstructorProfileId);

        if (instructorProfile.Status != InstructorProfileStatus.Active)
            throw new InstructorUnavailableException();

        var now = _dateTimeProvider.LocalNow;
        var today = DateOnly.FromDateTime(now);

        if (query.Date < today)
            return Array.Empty<AvailableSlotResult>();

        var availabilities =
            await _availabilityRepository.GetActiveByInstructorProfileIdAndDayAsync(
                instructorProfile.Id,
                query.Date.DayOfWeek,
                cancellationToken);

        if (availabilities.Count == 0)
            return Array.Empty<AvailableSlotResult>();

        var blockingSchedule =
            await _lessonRepository.GetBlockingScheduleAsync(
                instructorProfile.Id,
                query.Date,
                cancellationToken);

        var currentTime = TimeOnly.FromDateTime(now);

        return availabilities
            .SelectMany(availability => availability.GetSlots())
            .Where(slot =>
                query.Date > today ||
                slot.StartTime > currentTime)
            .Where(slot => !blockingSchedule.Any(lesson =>
                slot.StartTime < lesson.EndTime &&
                slot.EndTime > lesson.StartTime))
            .Distinct()
            .OrderBy(slot => slot.StartTime)
            .Select(slot => new AvailableSlotResult(
                slot.StartTime,
                slot.EndTime))
            .ToArray();
    }
}