
namespace BuildingHub.Modules.Identity.Application.Abstractions;

public interface IOtpCodeService
{
    string GenerateCode();

    byte[] ComputeHmac(
        Guid challengeId,
        string phoneE164,
        string code);

    bool VerifyCode(
        Guid challengeId,
        string phoneE164,
        string code,
        byte[] expectedHmac);
}
