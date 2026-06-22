using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Application.Abstractions.Services;
using TaskFlow.Application.Services.Tasks;

namespace TaskFlow.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ITaskService, TaskService>();
        return services;
    }
}