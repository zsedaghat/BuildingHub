
using System.Text.RegularExpressions;

namespace BuildingHub.Modules.Community.Domain.ValueObjects;

public static class MobileNumberNormalizer
{
    public static string Normalize(string mobile)
    {
        if (string.IsNullOrWhiteSpace(mobile))
            throw new ArgumentException(
                "Mobile number is required.",
                nameof(mobile));

        var value = mobile.Trim()
            .Replace(" ", "")
            .Replace("-", "")
            .Replace("(", "")
            .Replace(")", "");

        value = ConvertDigits(value);

        if (value.StartsWith("0098"))
            value = "+" + value[2..];

        else if (value.StartsWith("09"))
            value = "+98" + value[1..];

        else if (value.StartsWith("98"))
            value = "+" + value;

        if (!Regex.IsMatch(value, @"^\+989\d{9}$"))
            throw new ArgumentException(
                "Invalid Iranian mobile number.",
                nameof(mobile));

        return value;
    }

    private static string ConvertDigits(string value)
    {
        const string persian = "۰۱۲۳۴۵۶۷۸۹";
        const string arabic = "٠١٢٣٤٥٦٧٨٩";

        var chars = value.ToCharArray();

        for (var i = 0; i < chars.Length; i++)
        {
            var index = persian.IndexOf(chars[i]);

            if (index < 0)
                index = arabic.IndexOf(chars[i]);

            if (index >= 0)
                chars[i] = (char)('0' + index);
        }

        return new string(chars);
    }
}
