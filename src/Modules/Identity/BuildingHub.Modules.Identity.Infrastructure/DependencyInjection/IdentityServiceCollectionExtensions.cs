
using BuildingHub.Modules.Identity.Application.Abstractions;
using BuildingHub.Modules.Identity.Infrastructure.Identity;
using BuildingHub.Modules.Identity.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingHub.Modules.Identity.Infrastructure.DependencyInjection;

public static class IdentityServiceCollectionExtensions
{
    public static IServiceCollection AddIdentityInfrastructure(
        this IServiceCollection services,
        string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "DefaultConnection connection string is not configured.");
        }

        services.AddDbContext<IdentityDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                npgsqlOptions => npgsqlOptions.MigrationsHistoryTable(
                    "__EFMigrationsHistory",
                    "identity")));
        // Data Protection
        services.AddDataProtection();
        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                // User settings
                options.User.RequireUniqueEmail = false;

                // Password settings
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;

                // Lockout settings
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan =
                    TimeSpan.FromMinutes(15);
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<IdentityDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        services.AddSingleton<IOtpCodeService, OtpCodeService>();
        services.AddScoped<IOtpAttemptRepository, OtpAttemptRepository>();
        services.AddScoped<
    IOtpChallengeRepository,
    OtpChallengeRepository>();
        return services;
    }
}
