using DriveMatch.Application.Abstractions.Persistence;

namespace DriveMatch.Application.Features.Instructors.ChangeStatus;

public sealed class ChangeInstructorProfileStatusHandler
{
    private readonly IInstructorProfileRepository _instructorProfileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ChangeInstructorProfileStatusHandler(
        IInstructorProfileRepository instructorProfileRepository,
        IUnitOfWork unitOfWork)
    {
        _instructorProfileRepository = instructorProfileRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ChangeInstructorProfileStatusResult> HandleAsync(
        ChangeInstructorProfileStatusCommand command,
        CancellationToken cancellationToken = default)
    {
        var profile =
            await _instructorProfileRepository.GetByUserIdAsync(
                command.UserId,
                cancellationToken);

        if (profile is null)
            throw new InstructorProfileNotFoundException(command.UserId);

        if (command.IsActive)
            profile.Activate();
        else
            profile.Deactivate();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ChangeInstructorProfileStatusResult(
            profile.Id,
            profile.Status);
    }
}
