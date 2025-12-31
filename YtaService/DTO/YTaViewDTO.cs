namespace YtaService.DTO
{
    public class YTaViewDTO
    {
        public Guid Id { get; set; }
        public string HoTen { get; set; }
        public DateOnly? NgaySinh { get; set; }
        public string GioiTinh { get; set; }
        public string SoDienThoai { get; set; }
        public Guid? KhoaId { get; set; }
        public string ChungChiHanhNghe { get; set; }
    }
}
