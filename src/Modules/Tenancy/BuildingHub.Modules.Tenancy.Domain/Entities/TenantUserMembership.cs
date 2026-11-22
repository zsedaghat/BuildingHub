
using BuildingHub.Modules.Tenancy.Domain.Enums;
using BuildingHub.SharedKernel.Domain;

namespace BuildingHub.Modules.Tenancy.Domain.Entities;

public sealed class TenantUserMembership : Entity
{
    public Guid TenantId { get; private set; }

    public Guid UserId { get; private set; }

    public TenantMembershipRole Role { get; private set; }

    public TenantMembershipStatus Status { get; private set; }

    public TenantMembershipScopeMode ScopeMode { get; private set; }

    public DateTime StartAtUtc { get; private set; }

    public DateTime? EndAtUtc { get; private set; }

    public DateTime UpdatedAtUtc { get; private set; }

    public int Version { get; private set; }

    private TenantUserMembership()
    {
    }

    private TenantUserMembership(
        Guid tenantId,
        Guid userId,
        TenantMembershipRole role,
        TenantMembershipScopeMode scopeMode,
        DateTime startAtUtc)
    {
        TenantId = tenantId;
        UserId = userId;
        Role = role;
        ScopeMode = scopeMode;
        Status = TenantMembershipStatus.Active;
        StartAtUtc = startAtUtc;
        UpdatedAtUtc = DateTime.UtcNow;
        Version = 1;
    }

    public static TenantUserMembership Create(
        Guid tenantId,
        Guid userId,
        TenantMembershipRole role,
        TenantMembershipScopeMode scopeMode,
        DateTime startAtUtc)
    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException(
                "TenantId is required.",
                nameof(tenantId));

        if (userId == Guid.Empty)
            throw new ArgumentException(
                "UserId is required.",
                nameof(userId));

        if (!Enum.IsDefined(role))
            throw new ArgumentOutOfRangeException(
                nameof(role));

        if (!Enum.IsDefined(scopeMode))
            throw new ArgumentOutOfRangeException(
                nameof(scopeMode));

        if (startAtUtc.Kind != DateTimeKind.Utc)
            throw new ArgumentException(
                "StartAtUtc must be UTC.",
                nameof(startAtUtc));

        return new TenantUserMembership(
            tenantId,
            userId,
            role,
            scopeMode,
            startAtUtc);
    }

    public void ChangeRole(TenantMembershipRole role)
    {
        EnsureNotEnded();

        if (!Enum.IsDefined(role))
            throw new ArgumentOutOfRangeException(
                nameof(role));

        if (Role == role)
            return;

        Role = role;
        Touch();
    }

    public void ChangeStatus(TenantMembershipStatus status)
    {
        EnsureNotEnded();

        if (!Enum.IsDefined(status))
            throw new ArgumentOutOfRangeException(
                nameof(status));

        if (Status == status)
            return;

        Status = status;
        Touch();
    }

    public void ChangeScopeMode(
        TenantMembershipScopeMode scopeMode)
    {
        EnsureNotEnded();

        if (!Enum.IsDefined(scopeMode))
            throw new ArgumentOutOfRangeException(
                nameof(scopeMode));

        if (ScopeMode == scopeMode)
            return;

        ScopeMode = scopeMode;
        Touch();
    }

    public void End(DateTime endAtUtc)
    {
        EnsureNotEnded();

        if (endAtUtc.Kind != DateTimeKind.Utc)
            throw new ArgumentException(
                "EndAtUtc must be UTC.",
                nameof(endAtUtc));

        if (endAtUtc <= StartAtUtc)
            throw new ArgumentException(
                "EndAtUtc must be greater than StartAtUtc.",
                nameof(endAtUtc));

        EndAtUtc = endAtUtc;
        Touch();
    }

    private void EnsureNotEnded()
    {
        if (EndAtUtc.HasValue)
        {
            throw new InvalidOperationException(
                "Cannot modify an ended membership.");
        }
    }

    private void Touch()
    {
        UpdatedAtUtc = DateTime.UtcNow;
        Version++;
    }
}
