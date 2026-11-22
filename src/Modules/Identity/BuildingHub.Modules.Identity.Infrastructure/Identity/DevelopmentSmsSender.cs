
using BuildingHub.Modules.Identity.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace BuildingHub.Modules.Identity.Infrastructure.Identity;

public sealed class DevelopmentSmsSender : ISmsSender
{
    private readonly ILogger<DevelopmentSmsSender> _logger;

    public DevelopmentSmsSender(
        ILogger<DevelopmentSmsSender> logger)
    {
        _logger = logger;
    }

    public Task SendOtpAsync(
        string phoneE164,
        string code,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var maskedPhone = phoneE164.Length > 4
            ? phoneE164[..4] + "****" + phoneE164[^2..]
            : "****";

        _logger.LogInformation(
            "[DEVELOPMENT OTP] Phone: {Phone}, Code: {Code}",
            maskedPhone,
            code);

        return Task.CompletedTask;
    }
}
