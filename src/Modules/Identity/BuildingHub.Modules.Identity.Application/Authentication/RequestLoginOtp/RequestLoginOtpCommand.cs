
using MediatR;

namespace BuildingHub.Modules.Identity.Application.Authentication.RequestLoginOtp;

public sealed record RequestLoginOtpCommand(
    string PhoneE164
) : IRequest<RequestLoginOtpResult>;
