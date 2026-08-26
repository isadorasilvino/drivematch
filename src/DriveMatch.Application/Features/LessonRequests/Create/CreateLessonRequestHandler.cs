using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Domain.Entities;
using DriveMatch.Domain.Enums;

namespace DriveMatch.Application.Features.LessonRequests.Create;

public sealed class CreateLessonRequestHandler
{
    private readonly IStudentProfileRepository _studentProfileRepository;
    private readonly IInstructorProfileRepository _instructorProfileRepository;
    private readonly IAvailabilityRepository _availabilityRepository;
    private readonly ILessonRequestRepository _lessonRequestRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateLessonRequestHandler(
        IStudentProfileRepository studentProfileRepository,
        IInstructorProfileRepository instructorProfileRepository,
        IAvailabilityRepository availabilityRepository,
        ILessonRequestRepository lessonRequestRepository,
        IUnitOfWork unitOfWork)
    {
        _studentProfileRepository = studentProfileRepository;
        _instructorProfileRepository = instructorProfileRepository;
        _availabilityRepository = availabilityRepository;
        _lessonRequestRepository = lessonRequestRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateLessonRequestResult> HandleAsync(
        CreateLessonRequestCommand command,
        CancellationToken cancellationToken = default)
    {
        var studentProfile =
            await _studentProfileRepository.GetByIdAsync(
                command.StudentProfileId,
                cancellationToken);

        if (studentProfile is null)
            throw new StudentProfileNotFoundException(
                command.StudentProfileId);

        var instructorProfile =
            await _instructorProfileRepository.GetByIdAsync(
                command.InstructorProfileId,
                cancellationToken);

        if (instructorProfile is null)
            throw new InstructorProfileNotFoundException(
                command.InstructorProfileId);

        if (instructorProfile.Status != InstructorProfileStatus.Active)
            throw new InstructorNotActiveException(
                instructorProfile.Id);

        if (command.UsesStudentVehicle &&
            !instructorProfile.AcceptsStudentVehicle)
        {
            throw new StudentVehicleNotAcceptedException(
                instructorProfile.Id);
        }

        var hasAvailability =
            await _availabilityRepository.HasAvailabilityAsync(
                instructorProfile.Id,
                command.RequestedDate.DayOfWeek,
                command.StartTime,
                command.EndTime,
                cancellationToken);

        if (!hasAvailability)
            throw new InstructorUnavailableException();

        var lessonRequest = new LessonRequest(
            Guid.NewGuid(),
            studentProfile.Id,
            instructorProfile.Id,
            command.RequestedDate,
            command.StartTime,
            command.EndTime,
            command.UsesStudentVehicle,
            command.StudentMessage);

        await _lessonRequestRepository.AddAsync(
            lessonRequest,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new CreateLessonRequestResult(
            lessonRequest.Id,
            lessonRequest.StudentId,
            lessonRequest.InstructorId,
            lessonRequest.RequestedDate,
            lessonRequest.StartTime,
            lessonRequest.EndTime,
            lessonRequest.UsesStudentVehicle,
            lessonRequest.StudentMessage,
            lessonRequest.Status);
    }
}
