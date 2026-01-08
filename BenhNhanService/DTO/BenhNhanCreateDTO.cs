namespace BenhNhanService.DTO
{
    public class BenhNhanCreateDTO
    {
        public string HoTen { get; set; }
        public DateOnly NgaySinh { get; set; }
        public string GioiTinh { get; set; }
        public string DiaChi { get; set; }
        public string SoTheBaoHiem { get; set; }
        public decimal? MucHuong { get; set; }
        public DateTime? HanTheBHYT { get; set; }
    }



}
