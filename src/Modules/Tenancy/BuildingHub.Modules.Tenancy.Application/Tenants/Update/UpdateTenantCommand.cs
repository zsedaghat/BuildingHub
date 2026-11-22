
using MediatR;

namespace BuildingHub.Modules.Tenancy.Application.Tenants.Update;

public sealed record UpdateTenantCommand(
    Guid Id,
    string Name) : IRequest<UpdateTenantResponse?>;
