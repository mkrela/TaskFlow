namespace TaskFlow.Application.DTOs.Projects;

public sealed class ProjectDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public Guid OwnerUserId { get; init; }
}