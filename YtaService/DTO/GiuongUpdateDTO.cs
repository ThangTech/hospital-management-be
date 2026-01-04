using System;

namespace YtaService.DTO
{
    public class GiuongUpdateDTO
    {
        public Guid Id { get; set; }        // Bắt buộc là GUID
        public Guid KhoaId { get; set; }    // Bắt buộc là GUID
        public string LoaiGiuong { get; set; }
        public string TrangThai { get; set; }
        public decimal GiaTien { get; set; }
        public string TenGiuong { get; set; }
    }
}