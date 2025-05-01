using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace WeatherForecastHub.Authentication;

/// <summary>
/// 不做實際驗證的身份驗證處理器，用於解決身份驗證相關的系統例外
/// </summary>
public class NoAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public NoAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // 永遠回傳成功，但不包含任何實際身份資訊
        var principal = new ClaimsPrincipal(new ClaimsIdentity());
        var ticket = new AuthenticationTicket(principal, "NoAuth");
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
