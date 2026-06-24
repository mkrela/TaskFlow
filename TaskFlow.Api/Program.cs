using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TaskFlow.Application;
using TaskFlow.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();

// CORS (développement) : autoriser l'origine du client Blazor
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", policy =>
    {
        policy.WithOrigins("http://localhost:5186", "https://localhost:5186", "http://localhost:5016", "https://localhost:7161")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();  // IMPORTANT: Autorise les headers d'authentification
    });
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // dev only
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                Console.WriteLine("OnMessageReceived: Authorization header present? " + (ctx.Request.Headers.ContainsKey("Authorization")));
                return Task.CompletedTask;
            },
            OnTokenValidated = ctx =>
            {
                Console.WriteLine("OnTokenValidated: principal nameid = " + ctx.Principal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = ctx =>
            {
                Console.WriteLine("OnAuthenticationFailed: " + ctx.Exception);
                return Task.CompletedTask;
            },
            OnChallenge = ctx =>
            {
                Console.WriteLine("OnChallenge: error = " + ctx.Error + " description = " + ctx.ErrorDescription);
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler(/* unchanged */);

app.UseHttpsRedirection();

// Appliquer la policy CORS avant authentication/authorization
app.UseCors("DevCors");

// IMPORTANT : UseAuthentication avant UseAuthorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();