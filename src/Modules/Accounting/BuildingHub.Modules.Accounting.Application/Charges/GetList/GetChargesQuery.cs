
using BuildingHub.Modules.Accounting.Domain.Entities;
using MediatR;

namespace BuildingHub.Modules.Accounting.Application.Charges.GetAll;

public sealed record GetChargesQuery(Guid BuildingId)
    : IRequest<IReadOnlyList<Charge>>;
