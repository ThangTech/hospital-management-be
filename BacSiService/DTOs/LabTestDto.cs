using System;

namespace BacSiService.DTOs
{
    public class LabTestDto
    {
        public Guid Id { get; set; }
        public Guid? NhapVienId { get; set; }
        public Guid? BacSiId { get; set; }
        public string? LoaiXetNghiem { get; set; }
        public string? KetQua { get; set; }
        public DateTime? Ngay { get; set; }
    }
}
