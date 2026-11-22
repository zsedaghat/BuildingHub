
using BuildingHub.Modules.Tenancy.Domain.Enums;
using BuildingHub.SharedKernel.Domain;

namespace BuildingHub.Modules.Tenancy.Domain.Entities;

public sealed class Tenant : AggregateRoot
{
    public string Name { get; private set; } = null!;

    public TenantStatus Status { get; private set; }

    public DateTime UpdatedAtUtc { get; private set; }

    private Tenant()
    {
    }

    private Tenant(string name)
    {
        Name = NormalizeName(name);
        Status = TenantStatus.Active;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public static Tenant Create(string name)
        => new(name);

    public void Rename(string name)
    {
        Name = NormalizeName(name);
        Touch();
    }

    public void ChangeStatus(TenantStatus status)
    {
        if (!Enum.IsDefined(status))
            throw new ArgumentOutOfRangeException(nameof(status));

        Status = status;
        Touch();
    }

    private void Touch()
    {
        UpdatedAtUtc = DateTime.UtcNow;
    }

    private static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Tenant name is required.", nameof(name));

        return name.Trim();
    }
}
