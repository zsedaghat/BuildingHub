
using BuildingHub.Api.Extensions;
using BuildingHub.Api.Middlewares;
using BuildingHub.Modules.Identity.Application.Abstractions;
using BuildingHub.Modules.Identity.Infrastructure.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register all BuildingHub modules and services
builder.Services.AddBuildingHubServices(builder.Configuration);
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<
        ISmsSender,
        DevelopmentSmsSender>();
}

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.MapGet("/", () => "BuildingHub API is running!");

app.Run();
