using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.DTOs.Tasks;

public sealed class ChangeTaskStatusRequest
{
    public TaskFlow.Domain.Enums.TaskStatus Status { get; init; }
}