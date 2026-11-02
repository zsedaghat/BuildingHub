namespace BuildingHub.Web.Models.Pagination;

public sealed class PagedListViewModel<T>
{
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public int TotalCount { get; init; }
    public int TotalPages => TotalCount == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
    public int FirstItemNumber => TotalCount == 0 ? 0 : ((PageNumber - 1) * PageSize) + 1;
    public int LastItemNumber => Math.Min(PageNumber * PageSize, TotalCount);
}
