
using BuildingHub.Modules.Tenancy.Domain.Enums;
using BuildingHub.SharedKernel.Domain;

namespace BuildingHub.Modules.Tenancy.Domain.Entities;

public sealed class TenantMembershipScope : Entity
{
    public Guid MembershipId { get; private set; }

    public TenantMembershipScopeType ScopeType { get; private set; }

    public Guid ScopeId { get; private set; }

    public Guid CommunityId { get; private set; }

    public DateTime? RevokedAtUtc { get; private set; }

    private TenantMembershipScope()
    {
    }

    private TenantMembershipScope(
        Guid membershipId,
        TenantMembershipScopeType scopeType,
        Guid scopeId,
        Guid communityId)
    {
        MembershipId = membershipId;
        ScopeType = scopeType;
        ScopeId = scopeId;
        CommunityId = communityId;
    }

    public static TenantMembershipScope Create(
        Guid membershipId,
        TenantMembershipScopeType scopeType,
        Guid scopeId,
        Guid communityId)
    {
        if (membershipId == Guid.Empty)
            throw new ArgumentException(
                "MembershipId is required.");

        if (!Enum.IsDefined(scopeType))
            throw new ArgumentOutOfRangeException(
                nameof(scopeType));

        if (scopeId == Guid.Empty)
            throw new ArgumentException(
                "ScopeId is required.");

        if (communityId == Guid.Empty)
            throw new ArgumentException(
                "CommunityId is required.");

        if (scopeType == TenantMembershipScopeType.Community
            && scopeId != communityId)
        {
            throw new ArgumentException(
                "For Community scope, ScopeId must equal CommunityId.");
        }

        return new TenantMembershipScope(
            membershipId,
            scopeType,
            scopeId,
            communityId);
    }

    public void Revoke(DateTime revokedAtUtc)
    {
        if (RevokedAtUtc.HasValue)
            throw new InvalidOperationException(
                "Scope has already been revoked.");

        if (revokedAtUtc.Kind != DateTimeKind.Utc)
            throw new ArgumentException(
                "RevokedAtUtc must be UTC.");

        if (revokedAtUtc < CreatedAt)
            throw new ArgumentException(
                "RevokedAtUtc cannot be earlier than CreatedAt.");

        RevokedAtUtc = revokedAtUtc;
    }
}
