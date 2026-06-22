using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Application.Abstractions.Services;
using TaskFlow.Application.DTOs.Tasks;
using TaskFlow.Application.Services.Tasks;
using TaskFlow.Application.Validators.Tasks;

namespace TaskFlow.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ITaskService, TaskService>();

        services.AddScoped<IValidator<CreateTaskRequest>, CreateTaskRequestValidator>();
        services.AddScoped<IValidator<UpdateTaskRequest>, UpdateTaskRequestValidator>();
        services.AddScoped<IValidator<ChangeTaskStatusRequest>, ChangeTaskStatusRequestValidator>();

        return services;
    }
}