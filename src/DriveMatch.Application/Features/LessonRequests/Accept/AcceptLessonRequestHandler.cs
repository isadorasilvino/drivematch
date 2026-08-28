using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Features.LessonRequests;
using DriveMatch.Domain.Entities;

namespace DriveMatch.Application.Features.LessonRequests.Accept;

public sealed class AcceptLessonRequestHandler
{
    private readonly ILessonRequestRepository _lessonRequestRepository;
    private readonly IAvailabilityRepository _availabilityRepository;
    private readonly ILessonRepository _lessonRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IInstructorProfileRepository _instructorProfileRepository;

    public AcceptLessonRequestHandler(
        ILessonRequestRepository lessonRequestRepository,
        IAvailabilityRepository availabilityRepository,
        ILessonRepository lessonRepository,
        IInstructorProfileRepository instructorProfileRepository,
        IUnitOfWork unitOfWork)
    {
        _lessonRequestRepository = lessonRequestRepository;
        _availabilityRepository = availabilityRepository;
        _lessonRepository = lessonRepository;
        _instructorProfileRepository = instructorProfileRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AcceptLessonRequestResult> HandleAsync(
    AcceptLessonRequestCommand command,
    CancellationToken cancellationToken = default)
    {
        var lessonRequest = await _lessonRequestRepository.GetByIdAsync(
            command.LessonRequestId,
            cancellationToken);

        if (lessonRequest is null)
            throw new LessonRequestNotFoundException(command.LessonRequestId);

        var instructorProfile =
            await _instructorProfileRepository.GetByUserIdAsync(
                command.UserId,
                cancellationToken);

        if (instructorProfile is null ||
            instructorProfile.Id != lessonRequest.InstructorId)
        {
            throw new LessonRequestForbiddenException();
        }

        var hasAvailability =
            await _availabilityRepository.HasAvailabilityAsync(
                lessonRequest.InstructorId,
                lessonRequest.RequestedDate.DayOfWeek,
                lessonRequest.StartTime,
                lessonRequest.EndTime,
                cancellationToken);

        if (!hasAvailability)
            throw new InstructorUnavailableException();

        var hasConflict = await _lessonRepository.HasConflictAsync(
            lessonRequest.InstructorId,
            lessonRequest.RequestedDate,
            lessonRequest.StartTime,
            lessonRequest.EndTime,
            cancellationToken);

        if (hasConflict)
            throw new LessonScheduleConflictException();

        lessonRequest.Accept();
        lessonRequest.Confirm();

        var lesson = new Lesson(
            Guid.NewGuid(),
            lessonRequest.StudentId,
            lessonRequest.InstructorId,
            lessonRequest.Id,
            lessonRequest.RequestedDate,
            lessonRequest.StartTime,
            lessonRequest.EndTime);

        await _lessonRepository.AddAsync(
            lesson,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new AcceptLessonRequestResult(
            lessonRequest.Id,
            lessonRequest.Status,
            lesson.Id,
            lesson.Status,
            lesson.StudentId,
            lesson.InstructorId,
            lesson.ScheduledDate,
            lesson.StartTime,
            lesson.EndTime);
    }
}
