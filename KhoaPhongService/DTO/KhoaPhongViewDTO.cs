namespace KhoaPhongService.DTO
{
    public class KhoaPhongViewDTO
    {
        public Guid Id { get; set; }
        public string TenKhoa { get; set; }
        public string LoaiKhoa { get; set; }
        public int SoGiuongTieuChuan { get; set; }
        public int SoGiuongHienCo { get; set; }
    }
}
