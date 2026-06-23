using TaskFlow.Application.DTOs.Tasks;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Services.Tasks;

public interface ITaskService
{
    Task<TaskDto> CreateAsync(CreateTaskRequest request, CancellationToken cancellationToken = default);
    Task<TaskDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TaskDto>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<TaskDto?> UpdateAsync(Guid id, UpdateTaskRequest request, CancellationToken cancellationToken = default);
    Task<bool> ChangeStatusAsync(Guid id, Domain.Enums.TaskStatus status, CancellationToken cancellationToken = default);
}