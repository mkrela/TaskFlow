using System;
using System.Collections.Generic;
using System.Text;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Domain.Entities
{
    public class TaskItem
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Title { get; private set; } = string.Empty;
        private  string? Description { get; set; } = string.Empty;
        public Enums.TaskStatus Status { get; private set; } = Enums.TaskStatus.Todo;
        public TaskPriority Priority { get; private set; } = TaskPriority.Medium;
        public DateTime? DueDate { get; private set; }
        public Guid ProjectId { get; private set; }
        public Guid CreatedByUserId { get; private set; }

        private TaskItem() { }   // EF Core }

        public TaskItem(string title, Guid projectId, Guid createdByUserId)
        {
            Title = title;
            ProjectId = projectId;
            CreatedByUserId = createdByUserId;
         }

        public void Update(string title, string? description, TaskPriority priority, DateTime? dueDate)
        {
            Title = title;
            Description = description;
            Priority = priority;
            DueDate = dueDate;
        }

        public void ChangeStatus(Enums.TaskStatus status) => Status = status;
    }
}