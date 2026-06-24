using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TaskFlow.Client.Services;

/// <summary>
/// HttpMessageHandler personnalisé pour logger toutes les requêtes HTTP
/// et vérifier la présence du header Authorization.
/// </summary>
public class LoggingHttpMessageHandler : HttpClientHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Log de la requête
        Console.WriteLine($"[HTTP Request] {request.Method} {request.RequestUri}");

        // Vérifier la présence du header Authorization
        if (request.Headers.Authorization != null)
        {
            Console.WriteLine($"  → Authorization header present: Bearer {request.Headers.Authorization.Parameter?.Substring(0, Math.Min(10, request.Headers.Authorization.Parameter?.Length ?? 0))}...");
        }
        else
        {
            Console.WriteLine("  → WARNING: No Authorization header detected!");
        }

        // Afficher tous les headers pour debug
        foreach (var header in request.Headers)
        {
            if (header.Key != "Authorization")
            {
                Console.WriteLine($"  → {header.Key}: {string.Join(",", header.Value)}");
            }
        }

        // Envoyer la requête
        var response = await base.SendAsync(request, cancellationToken);

        // Log de la réponse
        Console.WriteLine($"[HTTP Response] {request.Method} {request.RequestUri} → {(int)response.StatusCode} {response.StatusCode}");

        return response;
    }
}
