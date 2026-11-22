
namespace BuildingHub.Modules.Identity.Application.Authentication.VerifyLoginOtp;

public sealed record VerifyLoginOtpResult(
    bool Succeeded,
    string? ErrorCode = null);
