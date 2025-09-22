namespace TMB.Challenge.Application.Common;

public class PaginatedResult<T>(List<T> items, int count, int pageNumber, int pageSize)
{
    public List<T> Items { get; } = items;
    public int PageNumber { get; } = pageNumber;
    public int PageSize { get; } = pageSize;
    public int TotalCount { get; } = count;
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}