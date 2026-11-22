
using BuildingHub.Modules.Identity.Application.Abstractions;
using BuildingHub.Modules.Identity.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

namespace BuildingHub.Modules.Identity.Infrastructure.Persistence;

public sealed class OtpAttemptRepository : IOtpAttemptRepository
{
    private readonly IdentityDbContext _dbContext;
    private readonly IOtpCodeService _otpCodeService;

    public OtpAttemptRepository(
        IdentityDbContext dbContext,
        IOtpCodeService otpCodeService)
    {
        _dbContext = dbContext;
        _otpCodeService = otpCodeService;
    }

    public async Task<OtpVerificationResult> VerifyAndConsumeAsync(
        Guid challengeId,
        string code,
        CancellationToken cancellationToken = default)
    {
        if (challengeId == Guid.Empty ||
            string.IsNullOrWhiteSpace(code) ||
            code.Length != 6 ||
            code.Any(c => c < '0' || c > '9'))
        {
            return OtpVerificationResult.Unavailable;
        }

        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync(
                cancellationToken);

        // PostgreSQL locks the challenge row until commit.
        var challenge = await _dbContext.PhoneOtpChallenges
            .FromSqlInterpolated(
                $"""
                SELECT *
                FROM identity.phone_otp_challenge
                WHERE id = {challengeId}
                FOR UPDATE
                """)
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken);

        if (challenge is null ||
            challenge.Status != OtpChallengeStatus.Active ||
            challenge.ConsumedAtUtc is not null ||
            challenge.ExpiresAtUtc <= DateTime.UtcNow ||
            challenge.AttemptCount >= challenge.MaxAttempts)
        {
            return OtpVerificationResult.Unavailable;
        }

        var isValid = _otpCodeService.VerifyCode(
            challenge.Id,
            challenge.PhoneE164,
            code,
            challenge.CodeHmac);

        if (!isValid)
        {
            var nextAttemptCount = challenge.AttemptCount + 1;

            var nextStatus =
                nextAttemptCount >= challenge.MaxAttempts
                    ? OtpChallengeStatus.Locked
                    : OtpChallengeStatus.Active;

            await _dbContext.PhoneOtpChallenges
                .Where(x => x.Id == challengeId)
                .ExecuteUpdateAsync(
                    setters => setters
                        .SetProperty(
                            x => x.AttemptCount,
                            nextAttemptCount)
                        .SetProperty(
                            x => x.Status,
                            nextStatus),
                    cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return OtpVerificationResult.InvalidCode;
        }

        await _dbContext.PhoneOtpChallenges
            .Where(x => x.Id == challengeId)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(
                        x => x.AttemptCount,
                        challenge.AttemptCount + 1)
                    .SetProperty(
                        x => x.Status,
                        OtpChallengeStatus.Consumed)
                    .SetProperty(
                        x => x.ConsumedAtUtc,
                        DateTime.UtcNow),
                cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return OtpVerificationResult.Success;
    }
}
