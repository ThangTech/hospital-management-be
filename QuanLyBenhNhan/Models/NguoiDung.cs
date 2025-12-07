using System;
using System.Collections.Generic;

namespace QuanLyBenhNhan.Models;

public partial class NguoiDung
{
    public Guid Id { get; set; }

    public string? TenDangNhap { get; set; }

    public string? MatKhauHash { get; set; }

    public string? VaiTro { get; set; }

    public virtual ICollection<NhatKyHeThong> NhatKyHeThongs { get; set; } = new List<NhatKyHeThong>();
}
