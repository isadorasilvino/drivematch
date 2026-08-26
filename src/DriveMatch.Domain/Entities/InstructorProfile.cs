using DriveMatch.Domain.Common;
using DriveMatch.Domain.Enums;
using DriveMatch.Domain.Exceptions;
using DriveMatch.Domain.ValueObjects;

namespace DriveMatch.Domain.Entities;

public class InstructorProfile : Entity
{
    public Guid UserId { get; private set; }
    public string Description { get; private set; }
    public int ExperienceYears { get; private set; }
    public string City { get; private set; }
    public string State { get; private set; }
    public Money PricePerLesson { get; private set; }
    public bool AcceptsBeginners { get; private set; }
    public bool AcceptsExperiencedStudents { get; private set; }
    public bool AcceptsStudentVehicle { get; private set; }
    public InstructorProfileStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private InstructorProfile()
    {
        Description = null!;
        City = null!;
        State = null!;
        PricePerLesson = null!;
    }

    public InstructorProfile(
        Guid id,
        Guid userId,
        string description,
        int experienceYears,
        string city,
        string state,
        Money pricePerLesson,
        bool acceptsBeginners,
        bool acceptsExperiencedStudents,
        bool acceptsStudentVehicle)
        : base(id)
    {
        if (userId == Guid.Empty)
            throw new DomainException("O identificador do usuário deve ser informado.");

        ValidateDescription(description);
        ValidateExperienceYears(experienceYears);
        ValidateLocation(city, state);

        PricePerLesson = pricePerLesson
            ?? throw new DomainException("O preço da aula deve ser informado.");

        UserId = userId;
        Description = description.Trim();
        ExperienceYears = experienceYears;
        City = city.Trim();
        State = state.Trim().ToUpperInvariant();
        AcceptsBeginners = acceptsBeginners;
        AcceptsExperiencedStudents = acceptsExperiencedStudents;
        AcceptsStudentVehicle = acceptsStudentVehicle;
        Status = InstructorProfileStatus.Draft;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateDescription(string description)
    {
        ValidateDescription(description);

        Description = description.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateExperienceYears(int experienceYears)
    {
        ValidateExperienceYears(experienceYears);

        ExperienceYears = experienceYears;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateLocation(string city, string state)
    {
        ValidateLocation(city, state);

        City = city.Trim();
        State = state.Trim().ToUpperInvariant();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePrice(Money pricePerLesson)
    {
        PricePerLesson = pricePerLesson
            ?? throw new DomainException("O preço da aula deve ser informado.");

        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateServicePreferences(
        bool acceptsBeginners,
        bool acceptsExperiencedStudents,
        bool acceptsStudentVehicle)
    {
        AcceptsBeginners = acceptsBeginners;
        AcceptsExperiencedStudents = acceptsExperiencedStudents;
        AcceptsStudentVehicle = acceptsStudentVehicle;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        Status = InstructorProfileStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        Status = InstructorProfileStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("A descrição profissional deve ser informada.");
    }

    private static void ValidateExperienceYears(int experienceYears)
    {
        if (experienceYears < 0)
            throw new DomainException("Os anos de experiência não podem ser negativos.");
    }

    private static void ValidateLocation(string city, string state)
    {
        if (string.IsNullOrWhiteSpace(city))
            throw new DomainException("A cidade deve ser informada.");

        if (string.IsNullOrWhiteSpace(state))
            throw new DomainException("O estado deve ser informado.");
    }
}
