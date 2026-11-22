
using MediatR;

namespace BuildingHub.Modules.Tenancy.Application.Memberships.End;

public sealed record EndMembershipCommand(
    Guid TenantId,
    Guid MembershipId,
    int ExpectedVersion
) : IRequest<bool>;
