using FluentValidation;
using TaskFlow.Application.Abstractions.Persistence;
using TaskFlow.Application.Abstractions.Services;
using TaskFlow.Application.DTOs.Tasks;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Services.Tasks;

public sealed class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IValidator<CreateTaskRequest> _createTaskValidator;
    private readonly IValidator<UpdateTaskRequest> _updateTaskValidator;

    public TaskService(
        ITaskRepository taskRepository,
        IValidator<CreateTaskRequest> createTaskValidator,
        IValidator<UpdateTaskRequest> updateTaskValidator)
    {
        _taskRepository = taskRepository;
        _createTaskValidator = createTaskValidator;
        _updateTaskValidator = updateTaskValidator;
    }

    public async Task<TaskDto> CreateAsync(CreateTaskRequest request, CancellationToken cancellationToken = default)
    {
        await _createTaskValidator.ValidateAndThrowAsync(request, cancellationToken);

        var task = new TaskItem(request.Title, request.ProjectId, request.CreatedByUserId);
        if (request.Description is not null || request.DueDate is not null)
        {
            task.Update(request.Title, request.Description, TaskPriority.Medium, request.DueDate);
        }

        await _taskRepository.AddAsync(task, cancellationToken);
        await _taskRepository.SaveChangesAsync(cancellationToken);

        return Map(task);
    }

    public async Task<TaskDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(id, cancellationToken);
        return task is null ? null : Map(task);
    }

    public async Task<IReadOnlyList<TaskDto>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var tasks = await _taskRepository.GetByProjectIdAsync(projectId, cancellationToken);
        return tasks.Select(Map).ToList();
    }

    public async Task<TaskDto?> UpdateAsync(Guid id, UpdateTaskRequest request, CancellationToken cancellationToken = default)
    {
        await _updateTaskValidator.ValidateAndThrowAsync(request, cancellationToken);

        var task = await _taskRepository.GetByIdAsync(id, cancellationToken);
        if (task is null)
        {
            return null;
        }

        task.Update(request.Title, request.Description, request.Priority, request.DueDate);
        await _taskRepository.SaveChangesAsync(cancellationToken);

        return Map(task);
    }

    public async Task<bool> ChangeStatusAsync(Guid id, TaskFlow.Domain.Enums.TaskStatus status, CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(id, cancellationToken);
        if (task is null)
        {
            return false;
        }

        task.ChangeStatus(status);
        await _taskRepository.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static TaskDto Map(TaskItem task) => new()
    {
        Id = task.Id,
        Title = task.Title,
        Description = task.Description,
        Status = task.Status,
        Priority = task.Priority,
        DueDate = task.DueDate,
        ProjectId = task.ProjectId,
        CreatedByUserId = task.CreatedByUserId
    };
}