
using BuildingHub.Modules.Tenancy.Application.Abstractions;
using MediatR;

namespace BuildingHub.Modules.Tenancy.Application.Tenants.GetList;

public sealed class GetTenantsQueryHandler
    : IRequestHandler<GetTenantsQuery, IReadOnlyList<GetTenantResponse>>
{
    private readonly ITenantRepository _tenantRepository;

    public GetTenantsQueryHandler(
        ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<IReadOnlyList<GetTenantResponse>> Handle(
        GetTenantsQuery request,
        CancellationToken cancellationToken)
    {
        var tenants = await _tenantRepository.GetAllAsync(
            cancellationToken);

        return tenants
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Id)
            .Select(x => new GetTenantResponse(
                x.Id,
                x.Name,
                x.Status))
            .ToList();
    }
}
