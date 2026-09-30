using BuildingHub.Modules.Community.Domain.Entities;

namespace BuildingHub.Modules.Community.Application.Abstractions;

public interface IPersonRepository : IRepository<Person>
{
    Task<Person?> GetByMobileAsync(
        string mobile,
        CancellationToken cancellationToken = default);
}