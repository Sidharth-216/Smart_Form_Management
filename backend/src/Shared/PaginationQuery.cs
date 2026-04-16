namespace Shared;

public sealed record PaginationQuery(int Page = 1, int PageSize = 12)
{
    public int SafePage => Page < 1 ? 1 : Page;
    public int SafePageSize => PageSize is < 1 or > 100 ? 12 : PageSize;
}
