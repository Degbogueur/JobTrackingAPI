namespace JobTrackingAPI.Contracts.Results;

public class PaginatedResult<T>
{
    public List<T> Items { get; }
    public int TotalItems { get; }
    public int PageIndex { get; }
    public int PageSize { get; }

    private PaginatedResult(
        List<T> items, 
        int totalItems, 
        int pageIndex, 
        int pageSize)
    {
        Items = items;
        TotalItems = totalItems;
        PageIndex = pageIndex;
        PageSize = pageSize;
    }

    public static PaginatedResult<T> Create(
        List<T> items,
        int totalItems,
        int pageIndex,
        int pageSize)
        => new PaginatedResult<T>(items, totalItems, pageIndex, pageSize);
}