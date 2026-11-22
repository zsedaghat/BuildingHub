
using MediatR;

namespace BuildingHub.Modules.Identity.Application.Authentication.VerifyLoginOtp;

public sealed record VerifyLoginOtpCommand(
    Guid ChallengeId,
    string Code
) : IRequest<VerifyLoginOtpResult>;
