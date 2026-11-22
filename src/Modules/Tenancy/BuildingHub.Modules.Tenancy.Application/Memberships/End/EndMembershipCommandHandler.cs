
using BuildingHub.Modules.Tenancy.Application.Abstractions;
using BuildingHub.Modules.Tenancy.Domain.Enums;
using BuildingHub.SharedKernel.Exceptions;
using MediatR;

namespace BuildingHub.Modules.Tenancy.Application.Memberships.End;

public sealed class EndMembershipCommandHandler
    : IRequestHandler<EndMembershipCommand, bool>
{
    private readonly ITenantMembershipRepository _repository;
    private readonly ITenantMembershipTransaction _transaction;

    public EndMembershipCommandHandler(
        ITenantMembershipRepository repository,
        ITenantMembershipTransaction transaction)
    {
        _repository = repository;
        _transaction = transaction;
    }

    public async Task<bool> Handle(
        EndMembershipCommand request,
        CancellationToken cancellationToken)
    {
        if (request.TenantId == Guid.Empty ||
            request.MembershipId == Guid.Empty)
            throw new ArgumentException(
                "TenantId and MembershipId are required.");

        if (request.ExpectedVersion < 1)
            throw new ArgumentOutOfRangeException(
                nameof(request.ExpectedVersion),
                "ExpectedVersion must be greater than zero.");

        var ended = false;

        await _transaction.ExecuteAsync(
            request.TenantId,
            async ct =>
            {
                var membership = await _repository.GetByIdAsync(
                    request.MembershipId, ct);

                if (membership is null ||
                    membership.TenantId != request.TenantId)
                    return;

                if (membership.Version != request.ExpectedVersion)
                    throw new ConcurrencyConflictException(
                        "Membership has been modified. " +
                        "Please reload and try again.");

                // Idempotent: already ended.
                if (membership.EndAtUtc.HasValue)
                {
                    ended = true;
                    return;
                }

                var nowUtc = DateTime.UtcNow;

                var isActiveAdmin =
                    membership.Role == TenantMembershipRole.TenantAdmin &&
                    membership.Status == TenantMembershipStatus.Active &&
                    membership.StartAtUtc <= nowUtc &&
                    (membership.EndAtUtc is null ||
                     membership.EndAtUtc > nowUtc);

                if (isActiveAdmin)
                {
                    var hasAnotherAdmin =
                        await _repository.HasAnotherActiveAdminAsync(
                            request.TenantId,
                            membership.Id,
                            nowUtc,
                            ct);

                    if (!hasAnotherAdmin)
                        throw new InvalidOperationException(
                            "Cannot end the last active TenantAdmin.");
                }

                membership.End(nowUtc);
                ended = true;
            },
            cancellationToken);

        return ended;
    }
}
