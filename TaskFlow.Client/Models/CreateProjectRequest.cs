// File: TaskFlow.Client/Models/CreateProjectRequest.cs
using System;

namespace TaskFlow.Client.Models;

public record CreateProjectRequest(string Name, Guid OwnerUserId);