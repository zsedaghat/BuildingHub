
namespace BuildingHub.Modules.Tenancy.Application.Abstractions;

public interface ITenantMembershipTransaction
{
    Task ExecuteAsync(
        Guid tenantId,
        Func<CancellationToken, Task> operation,
        CancellationToken cancellationToken = default);
}
