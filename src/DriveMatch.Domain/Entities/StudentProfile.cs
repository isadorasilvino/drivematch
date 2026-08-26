using DriveMatch.Domain.Common;
using DriveMatch.Domain.Enums;
using DriveMatch.Domain.Exceptions;

namespace DriveMatch.Domain.Entities;

public class StudentProfile : Entity
{
    public Guid UserId { get; private set; }
    public string City { get; private set; }
    public string State { get; private set; }
    public ExperienceLevel ExperienceLevel { get; private set; }
    public bool OwnsVehicle { get; private set; }
    public bool HasOwnVehicleForLessons { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private StudentProfile()
    {
        City = null!;
        State = null!;
    }

    public StudentProfile(
        Guid id,
        Guid userId,
        string city,
        string state,
        ExperienceLevel experienceLevel,
        bool ownsVehicle,
        bool hasOwnVehicleForLessons)
        : base(id)
    {
        if (userId == Guid.Empty)
            throw new DomainException("O identificador do usuário deve ser informado.");

        ValidateLocation(city, state);

        ValidateVehiclePreferences(
            ownsVehicle,
            hasOwnVehicleForLessons);

        UserId = userId;
        City = city.Trim();
        State = state.Trim().ToUpperInvariant();
        ExperienceLevel = experienceLevel;
        OwnsVehicle = ownsVehicle;
        HasOwnVehicleForLessons = hasOwnVehicleForLessons;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateLocation(string city, string state)
    {
        ValidateLocation(city, state);

        City = city.Trim();
        State = state.Trim().ToUpperInvariant();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateExperienceLevel(ExperienceLevel experienceLevel)
    {
        ExperienceLevel = experienceLevel;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateVehiclePreferences(
        bool ownsVehicle,
        bool hasOwnVehicleForLessons)
    {

        ValidateVehiclePreferences(
            ownsVehicle,
            hasOwnVehicleForLessons);

        OwnsVehicle = ownsVehicle;
        HasOwnVehicleForLessons = hasOwnVehicleForLessons;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidateLocation(string city, string state)
    {
        if (string.IsNullOrWhiteSpace(city))
            throw new DomainException("A cidade deve ser informada.");

        if (string.IsNullOrWhiteSpace(state))
            throw new DomainException("O estado deve ser informado.");
    }

    private static void ValidateVehiclePreferences(
    bool ownsVehicle,
    bool hasOwnVehicleForLessons)
    {
        if (hasOwnVehicleForLessons && !ownsVehicle)
            throw new DomainException(
                "O aluno não pode disponibilizar veículo próprio para as aulas sem possuir um veículo.");
    }
}
