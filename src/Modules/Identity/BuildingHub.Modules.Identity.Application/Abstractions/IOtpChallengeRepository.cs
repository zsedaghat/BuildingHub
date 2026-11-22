
namespace BuildingHub.Modules.Identity.Application.Abstractions;

public interface IOtpChallengeRepository
{
    Task<OtpChallengeCreationResult> CreateAsync(
        Guid id,
        string phoneE164,
        string purpose,
        byte[] codeHmac,
        DateTime nowUtc,
        CancellationToken cancellationToken = default);
}
