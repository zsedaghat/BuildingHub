
using BuildingHub.Modules.Identity.Infrastructure.Identity;
using BuildingHub.Modules.Tenancy.Application.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace BuildingHub.Api.Services;

public sealed class IdentityUserExistenceChecker
    : IIdentityUserExistenceChecker
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityUserExistenceChecker(
        UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<bool> ExistsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
            return false;

        cancellationToken.ThrowIfCancellationRequested();

        var user = await _userManager.FindByIdAsync(
            userId.ToString());

        return user is not null;
    }
}
