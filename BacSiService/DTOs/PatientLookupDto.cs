using System;

namespace BacSiService.DTOs
{
    public class PatientLookupDto
    {
        public Guid Id { get; set; }
        public string? HoTen { get; set; }
        public string? SoTheBaoHiem { get; set; }
        public string? DiaChi { get; set; }
        public string? GioiTinh { get; set; }
    }
}
