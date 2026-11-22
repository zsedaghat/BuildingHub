
using BuildingHub.Modules.Tenancy.Domain.Enums;
using MediatR;

namespace BuildingHub.Modules.Tenancy.Application.Memberships.ChangeRole;

public sealed record ChangeMembershipRoleCommand(
    Guid TenantId,
    Guid MembershipId,
    TenantMembershipRole NewRole,
    int ExpectedVersion
) : IRequest<bool>;
