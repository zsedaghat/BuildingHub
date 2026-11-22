
namespace BuildingHub.Modules.Identity.Application.Authentication.RequestLoginOtp;

public sealed record RequestLoginOtpResult(
    bool Accepted,
    Guid? ChallengeId = null,
    int? RetryAfterSeconds = null);
