namespace TaskFlow.Application.DTOs.Users;

public sealed class UpdateUserRequest
{
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}