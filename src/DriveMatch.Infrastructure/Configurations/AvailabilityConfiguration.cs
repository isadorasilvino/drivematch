using DriveMatch.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DriveMatch.Infrastructure.Configurations;

public sealed class AvailabilityConfiguration
    : IEntityTypeConfiguration<Availability>
{
    public void Configure(EntityTypeBuilder<Availability> builder)
    {
        builder.ToTable("availabilities");

        builder.HasKey(availability => availability.Id);

        builder.Property(availability => availability.Id)
            .ValueGeneratedNever();

        builder.Property(availability => availability.InstructorProfileId)
            .IsRequired();

        builder.Property(availability => availability.DayOfWeek)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(availability => availability.StartTime)
            .IsRequired();

        builder.Property(availability => availability.EndTime)
            .IsRequired();

        builder.Property(availability => availability.IsActive)
            .IsRequired();

        builder.HasIndex(availability => new
        {
            availability.InstructorProfileId,
            availability.DayOfWeek,
            availability.IsActive
        });

        builder.HasOne<InstructorProfile>()
            .WithMany()
            .HasForeignKey(availability => availability.InstructorProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
