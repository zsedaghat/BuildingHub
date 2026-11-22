
using BuildingHub.Api.Services;
using BuildingHub.Modules.Accounting.Application.Abstractions;
using BuildingHub.Modules.Accounting.Infrastructure.Persistence;
using BuildingHub.Modules.Accounting.Infrastructure.Repositories;

using BuildingHub.Modules.Community.Application.Abstractions;
using BuildingHub.Modules.Community.Application.Buildings.Create;
using BuildingHub.Modules.Community.Application.Buildings.Delete;
using BuildingHub.Modules.Community.Application.Buildings.GetById;
using BuildingHub.Modules.Community.Application.Buildings.GetList;
using BuildingHub.Modules.Community.Application.Buildings.Update;
using BuildingHub.Modules.Community.Application.Communities.Create;
using BuildingHub.Modules.Community.Application.Communities.Delete;
using BuildingHub.Modules.Community.Application.Communities.GetById;
using BuildingHub.Modules.Community.Application.Communities.GetList;
using BuildingHub.Modules.Community.Application.Communities.Update;
using BuildingHub.Modules.Community.Application.Floors.Create;
using BuildingHub.Modules.Community.Application.Floors.Delete;
using BuildingHub.Modules.Community.Application.Floors.GetByBuildingId;
using BuildingHub.Modules.Community.Application.Floors.GetById;
using BuildingHub.Modules.Community.Application.Floors.GetList;
using BuildingHub.Modules.Community.Application.Floors.Update;
using BuildingHub.Modules.Community.Application.Persons.Create;
using BuildingHub.Modules.Community.Application.Persons.Delete;
using BuildingHub.Modules.Community.Application.Persons.GetById;
using BuildingHub.Modules.Community.Application.Persons.GetByMobile;
using BuildingHub.Modules.Community.Application.Persons.GetList;
using BuildingHub.Modules.Community.Application.Persons.Update;
using BuildingHub.Modules.Community.Application.UnitPersons.Create;
using BuildingHub.Modules.Community.Application.UnitPersons.Delete;
using BuildingHub.Modules.Community.Application.UnitPersons.GetById;
using BuildingHub.Modules.Community.Application.UnitPersons.GetByPersonId;
using BuildingHub.Modules.Community.Application.UnitPersons.GetByUnitId;
using BuildingHub.Modules.Community.Application.UnitPersons.GetList;
using BuildingHub.Modules.Community.Application.UnitPersons.Update;
using BuildingHub.Modules.Community.Application.Units.Create;
using BuildingHub.Modules.Community.Application.Units.Delete;
using BuildingHub.Modules.Community.Application.Units.GetActiveByBuildingId;
using BuildingHub.Modules.Community.Application.Units.GetByBuildingId;
using BuildingHub.Modules.Community.Application.Units.GetByFloorId;
using BuildingHub.Modules.Community.Application.Units.GetById;
using BuildingHub.Modules.Community.Application.Units.GetList;
using BuildingHub.Modules.Community.Application.Units.Update;
using BuildingHub.Modules.Community.Infrastructure.Persistence;
using BuildingHub.Modules.Community.Infrastructure.Repositories;

using BuildingHub.Modules.Tenancy.Application.Abstractions;
using BuildingHub.Modules.Tenancy.Application.Tenants.Create;
using BuildingHub.Modules.Tenancy.Infrastructure.Persistence;
using BuildingHub.Modules.Tenancy.Infrastructure.Repositories;

using BuildingHub.Modules.Identity.Infrastructure.DependencyInjection;
using BuildingHub.Modules.Reference.Infrastructure.DependencyInjection;

using MediatR;
using Microsoft.EntityFrameworkCore;
using BuildingHub.Modules.Identity.Application.Authentication.RequestLoginOtp;

