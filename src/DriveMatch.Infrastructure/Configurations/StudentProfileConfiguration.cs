using DriveMatch.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DriveMatch.Infrastructure.Configurations;

public sealed class StudentProfileConfiguration
    : IEntityTypeConfiguration<StudentProfile>
{
    public void Configure(EntityTypeBuilder<StudentProfile> builder)
    {
        builder.ToTable("student_profiles");

        builder.HasKey(profile => profile.Id);

        builder.Property(profile => profile.Id)
            .ValueGeneratedNever();

        builder.Property(profile => profile.UserId)
            .IsRequired();

        builder.HasIndex(profile => profile.UserId)
            .IsUnique();

        builder.Property(profile => profile.City)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(profile => profile.State)
            .HasMaxLength(2)
            .IsRequired();

        builder.Property(profile => profile.ExperienceLevel)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(profile => profile.OwnsVehicle)
            .IsRequired();

        builder.Property(profile => profile.HasOwnVehicleForLessons)
            .IsRequired();

        builder.Property(profile => profile.CreatedAt)
            .IsRequired();

        builder.Property(profile => profile.UpdatedAt);

        builder.HasOne<User>()
            .WithOne()
            .HasForeignKey<StudentProfile>(profile => profile.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
