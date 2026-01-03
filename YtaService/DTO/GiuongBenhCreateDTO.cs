using System;

namespace YtaService.DTO
{
    public class GiuongBenhCreateDTO
    {
        public Guid KhoaId { get; set; }
        public string TenGiuong { get; set; }
        public string LoaiGiuong { get; set; }
        public decimal GiaTien { get; set; }
    }
}