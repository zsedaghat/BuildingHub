
using BuildingHub.Modules.Tenancy.Application.Abstractions;
using BuildingHub.Modules.Tenancy.Domain.Entities;
using BuildingHub.Modules.Tenancy.Domain.Enums;
using MediatR;

namespace BuildingHub.Modules.Tenancy.Application.Memberships.Create;

public sealed class CreateMembershipCommandHandler
    : IRequestHandler<CreateMembershipCommand, Guid>
{
    private readonly ITenantMembershipRepository _repository;
    private readonly ITenantMembershipTransaction _transaction;
    private readonly IIdentityUserExistenceChecker _userChecker;

    public CreateMembershipCommandHandler(
        ITenantMembershipRepository repository,
        ITenantMembershipTransaction transaction,
        IIdentityUserExistenceChecker userChecker)
    {
        _repository = repository;
        _transaction = transaction;
        _userChecker = userChecker;
    }

    public async Task<Guid> Handle(
        CreateMembershipCommand request,
        CancellationToken cancellationToken)
    {
        if (request.TenantId == Guid.Empty)
            throw new ArgumentException(
                "TenantId is required.",
                nameof(request.TenantId));

        if (request.UserId == Guid.Empty)
            throw new ArgumentException(
                "UserId is required.",
                nameof(request.UserId));

        var membershipId = Guid.Empty;

        await _transaction.ExecuteAsync(
            request.TenantId,
            async ct =>
            {
                // Bootstrap is allowed only when
                // the tenant has no open memberships.
                var hasMembership =
                    await _repository.HasAnyOpenMembershipAsync(
                        request.TenantId,
                        ct);

                if (hasMembership)
                {
                    throw new InvalidOperationException(
                        "Tenant bootstrap is already completed.");
                }

                // Identity is checked through an abstraction.
                var userExists = await _userChecker.ExistsAsync(
                    request.UserId,
                    ct);

                if (!userExists)
                {
                    throw new KeyNotFoundException(
                        "User was not found in Identity.");
                }

                var membership = TenantUserMembership.Create(
                    request.TenantId,
                    request.UserId,
                    TenantMembershipRole.TenantAdmin,
                    TenantMembershipScopeMode.All,
                    DateTime.UtcNow);

                await _repository.AddAsync(membership, ct);

                membershipId = membership.Id;
            },
            cancellationToken);

        return membershipId;
    }
}
