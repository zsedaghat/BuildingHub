
using BuildingHub.Modules.Tenancy.Application.Abstractions;
using BuildingHub.Modules.Tenancy.Domain.Entities;
using BuildingHub.Modules.Tenancy.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BuildingHub.Modules.Tenancy.Infrastructure.Repositories;

public sealed class TenantCommunityAssignmentRepository
    : ITenantCommunityAssignmentRepository
{
    private readonly TenancyDbContext _context;

    public TenantCommunityAssignmentRepository(
        TenancyDbContext context)
    {
        _context = context;
    }

    public async Task<TenantCommunityAssignment?>
        GetEffectiveByCommunityAsync(
            Guid communityId,
            DateTime atUtc,
            CancellationToken cancellationToken = default)
    {
        ValidateIds(communityId);
        ValidateUtc(atUtc);

        return await _context.TenantCommunityAssignments
            .SingleOrDefaultAsync(
                x =>
                    x.CommunityId == communityId &&
                    x.StartAtUtc <= atUtc &&
                    (x.EndAtUtc == null || x.EndAtUtc > atUtc),
                cancellationToken);
    }

    public async Task<IReadOnlyList<TenantCommunityAssignment>>
        GetEffectiveByTenantAsync(
            Guid tenantId,
            DateTime atUtc,
            CancellationToken cancellationToken = default)
    {
        ValidateIds(tenantId);
        ValidateUtc(atUtc);

        return await _context.TenantCommunityAssignments
            .AsNoTracking()
            .Where(
                x =>
                    x.TenantId == tenantId &&
                    x.StartAtUtc <= atUtc &&
                    (x.EndAtUtc == null || x.EndAtUtc > atUtc))
            .OrderBy(x => x.CommunityId)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasEffectiveAssignmentAsync(
        Guid tenantId,
        Guid communityId,
        DateTime atUtc,
        CancellationToken cancellationToken = default)
    {
        ValidateIds(tenantId, communityId);
        ValidateUtc(atUtc);

        return await _context.TenantCommunityAssignments
            .AsNoTracking()
            .AnyAsync(
                x =>
                    x.TenantId == tenantId &&
                    x.CommunityId == communityId &&
                    x.StartAtUtc <= atUtc &&
                    (x.EndAtUtc == null || x.EndAtUtc > atUtc),
                cancellationToken);
    }

    public async Task AddAsync(
        TenantCommunityAssignment assignment,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(assignment);

        await _context.TenantCommunityAssignments.AddAsync(
            assignment,
            cancellationToken);
    }

    private static void ValidateIds(params Guid[] ids)
    {
        if (ids.Any(x => x == Guid.Empty))
            throw new ArgumentException(
                "Identifiers cannot be empty.");
    }

    private static void ValidateUtc(DateTime value)
    {
        if (value.Kind != DateTimeKind.Utc)
            throw new ArgumentException(
                "The supplied date must be UTC.");
    }
}
