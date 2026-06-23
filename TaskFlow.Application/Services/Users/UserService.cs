using FluentValidation;
using TaskFlow.Application.Abstractions.Persistence;
using TaskFlow.Application.DTOs.Users;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Services.Users;

public sealed class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IValidator<CreateUserRequest> _createValidator;
    private readonly IValidator<UpdateUserRequest> _updateValidator;

    public UserService(
        IUserRepository userRepository,
        IValidator<CreateUserRequest> createValidator,
        IValidator<UpdateUserRequest> updateValidator)
    {
        _userRepository = userRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<UserDto> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);

        var user = new User(request.Name, request.Email);
        await _userRepository.AddAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return Map(user);
    }

    public async Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        return user is null ? null : Map(user);
    }

    public async Task<UserDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
        return user is null ? null : Map(user);
    }

    public async Task<UserDto?> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        await _update_validator_or_throw();

        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user is null) return null;

        // Domain entity has no Update method; update via reflection of constructor pattern
        // simple approach: create replacement values using private setters
        user.GetType().GetProperty("Name")!.SetValue(user, request.Name);
        user.GetType().GetProperty("Email")!.SetValue(user, request.Email);

        await _userRepository.SaveChangesAsync(cancellationToken);
        return Map(user);

        async Task _update_validator_or_throw()
        {
            await _updateValidator.ValidateAndThrowAsync(request, cancellationToken);
        }
    }

    private static UserDto Map(User user) => new()
    {
        Id = user.Id,
        Name = user.Name,
        Email = user.Email
    };
}