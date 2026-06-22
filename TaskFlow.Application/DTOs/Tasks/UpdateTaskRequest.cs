using System;
using System.Collections.Generic;
using System.Text;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.DTOs.Tasks
{
    public sealed class UpdateTaskRequest
    {
        public string Title { get; init; } = string.Empty;
        public string? Description { get; init; }
        public TaskPriority Priority { get; init; } = TaskPriority.Medium;
        public DateTime? DueDate { get; init; }
    }
}
