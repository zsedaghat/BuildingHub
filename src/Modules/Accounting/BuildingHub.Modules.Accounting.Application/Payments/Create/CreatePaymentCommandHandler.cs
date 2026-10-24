
using BuildingHub.Modules.Accounting.Application.Abstractions;
using MediatR;

namespace BuildingHub.Modules.Accounting.Application.Payments.Create;

public sealed class CreatePaymentCommandHandler
    : IRequestHandler<CreatePaymentCommand, Guid>
{
    private readonly IPaymentRepository _paymentRepository;

    public CreatePaymentCommandHandler(
        IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<Guid> Handle(
        CreatePaymentCommand request,
        CancellationToken cancellationToken)
    {
        if (request.UnitChargeId == Guid.Empty)
        {
            throw new ArgumentException(
                "UnitChargeId is required.");
        }

        if (request.Amount <= 0)
        {
            throw new ArgumentException(
                "Payment amount must be greater than zero.");
        }

        return await _paymentRepository.CreatePaymentWithBalanceCheckAsync(
            request.UnitChargeId,
            request.Amount,
            request.PaidAt,
            request.TrackingNumber,
            cancellationToken);
    }
}
