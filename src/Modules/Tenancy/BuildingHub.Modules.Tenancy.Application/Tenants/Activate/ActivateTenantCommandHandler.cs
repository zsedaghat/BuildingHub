
using BuildingHub.Modules.Tenancy.Application.Abstractions;
using BuildingHub.Modules.Tenancy.Domain.Enums;
using MediatR;

namespace BuildingHub.Modules.Tenancy.Application.Tenants.Activate;

public sealed class ActivateTenantCommandHandler
    : IRequestHandler<ActivateTenantCommand, bool>
{
    private readonly ITenantRepository _tenantRepository;

    public ActivateTenantCommandHandler(
        ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<bool> Handle(
        ActivateTenantCommand request,
        CancellationToken cancellationToken)
    {
        var tenant = await _tenantRepository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (tenant is null)
            return false;

        tenant.ChangeStatus(TenantStatus.Active);

        await _tenantRepository.UpdateAsync(
            tenant,
            cancellationToken);

        return true;
    }
}
