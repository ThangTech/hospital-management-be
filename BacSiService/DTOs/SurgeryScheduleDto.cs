using System;

namespace BacSiService.DTOs
{
    public class SurgeryScheduleDto
    {
        public Guid Id { get; set; }
        public Guid? NhapVienId { get; set; }
        public Guid? BacSiChinhId { get; set; }
        public string? LoaiPhauThuat { get; set; }
        public string? Ekip { get; set; }
        public DateTime? Ngay { get; set; }
        public string? PhongMo { get; set; }
        public string? TrangThai { get; set; }

        public string? TenBenhNhan { get; set; }
        public DateTime? NgaySinhBenhNhan { get; set; }
        public Guid? BenhNhanId { get; set; }
        
        // Thêm Chi phí và Tên bác sĩ
        public decimal? ChiPhi { get; set; }
        public string? TenBacSi { get; set; }
    }
}
