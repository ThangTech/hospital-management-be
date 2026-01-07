using System;
using System.Collections.Generic;

namespace YtaService.DTO
{
    public class XuatVienDTO
    {
        public Guid Id { get; set; }
        public DateTime? NgayXuat { get; set; }
        public string ChanDoanXuatVien { get; set; } // Chẩn đoán lúc ra viện
        public string LoiDanBacSi { get; set; }      // Lời dặn của bác sĩ
        public string GhiChu { get; set; }
    }

    // Danh sách bệnh nhân sẵn sàng xuất viện
    public class SanSangXuatVienDTO
    {
        public Guid NhapVienId { get; set; }
        public string TenBenhNhan { get; set; }
        public string TenGiuong { get; set; }
        public string TenKhoa { get; set; }
        public DateTime NgayNhap { get; set; }
        public int SoNgayNam { get; set; }
        public decimal TongTien { get; set; }
    }

    // Thông tin xem trước xuất viện
    public class XuatVienPreviewDTO
    {
        public Guid NhapVienId { get; set; }
        public string TenBenhNhan { get; set; }
        public DateTime NgayNhap { get; set; }
        public int SoNgayNam { get; set; }
        public decimal TongTienHoaDon { get; set; }
        public decimal DaThanhToan { get; set; }
        public decimal ConNo { get; set; }
        public bool SanSangXuatVien { get; set; }
        public List<HoaDonDTO> DanhSachHoaDon { get; set; }
    }

    public class HoaDonDTO
    {
        public Guid Id { get; set; }
        public decimal TongTien { get; set; }
        public string TrangThai { get; set; }
        public DateTime? Ngay { get; set; }
    }

    // Lịch sử xuất viện
    public class LichSuXuatVienDTO
    {
        public Guid NhapVienId { get; set; }
        public string TenBenhNhan { get; set; }
        public string TenKhoa { get; set; }
        public DateTime NgayNhap { get; set; }
        public DateTime NgayXuat { get; set; }
        public int SoNgayNam { get; set; }
        public string ChanDoanXuatVien { get; set; }
        public string LoiDanBacSi { get; set; }
        public string GhiChu { get; set; }
    }
}
