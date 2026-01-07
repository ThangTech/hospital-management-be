using System;
using System.Collections.Generic;

namespace YtaService.Models;

public partial class BenhNhan
{
    public Guid Id { get; set; }

    public string? HoTen { get; set; }

    public DateOnly? NgaySinh { get; set; }

    public string? GioiTinh { get; set; }

    public string? DiaChi { get; set; }

    public string? SoTheBaoHiem { get; set; }
    public decimal? MucHuong { get; set; } // Mức hưởng BHYT (0.8, 0.95, 1.0)

    [System.Text.Json.Serialization.JsonIgnore]
    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

    [System.Text.Json.Serialization.JsonIgnore]
    public virtual ICollection<HoSoBenhAn> HoSoBenhAns { get; set; } = new List<HoSoBenhAn>();

    [System.Text.Json.Serialization.JsonIgnore]
    public virtual ICollection<NhapVien> NhapViens { get; set; } = new List<NhapVien>();
}
