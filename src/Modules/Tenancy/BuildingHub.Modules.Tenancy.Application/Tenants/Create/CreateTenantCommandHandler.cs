
using BuildingHub.Modules.Tenancy.Application.Abstractions;
using BuildingHub.Modules.Tenancy.Domain.Entities;
using MediatR;

namespace BuildingHub.Modules.Tenancy.Application.Tenants.Create;

public sealed class CreateTenantCommandHandler
    : IRequestHandler<CreateTenantCommand, Guid>
{
    private readonly ITenantRepository _tenantRepository;

    public CreateTenantCommandHandler(
        ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<Guid> Handle(
        CreateTenantCommand request,
        CancellationToken cancellationToken)
    {
        var tenant = Tenant.Create(request.Name);

        await _tenantRepository.AddAsync(
            tenant,
            cancellationToken);

        return tenant.Id;
    }
}
