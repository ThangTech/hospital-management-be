namespace BenhNhanService.DTO
{
    public class BenhNhanSearchDTO
    {
        public class BenhNhanSearchModel
        {
            public int PageIndex { get; set; } = 1;
            public int PageSize { get; set; } = 10;

            // Tiêu chí tìm kiếm/lọc
            public string? Id { get; set; }
            public string? HoTen { get; set; }
            public string? DiaChi { get; set; }
            public string? SoTheBaoHiem { get; set; }
            public int? NamSinh { get; set; }
        }

        // 2. OUTPUT: DTO dùng để Trả về kết quả phân trang chuẩn
        public class PagedResult<T>
        {
            public int PageIndex { get; set; }
            public int PageSize { get; set; }
            public long TotalRecords { get; set; } // Tổng số bản ghi tìm thấy (để tính số trang)
            public List<T> Items { get; set; }     // Danh sách dữ liệu của trang hiện tại
        }
    }
}
