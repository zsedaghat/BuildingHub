
namespace BuildingHub.Modules.Identity.Application.Abstractions;

public enum OtpChallengeCreationStatus
{
    Created,
    Cooldown,
    HourlyLimit
}

public sealed record OtpChallengeCreationResult(
    OtpChallengeCreationStatus Status,
    int RetryAfterSeconds = 0);
