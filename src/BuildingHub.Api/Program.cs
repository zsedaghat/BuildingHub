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
using BuildingHub.Modules.Community.Application.Units.GetByBuildingId;
using BuildingHub.Modules.Community.Application.Units.GetByFloorId;
using BuildingHub.Modules.Community.Application.Units.GetById;
using BuildingHub.Modules.Community.Application.Units.GetList;
using BuildingHub.Modules.Community.Application.Units.Update;
using BuildingHub.Modules.Community.Infrastructure.Persistence;
using BuildingHub.Modules.Community.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<CommunityDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ICommunityRepository, CommunityRepository>();
// Community CRUD Handlers
builder.Services.AddScoped<CreateCommunityCommandHandler>();
builder.Services.AddScoped<GetCommunitiesQueryHandler>();
builder.Services.AddScoped<GetCommunityByIdQueryHandler>();
builder.Services.AddScoped<UpdateCommunityCommandHandler>();
builder.Services.AddScoped<DeleteCommunityCommandHandler>();


builder.Services.AddScoped<IBuildingRepository, BuildingRepository>();
// Buildings CRUD Handlers
builder.Services.AddScoped<CreateBuildingCommandHandler>();
builder.Services.AddScoped<GetBuildingsQueryHandler>();
builder.Services.AddScoped<GetBuildingsByCommunityIdQueryHandler>();
builder.Services.AddScoped<GetBuildingByIdQueryHandler>();
builder.Services.AddScoped<UpdateBuildingCommandHandler>();
builder.Services.AddScoped<DeleteBuildingCommandHandler>();

builder.Services.AddScoped<IFloorRepository, FloorRepository>();
// Floors CRUD Handlers
builder.Services.AddScoped<CreateFloorCommandHandler>();
builder.Services.AddScoped<GetFloorsQueryHandler>();
builder.Services.AddScoped<GetFloorsByBuildingIdQueryHandler>();
builder.Services.AddScoped<GetFloorByIdQueryHandler>();
builder.Services.AddScoped<UpdateFloorCommandHandler>();
builder.Services.AddScoped<DeleteFloorCommandHandler>();

builder.Services.AddScoped<IUnitRepository, UnitRepository>();
// Units CRUD Handlers
builder.Services.AddScoped<CreateUnitCommandHandler>();
builder.Services.AddScoped<GetUnitsQueryHandler>();
builder.Services.AddScoped<GetUnitsByBuildingIdQueryHandler>();
builder.Services.AddScoped<GetUnitsByFloorIdQueryHandler>();
builder.Services.AddScoped<GetUnitByIdQueryHandler>();
builder.Services.AddScoped<UpdateUnitCommandHandler>();
builder.Services.AddScoped<DeleteUnitCommandHandler>();

builder.Services.AddScoped<IPersonRepository, PersonRepository>();
// Persons CRUD Handlers
builder.Services.AddScoped<CreatePersonCommandHandler>();
builder.Services.AddScoped<GetPersonsQueryHandler>();
builder.Services.AddScoped<GetPersonByIdQueryHandler>();
builder.Services.AddScoped<UpdatePersonCommandHandler>();
builder.Services.AddScoped<DeletePersonCommandHandler>();
builder.Services.AddScoped<GetPersonByMobileQueryHandler>();

builder.Services.AddScoped<IUnitPersonRepository, UnitPersonRepository>();
// UnitPersons Handlers
builder.Services.AddScoped<CreateUnitPersonCommandHandler>();
builder.Services.AddScoped<GetUnitPersonsQueryHandler>();
builder.Services.AddScoped<GetUnitPersonByIdQueryHandler>();
builder.Services.AddScoped<GetUnitPersonsByUnitIdQueryHandler>();
builder.Services.AddScoped<GetUnitPersonsByPersonIdQueryHandler>();
builder.Services.AddScoped<UpdateUnitPersonCommandHandler>();
builder.Services.AddScoped<DeleteUnitPersonCommandHandler>();

var app = builder.Build();

app.UseHttpsRedirection();

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.MapGet("/", () => "BuildingHub API is running!");

app.Run();