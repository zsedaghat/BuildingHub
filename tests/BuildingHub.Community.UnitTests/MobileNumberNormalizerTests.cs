
using BuildingHub.Modules.Community.Domain.ValueObjects;

namespace BuildingHub.Community.UnitTests;

public sealed class MobileNumberNormalizerTests
{
    [Theory]
    [InlineData("09121234567", "+989121234567")]
    [InlineData("989121234567", "+989121234567")]
    [InlineData("+989121234567", "+989121234567")]
    [InlineData("00989121234567", "+989121234567")]
    [InlineData("۰۹۱۲۱۲۳۴۵۶۷", "+989121234567")]
    [InlineData("٠٩١٢١٢٣٤٥٦٧", "+989121234567")]
    [InlineData("0912 123 4567", "+989121234567")]
    public void Normalize_ValidMobile_ReturnsE164(
        string input,
        string expected)
    {
        var result = MobileNumberNormalizer.Normalize(input);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("12345")]
    [InlineData("02112345678")]
    [InlineData("+98912123456")]
    [InlineData("+9891212345678")]
    [InlineData("+14155552671")]
    public void Normalize_InvalidMobile_ThrowsArgumentException(
        string input)
    {
        Assert.Throws<ArgumentException>(
            () => MobileNumberNormalizer.Normalize(input));
    }

    [Fact]
    public void Normalize_NullMobile_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => MobileNumberNormalizer.Normalize(null!));
    }

    [Fact]
    public void Normalize_AlreadyNormalized_IsIdempotent()
    {
        const string mobile = "+989121234567";

        var result = MobileNumberNormalizer.Normalize(
            MobileNumberNormalizer.Normalize(mobile));

        Assert.Equal(mobile, result);
    }
}
