using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Features.Instructors.Search;
using DriveMatch.Domain.Entities;
using DriveMatch.Domain.Enums;
using DriveMatch.Domain.ValueObjects;

namespace DriveMatch.UnitTests.Application.Instructors.Search;

public class SearchInstructorsHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldNormalizeFiltersBeforeSearching()
    {
        var repository = new FakeInstructorProfileRepository();
        var handler = new SearchInstructorsHandler(repository);

        var query = new SearchInstructorsQuery(
            "  Belo Horizonte  ",
            " mg ",
            ExperienceLevel.Beginner,
            true,
            150m);

        await handler.HandleAsync(query);

        Assert.Equal("Belo Horizonte", repository.LastCity);
        Assert.Equal("MG", repository.LastState);
        Assert.Equal(ExperienceLevel.Beginner, repository.LastExperienceLevel);
        Assert.True(repository.LastUsesStudentVehicle);
        Assert.Equal(150m, repository.LastMaxPricePerLesson);
    }

    [Fact]
    public async Task HandleAsync_ShouldMapRepositoryResults()
    {
        var instructor = new InstructorProfile(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Instrutor experiente.",
            5,
            "Belo Horizonte",
            "MG",
            new Money(120m),
            true,
            true,
            true);

        instructor.Activate();

        var repository = new FakeInstructorProfileRepository(
            new[] { instructor });

        var handler = new SearchInstructorsHandler(repository);

        var result = await handler.HandleAsync(
            new SearchInstructorsQuery(
                "Belo Horizonte",
                "MG",
                ExperienceLevel.Beginner,
                true,
                null));

        var item = Assert.Single(result);

        Assert.Equal(instructor.Id, item.InstructorProfileId);
        Assert.Equal(instructor.UserId, item.UserId);
        Assert.Equal(instructor.Description, item.Description);
        Assert.Equal(5, item.ExperienceYears);
        Assert.Equal("Belo Horizonte", item.City);
        Assert.Equal("MG", item.State);
        Assert.Equal(120m, item.PricePerLesson);
        Assert.Equal("BRL", item.Currency);
        Assert.True(item.AcceptsBeginners);
        Assert.True(item.AcceptsExperiencedStudents);
        Assert.True(item.AcceptsStudentVehicle);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnEmptyCollection_WhenNoInstructorMatches()
    {
        var handler = new SearchInstructorsHandler(
            new FakeInstructorProfileRepository());

        var result = await handler.HandleAsync(
            new SearchInstructorsQuery(
                "Belo Horizonte",
                "MG",
                ExperienceLevel.Beginner,
                false,
                null));

        Assert.Empty(result);
    }

    private sealed class FakeInstructorProfileRepository
        : IInstructorProfileRepository
    {
        private readonly IReadOnlyCollection<InstructorProfile> _results;

        public FakeInstructorProfileRepository(
            IReadOnlyCollection<InstructorProfile>? results = null)
        {
            _results = results ?? Array.Empty<InstructorProfile>();
        }

        public string? LastCity { get; private set; }
        public string? LastState { get; private set; }
        public ExperienceLevel LastExperienceLevel { get; private set; }
        public bool LastUsesStudentVehicle { get; private set; }
        public decimal? LastMaxPricePerLesson { get; private set; }

        public Task<InstructorProfile?> GetByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<InstructorProfile?>(null);
        }

        public Task<bool> ExistsByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }

        public Task<IReadOnlyCollection<InstructorProfile>> SearchAsync(
            string city,
            string state,
            ExperienceLevel experienceLevel,
            bool usesStudentVehicle,
            decimal? maxPricePerLesson,
            CancellationToken cancellationToken = default)
        {
            LastCity = city;
            LastState = state;
            LastExperienceLevel = experienceLevel;
            LastUsesStudentVehicle = usesStudentVehicle;
            LastMaxPricePerLesson = maxPricePerLesson;

            return Task.FromResult(_results);
        }

        public Task AddAsync(
            InstructorProfile instructorProfile,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
