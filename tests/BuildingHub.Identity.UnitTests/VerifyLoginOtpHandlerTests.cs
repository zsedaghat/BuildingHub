
using BuildingHub.Modules.Identity.Application.Abstractions;
using BuildingHub.Modules.Identity.Application.Authentication.VerifyLoginOtp;
using Xunit;

namespace BuildingHub.Identity.UnitTests;

public sealed class VerifyLoginOtpHandlerTests
{
    [Fact]
    public async Task ValidCode_ShouldSucceed()
    {
        var handler = CreateHandler(
            OtpVerificationResult.Success);

        var result = await handler.Handle(
            new VerifyLoginOtpCommand(
                Guid.NewGuid(), "123456"),
            CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.Null(result.ErrorCode);
    }

    [Fact]
    public async Task InvalidCode_ShouldFail()
    {
        var handler = CreateHandler(
            OtpVerificationResult.InvalidCode);

        var result = await handler.Handle(
            new VerifyLoginOtpCommand(
                Guid.NewGuid(), "000000"),
            CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal("InvalidCode", result.ErrorCode);
    }

    [Fact]
    public async Task UnavailableChallenge_ShouldFail()
    {
        var handler = CreateHandler(
            OtpVerificationResult.Unavailable);

        var result = await handler.Handle(
            new VerifyLoginOtpCommand(
                Guid.NewGuid(), "123456"),
            CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal(
            "InvalidOrExpiredOtp",
            result.ErrorCode);
    }

    private static VerifyLoginOtpHandler CreateHandler(
        OtpVerificationResult verificationResult)
    {
        return new VerifyLoginOtpHandler(
            new FakeOtpAttemptRepository(verificationResult));
    }

    private sealed class FakeOtpAttemptRepository
        : IOtpAttemptRepository
    {
        private readonly OtpVerificationResult _result;

        public FakeOtpAttemptRepository(
            OtpVerificationResult result)
        {
            _result = result;
        }

        public Task<OtpVerificationResult> VerifyAndConsumeAsync(
            Guid challengeId,
            string code,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_result);
        }
    }
}
