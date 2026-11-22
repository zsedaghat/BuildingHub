using MediatR;

namespace BuildingHub.Modules.Tenancy.Application.Tenants.Activate;

public sealed record ActivateTenantCommand(Guid Id) : IRequest<bool>;
