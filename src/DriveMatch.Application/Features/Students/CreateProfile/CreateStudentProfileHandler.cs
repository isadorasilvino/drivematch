using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Domain.Entities;
using DriveMatch.Domain.Enums;

namespace DriveMatch.Application.Features.Students.CreateProfile;

public sealed class CreateStudentProfileHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IStudentProfileRepository _studentProfileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateStudentProfileHandler(
        IUserRepository userRepository,
        IStudentProfileRepository studentProfileRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _studentProfileRepository = studentProfileRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateStudentProfileResult> HandleAsync(
        CreateStudentProfileCommand command,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(
            command.UserId,
            cancellationToken);

        if (user is null)
            throw new UserNotFoundException(command.UserId);

        if (user.Role != UserRole.Student)
            throw new InvalidUserRoleException();

        var profileAlreadyExists =
            await _studentProfileRepository.ExistsByUserIdAsync(
                command.UserId,
                cancellationToken);

        if (profileAlreadyExists)
            throw new StudentProfileAlreadyExistsException(command.UserId);

        var profile = new StudentProfile(
            Guid.NewGuid(),
            command.UserId,
            command.City,
            command.State,
            command.ExperienceLevel,
            command.OwnsVehicle,
            command.HasOwnVehicleForLessons);

        await _studentProfileRepository.AddAsync(
            profile,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new CreateStudentProfileResult(
            profile.Id,
            profile.UserId,
            profile.City,
            profile.State,
            profile.ExperienceLevel,
            profile.OwnsVehicle,
            profile.HasOwnVehicleForLessons);
    }
}
