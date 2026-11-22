
using BuildingHub.Modules.Identity.Application.Abstractions;
using MediatR;

namespace BuildingHub.Modules.Identity.Application.Authentication.VerifyLoginOtp;

public sealed class VerifyLoginOtpHandler
    : IRequestHandler<VerifyLoginOtpCommand, VerifyLoginOtpResult>
{
    private readonly IOtpAttemptRepository _otpAttemptRepository;

    public VerifyLoginOtpHandler(
        IOtpAttemptRepository otpAttemptRepository)
    {
        _otpAttemptRepository = otpAttemptRepository;
    }

    public async Task<VerifyLoginOtpResult> Handle(
        VerifyLoginOtpCommand request,
        CancellationToken cancellationToken)
    {
        var result = await _otpAttemptRepository.VerifyAndConsumeAsync(
            request.ChallengeId,
            request.Code,
            cancellationToken);

        return result switch
        {
            OtpVerificationResult.Success =>
                new VerifyLoginOtpResult(
                    Succeeded: true),

            OtpVerificationResult.InvalidCode =>
                new VerifyLoginOtpResult(
                    Succeeded: false,
                    ErrorCode: "InvalidCode"),

            _ => new VerifyLoginOtpResult(
                Succeeded: false,
                ErrorCode: "InvalidOrExpiredOtp")
        };
    }
}
