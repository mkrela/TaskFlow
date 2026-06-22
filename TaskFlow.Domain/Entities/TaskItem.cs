using TaskFlow.Domain.Enums;

namespace TaskFlow.Domain.Entities;

public class TaskItem
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public TaskFlow.Domain.Enums.TaskStatus Status { get; private set; } = TaskFlow.Domain.Enums.TaskStatus.Todo;
    public TaskPriority Priority { get; private set; } = TaskPriority.Medium;
    public DateTime? DueDate { get; private set; }
    public Guid ProjectId { get; private set; }
    public Guid CreatedByUserId { get; private set; }

    private TaskItem() { } // EF Core

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

    public void ChangeStatus( TaskFlow.Domain.Enums.TaskStatus status) => Status = status;
}