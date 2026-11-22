
namespace BuildingHub.Modules.Reference.Domain.Entities;

public sealed class Currency
{
    public string Code { get; private set; } = null!;

    public string Name { get; private set; } = null!;

    public byte MinorUnitDigits { get; private set; }

    public bool IsActive { get; private set; }

    private Currency()
    {
    }

    public Currency(
        string code,
        string name,
        byte minorUnitDigits)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException(
                "Currency code is required.", nameof(code));

        if (code.Length != 3 ||
            code.Any(c => c < 'A' || c > 'Z'))
            throw new ArgumentException(
                "Currency code must contain three uppercase letters.",
                nameof(code));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Currency name is required.", nameof(name));

        if (minorUnitDigits > 6)
            throw new ArgumentOutOfRangeException(
                nameof(minorUnitDigits));

        Code = code;
        Name = name.Trim();
        MinorUnitDigits = minorUnitDigits;
        IsActive = true;
    }
}
