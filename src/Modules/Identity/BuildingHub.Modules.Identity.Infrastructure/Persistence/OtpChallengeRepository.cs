
using BuildingHub.Modules.Identity.Application.Abstractions;
using BuildingHub.Modules.Identity.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;

namespace BuildingHub.Modules.Identity.Infrastructure.Persistence;

public sealed class OtpChallengeRepository : IOtpChallengeRepository
{
    private const int CooldownSeconds = 60;
    private const int MaxRequestsPerHour = 5;

    private readonly IdentityDbContext _dbContext;

    public OtpChallengeRepository(IdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<OtpChallengeCreationResult> CreateAsync(
        Guid id,
        string phoneE164,
        string purpose,
        byte[] codeHmac,
        DateTime nowUtc,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(codeHmac);

        if (id == Guid.Empty)
            throw new ArgumentException(
                "Challenge ID is required.",
                nameof(id));

        if (string.IsNullOrWhiteSpace(phoneE164))
            throw new ArgumentException(
                "Phone number is required.",
                nameof(phoneE164));

        if (nowUtc.Kind != DateTimeKind.Utc)
            throw new ArgumentException(
                "Timestamp must be UTC.",
                nameof(nowUtc));

        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync(
                cancellationToken);

        var connection = _dbContext.Database.GetDbConnection();

        // Serialize OTP creation for the same phone and purpose.
        await using (var command = connection.CreateCommand())
        {
            command.Transaction = transaction.GetDbTransaction();

            command.CommandText = """
                SELECT pg_advisory_xact_lock(
                    hashtextextended(@lock_key, 0)
                );
                """;

            command.Parameters.Add(
                new NpgsqlParameter(
                    "lock_key",
                    $"{phoneE164}:{purpose}"));

            await command.ExecuteNonQueryAsync(
                cancellationToken);
        }

        var oneHourAgo = nowUtc.AddHours(-1);

        var recentChallenges = _dbContext.PhoneOtpChallenges
            .Where(x =>
                x.PhoneE164 == phoneE164 &&
                x.Purpose == purpose &&
                x.CreatedAtUtc > oneHourAgo &&
                x.CreatedAtUtc <= nowUtc);

        // Check resend cooldown.
        var latestRequest = await recentChallenges
            .MaxAsync(
                x => (DateTime?)x.CreatedAtUtc,
                cancellationToken);

        if (latestRequest.HasValue)
        {
            var cooldownEndsAt =
                latestRequest.Value.AddSeconds(CooldownSeconds);

            if (cooldownEndsAt > nowUtc)
            {
                var retryAfter = Math.Max(
                    1,
                    (int)Math.Ceiling(
                        (cooldownEndsAt - nowUtc).TotalSeconds));

                return new OtpChallengeCreationResult(
                    OtpChallengeCreationStatus.Cooldown,
                    retryAfter);
            }
        }

        // Check hourly request limit.
        var requestCount = await recentChallenges.CountAsync(
            cancellationToken);

        if (requestCount >= MaxRequestsPerHour)
        {
            var oldestRequest = await recentChallenges
                .MinAsync(
                    x => x.CreatedAtUtc,
                    cancellationToken);

            var retryAt = oldestRequest.AddHours(1);

            var retryAfter = Math.Max(
                1,
                (int)Math.Ceiling(
                    (retryAt - nowUtc).TotalSeconds));

            return new OtpChallengeCreationResult(
                OtpChallengeCreationStatus.HourlyLimit,
                retryAfter);
        }

        // Supersede the previous active challenge.
        await _dbContext.PhoneOtpChallenges
            .Where(x =>
                x.PhoneE164 == phoneE164 &&
                x.Purpose == purpose &&
                x.Status == OtpChallengeStatus.Active)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(
                    x => x.Status,
                    OtpChallengeStatus.Superseded),
                cancellationToken);

        // Create the new challenge.
        var challenge = PhoneOtpChallenge.Create(
            id,
            phoneE164,
            purpose,
            codeHmac,
            nowUtc);

        await _dbContext.PhoneOtpChallenges.AddAsync(
            challenge,
            cancellationToken);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        await transaction.CommitAsync(
            cancellationToken);

        return new OtpChallengeCreationResult(
            OtpChallengeCreationStatus.Created);
    }
}
