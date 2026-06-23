using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using TaskFlow.Application.DTOs.Projects;
using TaskFlow.Application.Services.Projects;
using TaskFlow.Application.Validators.Projects;
using TaskFlow.Domain.Entities;
using TaskFlow.Application.Abstractions.Persistence;
using Xunit;

namespace TaskFlow.Tests.Services;

public class ProjectServiceTests
{
    [Fact]
    public async Task CreateAsync_Should_Create_Project_And_Save()
    {
        // Arrange
        var repository = new FakeProjectRepository();
        var service = new ProjectService(
            repository,
            new CreateProjectRequestValidator(),
            new UpdateProjectRequestValidator());

        var request = new CreateProjectRequest
        {
            Name = "Projet Test",
            OwnerUserId = Guid.NewGuid()
        };

        // Act
        var result = await service.CreateAsync(request, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("Projet Test", result.Name);
        Assert.Equal(1, repository.Items.Count);
        Assert.Equal(1, repository.SaveChangesCallCount);
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_When_Found()
    {
        // Arrange
        var repository = new FakeProjectRepository();
        var existing = new Project("Initial", Guid.NewGuid());
        repository.Items.Add(existing);

        var service = new ProjectService(
            repository,
            new CreateProjectRequestValidator(),
            new UpdateProjectRequestValidator());

        var request = new UpdateProjectRequest { Name = "Nom Modifié" };

        // Act
        var updated = await service.UpdateAsync(existing.Id, request, CancellationToken.None);

        // Assert
        Assert.NotNull(updated);
        Assert.Equal("Nom Modifié", updated!.Name);
        Assert.Equal(1, repository.SaveChangesCallCount);
    }

    // Fake repository minimal pour tests
    private sealed class FakeProjectRepository : IProjectRepository
    {
        public List<Project> Items { get; } = new();
        public int SaveChangesCallCount { get; private set; }

        public Task AddAsync(Project project, CancellationToken cancellationToken = default)
        {
            Items.Add(project);
            return Task.CompletedTask;
        }

        public Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var item = Items.FirstOrDefault(x => x.Id == id);
            return Task.FromResult<Project?>(item);
        }

        public Task<IReadOnlyList<Project>> GetByOwnerUserIdAsync(Guid ownerUserId, CancellationToken cancellationToken = default)
        {
            IReadOnlyList<Project> list = Items.Where(x => x.OwnerUserId == ownerUserId).ToList();
            return Task.FromResult(list);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveChangesCallCount++;
            return Task.CompletedTask;
        }
    }
}