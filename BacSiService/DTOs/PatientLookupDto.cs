using System;

namespace BacSiService.DTOs
{
    public class PatientLookupDto
    {
        public Guid Id { get; set; }
        public string? HoTen { get; set; }
        public string? SoTheBaoHiem { get; set; }
        public string? DiaChi { get; set; }
        public string? GioiTinh { get; set; }
        public Guid? NhapVienId { get; set; }
        public DateTime? NgayNhap { get; set; }
        public DateTime? NgayXuat { get; set; }
        public string? TrangThaiNhapVien { get; set; }

        public Guid? KhoaId { get; set; }
        public string? TenKhoa { get; set; }
    }
}
