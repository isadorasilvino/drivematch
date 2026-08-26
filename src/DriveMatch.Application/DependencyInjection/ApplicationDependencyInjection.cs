using DriveMatch.Application.Features.Instructors.CreateProfile;
using DriveMatch.Application.Features.Instructors.UpdateProfile;
using DriveMatch.Application.Features.Students.CreateProfile;
using DriveMatch.Application.Features.Students.UpdateProfile;
using DriveMatch.Application.Features.Users.Register;
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

        return services;
    }
}