using MediatR;

namespace BuildingHub.Modules.Tenancy.Application.Tenants.Deactivate;

public sealed record DeactivateTenantCommand(Guid Id) : IRequest<bool>;
