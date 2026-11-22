
using BuildingHub.Modules.Community.Application.Abstractions;
using BuildingHub.Modules.Tenancy.Domain.Enums;
using BuildingHub.Modules.Tenancy.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BuildingHub.Api.Services;

public sealed class TenantExistenceChecker : ITenantExistenceChecker
{
    private readonly TenancyDbContext _dbContext;

    public TenantExistenceChecker(TenancyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> ExistsAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Tenants.AnyAsync(
            x => x.Id == tenantId &&
                 x.Status == TenantStatus.Active,
            cancellationToken);
    }
}
