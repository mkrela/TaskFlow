using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Application.DTOs.Tasks;
using TaskFlow.Application.Services.Tasks;
using TaskFlow.Application.Validators.Tasks;
using TaskFlow.Application.DTOs.Projects;
using TaskFlow.Application.Services.Projects;
using TaskFlow.Application.Validators.Projects;

namespace TaskFlow.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<IProjectService, ProjectService>();

        services.AddScoped<IValidator<CreateTaskRequest>, CreateTaskRequestValidator>();
        services.AddScoped<IValidator<UpdateTaskRequest>, UpdateTaskRequestValidator>();
        services.AddScoped<IValidator<ChangeTaskStatusRequest>, ChangeTaskStatusRequestValidator>();

        services.AddScoped<IValidator<CreateProjectRequest>, CreateProjectRequestValidator>();
        services.AddScoped<IValidator<UpdateProjectRequest>, UpdateProjectRequestValidator>();

        return services;
    }
}