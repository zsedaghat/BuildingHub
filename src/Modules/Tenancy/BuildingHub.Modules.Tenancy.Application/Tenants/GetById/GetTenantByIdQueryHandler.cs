
using BuildingHub.Modules.Tenancy.Application.Abstractions;
using MediatR;

namespace BuildingHub.Modules.Tenancy.Application.Tenants.GetById;

public sealed class GetTenantByIdQueryHandler
    : IRequestHandler<GetTenantByIdQuery, GetTenantByIdResponse?>
{
    private readonly ITenantRepository _tenantRepository;

    public GetTenantByIdQueryHandler(
        ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<GetTenantByIdResponse?> Handle(
        GetTenantByIdQuery request,
        CancellationToken cancellationToken)
    {
        var tenant = await _tenantRepository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (tenant is null)
            return null;

        return new GetTenantByIdResponse(
            tenant.Id,
            tenant.Name,
            tenant.Status);
    }
}
