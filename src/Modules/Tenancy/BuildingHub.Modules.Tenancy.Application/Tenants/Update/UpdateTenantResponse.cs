
using BuildingHub.Modules.Tenancy.Domain.Enums;

namespace BuildingHub.Modules.Tenancy.Application.Tenants.Update;

public sealed record UpdateTenantResponse(
    Guid Id,
    string Name,
    TenantStatus Status);
