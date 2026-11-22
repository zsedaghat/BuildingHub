
using BuildingHub.Modules.Tenancy.Domain.Enums;

namespace BuildingHub.Modules.Tenancy.Application.Tenants.GetList;

public sealed record GetTenantResponse(
    Guid Id,
    string Name,
    TenantStatus Status);
