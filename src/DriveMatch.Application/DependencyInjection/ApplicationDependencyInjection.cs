using DriveMatch.Application.Features.Instructors.CreateProfile;
using DriveMatch.Application.Features.Instructors.UpdateProfile;
using DriveMatch.Application.Features.Students.CreateProfile;
using DriveMatch.Application.Features.Students.UpdateProfile;
using DriveMatch.Application.Features.Users.Register;
using DriveMatch.Application.Features.Availabilities.ChangeStatus;
using DriveMatch.Application.Features.Availabilities.Create;
using DriveMatch.Application.Features.Availabilities.Update;
using DriveMatch.Application.Features.Instructors.Search;
using DriveMatch.Application.Features.Instructors.ChangeStatus;
using Microsoft.Extensions.DependencyInjection;

namespace DriveMatch.Application.DependencyInjection;

public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<RegisterUserHandler>();

        services.AddScoped<CreateStudentProfileHandler>();
        services.AddScoped<UpdateStudentProfileHandler>();

        services.AddScoped<CreateInstructorProfileHandler>();
        services.AddScoped<UpdateInstructorProfileHandler>();
        services.AddScoped<CreateAvailabilityHandler>();
        services.AddScoped<UpdateAvailabilityHandler>();
        services.AddScoped<ChangeAvailabilityStatusHandler>();
        services.AddScoped<SearchInstructorsHandler>();
        services.AddScoped<ChangeInstructorProfileStatusHandler>();

        return services;
    }
}