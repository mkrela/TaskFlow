namespace TaskFlow.Client.Models;

public record CreateTaskRequest(string Title, Guid ProjectId, Guid CreatedByUserId, string? Description = null, DateTime? DueDate = null);