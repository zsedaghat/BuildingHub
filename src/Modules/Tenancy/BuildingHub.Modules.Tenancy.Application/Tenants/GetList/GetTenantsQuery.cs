using MediatR;

namespace BuildingHub.Modules.Tenancy.Application.Tenants.GetList;

public sealed record GetTenantsQuery
    : IRequest<IReadOnlyList<GetTenantResponse>>;
