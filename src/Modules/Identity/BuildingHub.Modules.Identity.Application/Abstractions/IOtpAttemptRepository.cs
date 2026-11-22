
namespace BuildingHub.Modules.Identity.Application.Abstractions;

public interface IOtpAttemptRepository
{
    Task<OtpVerificationResult> VerifyAndConsumeAsync(
        Guid challengeId,
        string code,
        CancellationToken cancellationToken = default);
}