namespace BuildingHub.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBuildingHubServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("DefaultConnection");

        // DbContexts
        services.AddDbContext<CommunityDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddDbContext<AccountingDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddDbContext<TenancyDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddIdentityInfrastructure(connectionString);
        services.AddReferenceInfrastructure(connectionString);

        // MediatR

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(IChargeRepository).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(CreateCommunityCommand).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(CreateTenantCommand).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(RequestLoginOtpCommand).Assembly);
        });
        // API Services
        services.AddScoped<IBuildingUnitProvider, BuildingUnitProvider>();
        services.AddScoped<ITenantExistenceChecker, TenantExistenceChecker>();
        services.AddScoped<ICommunityExistenceChecker, CommunityExistenceChecker>();

        // Community
        services.AddScoped<ICommunityRepository, CommunityRepository>();

        services.AddScoped<CreateCommunityCommandHandler>();
        services.AddScoped<GetCommunitiesQueryHandler>();
        services.AddScoped<GetCommunityByIdQueryHandler>();
        services.AddScoped<UpdateCommunityCommandHandler>();
        services.AddScoped<DeleteCommunityCommandHandler>();

        services.AddScoped<IBuildingRepository, BuildingRepository>();

        services.AddScoped<CreateBuildingCommandHandler>();
        services.AddScoped<GetBuildingsQueryHandler>();
        services.AddScoped<GetBuildingsByCommunityIdQueryHandler>();
        services.AddScoped<GetBuildingByIdQueryHandler>();
        services.AddScoped<UpdateBuildingCommandHandler>();
        services.AddScoped<DeleteBuildingCommandHandler>();

        services.AddScoped<IFloorRepository, FloorRepository>();

        services.AddScoped<CreateFloorCommandHandler>();
        services.AddScoped<GetFloorsQueryHandler>();
        services.AddScoped<GetFloorsByBuildingIdQueryHandler>();
        services.AddScoped<GetFloorByIdQueryHandler>();
        services.AddScoped<UpdateFloorCommandHandler>();
        services.AddScoped<DeleteFloorCommandHandler>();

        services.AddScoped<IUnitRepository, UnitRepository>();

        services.AddScoped<CreateUnitCommandHandler>();
        services.AddScoped<GetUnitsQueryHandler>();
        services.AddScoped<GetUnitsByBuildingIdQueryHandler>();
        services.AddScoped<GetUnitsByFloorIdQueryHandler>();
        services.AddScoped<GetUnitByIdQueryHandler>();
        services.AddScoped<UpdateUnitCommandHandler>();
        services.AddScoped<DeleteUnitCommandHandler>();

        services.AddScoped<IPersonRepository, PersonRepository>();

        services.AddScoped<CreatePersonCommandHandler>();
        services.AddScoped<GetPersonsQueryHandler>();
        services.AddScoped<GetPersonByIdQueryHandler>();
        services.AddScoped<UpdatePersonCommandHandler>();
        services.AddScoped<DeletePersonCommandHandler>();
        services.AddScoped<GetPersonByMobileQueryHandler>();

        services.AddScoped<IUnitPersonRepository, UnitPersonRepository>();

        services.AddScoped<CreateUnitPersonCommandHandler>();
        services.AddScoped<GetUnitPersonsQueryHandler>();
        services.AddScoped<GetUnitPersonByIdQueryHandler>();
        services.AddScoped<GetUnitPersonsByUnitIdQueryHandler>();
        services.AddScoped<GetUnitPersonsByPersonIdQueryHandler>();
        services.AddScoped<UpdateUnitPersonCommandHandler>();
        services.AddScoped<DeleteUnitPersonCommandHandler>();

        // Accounting
        services.AddScoped<IChargeRepository, ChargeRepository>();
        services.AddScoped<IUnitChargeRepository, UnitChargeRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IExpenseRepository, ExpenseRepository>();
        services.AddScoped<IChargeExpenseRepository, ChargeExpenseRepository>();

        // Tenancy
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<
    ITenantMembershipRepository,
    TenantMembershipRepository>();
        services.AddScoped<
    ITenantMembershipTransaction,
    TenantMembershipTransaction>();
        services.AddScoped<
    IIdentityUserExistenceChecker,
    IdentityUserExistenceChecker>();
        services.AddScoped<
    ITenantCommunityAssignmentRepository,
    TenantCommunityAssignmentRepository>();
        return services;
    }
}
