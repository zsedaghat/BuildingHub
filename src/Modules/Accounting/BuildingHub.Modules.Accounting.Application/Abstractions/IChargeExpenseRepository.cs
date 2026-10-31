
using BuildingHub.Modules.Accounting.Domain.Entities;

namespace BuildingHub.Modules.Accounting.Application.Abstractions;

public interface IChargeExpenseRepository : IRepository<ChargeExpense>
{
    Task<decimal> GetTotalAllocatedAmountByExpenseIdAsync(
        Guid expenseId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        Guid expenseId,
        Guid chargeId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ChargeExpense>> GetByExpenseIdAsync(
        Guid expenseId,
        CancellationToken cancellationToken = default);

    Task AddAllocationSafelyAsync(
        ChargeExpense allocation,
        CancellationToken cancellationToken = default);


    Task UpdateAllocationSafelyAsync(
        Guid expenseId,
        Guid allocationId,
        decimal newAmount,
        CancellationToken cancellationToken = default);



}
