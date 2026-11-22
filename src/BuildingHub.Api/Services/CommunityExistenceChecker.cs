
using BuildingHub.Modules.Tenancy.Application.Abstractions;
using BuildingHub.Modules.Tenancy.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BuildingHub.Api.Services;

public sealed class CommunityExistenceChecker
    : ICommunityExistenceChecker
{
    private readonly TenancyDbContext _dbContext;

    public CommunityExistenceChecker(
        TenancyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> HasCommunitiesAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.TenantCommunityAssignments
            .AnyAsync(
                x => x.TenantId == tenantId,
                cancellationToken);
    }
}
