
using MediatR;

namespace BuildingHub.Modules.Tenancy.Application.Tenants.Create;

public sealed record CreateTenantCommand(
    string Name) : IRequest<Guid>;
