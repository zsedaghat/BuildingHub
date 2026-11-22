
using MediatR;

namespace BuildingHub.Modules.Tenancy.Application.Memberships.Create;

public sealed record CreateMembershipCommand(
    Guid TenantId,
    Guid UserId
) : IRequest<Guid>;
