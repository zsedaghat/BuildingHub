
using BuildingHub.Modules.Tenancy.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BuildingHub.Modules.Tenancy.Infrastructure.Persistence;

public sealed class TenancyDbContext : DbContext
{
    public TenancyDbContext(
        DbContextOptions<TenancyDbContext> options)
        : base(options)
    {
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();

    public DbSet<TenantCommunityAssignment>
        TenantCommunityAssignments =>
            Set<TenantCommunityAssignment>();

    public DbSet<TenantUserMembership>
        TenantUserMemberships =>
            Set<TenantUserMembership>();
    public DbSet<TenantMembershipScope> TenantMembershipScopes
    => Set<TenantMembershipScope>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("tenancy");

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(TenancyDbContext).Assembly);
    }
}
