using System;

namespace BacSiService.DTOs
{
    public class DoctorDto
    {
        public Guid? Id { get; set; }
        public string? HoTen { get; set; }
        public string? ChuyenKhoa { get; set; }
        public string? ThongTinLienHe { get; set; }
        public Guid? KhoaId { get; set; }
    }
}
