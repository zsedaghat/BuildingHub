namespace BuildingHub.Modules.Community.Application.Abstractions;

public static class PaginationDefaults
{
    public const int DefaultPageNumber = 1;
    public const int DefaultPageSize = 10;
    public const int MaximumPageSize = 100;

    public static (int PageNumber, int PageSize) Normalize(int pageNumber, int pageSize)
    {
        var normalizedPageNumber = Math.Max(1, pageNumber);
        var normalizedPageSize = Math.Clamp(pageSize, 1, MaximumPageSize);
        return (normalizedPageNumber, normalizedPageSize);
    }
}
