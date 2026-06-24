using System;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace TaskFlow.Client.Services;

/// <summary>
/// Service d'authentification centralisé pour Blazor WASM.
/// Gère le token en mémoire (localStorage optionnel via JS interop).
/// </summary>
public interface IAuthService
{
    Task<bool> LoginAsync(string username, string password);
    Task LogoutAsync();
    Task<string?> GetTokenAsync();
    event Action? OnLoggedIn;
    event Action? OnLoggedOut;
}

public class AuthService : IAuthService
{
    private readonly HttpClient _http;
    private const string TOKEN_KEY = "auth_token";
    private string? _cachedToken;

    public event Action? OnLoggedIn;
    public event Action? OnLoggedOut;

    public AuthService(HttpClient http)
    {
        _http = http;
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        try
        {
            Console.WriteLine("[AuthService] Attempting login...");
            var response = await _http.PostAsJsonAsync("api/auth/login", new { username, password });

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[AuthService] Login failed with status {response.StatusCode}");
                return false;
            }

            using var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
            var token = doc.RootElement.GetProperty("token").GetString();

            if (string.IsNullOrWhiteSpace(token))
            {
                Console.WriteLine("[AuthService] Login response contained no token");
                return false;
            }

            // Persister le token en mémoire
            _cachedToken = token;
            Console.WriteLine($"[AuthService] Login successful, token stored (truncated): {token.Substring(0, Math.Min(10, token.Length))}...");

            OnLoggedIn?.Invoke();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AuthService] Login error: {ex.Message}");
            return false;
        }
    }

    public async Task LogoutAsync()
    {
        _cachedToken = null;
        OnLoggedOut?.Invoke();
        Console.WriteLine("[AuthService] Logged out, token cleared");
        await Task.CompletedTask;
    }

    public async Task<string?> GetTokenAsync()
    {
        await Task.CompletedTask;
        return _cachedToken;
    }
}

