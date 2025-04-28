namespace JobTrackingAPI.Contracts.Results;

public class PaginatedResult<T>
{
    public List<T> Items { get; }
    public int TotalCount { get; }
    public int PageIndex { get; }
    public int PageSize { get; }

    private PaginatedResult(
        List<T> items, 
        int totalCount, 
        int pageIndex, 
        int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageIndex = pageIndex;
        PageSize = pageSize;
    }

    public static PaginatedResult<T> Create(
        List<T> items,
        int totalCount,
        int pageIndex,
        int pageSize)
        => new PaginatedResult<T>(items, totalCount, pageIndex, pageSize);
}