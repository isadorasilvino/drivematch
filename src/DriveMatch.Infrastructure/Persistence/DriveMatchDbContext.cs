using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DriveMatch.Infrastructure.Persistence;

public sealed class DriveMatchDbContext
    : DbContext, IUnitOfWork
{
    public DriveMatchDbContext(
        DbContextOptions<DriveMatchDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<StudentProfile> StudentProfiles
        => Set<StudentProfile>();

    public DbSet<InstructorProfile> InstructorProfiles
        => Set<InstructorProfile>();

    public DbSet<Availability> Availabilities
        => Set<Availability>();

    public DbSet<LessonRequest> LessonRequests
        => Set<LessonRequest>();

    public DbSet<Lesson> Lessons
        => Set<Lesson>();

    public DbSet<Review> Reviews
        => Set<Review>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(DriveMatchDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
