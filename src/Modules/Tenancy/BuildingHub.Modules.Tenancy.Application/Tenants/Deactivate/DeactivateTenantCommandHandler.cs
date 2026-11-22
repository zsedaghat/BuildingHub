
using BuildingHub.Modules.Tenancy.Application.Abstractions;
using BuildingHub.Modules.Tenancy.Domain.Enums;
using MediatR;

namespace BuildingHub.Modules.Tenancy.Application.Tenants.Deactivate;

public sealed class DeactivateTenantCommandHandler
    : IRequestHandler<DeactivateTenantCommand, bool>
{
    private readonly ITenantRepository _tenantRepository;

    public DeactivateTenantCommandHandler(
        ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<bool> Handle(
        DeactivateTenantCommand request,
        CancellationToken cancellationToken)
    {
        var tenant = await _tenantRepository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (tenant is null)
            return false;

        tenant.ChangeStatus(TenantStatus.Suspended);

        await _tenantRepository.UpdateAsync(
            tenant,
            cancellationToken);

        return true;
    }
}
