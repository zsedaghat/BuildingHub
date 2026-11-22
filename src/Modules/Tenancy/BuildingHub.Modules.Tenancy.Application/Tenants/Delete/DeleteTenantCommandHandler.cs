
using BuildingHub.Modules.Tenancy.Application.Abstractions;
using BuildingHub.Modules.Tenancy.Application.Exceptions;
using MediatR;

namespace BuildingHub.Modules.Tenancy.Application.Tenants.Delete;

public sealed class DeleteTenantCommandHandler
    : IRequestHandler<DeleteTenantCommand, bool>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly ICommunityExistenceChecker _communityExistenceChecker;

    public DeleteTenantCommandHandler(
        ITenantRepository tenantRepository,
        ICommunityExistenceChecker communityExistenceChecker)
    {
        _tenantRepository = tenantRepository;
        _communityExistenceChecker = communityExistenceChecker;
    }

    public async Task<bool> Handle(
        DeleteTenantCommand request,
        CancellationToken cancellationToken)
    {
        var tenant = await _tenantRepository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (tenant is null)
            return false;

        var hasCommunities = await _communityExistenceChecker
            .HasCommunitiesAsync(request.Id, cancellationToken);

        if (hasCommunities)
            throw new TenantHasCommunitiesException();

        await _tenantRepository.DeleteAsync(
            tenant,
            cancellationToken);

        return true;
    }
}
