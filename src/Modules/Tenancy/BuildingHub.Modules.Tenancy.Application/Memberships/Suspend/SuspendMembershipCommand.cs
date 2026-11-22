
using MediatR;

namespace BuildingHub.Modules.Tenancy.Application.Memberships.Suspend;

public sealed record SuspendMembershipCommand(
    Guid TenantId,
    Guid MembershipId,
    int ExpectedVersion
) : IRequest<bool>;
