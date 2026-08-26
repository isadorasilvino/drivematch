using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Domain.Entities;
using DriveMatch.Domain.Enums;
using DriveMatch.Domain.ValueObjects;

namespace DriveMatch.Application.Features.Instructors.CreateProfile;

public sealed class CreateInstructorProfileHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IInstructorProfileRepository _instructorProfileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateInstructorProfileHandler(
        IUserRepository userRepository,
        IInstructorProfileRepository instructorProfileRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _instructorProfileRepository = instructorProfileRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateInstructorProfileResult> HandleAsync(
        CreateInstructorProfileCommand command,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(
            command.UserId,
            cancellationToken);

        if (user is null)
            throw new UserNotFoundException(command.UserId);

        if (user.Role != UserRole.Instructor)
            throw new InvalidUserRoleException();

        var profileAlreadyExists =
            await _instructorProfileRepository.ExistsByUserIdAsync(
                command.UserId,
                cancellationToken);

        if (profileAlreadyExists)
            throw new InstructorProfileAlreadyExistsException(command.UserId);

        var price = new Money(command.PricePerLesson);

        var profile = new InstructorProfile(
            Guid.NewGuid(),
            command.UserId,
            command.Description,
            command.ExperienceYears,
            command.City,
            command.State,
            price,
            command.AcceptsBeginners,
            command.AcceptsExperiencedStudents,
            command.AcceptsStudentVehicle);

        await _instructorProfileRepository.AddAsync(
            profile,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new CreateInstructorProfileResult(
            profile.Id,
            profile.UserId,
            profile.Description,
            profile.ExperienceYears,
            profile.City,
            profile.State,
            profile.PricePerLesson.Amount,
            profile.PricePerLesson.Currency,
            profile.AcceptsBeginners,
            profile.AcceptsExperiencedStudents,
            profile.AcceptsStudentVehicle,
            profile.Status);
    }
}
