using BuildingHub.Modules.Community.Domain.Entities;

namespace BuildingHub.Modules.Community.Application.Abstractions;

public interface IUnitPersonRepository : IRepository<UnitPerson>
{
    Task<IReadOnlyList<UnitPerson>> GetByUnitIdAsync(
        Guid unitId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<UnitPerson>> GetByPersonIdAsync(
        Guid personId,
        CancellationToken cancellationToken = default);
}