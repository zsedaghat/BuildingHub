
using BuildingHub.Modules.Accounting.Domain.Entities;

namespace BuildingHub.Modules.Accounting.Application.Abstractions;

public interface IExpenseRepository : IRepository<Expense>
{
    Task<IReadOnlyList<Expense>> GetByBuildingIdAsync(
        Guid buildingId,
        CancellationToken cancellationToken = default);
}
