using DriveMatch.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DriveMatch.Infrastructure.Configurations;

public sealed class InstructorProfileConfiguration
    : IEntityTypeConfiguration<InstructorProfile>
{
    public void Configure(EntityTypeBuilder<InstructorProfile> builder)
    {
        builder.ToTable("instructor_profiles");

        builder.HasKey(profile => profile.Id);

        builder.Property(profile => profile.Id)
            .ValueGeneratedNever();

        builder.Property(profile => profile.UserId)
            .IsRequired();

        builder.HasIndex(profile => profile.UserId)
            .IsUnique();

        builder.Property(profile => profile.Description)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(profile => profile.ExperienceYears)
            .IsRequired();

        builder.Property(profile => profile.City)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(profile => profile.State)
            .HasMaxLength(2)
            .IsRequired();

        builder.OwnsOne(profile => profile.PricePerLesson, money =>
        {
            money.Property(value => value.Amount)
                .HasColumnName("price_per_lesson")
                .HasPrecision(10, 2)
                .IsRequired();

            money.Property(value => value.Currency)
                .HasColumnName("currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Property(profile => profile.AcceptsBeginners)
            .IsRequired();

        builder.Property(profile => profile.AcceptsExperiencedStudents)
            .IsRequired();

        builder.Property(profile => profile.AcceptsStudentVehicle)
            .IsRequired();

        builder.Property(profile => profile.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(profile => profile.CreatedAt)
            .IsRequired();

        builder.Property(profile => profile.UpdatedAt);

        builder.HasOne<User>()
            .WithOne()
            .HasForeignKey<InstructorProfile>(profile => profile.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
