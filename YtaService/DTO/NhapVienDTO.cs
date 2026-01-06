namespace YtaService.DTO
{
    public class NhapVienCreateDTO
    {
        public Guid BenhNhanId { get; set; }
        public Guid GiuongId { get; set; }
        public Guid KhoaId { get; set; }
        public string LyDoNhap { get; set; }
    }
    public class NhapVienViewDTO
    {
        public Guid Id { get; set; }

        public Guid BenhNhanId { get; set; }
        public string TenBenhNhan { get; set; } 

        public Guid GiuongId { get; set; }
        public string TenGiuong { get; set; }  

        public Guid KhoaId { get; set; }
        public string TenKhoa { get; set; }    
        public string LyDoNhap { get; set; }
        public DateTime NgayNhap { get; set; }
        public DateTime? NgayXuat { get; set; }
        public string TrangThai { get; set; }
    }

    public class NhapVienUpdateDTO
    {
        public Guid Id { get; set; }
        public string LyDoNhap { get; set; }
        public string TrangThai { get; set; }
        public DateTime? NgayXuat { get; set; }
    }

    public class ChuyenGiuongDTO
    {
        public Guid NhapVienId { get; set; }
        public Guid GiuongMoiId { get; set; }
        public string LyDoChuyenGiuong { get; set; }
    }

    // DTO cho chức năng tìm kiếm
    public class NhapVienSearchDTO
    {
        public string? TenBenhNhan { get; set; }
        public Guid? KhoaId { get; set; }
        public string? TrangThai { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
    }

}
