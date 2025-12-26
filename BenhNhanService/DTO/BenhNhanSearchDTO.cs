namespace BenhNhanService.DTO
{
    public class BenhNhanSearchDTO
    {
        public class BenhNhanSearchModel
        {
            public int PageIndex { get; set; } = 1;  // Trang số mấy (Mặc định trang 1)
            public int PageSize { get; set; } = 10;  // Lấy bao nhiêu dòng (Mặc định 10 dòng để tránh nặng máy)

            // 3 Tiêu chí tìm kiếm/lọc
            public string? HoTen { get; set; }       // Tiêu chí 1: Tìm gần đúng tên
            public string? DiaChi { get; set; }      // Tiêu chí 2: Tìm gần đúng địa chỉ
            public string? SoTheBaoHiem { get; set; }// Tiêu chí 3: Tìm chính xác số thẻ
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
