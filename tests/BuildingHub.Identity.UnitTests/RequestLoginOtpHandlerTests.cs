
using BuildingHub.Modules.Identity.Application.Abstractions;
using BuildingHub.Modules.Identity.Application.Authentication.RequestLoginOtp;

namespace BuildingHub.Identity.UnitTests;

public sealed class RequestLoginOtpHandlerTests
{
    [Fact]
    public async Task Handle_InvalidPhone_ShouldThrowException()
    {
        var sms = new FakeSmsSender();
        var handler = CreateHandler(
            sms,
            new OtpChallengeCreationResult(
                OtpChallengeCreationStatus.Created));

        await Assert.ThrowsAsync<ArgumentException>(
            () => handler.Handle(
                new RequestLoginOtpCommand("09123456789"),
                CancellationToken.None));

        Assert.Equal(0, sms.SendCount);
    }

    [Fact]
    public async Task Handle_ValidPhone_ShouldSendSms()
    {
        var sms = new FakeSmsSender();

        var handler = CreateHandler(
            sms,
            new OtpChallengeCreationResult(
                OtpChallengeCreationStatus.Created));

        var result = await handler.Handle(
            new RequestLoginOtpCommand("+989123456789"),
            CancellationToken.None);

        Assert.True(result.Accepted);
        Assert.NotNull(result.ChallengeId);
        Assert.NotEqual(Guid.Empty, result.ChallengeId.Value);
        Assert.Equal(1, sms.SendCount);
    }

    [Fact]
    public async Task Handle_Cooldown_ShouldNotSendSms()
    {
        var sms = new FakeSmsSender();

        var handler = CreateHandler(
            sms,
            new OtpChallengeCreationResult(
                OtpChallengeCreationStatus.Cooldown,
                30));

        var result = await handler.Handle(
            new RequestLoginOtpCommand("+989123456789"),
            CancellationToken.None);

        Assert.True(result.Accepted);
        Assert.Null(result.ChallengeId);
        Assert.Equal(30, result.RetryAfterSeconds);
        Assert.Equal(0, sms.SendCount);
    }

    private static RequestLoginOtpHandler CreateHandler(
        FakeSmsSender sms,
        OtpChallengeCreationResult repositoryResult)
    {
        return new RequestLoginOtpHandler(
            new FakeOtpCodeService(),
            new FakeOtpChallengeRepository(repositoryResult),
            sms);
    }

    private sealed class FakeSmsSender : ISmsSender
    {
        public int SendCount { get; private set; }

        public Task SendOtpAsync(
            string phoneE164,
            string code,
            CancellationToken cancellationToken = default)
        {
            SendCount++;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeOtpCodeService : IOtpCodeService
    {
        public string GenerateCode() => "123456";

        public byte[] ComputeHmac(
            Guid challengeId,
            string phoneE164,
            string code)
        {
            return new byte[32];
        }

        public bool VerifyCode(
            Guid challengeId,
            string phoneE164,
            string code,
            byte[] expectedHmac)
        {
            return true;
        }
    }

    private sealed class FakeOtpChallengeRepository
        : IOtpChallengeRepository
    {
        private readonly OtpChallengeCreationResult _result;

        public FakeOtpChallengeRepository(
            OtpChallengeCreationResult result)
        {
            _result = result;
        }

        public Task<OtpChallengeCreationResult> CreateAsync(
            Guid id,
            string phoneE164,
            string purpose,
            byte[] codeHmac,
            DateTime nowUtc,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_result);
        }
    }
}
