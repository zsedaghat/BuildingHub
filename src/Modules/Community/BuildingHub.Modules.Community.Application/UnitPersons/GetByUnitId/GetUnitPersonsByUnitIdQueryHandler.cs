using BuildingHub.Modules.Community.Application.Abstractions;

namespace BuildingHub.Modules.Community.Application.UnitPersons.GetByUnitId;

public sealed class GetUnitPersonsByUnitIdQueryHandler
{
    private readonly IUnitPersonRepository _unitPersonRepository;
    private readonly IPersonRepository _personRepository;

    public GetUnitPersonsByUnitIdQueryHandler(
        IUnitPersonRepository unitPersonRepository,
        IPersonRepository personRepository)
    {
        _unitPersonRepository = unitPersonRepository;
        _personRepository = personRepository;
    }

    public async Task<IReadOnlyList<GetUnitPersonsByUnitIdResponse>> Handle(
        GetUnitPersonsByUnitIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var relations = await _unitPersonRepository.GetByUnitIdAsync(
            query.UnitId,
            cancellationToken);

        var result = new List<GetUnitPersonsByUnitIdResponse>();

        foreach (var relation in relations)
        {
            var person = await _personRepository.GetByIdAsync(
                relation.PersonId,
                cancellationToken);

            if (person is null)
                continue;

            result.Add(new GetUnitPersonsByUnitIdResponse(
                relation.Id,
                relation.UnitId,
                relation.PersonId,
                person.FirstName,
                person.LastName,
                person.Mobile,
                person.Email,
                relation.Role,
                relation.IsPrimary,
                relation.StartDate,
                relation.EndDate));
        }

        return result;
    }
}