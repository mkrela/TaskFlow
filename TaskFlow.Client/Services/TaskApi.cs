using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TaskFlow.Client.Models;

namespace TaskFlow.Client.Services;

public class TaskApi
{
    private readonly HttpClient _http;
    public TaskApi(HttpClient http) => _http = http;

    public async Task<TaskDto> CreateAsync(CreateTaskRequest req)
    {
        var resp = await _http.PostAsJsonAsync("api/tasks", req);
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<TaskDto>() ?? throw new InvalidOperationException("Empty response");
    }

    public Task<TaskDto?> GetByIdAsync(Guid id)
        => _http.GetFromJsonAsync<TaskDto>($"api/tasks/{id}");

    public async Task<List<TaskDto>> GetByProjectAsync(Guid projectId)
        => await _http.GetFromJsonAsync<List<TaskDto>>($"api/tasks/project/{projectId}") ?? new List<TaskDto>();

    public async Task<TaskDto?> UpdateAsync(Guid id, UpdateTaskRequest req)
    {
        var resp = await _http.PutAsJsonAsync($"api/tasks/{id}", req);
        if (!resp.IsSuccessStatusCode) return null;
        return await resp.Content.ReadFromJsonAsync<TaskDto>();
    }

    public async Task<bool> ChangeStatusAsync(Guid id, ChangeTaskStatusRequest req)
    {
        var message = new HttpRequestMessage(new HttpMethod("PATCH"), $"api/tasks/{id}/status")
        {
            Content = JsonContent.Create(req)
        };
        var resp = await _http.SendAsync(message);
        return resp.IsSuccessStatusCode;
    }
}