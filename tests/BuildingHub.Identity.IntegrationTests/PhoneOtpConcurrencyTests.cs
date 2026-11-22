
using System.Security.Cryptography;
using BuildingHub.Identity.IntegrationTests.Infrastructure;
using BuildingHub.Modules.Identity.Application.Abstractions;
using BuildingHub.Modules.Identity.Infrastructure.Identity;
using BuildingHub.Modules.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace BuildingHub.Identity.IntegrationTests;

public class PhoneOtpConcurrencyTests
{
    private const string CorrectCode = "123456";
    private const string WrongCode = "999999";

    private static IdentityDbContext CreateDb()
    {
        return new IdentityDbContext(
            IdentityTestDatabase.CreateOptions());
    }

    private static string NewPhone()
    {
        return "+989" +
            Random.Shared.NextInt64(100000000, 999999999);
    }

    private static OtpCodeService CreateCodeService()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["Security:OtpPepper"] =
                        Convert.ToBase64String(
                            RandomNumberGenerator.GetBytes(32))
                })
            .Build();

        return new OtpCodeService(configuration);
    }

    private static async Task CreateChallengeAsync(
        Guid id,
        string phone,
        byte[] hmac,
        DateTime createdAtUtc,
        int maxAttempts = 5)
    {
        await using var db = CreateDb();

        db.PhoneOtpChallenges.Add(
            PhoneOtpChallenge.Create(
                id,
                phone,
                "Login",
                hmac,
                createdAtUtc,
                maxAttempts: maxAttempts));

        await db.SaveChangesAsync();
    }

    private static async Task<OtpVerificationResult> VerifyAsync(
        Guid id,
        string code,
        OtpCodeService codeService)
    {
        await using var db = CreateDb();

        var repository = new OtpAttemptRepository(
            db, codeService);

        return await repository.VerifyAndConsumeAsync(
            id, code);
    }

    [Fact]
    public async Task Cannot_Create_Two_Active_Otps_For_Same_Phone()
    {
        var phone = NewPhone();
        var now = DateTime.UtcNow;
        var hmac = RandomNumberGenerator.GetBytes(32);

        await CreateChallengeAsync(
            Guid.NewGuid(), phone, hmac, now);

        await using var db = CreateDb();

        db.PhoneOtpChallenges.Add(
            PhoneOtpChallenge.Create(
                Guid.NewGuid(),
                phone,
                "Login",
                hmac,
                now));

        var exception =
            await Assert.ThrowsAsync<DbUpdateException>(
                () => db.SaveChangesAsync());

        var postgresException =
            Assert.IsType<Npgsql.PostgresException>(
                exception.InnerException);

        Assert.Equal(
            Npgsql.PostgresErrorCodes.UniqueViolation,
            postgresException.SqlState);
    }

    [Fact]
    public async Task Only_One_Request_Can_Consume_Same_Otp()
    {
        var id = Guid.NewGuid();
        var phone = NewPhone();
        var codeService = CreateCodeService();

        var hmac = codeService.ComputeHmac(
            id, phone, CorrectCode);

        await CreateChallengeAsync(
            id, phone, hmac, DateTime.UtcNow);

        var results = await Task.WhenAll(
            Task.Run(() => VerifyAsync(
                id, CorrectCode, codeService)),
            Task.Run(() => VerifyAsync(
                id, CorrectCode, codeService)));

        Assert.Single(
            results,
            x => x == OtpVerificationResult.Success);

        Assert.Single(
            results,
            x => x == OtpVerificationResult.Unavailable);

        await using var db = CreateDb();

        var challenge = await db.PhoneOtpChallenges
            .AsNoTracking()
            .SingleAsync(x => x.Id == id);

        Assert.Equal(
            OtpChallengeStatus.Consumed,
            challenge.Status);

        Assert.NotNull(challenge.ConsumedAtUtc);
    }

    [Fact]
    public async Task Wrong_Otp_Code_Should_Increase_Attempt_Count()
    {
        var id = Guid.NewGuid();
        var phone = NewPhone();
        var codeService = CreateCodeService();

        var hmac = codeService.ComputeHmac(
            id, phone, CorrectCode);

        await CreateChallengeAsync(
            id, phone, hmac, DateTime.UtcNow);

        var result = await VerifyAsync(
            id, WrongCode, codeService);

        Assert.Equal(
            OtpVerificationResult.InvalidCode,
            result);

        await using var db = CreateDb();

        var challenge = await db.PhoneOtpChallenges
            .AsNoTracking()
            .SingleAsync(x => x.Id == id);

        Assert.Equal(1, challenge.AttemptCount);
        Assert.Equal(
            OtpChallengeStatus.Active,
            challenge.Status);
        Assert.Null(challenge.ConsumedAtUtc);
    }

    [Fact]
    public async Task Otp_Should_Lock_After_Five_Wrong_Attempts()
    {
        var id = Guid.NewGuid();
        var phone = NewPhone();
        var codeService = CreateCodeService();

        var hmac = codeService.ComputeHmac(
            id, phone, CorrectCode);

        await CreateChallengeAsync(
            id, phone, hmac, DateTime.UtcNow,
            maxAttempts: 5);

        for (var i = 0; i < 5; i++)
        {
            var result = await VerifyAsync(
                id, WrongCode, codeService);

            Assert.Equal(
                OtpVerificationResult.InvalidCode,
                result);
        }

        await using (var db = CreateDb())
        {
            var challenge = await db.PhoneOtpChallenges
                .AsNoTracking()
                .SingleAsync(x => x.Id == id);

            Assert.Equal(5, challenge.AttemptCount);
            Assert.Equal(
                OtpChallengeStatus.Locked,
                challenge.Status);
            Assert.Null(challenge.ConsumedAtUtc);
        }

        var correctCodeResult = await VerifyAsync(
            id, CorrectCode, codeService);

        Assert.Equal(
            OtpVerificationResult.Unavailable,
            correctCodeResult);
    }

    [Fact]
    public async Task Expired_Otp_Should_Not_Be_Consumed()
    {
        var id = Guid.NewGuid();
        var phone = NewPhone();
        var codeService = CreateCodeService();

        var hmac = codeService.ComputeHmac(
            id, phone, CorrectCode);

        await CreateChallengeAsync(
            id,
            phone,
            hmac,
            DateTime.UtcNow.AddMinutes(-10));

        var result = await VerifyAsync(
            id, CorrectCode, codeService);

        Assert.Equal(
            OtpVerificationResult.Unavailable,
            result);

        await using var db = CreateDb();

        var challenge = await db.PhoneOtpChallenges
            .AsNoTracking()
            .SingleAsync(x => x.Id == id);

        Assert.Null(challenge.ConsumedAtUtc);
        Assert.Equal(
            OtpChallengeStatus.Active,
            challenge.Status);
    }

    [Fact]
    public async Task Resend_Should_Supersede_Previous_Otp()
    {
        var phone = NewPhone();
        var oldId = Guid.NewGuid();
        var newId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        await CreateChallengeAsync(
            oldId,
            phone,
            RandomNumberGenerator.GetBytes(32),
            now.AddSeconds(-61));

        await using (var db = CreateDb())
        {
            var repository = new OtpChallengeRepository(db);

            var result = await repository.CreateAsync(
                newId,
                phone,
                "Login",
                RandomNumberGenerator.GetBytes(32),
                now);

            Assert.Equal(
                OtpChallengeCreationStatus.Created,
                result.Status);
        }

        await using (var db = CreateDb())
        {
            var challenges = await db.PhoneOtpChallenges
                .AsNoTracking()
                .Where(x => x.PhoneE164 == phone)
                .ToListAsync();

            Assert.Equal(2, challenges.Count);

            Assert.Equal(
                OtpChallengeStatus.Superseded,
                challenges.Single(x => x.Id == oldId).Status);

            Assert.Equal(
                OtpChallengeStatus.Active,
                challenges.Single(x => x.Id == newId).Status);
        }
    }

    [Fact]
    public async Task Resend_Before_60_Seconds_Should_Return_Cooldown()
    {
        var phone = NewPhone();
        var oldId = Guid.NewGuid();
        var newId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        await CreateChallengeAsync(
            oldId,
            phone,
            RandomNumberGenerator.GetBytes(32),
            now.AddSeconds(-30));

        await using (var db = CreateDb())
        {
            var repository = new OtpChallengeRepository(db);

            var result = await repository.CreateAsync(
                newId,
                phone,
                "Login",
                RandomNumberGenerator.GetBytes(32),
                now);

            Assert.Equal(
                OtpChallengeCreationStatus.Cooldown,
                result.Status);

            Assert.Equal(30, result.RetryAfterSeconds);
        }

        await using (var db = CreateDb())
        {
            var challenges = await db.PhoneOtpChallenges
                .AsNoTracking()
                .Where(x => x.PhoneE164 == phone)
                .ToListAsync();

            Assert.Single(challenges);
            Assert.Equal(oldId, challenges[0].Id);

            Assert.Equal(
                OtpChallengeStatus.Active,
                challenges[0].Status);
        }
    }

    [Fact]
    public async Task Sixth_Otp_Request_Should_Be_Blocked()
    {
        var phone = NewPhone();
        var now = DateTime.UtcNow;

        for (var i = 0; i < 5; i++)
        {
            await using var db = CreateDb();

            var repository = new OtpChallengeRepository(db);

            var result = await repository.CreateAsync(
                Guid.NewGuid(),
                phone,
                "Login",
                RandomNumberGenerator.GetBytes(32),
                now.AddMinutes(-50 + i * 10));

            Assert.Equal(
                OtpChallengeCreationStatus.Created,
                result.Status);
        }

        await using (var db = CreateDb())
        {
            var repository = new OtpChallengeRepository(db);

            var result = await repository.CreateAsync(
                Guid.NewGuid(),
                phone,
                "Login",
                RandomNumberGenerator.GetBytes(32),
                now);

            Assert.Equal(
                OtpChallengeCreationStatus.HourlyLimit,
                result.Status);

            Assert.True(result.RetryAfterSeconds > 0);
        }

        await using (var db = CreateDb())
        {
            var challenges = await db.PhoneOtpChallenges
                .AsNoTracking()
                .Where(x =>
                    x.PhoneE164 == phone &&
                    x.Purpose == "Login")
                .ToListAsync();

            Assert.Equal(5, challenges.Count);

            Assert.Equal(
                1,
                challenges.Count(x =>
                    x.Status == OtpChallengeStatus.Active));

            Assert.Equal(
                4,
                challenges.Count(x =>
                    x.Status == OtpChallengeStatus.Superseded));
        }
    }
}
