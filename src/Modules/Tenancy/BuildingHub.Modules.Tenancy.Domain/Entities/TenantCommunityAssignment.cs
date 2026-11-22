
using BuildingHub.SharedKernel.Domain;

namespace BuildingHub.Modules.Tenancy.Domain.Entities;

public sealed class TenantCommunityAssignment : AggregateRoot
{
    public Guid TenantId { get; private set; }

    public Guid CommunityId { get; private set; }

    public DateTime StartAtUtc { get; private set; }

    public DateTime? EndAtUtc { get; private set; }

    public string TransferReason { get; private set; } = null!;

    public Guid CreatedByUserId { get; private set; }

    public DateTime UpdatedAtUtc { get; private set; }

    public int Version { get; private set; }

    private TenantCommunityAssignment()
    {
    }

    private TenantCommunityAssignment(
        Guid tenantId,
        Guid communityId,
        DateTime startAtUtc,
        string transferReason,
        Guid createdByUserId)
    {
        TenantId = tenantId;
        CommunityId = communityId;
        StartAtUtc = startAtUtc;
        TransferReason = NormalizeReason(transferReason);
        CreatedByUserId = createdByUserId;
        UpdatedAtUtc = DateTime.UtcNow;
        Version = 1;
    }

    public static TenantCommunityAssignment Create(
        Guid tenantId,
        Guid communityId,
        DateTime startAtUtc,
        string transferReason,
        Guid createdByUserId)
    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException("TenantId is required.");

        if (communityId == Guid.Empty)
            throw new ArgumentException("CommunityId is required.");

        if (createdByUserId == Guid.Empty)
            throw new ArgumentException("CreatedByUserId is required.");

        if (startAtUtc.Kind != DateTimeKind.Utc)
            throw new ArgumentException(
                "StartAtUtc must be UTC.");

        if (startAtUtc > DateTime.UtcNow)
            throw new ArgumentException(
                "Future assignments are not allowed.");

        return new TenantCommunityAssignment(
            tenantId,
            communityId,
            startAtUtc,
            transferReason,
            createdByUserId);
    }

    public void End(DateTime endAtUtc)
    {
        if (EndAtUtc.HasValue)
            throw new InvalidOperationException(
                "Assignment has already ended.");

        if (endAtUtc.Kind != DateTimeKind.Utc)
            throw new ArgumentException(
                "EndAtUtc must be UTC.");

        if (endAtUtc <= StartAtUtc)
            throw new ArgumentException(
                "EndAtUtc must be greater than StartAtUtc.");

        EndAtUtc = endAtUtc;
        UpdatedAtUtc = DateTime.UtcNow;
        Version++;
    }

    private static string NormalizeReason(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException(
                "Transfer reason is required.",
                nameof(reason));

        return reason.Trim();
    }
}
