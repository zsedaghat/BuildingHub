using MediatR;

namespace BuildingHub.Modules.Tenancy.Application.Tenants.GetById;

public sealed record GetTenantByIdQuery(Guid Id)
    : IRequest<GetTenantByIdResponse?>;
