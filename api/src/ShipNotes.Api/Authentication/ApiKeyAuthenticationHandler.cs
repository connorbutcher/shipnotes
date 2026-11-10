using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using ShipNotes.Application.Abstractions;

namespace ShipNotes.Api.Authentication;

/// <summary>Authenticates consuming applications by the API key sent in the <c>X-Api-Key</c> header.</summary>
internal sealed class ApiKeyAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IClientApplicationRepository applications,
    IApiKeyService apiKeys)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(ApiKeyAuthenticationDefaults.HeaderName, out var values)
            || values.ToString() is not { Length: > 0 } apiKey)
        {
            return AuthenticateResult.NoResult();
        }

        var application = await applications.FindByApiKeyHashAsync(apiKeys.Hash(apiKey), Context.RequestAborted);
        if (application is null)
        {
            return AuthenticateResult.Fail("Invalid API key.");
        }

        Claim[] claims =
        [
            new(ApiKeyAuthenticationDefaults.ApplicationIdClaim, application.Id.ToString()),
            new(ClaimTypes.Name, application.Name),
        ];
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, Scheme.Name));
        return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));
    }
}
