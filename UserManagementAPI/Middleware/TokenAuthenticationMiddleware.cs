using System.Security.Cryptography;
using System.Text;

namespace UserManagementAPI.Middleware;

public sealed class TokenAuthenticationMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<TokenAuthenticationMiddleware> logger)
{
    private const string AuthorizationScheme = "Bearer ";
    private const string DefaultToken = "techhive-demo-token";

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase))
        {
            await next(context);
            return;
        }

        var configuredToken = configuration["ApiToken"] ?? DefaultToken;
        var authorization = context.Request.Headers.Authorization.ToString();
        var suppliedToken = authorization.StartsWith(AuthorizationScheme, StringComparison.OrdinalIgnoreCase)
            ? authorization[AuthorizationScheme.Length..].Trim()
            : string.Empty;

        if (!TokensMatch(suppliedToken, configuredToken))
        {
            logger.LogWarning("Rejected unauthorized request {Method} {Path}", context.Request.Method, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            context.Response.Headers.WWWAuthenticate = "Bearer";
            await context.Response.WriteAsJsonAsync(new { error = "Unauthorized." });
            return;
        }

        await next(context);
    }

    private static bool TokensMatch(string suppliedToken, string configuredToken)
    {
        var suppliedBytes = Encoding.UTF8.GetBytes(suppliedToken);
        var configuredBytes = Encoding.UTF8.GetBytes(configuredToken);
        return CryptographicOperations.FixedTimeEquals(suppliedBytes, configuredBytes);
    }
}