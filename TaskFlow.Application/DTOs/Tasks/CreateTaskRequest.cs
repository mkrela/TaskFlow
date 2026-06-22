using System;
using System.Collections.Generic;
using System.Text;

namespace TaskFlow.Application.DTOs.Tasks
{
    public sealed class CreateTaskRequest
    {
        public string Title { get; init; } = string.Empty;
        public string? Description { get; init; }
        public Guid ProjectId { get; init; }
        public Guid CreatedByUserId { get; init; }
        public DateTime? DueDate { get; init; }
    }
}
