using System;
using System.Collections.Generic;
using System.Text;

namespace TaskFlow.Domain.Entities;

public class User
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;

    private User() { } // EF Core

    public User(string name, string email)
    {
        Name = name;
        Email = email;
    }
}