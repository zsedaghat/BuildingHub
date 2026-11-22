using BuildingHub.SharedKernel.Domain;

namespace BuildingHub.Modules.Tenancy.Domain.Entities;

/// <summary>
/// اطلاعات ثبتی/حقوقی اختیاری مربوط به یک Tenant.
/// رابطه یک‌به‌یک آن با Tenant در لایه Infrastructure پیکربندی می‌شود.
/// </summary>
public sealed class Organization : Entity
{
    public Guid TenantId { get; private set; }
    public string Name { get; private set; } = null!;
    public string? RegistrationNumber { get; private set; }
    public string? NationalId { get; private set; }
    public string? Phone { get; private set; }
    public string? Address { get; private set; }

    private Organization()
    {
    }

    private Organization(
        Guid tenantId,
        string name,
        string? registrationNumber,
        string? nationalId,
        string? phone,
        string? address)
    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException("TenantId is required.", nameof(tenantId));

        TenantId = tenantId;
        Update(name, registrationNumber, nationalId, phone, address);
    }

    public static Organization Create(
        Guid tenantId,
        string name,
        string? registrationNumber = null,
        string? nationalId = null,
        string? phone = null,
        string? address = null)
        => new(tenantId, name, registrationNumber, nationalId, phone, address);

    public void Update(
        string name,
        string? registrationNumber,
        string? nationalId,
        string? phone,
        string? address)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Organization name is required.", nameof(name));

        Name = name.Trim();
        RegistrationNumber = NormalizeOptional(registrationNumber);
        NationalId = NormalizeOptional(nationalId);
        Phone = NormalizeOptional(phone);
        Address = NormalizeOptional(address);
    }

    private static string? NormalizeOptional(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
