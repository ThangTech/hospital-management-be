namespace YtaService.DTO
{

    public class PagedResult<T>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public long TotalRecords { get; set; }
        public List<T> Items { get; set; }
    }
}