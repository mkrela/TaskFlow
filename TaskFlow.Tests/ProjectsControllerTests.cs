using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Api.Controllers;
using TaskFlow.Application.DTOs.Projects;
using TaskFlow.Application.Services.Projects;
using TaskFlow.Api;
using Xunit;

namespace TaskFlow.Tests.Controllers;

public class ProjectsControllerTests
{
    [Fact]
    public async Task Create_Returns_CreatedAtAction_With_Project()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var service = new FakeProjectService();
        var controller = new ProjectsController(service);

        var request = new CreateProjectRequest
        {
            Name = "Mon projet",
            OwnerUserId = ownerId
        };

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
        var dto = Assert.IsType<ProjectDto>(createdResult.Value);
        Assert.Equal("GetById", createdResult.ActionName);
        Assert.Equal(dto.Id, (Guid)createdResult.RouteValues!["id"]!);
        Assert.Equal("Mon projet", dto.Name);
        Assert.Equal(ownerId, dto.OwnerUserId);
    }

    [Fact]
    public async Task GetById_Returns_NotFound_When_Not_Exist()
    {
        // Arrange
        var service = new FakeProjectService(); // no project created
        var controller = new ProjectsController(service);

        // Act
        var actionResult = await controller.GetById(Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }

    // Fake IProjectService minimal implementation pour tests unitaires
    private sealed class FakeProjectService : IProjectService
    {
        public ProjectDto? LastCreated { get; private set; }

        public Task<ProjectDto> CreateAsync(CreateProjectRequest request, CancellationToken cancellationToken = default)
        {
            var dto = new ProjectDto
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                OwnerUserId = request.OwnerUserId
            };
            LastCreated = dto;
            return Task.FromResult(dto);
        }

        public Task<ProjectDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(LastCreated is not null && LastCreated.Id == id ? LastCreated : null);

        public Task<IReadOnlyList<ProjectDto>> GetByOwnerUserIdAsync(Guid ownerUserId, CancellationToken cancellationToken = default)
        {
            IReadOnlyList<ProjectDto> list = LastCreated is not null && LastCreated.OwnerUserId == ownerUserId
                ? new List<ProjectDto> { LastCreated }
                : new List<ProjectDto>();
            return Task.FromResult(list);
        }

        public Task<ProjectDto?> UpdateAsync(Guid id, UpdateProjectRequest request, CancellationToken cancellationToken = default)
        {
            if (LastCreated is null || LastCreated.Id != id) return Task.FromResult<ProjectDto?>(null);
            LastCreated = new ProjectDto { Id = id, Name = request.Name, OwnerUserId = LastCreated.OwnerUserId };
            return Task.FromResult<ProjectDto?>(LastCreated);
        }
    }
}