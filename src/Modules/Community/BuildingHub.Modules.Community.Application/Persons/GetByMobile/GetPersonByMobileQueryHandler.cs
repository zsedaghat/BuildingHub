using BuildingHub.Modules.Community.Application.Abstractions;

namespace BuildingHub.Modules.Community.Application.Persons.GetByMobile;

public sealed class GetPersonByMobileQueryHandler
{
    private readonly IPersonRepository _repository;

    public GetPersonByMobileQueryHandler(
        IPersonRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetPersonByMobileResponse?> Handle(
        GetPersonByMobileQuery query,
        CancellationToken cancellationToken = default)
    {
        var person = await _repository.GetByMobileAsync(
            query.Mobile,
            cancellationToken);

        if (person is null)
            return null;

        return new GetPersonByMobileResponse(
            person.Id,
            person.FirstName,
            person.LastName,
            person.Mobile,
            person.Email,
            person.NationalId);
    }
}