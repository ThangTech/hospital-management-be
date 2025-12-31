using System;

namespace BacSiService.DTOs
{
    public class MedicalRecordDto
    {
        public Guid Id { get; set; }
        public Guid? NhapVienId { get; set; }
        public Guid? BenhNhanId { get; set; }
        public Guid? BacSiId { get; set; }
        public Guid? DieuDuongId { get; set; }
        public Guid? KhoaId { get; set; }
        public string? MoTa { get; set; }
        public DateTime? Ngay { get; set; }
    }
}
