using JobTrackingAPI.Contracts.Repositories;
using JobTrackingAPI.Contracts.Services;
using JobTrackingAPI.Data;
using JobTrackingAPI.Handlers;
using JobTrackingAPI.Repositories;
using JobTrackingAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace JobTrackingAPI.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<JobTrackingDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DatabaseConnection")));
        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IApplicationRepository, ApplicationRepository>();
        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IApplicationService, ApplicationService>();
        services.AddScoped<IFileService, LocalFileService>();
        return services;
    }

    public static IServiceCollection AddExceptionHandlers(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        return services;
    }
}
