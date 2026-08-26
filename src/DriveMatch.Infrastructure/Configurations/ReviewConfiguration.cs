using DriveMatch.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DriveMatch.Infrastructure.Configurations;

public sealed class ReviewConfiguration
    : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("reviews");

        builder.HasKey(review => review.Id);

        builder.Property(review => review.Id)
            .ValueGeneratedNever();

        builder.Property(review => review.LessonId)
            .IsRequired();

        builder.HasIndex(review => review.LessonId)
            .IsUnique();

        builder.Property(review => review.StudentId)
            .IsRequired();

        builder.Property(review => review.InstructorId)
            .IsRequired();

        builder.Property(review => review.Rating)
            .IsRequired();

        builder.Property(review => review.Comment)
            .HasMaxLength(2000);

        builder.Property(review => review.CreatedAt)
            .IsRequired();

        builder.HasOne<Lesson>()
            .WithOne()
            .HasForeignKey<Review>(review => review.LessonId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<StudentProfile>()
            .WithMany()
            .HasForeignKey(review => review.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<InstructorProfile>()
            .WithMany()
            .HasForeignKey(review => review.InstructorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
