
using BuildingHub.Modules.Accounting.Application.Abstractions;
using MediatR;

namespace BuildingHub.Modules.Accounting.Application.Expenses.UpdateAllocation;

public sealed class UpdateExpenseAllocationCommandHandler
    : IRequestHandler<UpdateExpenseAllocationCommand>
{
    private readonly IChargeExpenseRepository _chargeExpenseRepository;

    public UpdateExpenseAllocationCommandHandler(
        IChargeExpenseRepository chargeExpenseRepository)
    {
        _chargeExpenseRepository = chargeExpenseRepository;
    }

    public async Task Handle(
        UpdateExpenseAllocationCommand request,
        CancellationToken cancellationToken)
    {
        await _chargeExpenseRepository.UpdateAllocationSafelyAsync(
            request.ExpenseId,
            request.AllocationId,
            request.AllocatedAmount,
            cancellationToken);
    }
}
