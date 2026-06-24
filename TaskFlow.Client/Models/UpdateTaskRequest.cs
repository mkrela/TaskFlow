namespace TaskFlow.Client.Models;

public record UpdateTaskRequest(string Title, string? Description, string Priority, DateTime? DueDate);