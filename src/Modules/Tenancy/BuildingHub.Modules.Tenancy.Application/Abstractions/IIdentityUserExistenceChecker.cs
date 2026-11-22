
namespace BuildingHub.Modules.Tenancy.Application.Abstractions;

public interface IIdentityUserExistenceChecker
{
    Task<bool> ExistsAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
