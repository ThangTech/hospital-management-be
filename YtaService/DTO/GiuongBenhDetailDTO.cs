using System;
using System.Collections.Generic;

namespace YtaService.DTO
{
    public class GiuongBenhDetailDTO
    {
        public Guid Id { get; set; }
        public string TenGiuong { get; set; }
        public string LoaiGiuong { get; set; }
        public decimal GiaTien { get; set; }
        public string TrangThai { get; set; }

        // Chỉ chứa thông tin Khoa cần thiết
        public KhoaSimpleDTO Khoa { get; set; }

        // Danh sách người nằm (đã lọc gọn)
        public List<NhapVienSimpleDTO> NhapViens { get; set; }
    }

    public class KhoaSimpleDTO
    {
        public string TenKhoa { get; set; }
        public string LoaiKhoa { get; set; }
    }

    public class NhapVienSimpleDTO
    {
        public DateTime NgayNhap { get; set; }
        public BenhNhanSimpleDTO BenhNhan { get; set; }
    }

    public class BenhNhanSimpleDTO
    {
        public string HoTen { get; set; }
        public string GioiTinh { get; set; }
        public string SoTheBaoHiem { get; set; }
        public DateTime? NgaySinh { get; set; }
    }
}