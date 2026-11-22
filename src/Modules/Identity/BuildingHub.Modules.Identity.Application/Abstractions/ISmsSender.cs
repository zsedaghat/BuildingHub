
namespace BuildingHub.Modules.Identity.Application.Abstractions;

public interface ISmsSender
{
    Task SendOtpAsync(
        string phoneE164,
        string code,
        CancellationToken cancellationToken = default);
}
