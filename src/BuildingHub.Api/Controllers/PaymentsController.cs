
using BuildingHub.Modules.Accounting.Application.Payments.Create;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BuildingHub.Api.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly ISender _sender;

    public PaymentsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreatePaymentCommand command,
        CancellationToken cancellationToken)
    {
        var paymentId = await _sender.Send(command, cancellationToken);

        return Created(
            $"/api/payments/{paymentId}",
            new { id = paymentId });
    }
}
