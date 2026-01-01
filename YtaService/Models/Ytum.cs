using System;
using System.Collections.Generic;

namespace YtaService.Models;

public partial class YTa
{
    public Guid Id { get; set; }

    public string HoTen { get; set; } = null!;

    public DateOnly? NgaySinh { get; set; }

    public string? GioiTinh { get; set; }

    public string? SoDienThoai { get; set; }

    public Guid? KhoaId { get; set; }

    public string? ChungChiHanhNghe { get; set; }

    public virtual KhoaPhong? Khoa { get; set; }
}
