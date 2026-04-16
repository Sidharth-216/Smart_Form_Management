namespace Shared;

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, long TotalCount)
{
    public long TotalPages => PageSize == 0 ? 0 : (long)Math.Ceiling(TotalCount / (double)PageSize);
}
