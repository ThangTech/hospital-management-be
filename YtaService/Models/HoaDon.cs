using System;
using System.Collections.Generic;

namespace YtaService.Models;

public partial class HoaDon
{
    public Guid Id { get; set; }

    public Guid? BenhNhanId { get; set; }

    public Guid? NhapVienId { get; set; }

    public decimal? TongTien { get; set; }

    public decimal? BaoHiemChiTra { get; set; }

    public decimal? BenhNhanThanhToan { get; set; }

    public DateTime? Ngay { get; set; }

    public string? TrangThai { get; set; }

    public virtual BenhNhan? BenhNhan { get; set; }

    public virtual NhapVien? NhapVien { get; set; }
}
