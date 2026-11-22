
using BuildingHub.Modules.Identity.Infrastructure.Identity;
using BuildingHub.Modules.Identity.Infrastructure.Persistence.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BuildingHub.Modules.Identity.Infrastructure.Persistence;

public sealed class IdentityDbContext
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public IdentityDbContext(
        DbContextOptions<IdentityDbContext> options)
        : base(options)
    {
    }

    public DbSet<PhoneOtpChallenge> PhoneOtpChallenges =>
        Set<PhoneOtpChallenge>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("identity");

        builder.ApplyConfiguration(
            new PhoneOtpChallengeConfiguration());
    }
}
