using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Domain.Entities;
using DriveMatch.Domain.Enums;
using DriveMatch.Domain.ValueObjects;

namespace DriveMatch.UnitTests.Application.Lessons;

internal static class LessonTestHelpers
{
    public static Lesson CreateLesson(
        Guid instructorProfileId)
    {
        return new Lesson(
            Guid.NewGuid(),
            Guid.NewGuid(),
            instructorProfileId,
            Guid.NewGuid(),
            new DateOnly(2026, 8, 31),
            new TimeOnly(14, 0),
            new TimeOnly(15, 0));
    }

    public static InstructorProfile CreateInstructorProfile(
        Guid profileId,
        Guid userId)
    {
        return new InstructorProfile(
            profileId,
            userId,
            "Instrutor de teste",
            5,
            "Belo Horizonte",
            "MG",
            new Money(80m),
            true,
            true,
            true);
    }
}

internal sealed class FakeInstructorProfileRepository
    : IInstructorProfileRepository
{
    private readonly InstructorProfile? _profile;

    public FakeInstructorProfileRepository(
        InstructorProfile? profile)
    {
        _profile = profile;
    }

    public Task<InstructorProfile?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            _profile?.Id == id ? _profile : null);
    }

    public Task<InstructorProfile?> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            _profile?.UserId == userId
                ? _profile
                : null);
    }

    public Task<bool> ExistsByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            _profile?.UserId == userId);
    }

    public Task<IReadOnlyCollection<InstructorProfile>> SearchAsync(
        string city,
        string state,
        ExperienceLevel experienceLevel,
        bool usesStudentVehicle,
        decimal? maxPricePerLesson,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<InstructorProfile> result =
            Array.Empty<InstructorProfile>();

        return Task.FromResult(result);
    }

    public Task AddAsync(
        InstructorProfile instructorProfile,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}