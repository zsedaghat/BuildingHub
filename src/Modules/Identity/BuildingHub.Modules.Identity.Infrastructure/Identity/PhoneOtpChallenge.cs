
namespace BuildingHub.Modules.Identity.Infrastructure.Identity;

public sealed class PhoneOtpChallenge
{
    private PhoneOtpChallenge()
    {
    }

    public Guid Id { get; private set; }

    public string PhoneE164 { get; private set; } = null!;

    public string Purpose { get; private set; } = null!;

    public byte[] CodeHmac { get; private set; } = null!;

    public OtpChallengeStatus Status { get; private set; }

    public int AttemptCount { get; private set; }

    public int MaxAttempts { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime ExpiresAtUtc { get; private set; }

    public DateTime ResendAvailableAtUtc { get; private set; }

    public DateTime? ConsumedAtUtc { get; private set; }

    public static PhoneOtpChallenge Create(
       Guid id,
       string phoneE164,
       string purpose,
       byte[] codeHmac,
       DateTime nowUtc,
       int expirationMinutes = 5,
       int maxAttempts = 5,
       int resendCooldownSeconds = 60)
    {

        if (id == Guid.Empty)
            throw new ArgumentException(
                "Challenge ID is required.",
                nameof(id));

        if (string.IsNullOrWhiteSpace(phoneE164))
            throw new ArgumentException(
                "Phone number is required.",
                nameof(phoneE164));

        if (purpose is not ("Login" or "ConfirmPhone" or "ChangePhone"))
            throw new ArgumentException(
                "Invalid OTP purpose.",
                nameof(purpose));

        if (codeHmac is null || codeHmac.Length != 32)
            throw new ArgumentException(
                "OTP HMAC must be 32 bytes.",
                nameof(codeHmac));

        if (expirationMinutes <= 0 ||
            maxAttempts <= 0 ||
            resendCooldownSeconds <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(expirationMinutes),
                "OTP limits must be positive.");

        if (nowUtc.Kind != DateTimeKind.Utc)
            throw new ArgumentException(
                "Timestamp must be UTC.",
                nameof(nowUtc));

        return new PhoneOtpChallenge
        {
            Id = id,
            PhoneE164 = phoneE164,
            Purpose = purpose,
            CodeHmac = (byte[])codeHmac.Clone(),
            Status = OtpChallengeStatus.Active,
            AttemptCount = 0,
            MaxAttempts = maxAttempts,
            CreatedAtUtc = nowUtc,
            ExpiresAtUtc = nowUtc.AddMinutes(expirationMinutes),
            ResendAvailableAtUtc =
                nowUtc.AddSeconds(resendCooldownSeconds)
        };
    }
}
