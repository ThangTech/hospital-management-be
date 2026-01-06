namespace YtaService.DTO
{
    // DTO dùng để thêm mới (Input)
    public class HoSoBenhAnCreateDTO
    {
        public Guid? NhapVienId { get; set; } // Liên kết với đợt nhập viện
        public Guid? BacSiPhuTrachId { get; set; }
        public string? TienSuBenh { get; set; }
        public string? ChanDoanBanDau { get; set; }
        public string? PhuongAnDieuTri { get; set; }
        // NgayLap sẽ tự lấy ngày hiện tại
    }

    // DTO dùng để hiển thị (Output) - Có thể thêm ChanDoanRaVien, KetQuaDieuTri
    public class HoSoBenhAnViewDTO
    {
        public Guid Id { get; set; }
        public Guid? NhapVienId { get; set; }
        public Guid? BacSiPhuTrachId { get; set; }
        public string? TienSuBenh { get; set; }
        public string? ChanDoanBanDau { get; set; }
        public string? PhuongAnDieuTri { get; set; }
        public string? ChanDoanRaVien { get; set; }
        public string? KetQuaDieuTri { get; set; }
        public DateTime? NgayLap { get; set; }
    }
}