namespace BuildingHub.Modules.Community.Application.Abstractions;

public interface IRepository<T>
    where T : class
{
    Task AddAsync(
        T entity,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<T>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<T?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        T entity,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        T entity,
        CancellationToken cancellationToken = default);
}