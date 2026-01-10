using System;

namespace BacSiService.DTOs
{
    public class MedicalRecordDto
    {
        public Guid Id { get; set; }
        public Guid? NhapVienId { get; set; }

        // Hồ sơ bệnh án fields
        public string? TienSuBenh { get; set; }
        public string? ChanDoanBanDau { get; set; }
        public string? PhuongAnDieuTri { get; set; }
        public string? KetQuaDieuTri { get; set; }
        public string? ChanDoanRaVien { get; set; }

        public DateTime? NgayLap { get; set; }
        public Guid? BacSiPhuTrachId { get; set; }
        public string? TenBacSi { get; set; }

        // Thông tin bệnh nhân (từ BenhNhan qua NhapVien)
        public string? TenBenhNhan { get; set; }
        public DateTime? NgaySinhBenhNhan { get; set; }
        public Guid? BenhNhanId { get; set; }
        public string? GioiTinh { get; set; }
        public string? DiaChi { get; set; }
        public string? SoTheBaoHiem { get; set; }
        public string? DanToc { get; set; }
        public string? SoCMND { get; set; }
        
        // Thông tin nhập viện (từ NhapVien)
        public DateTime? NgayNhap { get; set; }
        public DateTime? NgayXuat { get; set; }
        public string? LyDoNhap { get; set; }
        public string? TrangThaiNhapVien { get; set; }
        
        // Thông tin khoa/giường
        public string? TenKhoa { get; set; }
        public string? TenGiuong { get; set; }
    }
}
