using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using TaskFlow.Application.DTOs.Users;
using TaskFlow.Application.Services.Users;
using TaskFlow.Application.Validators.Users;
using TaskFlow.Domain.Entities;
using TaskFlow.Application.Abstractions.Persistence;
using Xunit;

namespace TaskFlow.Tests.Services;

public class UserServiceTests
{
    [Fact]
    public async Task CreateAsync_Should_Create_And_Save()
    {
        var repo = new FakeUserRepository();
        var service = new UserService(repo, new CreateUserRequestValidator(), new UpdateUserRequestValidator());

        var request = new CreateUserRequest { Name = "Test", Email = "t@test.com" };

        var dto = await service.CreateAsync(request, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, dto.Id);
        Assert.Equal("Test", dto.Name);
        Assert.Equal(1, repo.Items.Count);
        Assert.Equal(1, repo.SaveChangesCallCount);
    }

    [Fact]
    public async Task UpdateAsync_Should_Return_Null_When_Not_Found()
    {
        var repo = new FakeUserRepository();
        var service = new UserService(repo, new CreateUserRequestValidator(), new UpdateUserRequestValidator());

        var result = await service.UpdateAsync(Guid.NewGuid(), new UpdateUserRequest { Name = "X", Email = "x@x.com" });

        Assert.Null(result);
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        public System.Collections.Generic.List<User> Items { get; } = new();
        public int SaveChangesCallCount { get; private set; }

        public Task AddAsync(User user, CancellationToken cancellationToken = default)
        {
            Items.Add(user);
            return Task.CompletedTask;
        }

        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<User?>(Items.FirstOrDefault(u => u.Email == email));
        }

        public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<User?>(Items.FirstOrDefault(u => u.Id == id));
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveChangesCallCount++;
            return Task.CompletedTask;
        }
    }
}