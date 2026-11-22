
using BuildingHub.Modules.Tenancy.Application.Abstractions;
using BuildingHub.Modules.Tenancy.Domain.Enums;
using BuildingHub.SharedKernel.Exceptions;
using MediatR;

namespace BuildingHub.Modules.Tenancy.Application.Memberships.ChangeRole;

public sealed class ChangeMembershipRoleCommandHandler
    : IRequestHandler<ChangeMembershipRoleCommand, bool>
{
    private readonly ITenantMembershipRepository _repository;
    private readonly ITenantMembershipTransaction _transaction;

    public ChangeMembershipRoleCommandHandler(
        ITenantMembershipRepository repository,
        ITenantMembershipTransaction transaction)
    {
        _repository = repository;
        _transaction = transaction;
    }

    public async Task<bool> Handle(
        ChangeMembershipRoleCommand request,
        CancellationToken cancellationToken)
    {
        if (request.TenantId == Guid.Empty)
            throw new ArgumentException(
                "TenantId is required.",
                nameof(request.TenantId));

        if (request.MembershipId == Guid.Empty)
            throw new ArgumentException(
                "MembershipId is required.",
                nameof(request.MembershipId));

        if (!Enum.IsDefined(request.NewRole))
            throw new ArgumentOutOfRangeException(
                nameof(request.NewRole));

        if (request.ExpectedVersion < 1)
            throw new ArgumentOutOfRangeException(
                nameof(request.ExpectedVersion),
                "ExpectedVersion must be greater than zero.");

        var changed = false;

        await _transaction.ExecuteAsync(
            request.TenantId,
            async ct =>
            {
                var membership = await _repository.GetByIdAsync(
                    request.MembershipId,
                    ct);

                if (membership is null ||
                    membership.TenantId != request.TenantId)
                {
                    return;
                }

                // Reject requests based on stale data.
                if (membership.Version != request.ExpectedVersion)
                {
                    throw new ConcurrencyConflictException(
                        "Membership has been modified. " +
                        "Please reload and try again.");
                }

                if (membership.EndAtUtc.HasValue)
                {
                    throw new InvalidOperationException(
                        "Cannot change the role of an ended membership.");
                }

                // No change is required.
                if (membership.Role == request.NewRole)
                {
                    changed = true;
                    return;
                }

                var nowUtc = DateTime.UtcNow;

                // Protect the last active TenantAdmin.
                var isActiveAdmin =
                    membership.Role == TenantMembershipRole.TenantAdmin &&
                    membership.Status == TenantMembershipStatus.Active &&
                    membership.StartAtUtc <= nowUtc &&
                    (membership.EndAtUtc is null ||
                     membership.EndAtUtc > nowUtc);

                if (isActiveAdmin &&
                    request.NewRole != TenantMembershipRole.TenantAdmin)
                {
                    var hasAnotherAdmin =
                        await _repository.HasAnotherActiveAdminAsync(
                            request.TenantId,
                            membership.Id,
                            nowUtc,
                            ct);

                    if (!hasAnotherAdmin)
                    {
                        throw new InvalidOperationException(
                            "Cannot change the role of the last active TenantAdmin.");
                    }
                }

                membership.ChangeRole(request.NewRole);

                changed = true;
            },
            cancellationToken);

        return changed;
    }
}
