namespace Galosoft.IaaS.Core
{
    public class PagedRequest<T>
        : PagedRequestDto
    {
        public T? Condition { get; set; }
    }

    public class PagedRequestDto
    {
        public string? Q { get; set; }
        public int PageNo { get; set; } = 1;
        public int PageSize { get; set; } = 30;
    }
}
