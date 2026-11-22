
using BuildingHub.Modules.Tenancy.Domain.Enums;

namespace BuildingHub.Modules.Tenancy.Application.Tenants.GetById;

public sealed record GetTenantByIdResponse(
    Guid Id,
    string Name,
    TenantStatus Status);
