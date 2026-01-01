namespace YtaService.DTO
{
    public class YTaSearchDTO
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? HoTen { get; set; }
        public string? SoDienThoai { get; set; }
    }
}
