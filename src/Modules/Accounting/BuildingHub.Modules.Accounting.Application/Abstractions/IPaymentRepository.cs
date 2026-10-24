
using BuildingHub.Modules.Accounting.Domain.Entities;

namespace BuildingHub.Modules.Accounting.Application.Abstractions;

public interface IPaymentRepository : IRepository<Payment>
{
    Task<IReadOnlyList<Payment>> GetByUnitChargeIdAsync(
        Guid unitChargeId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Payment>> GetByUnitChargeIdsAsync(
        IReadOnlyCollection<Guid> unitChargeIds,
        CancellationToken cancellationToken = default);

    Task<Guid> CreatePaymentWithBalanceCheckAsync(
        Guid unitChargeId,
        decimal amount,
        DateTime paidAt,
        string? trackingNumber,
        CancellationToken cancellationToken = default);
}
