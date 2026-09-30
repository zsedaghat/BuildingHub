using BuildingHub.Modules.Community.Application.Abstractions;

namespace BuildingHub.Modules.Community.Application.Persons.GetList;

public sealed class GetPersonsQueryHandler
{
    private readonly IPersonRepository _repository;

    public GetPersonsQueryHandler(
        IPersonRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<GetPersonsResponse>> Handle(
        GetPersonsQuery query,
        CancellationToken cancellationToken = default)
    {
        var persons = await _repository.GetAllAsync(
            cancellationToken);

        return persons
            .Select(person => new GetPersonsResponse(
                person.Id,
                person.FirstName,
                person.LastName,
                person.Mobile,
                person.Email,
                person.NationalId))
            .ToList();
    }
}