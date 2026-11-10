using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using ShipNotes.Api.Authentication;
using ShipNotes.Api.Authorization;
using ShipNotes.Api.ErrorHandling;
using ShipNotes.Api.Media;
using ShipNotes.Application.Abstractions;

namespace ShipNotes.Api.Extensions;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddOpenApi();
        services.AddProblemDetails();
        services.AddExceptionHandler<ProblemDetailsExceptionHandler>();
        services.AddHttpContextAccessor();
        services.AddScoped<IMediaUrlProvider, HttpMediaUrlProvider>();

        services.AddApiKeyAuthentication();
        services.AddShipNotesCors(configuration);
        services.AddPublicApiRateLimiting();

        return services;
    }

    private static void AddApiKeyAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication()
            .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(ApiKeyAuthenticationDefaults.Scheme, null);
        services.AddSingleton<IAuthorizationHandler, ApiKeyMatchesApplicationHandler>();
        services.AddAuthorizationBuilder()
            .AddPolicy(AuthorizationPolicies.ApplicationApiKey, policy => policy
                .AddAuthenticationSchemes(ApiKeyAuthenticationDefaults.Scheme)
                .RequireAuthenticatedUser()
                .AddRequirements(new ApiKeyMatchesApplicationRequirement()));
    }

    private static void AddShipNotesCors(this IServiceCollection services, IConfiguration configuration)
    {
        var adminOrigins = configuration.GetSection("Cors:AdminOrigins").Get<string[]>() ?? [];

        services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicies.AdminUi, policy =>
                policy.WithOrigins(adminOrigins).AllowAnyHeader().AllowAnyMethod());

            options.AddPolicy(CorsPolicies.PublicApi, policy =>
                policy.AllowAnyOrigin().WithHeaders(ApiKeyAuthenticationDefaults.HeaderName, "Content-Type").WithMethods("GET"));
        });
    }

    private static void AddPublicApiRateLimiting(this IServiceCollection services) =>
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddPolicy(RateLimitPolicies.PublicApi, context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    context.Request.Headers[ApiKeyAuthenticationDefaults.HeaderName].ToString() is { Length: > 0 } key
                        ? key
                        : context.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
                    _ => new FixedWindowRateLimiterOptions { PermitLimit = 120, Window = TimeSpan.FromMinutes(1) }));
        });
}
