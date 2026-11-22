
using System.Text.RegularExpressions;
using BuildingHub.Modules.Identity.Application.Abstractions;
using MediatR;

namespace BuildingHub.Modules.Identity.Application.Authentication.RequestLoginOtp;

public sealed class RequestLoginOtpHandler
    : IRequestHandler<RequestLoginOtpCommand, RequestLoginOtpResult>
{
    private const string Purpose = "Login";

    private readonly IOtpCodeService _otpCodeService;
    private readonly IOtpChallengeRepository _challengeRepository;
    private readonly ISmsSender _smsSender;

    public RequestLoginOtpHandler(
        IOtpCodeService otpCodeService,
        IOtpChallengeRepository challengeRepository,
        ISmsSender smsSender)
    {
        _otpCodeService = otpCodeService;
        _challengeRepository = challengeRepository;
        _smsSender = smsSender;
    }

    public async Task<RequestLoginOtpResult> Handle(
        RequestLoginOtpCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.PhoneE164) ||
            !Regex.IsMatch(
                request.PhoneE164,
                @"^\+[1-9]\d{7,14}$"))
        {
            throw new ArgumentException(
                "Phone number must be in E.164 format.");
        }

        var nowUtc = DateTime.UtcNow;

        var challengeId = Guid.CreateVersion7();

        var code = _otpCodeService.GenerateCode();

        var codeHmac = _otpCodeService.ComputeHmac(
            challengeId,
            request.PhoneE164,
            code);

        var creationResult = await _challengeRepository.CreateAsync(
            challengeId,
            request.PhoneE164,
            Purpose,
            codeHmac,
            nowUtc,
            cancellationToken);

        if (creationResult.Status != OtpChallengeCreationStatus.Created)
        {
            return new RequestLoginOtpResult(
                Accepted: true,
                RetryAfterSeconds: creationResult.RetryAfterSeconds);
        }

        // Send SMS only after the challenge transaction has committed.
        await _smsSender.SendOtpAsync(
            request.PhoneE164,
            code,
            cancellationToken);

        return new RequestLoginOtpResult(
    Accepted: true,
    ChallengeId: challengeId);
    }
}
