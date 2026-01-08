using System;

namespace BenhNhanService.DTO
{
    // DTO cho yêu cầu kiểm tra thẻ BHYT
    public class YeuCauKiemTraBHYT
    {
        public string SoTheBaoHiem { get; set; }
        public bool IsCapCuu { get; set; } = false; // Thêm flag cấp cứu
    }

    // DTO cho kết quả trả về khi kiểm tra thẻ
    public class KetQuaKiemTraBHYT
    {
        public bool HopLe { get; set; }
        public string ThongBao { get; set; }
        public string MaDoiTuong { get; set; }
        public decimal MucHuong { get; set; }
        public DateTime? HanThe { get; set; }
        public bool DaHetHan => HanThe.HasValue && HanThe.Value < DateTime.Now;
        public string MaNoiDK { get; set; } // Mã nơi đăng ký ban đầu
        public string GoiYTuyen { get; set; } // "Đúng tuyến" hoặc "Trái tuyến"
    }

    // DTO cho yêu cầu tính toán chi phí BHYT
    public class YeuCauTinhPhiBHYT
    {
        public Guid IdBenhNhan { get; set; }
        public decimal TongTien { get; set; }
        public bool DungTuyen { get; set; }
        public bool LaCapCuu { get; set; } = false; // Trường hợp cấp cứu
        public bool CoGiayChuyenVien { get; set; } = false; // Có giấy chuyển tuyến
        public DateTime NgayHoaDon { get; set; } = DateTime.Now;
    }

    // DTO cho kết quả tính toán chi phí
    public class KetQuaTinhPhiBHYT
    {
        public decimal TongTien { get; set; }
        public decimal BaoHiemChiTra { get; set; }
        public decimal BenhNhanPhaiTra { get; set; }
        public decimal TyLeHuong { get; set; }
        public string DienGiai { get; set; }
    }
}
