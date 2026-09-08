using DriveMatch.Application.Abstractions.Persistence;
using DriveMatch.Application.Abstractions.Services;
using DriveMatch.Application.Abstractions.Time;
using DriveMatch.Infrastructure.Persistence;
using DriveMatch.Infrastructure.Repositories;
using DriveMatch.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DriveMatch.Infrastructure.DependencyInjection;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "A connection string 'DefaultConnection' não foi configurada.");

        services.AddDbContext<DriveMatchDbContext>(options =>
            options.UseNpgsql(connectionString));

        var schedulingTimeZoneId =
            configuration["Scheduling:TimeZoneId"]
            ?? throw new InvalidOperationException(
                "O fuso horário da agenda não foi configurado.");

        TimeZoneInfo schedulingTimeZone;

        try
        {
            schedulingTimeZone =
                TimeZoneInfo.FindSystemTimeZoneById(schedulingTimeZoneId);
        }
        catch (TimeZoneNotFoundException exception)
        {
            throw new InvalidOperationException(
                $"O fuso horário '{schedulingTimeZoneId}' não foi encontrado.",
                exception);
        }
        catch (InvalidTimeZoneException exception)
        {
            throw new InvalidOperationException(
                $"O fuso horário '{schedulingTimeZoneId}' é inválido.",
                exception);
        }

        services.AddScoped<IUnitOfWork>(
            provider => provider.GetRequiredService<DriveMatchDbContext>());

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IStudentProfileRepository, StudentProfileRepository>();
        services.AddScoped<IInstructorProfileRepository, InstructorProfileRepository>();
        services.AddScoped<IAvailabilityRepository, AvailabilityRepository>();
        services.AddScoped<ILessonRequestRepository, LessonRequestRepository>();
        services.AddScoped<ILessonRepository, LessonRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<IUserAuthenticationRepository, UserRepository>();
        services.AddScoped<ITokenService, JwtTokenService>();

        services.AddScoped<IPasswordHasher, PasswordHasher>();

        services.AddSingleton(schedulingTimeZone);
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

        return services;
    }
}
