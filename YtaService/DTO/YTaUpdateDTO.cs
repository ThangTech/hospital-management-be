namespace YtaService.DTO
{
    public class YTaUpdateDTO
    {
        public Guid Id { get; set; } // Bắt buộc có ID để biết sửa ai
        public string HoTen { get; set; }
        public DateOnly? NgaySinh { get; set; }
        public string GioiTinh { get; set; }
        public string SoDienThoai { get; set; }
        public Guid? KhoaId { get; set; }
        public string ChungChiHanhNghe { get; set; }
    }
}
