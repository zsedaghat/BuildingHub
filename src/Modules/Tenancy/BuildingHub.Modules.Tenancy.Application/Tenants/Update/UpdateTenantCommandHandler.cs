
using BuildingHub.Modules.Tenancy.Application.Abstractions;
using MediatR;

namespace BuildingHub.Modules.Tenancy.Application.Tenants.Update;

public sealed class UpdateTenantCommandHandler
    : IRequestHandler<UpdateTenantCommand, UpdateTenantResponse?>
{
    private readonly ITenantRepository _tenantRepository;

    public UpdateTenantCommandHandler(
        ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<UpdateTenantResponse?> Handle(
        UpdateTenantCommand request,
        CancellationToken cancellationToken)
    {
        var tenant = await _tenantRepository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (tenant is null)
            return null;

        tenant.Rename(request.Name);

        await _tenantRepository.UpdateAsync(
            tenant,
            cancellationToken);

        return new UpdateTenantResponse(
            tenant.Id,
            tenant.Name,
            tenant.Status);
    }
}
