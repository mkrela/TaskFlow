namespace TaskFlow.Application.DTOs.Users;

public sealed class CreateUserRequest
{
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}