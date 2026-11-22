
using System.Security.Cryptography;
using System.Text;
using BuildingHub.Modules.Identity.Application.Abstractions;
using Microsoft.Extensions.Configuration;

namespace BuildingHub.Modules.Identity.Infrastructure.Identity;

public sealed class OtpCodeService : IOtpCodeService
{
    private readonly byte[] _pepper;

    public OtpCodeService(IConfiguration configuration)
    {
        var pepperBase64 = configuration["Security:OtpPepper"];

        if (string.IsNullOrWhiteSpace(pepperBase64))
        {
            throw new InvalidOperationException(
                "Security:OtpPepper is not configured.");
        }

        try
        {
            _pepper = Convert.FromBase64String(pepperBase64);
        }
        catch (FormatException ex)
        {
            throw new InvalidOperationException(
                "OTP pepper must be Base64 encoded.",
                ex);
        }

        if (_pepper.Length < 32)
        {
            throw new InvalidOperationException(
                "OTP pepper must contain at least 32 bytes.");
        }
    }

    public string GenerateCode()
    {
        var number = RandomNumberGenerator.GetInt32(
            0,
            1_000_000);

        return number.ToString("D6");
    }

    public byte[] ComputeHmac(
        Guid challengeId,
        string phoneE164,
        string code)
    {
        ValidateInput(challengeId, phoneE164, code);

        var payload = Encoding.UTF8.GetBytes(
            $"{challengeId:N}|{phoneE164}|{code}");

        return HMACSHA256.HashData(_pepper, payload);
    }

    public bool VerifyCode(
        Guid challengeId,
        string phoneE164,
        string code,
        byte[] expectedHmac)
    {
        ValidateInput(challengeId, phoneE164, code);

        if (expectedHmac is null ||
            expectedHmac.Length != 32)
        {
            return false;
        }

        var actualHmac = ComputeHmac(
            challengeId,
            phoneE164,
            code);

        return CryptographicOperations.FixedTimeEquals(
            actualHmac,
            expectedHmac);
    }

    private static void ValidateInput(
        Guid challengeId,
        string phoneE164,
        string code)
    {
        if (challengeId == Guid.Empty)
            throw new ArgumentException(
                "Challenge ID is required.",
                nameof(challengeId));

        if (string.IsNullOrWhiteSpace(phoneE164) ||
            !phoneE164.StartsWith('+'))
            throw new ArgumentException(
                "Phone must be normalized to E.164.",
                nameof(phoneE164));

        if (code is null ||
            code.Length != 6 ||
            code.Any(c => c < '0' || c > '9'))
            throw new ArgumentException(
                "OTP code must contain exactly six digits.",
                nameof(code));
    }
}
