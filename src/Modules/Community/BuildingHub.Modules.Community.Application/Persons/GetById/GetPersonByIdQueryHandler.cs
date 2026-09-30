using BuildingHub.Modules.Community.Application.Abstractions;

namespace BuildingHub.Modules.Community.Application.Persons.GetById;

public sealed class GetPersonByIdQueryHandler
{
    private readonly IPersonRepository _repository;

    public GetPersonByIdQueryHandler(
        IPersonRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetPersonByIdResponse?> Handle(
        GetPersonByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var person = await _repository.GetByIdAsync(
            query.Id,
            cancellationToken);

        if (person is null)
            return null;

        return new GetPersonByIdResponse(
            person.Id,
            person.FirstName,
            person.LastName,
            person.Mobile,
            person.Email,
            person.NationalId);
    }
}