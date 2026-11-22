using MediatR;

namespace BuildingHub.Modules.Tenancy.Application.Tenants.Delete;

public sealed record DeleteTenantCommand(Guid Id) : IRequest<bool>;
