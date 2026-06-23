using FluentValidation;
using TaskFlow.Application.Abstractions.Persistence;
using TaskFlow.Application.DTOs.Projects;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Services.Projects;

public sealed class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IValidator<CreateProjectRequest> _createValidator;
    private readonly IValidator<UpdateProjectRequest> _updateValidator;

    public ProjectService(
        IProjectRepository projectRepository,
        IValidator<CreateProjectRequest> createValidator,
        IValidator<UpdateProjectRequest> updateValidator)
    {
        _projectRepository = projectRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<ProjectDto> CreateAsync(CreateProjectRequest request, CancellationToken cancellationToken = default)
    {
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);

        var project = new Project(request.Name, request.OwnerUserId);
        await _projectRepository.AddAsync(project, cancellationToken);
        await _projectRepository.SaveChangesAsync(cancellationToken);

        return Map(project);
    }

    public async Task<ProjectDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var project = await _projectRepository.GetByIdAsync(id, cancellationToken);
        return project is null ? null : Map(project);
    }

    public async Task<IReadOnlyList<ProjectDto>> GetByOwnerUserIdAsync(Guid ownerUserId, CancellationToken cancellationToken = default)
    {
        var projects = await _projectRepository.GetByOwnerUserIdAsync(ownerUserId, cancellationToken);
        return projects.Select(Map).ToList();
    }

    public async Task<ProjectDto?> UpdateAsync(Guid id, UpdateProjectRequest request, CancellationToken cancellationToken = default)
    {
        await _updateValidator.ValidateAndThrowAsync(request, cancellationToken);

        var project = await _projectRepository.GetByIdAsync(id, cancellationToken);
        if (project is null) return null;

        project.Update(request.Name);
        await _projectRepository.SaveChangesAsync(cancellationToken);

        return Map(project);
    }

    private static ProjectDto Map(Project project) => new()
    {
        Id = project.Id,
        Name = project.Name,
        OwnerUserId = project.OwnerUserId
    };
}