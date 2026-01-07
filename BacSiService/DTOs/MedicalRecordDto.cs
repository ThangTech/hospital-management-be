using System;

namespace BacSiService.DTOs
{
    public class MedicalRecordDto
    {
        public Guid Id { get; set; }
        public Guid? NhapVienId { get; set; }

        public string? TienSuBenh { get; set; }
        public string? ChanDoanBanDau { get; set; }
        public string? PhuongAnDieuTri { get; set; }
        public string? KetQuaDieuTri { get; set; }
        public string? ChanDoanRaVien { get; set; }

        public DateTime? NgayLap { get; set; }
        public Guid? BacSiPhuTrachId { get; set; }

        public string? TenBenhNhan { get; set; }
        public DateTime? NgaySinhBenhNhan { get; set; }
        public Guid? BenhNhanId { get; set; }
    }
}
