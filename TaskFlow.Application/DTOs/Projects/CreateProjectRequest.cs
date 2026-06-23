namespace TaskFlow.Application.DTOs.Projects;

public sealed class CreateProjectRequest
{
    public string Name { get; init; } = string.Empty;
    public Guid OwnerUserId { get; init; }
}