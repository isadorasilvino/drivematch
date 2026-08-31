using DriveMatch.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DriveMatch.Infrastructure.Configurations;

public sealed class LessonConfiguration
    : IEntityTypeConfiguration<Lesson>
{
    public void Configure(EntityTypeBuilder<Lesson> builder)
    {
        builder.ToTable("lessons");

        builder.HasKey(lesson => lesson.Id);

        builder.Property(lesson => lesson.Id)
            .ValueGeneratedNever();

        builder.Property(lesson => lesson.StudentId)
            .IsRequired();

        builder.Property(lesson => lesson.InstructorId)
            .IsRequired();

        builder.Property(lesson => lesson.LessonRequestId)
            .IsRequired();

        builder.HasIndex(lesson => lesson.LessonRequestId)
            .IsUnique();

        builder.Property(lesson => lesson.ScheduledDate)
            .IsRequired();

        builder.Property(lesson => lesson.StartTime)
            .IsRequired();

        builder.Property(lesson => lesson.EndTime)
            .IsRequired();

        builder.Property(lesson => lesson.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(lesson => lesson.StartedAt);
        builder.Property(lesson => lesson.CheckInAt);

        builder.Property(lesson => lesson.CheckInToken)
            .HasMaxLength(32);

        builder.Property(lesson => lesson.CheckInTokenExpiresAt);

        builder.Property(lesson => lesson.CompletedAt);
        builder.Property(lesson => lesson.CancelledAt);

        builder.Property(lesson => lesson.CreatedAt)
            .IsRequired();

        builder.HasIndex(lesson => new
        {
            lesson.InstructorId,
            lesson.ScheduledDate,
            lesson.Status
        });

        builder.HasOne<StudentProfile>()
            .WithMany()
            .HasForeignKey(lesson => lesson.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<InstructorProfile>()
            .WithMany()
            .HasForeignKey(lesson => lesson.InstructorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<LessonRequest>()
            .WithOne()
            .HasForeignKey<Lesson>(lesson => lesson.LessonRequestId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
