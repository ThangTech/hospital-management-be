namespace BacSiService.DTOs
{
    public class SearchRequestDTO
    {
        public string? SearchTerm { get; set; }          
        public string? HoTen { get; set; }               
        public string? ChuyenKhoa { get; set; }          
        public string? ThongTinLienHe { get; set; }
        public Guid KhoaId { get; set; }
        public string? SortBy { get; set; }              
        public string? SortOrder { get; set; } = "ASC";  
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
