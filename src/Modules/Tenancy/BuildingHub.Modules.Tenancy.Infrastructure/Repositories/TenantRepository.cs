using BuildingHub.Modules.Tenancy.Application.Abstractions;
using BuildingHub.Modules.Tenancy.Domain.Entities;
using BuildingHub.Modules.Tenancy.Infrastructure.Persistence;

namespace BuildingHub.Modules.Tenancy.Infrastructure.Repositories;

public sealed class TenantRepository
    : Repository<Tenant>, ITenantRepository
{
    public TenantRepository(TenancyDbContext context)
        : base(context)
    {
    }
}