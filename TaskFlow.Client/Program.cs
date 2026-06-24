using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using TaskFlow.Client;
using TaskFlow.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

// Enregistrer le LoggingHttpMessageHandler pour être utilisé comme handler personnalisé
builder.Services.AddScoped<LoggingHttpMessageHandler>();

// Créer un HttpClient qui utilise le handler
builder.Services.AddScoped(sp =>
{
    var handler = sp.GetRequiredService<LoggingHttpMessageHandler>();
    var client = new HttpClient(handler);
    client.BaseAddress = new Uri("http://localhost:5016/");
    return client;
});

// Services d'authentification et API
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ProjectApi>();
builder.Services.AddScoped<TaskApi>();

await builder.Build().RunAsync();
