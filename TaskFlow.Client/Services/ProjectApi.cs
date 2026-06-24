using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TaskFlow.Client.Models;

namespace TaskFlow.Client.Services;

public class ProjectApi
{
    private readonly HttpClient _http;
    private readonly IAuthService _authService;

    public ProjectApi(HttpClient http, IAuthService authService)
    {
        _http = http;
        _authService = authService;
    }

    private async Task<HttpRequestMessage> CreateJsonRequest(HttpMethod method, string url, object? body = null)
    {
        var req = new HttpRequestMessage(method, url);
        if (body is not null)
        {
            var json = JsonSerializer.Serialize(body);
            req.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        // Récupérer le token depuis AuthService
        var token = await _authService.GetTokenAsync();

        if (!string.IsNullOrWhiteSpace(token))
        {
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            Console.WriteLine($"[ProjectApi] Authorization header attached: Bearer {token.Substring(0, Math.Min(8, token.Length))}...");
        }
        else
        {
            Console.WriteLine($"[ProjectApi] WARNING: No bearer token for {method} {url}");
        }

        return req;
    }

    public async Task<ProjectDto> CreateAsync(CreateProjectRequest req)
    {
        using var httpReq = await CreateJsonRequest(HttpMethod.Post, "api/projects", req);
        using var resp = await _http.SendAsync(httpReq);
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<ProjectDto>() ?? throw new InvalidOperationException("Empty response");
    }

    public async Task<ProjectDto?> GetByIdAsync(Guid id)
    {
        using var httpReq = await CreateJsonRequest(HttpMethod.Get, $"api/projects/{id}");
        using var resp = await _http.SendAsync(httpReq);
        if (!resp.IsSuccessStatusCode) return null;
        return await resp.Content.ReadFromJsonAsync<ProjectDto>();
    }

    public async Task<List<ProjectDto>> GetByOwnerAsync(Guid ownerId)
    {
        using var httpReq = await CreateJsonRequest(HttpMethod.Get, $"api/projects/owner/{ownerId}");
        using var resp = await _http.SendAsync(httpReq);
        if (!resp.IsSuccessStatusCode) return new List<ProjectDto>();
        return await resp.Content.ReadFromJsonAsync<List<ProjectDto>>() ?? new List<ProjectDto>();
    }

    public async Task<ProjectDto?> UpdateAsync(Guid id, UpdateProjectRequest req)
    {
        using var httpReq = await CreateJsonRequest(HttpMethod.Put, $"api/projects/{id}", req);
        using var resp = await _http.SendAsync(httpReq);
        if (!resp.IsSuccessStatusCode) return null;
        return await resp.Content.ReadFromJsonAsync<ProjectDto>();
    }
}
