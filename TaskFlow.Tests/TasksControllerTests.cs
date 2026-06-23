using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TaskFlow.Api.Controllers;
using TaskFlow.Application.DTOs.Tasks;
using TaskFlow.Application.Services.Tasks;
using TaskFlow.Domain.Enums;
using Xunit;

// Alias pour lever l'ambiguïté avec System.Threading.Tasks.TaskStatus
using DomainTaskStatus = TaskFlow.Domain.Enums.TaskStatus;

namespace TaskFlow.Tests.Controllers;

public class TasksControllerTests
{
    [Fact]
    public async Task Create_Returns_CreatedAtAction_With_Task()
    {
        // Arrange
        var service = new FakeTaskService();
        var controller = new TasksController(service);

        var request = new CreateTaskRequest
        {
            Title = "Tâche test",
            ProjectId = Guid.NewGuid(),
            CreatedByUserId = Guid.NewGuid()
        };

        // Act
        var actionResult = await controller.Create(request, CancellationToken.None);

        // Assert
        var created = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
        var dto = Assert.IsType<TaskDto>(created.Value);
        Assert.Equal("GetById", created.ActionName);
        Assert.Equal(dto.Id, (Guid)created.RouteValues!["id"]!);
        Assert.Equal("Tâche test", dto.Title);
    }

    [Fact]
    public async Task ChangeStatus_Returns_NoContent_When_Updated()
    {
        // Arrange
        var service = new FakeTaskService { ChangeStatusResult = true };
        var controller = new TasksController(service);
        var id = Guid.NewGuid();

        // Act
        var result = await controller.ChangeStatus(id, new ChangeTaskStatusRequest { Status = DomainTaskStatus.Done }, CancellationToken.None);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    // Fake ITaskService minimal
    private sealed class FakeTaskService : ITaskService
    {
        public bool ChangeStatusResult { get; set; } = false;

        public Task<TaskDto> CreateAsync(CreateTaskRequest request, CancellationToken cancellationToken = default)
        {
            var dto = new TaskDto
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                Status = DomainTaskStatus.Todo,
                Priority = TaskPriority.Medium,
                DueDate = request.DueDate,
                ProjectId = request.ProjectId,
                CreatedByUserId = request.CreatedByUserId
            };
            return Task.FromResult(dto);
        }

        public Task<bool> ChangeStatusAsync(Guid id, TaskFlow.Domain.Enums.TaskStatus status, CancellationToken cancellationToken = default)
            => Task.FromResult(ChangeStatusResult);

        public Task<TaskDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<TaskDto?>(null);

        public Task<IReadOnlyList<TaskDto>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<TaskDto>>(new List<TaskDto>());

        public Task<TaskDto?> UpdateAsync(Guid id, UpdateTaskRequest request, CancellationToken cancellationToken = default)
            => Task.FromResult<TaskDto?>(null);
    }
}