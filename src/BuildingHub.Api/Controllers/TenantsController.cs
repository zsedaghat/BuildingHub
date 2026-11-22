
using BuildingHub.Modules.Tenancy.Application.Tenants.Create;
using BuildingHub.Modules.Tenancy.Application.Tenants.GetById;
using BuildingHub.Modules.Tenancy.Application.Tenants.GetList;
using BuildingHub.Modules.Tenancy.Application.Tenants.Update;
using BuildingHub.Modules.Tenancy.Application.Tenants.Delete;
using BuildingHub.Modules.Tenancy.Application.Tenants.Activate;
using BuildingHub.Modules.Tenancy.Application.Tenants.Deactivate;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BuildingHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class TenantsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TenantsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // POST: api/tenants
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateTenantCommand command,
        CancellationToken cancellationToken)
    {
        var tenantId = await _mediator.Send(
            command,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = tenantId },
            new { id = tenantId });
    }

    // GET: api/tenants
    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var tenants = await _mediator.Send(
            new GetTenantsQuery(),
            cancellationToken);

        return Ok(tenants);
    }

    // GET: api/tenants/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var tenant = await _mediator.Send(
            new GetTenantByIdQuery(id),
            cancellationToken);

        if (tenant is null)
            return NotFound();

        return Ok(tenant);
    }


    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateTenantRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateTenantCommand(
            id,
            request.Name);

        var result = await _mediator.Send(
            command,
            cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }


    // DELETE: api/tenants/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new DeleteTenantCommand(id),
            cancellationToken);

        if (!result)
            return NotFound();

        return NoContent();
    }

    // PATCH: api/tenants/{id}/activate
    [HttpPatch("{id:guid}/activate")]
    public async Task<IActionResult> Activate(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new ActivateTenantCommand(id),
            cancellationToken);

        if (!result)
            return NotFound();

        return NoContent();
    }

    // PATCH: api/tenants/{id}/deactivate
    [HttpPatch("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new DeactivateTenantCommand(id),
            cancellationToken);

        if (!result)
            return NotFound();

        return NoContent();
    }
}


public sealed record UpdateTenantRequest(string Name);
