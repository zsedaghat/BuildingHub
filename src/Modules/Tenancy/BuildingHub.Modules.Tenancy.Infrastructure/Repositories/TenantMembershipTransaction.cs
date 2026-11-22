
using BuildingHub.Modules.Tenancy.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace BuildingHub.Modules.Tenancy.Infrastructure.Persistence;

public sealed class TenantMembershipTransaction
    : ITenantMembershipTransaction
{
    private readonly TenancyDbContext _context;

    public TenantMembershipTransaction(
        TenancyDbContext context)
    {
        _context = context;
    }

    public async Task ExecuteAsync(
        Guid tenantId,
        Func<CancellationToken, Task> operation,
        CancellationToken cancellationToken = default)
    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException(
                "TenantId is required.",
                nameof(tenantId));

        ArgumentNullException.ThrowIfNull(operation);

        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                cancellationToken);

        // Lock the tenant row for this transaction.
        var tenant = await _context.Tenants
            .FromSqlInterpolated(
                $"SELECT * FROM tenancy.tenant WHERE id = {tenantId} FOR UPDATE")
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken);

        if (tenant is null)
            throw new KeyNotFoundException(
                "Tenant was not found.");

        // Execute membership checks and changes
        // while the tenant row remains locked.
        await operation(cancellationToken);

        await _context.SaveChangesAsync(
            cancellationToken);

        await transaction.CommitAsync(
            cancellationToken);
    }
}
