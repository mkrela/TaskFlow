using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Api.Controllers;
using TaskFlow.Application.DTOs.Users;
using TaskFlow.Application.Services.Users;
using Xunit;

namespace TaskFlow.Tests.Controllers;

public class UsersControllerTests
{
    [Fact]
    public async Task Create_Returns_CreatedAtAction()
    {
        var service = new FakeUserService();
        var controller = new UsersController(service);

        var request = new CreateUserRequest { Name = "U", Email = "u@u.com" };

        var result = await controller.Create(request, CancellationToken.None);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var dto = Assert.IsType<UserDto>(created.Value);
        Assert.Equal("GetById", created.ActionName);
        Assert.Equal(dto.Id, (Guid)created.RouteValues!["id"]!);
    }

    private sealed class FakeUserService : IUserService
    {
        public Task<UserDto> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
            => Task.FromResult(new UserDto { Id = Guid.NewGuid(), Name = request.Name, Email = request.Email });

        public Task<UserDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
            => Task.FromResult<UserDto?>(null);

        public Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<UserDto?>(null);

        public Task<UserDto?> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken = default)
            => Task.FromResult<UserDto?>(null);
    }
}