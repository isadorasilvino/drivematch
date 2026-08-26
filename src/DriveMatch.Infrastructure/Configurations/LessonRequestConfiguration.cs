using DriveMatch.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DriveMatch.Infrastructure.Configurations;

public sealed class LessonRequestConfiguration
    : IEntityTypeConfiguration<LessonRequest>
{
    public void Configure(EntityTypeBuilder<LessonRequest> builder)
    {
        builder.ToTable("lesson_requests");

        builder.HasKey(request => request.Id);

        builder.Property(request => request.Id)
            .ValueGeneratedNever();

        builder.Property(request => request.StudentId)
            .IsRequired();

        builder.Property(request => request.InstructorId)
            .IsRequired();

        builder.Property(request => request.RequestedDate)
            .IsRequired();

        builder.Property(request => request.StartTime)
            .IsRequired();

        builder.Property(request => request.EndTime)
            .IsRequired();

        builder.Property(request => request.UsesStudentVehicle)
            .IsRequired();

        builder.Property(request => request.StudentMessage)
            .HasMaxLength(1000);

        builder.Property(request => request.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(request => request.CreatedAt)
            .IsRequired();

        builder.Property(request => request.UpdatedAt);

        builder.HasIndex(request => new
        {
            request.InstructorId,
            request.RequestedDate,
            request.Status
        });

        builder.HasOne<StudentProfile>()
            .WithMany()
            .HasForeignKey(request => request.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<InstructorProfile>()
            .WithMany()
            .HasForeignKey(request => request.InstructorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
