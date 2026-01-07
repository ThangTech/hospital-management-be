using System;

namespace YtaService.DTO
{
    public class HoaDonCreateDTO
    {
        public Guid BenhNhanId { get; set; }
        public Guid NhapVienId { get; set; }
        public decimal TongTien { get; set; }
        public decimal? BaoHiemChiTra { get; set; }
        public string? GhiChu { get; set; }
    }

    public class HoaDonViewDTO
    {
        public Guid Id { get; set; }
        public Guid BenhNhanId { get; set; }
        public string TenBenhNhan { get; set; }
        public Guid NhapVienId { get; set; }
        public decimal TongTien { get; set; }
        public decimal BaoHiemChiTra { get; set; }
        public decimal BenhNhanThanhToan { get; set; }
        public DateTime? Ngay { get; set; }
        public string TrangThai { get; set; }
    }

    public class HoaDonThanhToanDTO
    {
        public Guid Id { get; set; }
        public decimal SoTien { get; set; }
        public string? GhiChu { get; set; }
    }

    public class HoaDonPreviewDTO
    {
        public Guid BenhNhanId { get; set; }
        public string TenBenhNhan { get; set; }
        public Guid NhapVienId { get; set; }
        public decimal MucHuong { get; set; }
        public string TenGiuong { get; set; }
        public decimal GiaGiuong { get; set; }
        public double SoNgayNam { get; set; }
        public decimal TongTienGoiY { get; set; }
        public decimal BaoHiemChiTraGoiY => TongTienGoiY * MucHuong;
        public decimal BenhNhanPhaiTraGoiY => TongTienGoiY - BaoHiemChiTraGoiY;
    }
}
