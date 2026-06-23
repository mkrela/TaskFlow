using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Abstractions.Persistence;
using TaskFlow.Domain.Entities;
using TaskFlow.Infrastructure.Data;

namespace TaskFlow.Infrastructure.Repositories;

public sealed class ProjectRepository : IProjectRepository
{
    private readonly TaskFlowDbContext _dbContext;

    public ProjectRepository(TaskFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _dbContext.Projects.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Project>> GetByOwnerUserIdAsync(Guid ownerUserId, CancellationToken cancellationToken = default)
        => await _dbContext.Projects
            .Where(p => p.OwnerUserId == ownerUserId)
            .ToListAsync(cancellationToken);

    public Task AddAsync(Project project, CancellationToken cancellationToken = default)
        => _dbContext.Projects.AddAsync(project, cancellationToken).AsTask();

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => _dbContext.SaveChangesAsync(cancellationToken);
}