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

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

    public virtual ICollection<NhapVien> NhapViens { get; set; } = new List<NhapVien>();
}
