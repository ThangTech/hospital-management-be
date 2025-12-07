using System;
using System.Collections.Generic;

namespace QuanLyBenhNhan.Models;

public partial class NhatKyHeThong
{
    public Guid Id { get; set; }

    public Guid? NguoiDungId { get; set; }

    public string? HanhDong { get; set; }

    public DateTime? ThoiGian { get; set; }

    public string? MoTa { get; set; }

    public virtual NguoiDung? NguoiDung { get; set; }
}
