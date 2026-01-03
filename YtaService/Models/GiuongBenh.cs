using System;
using System.Collections.Generic;

namespace YtaService.Models;

public partial class GiuongBenh
{
    public Guid Id { get; set; }

    public Guid? KhoaId { get; set; }

    public string? LoaiGiuong { get; set; }

    public string? TrangThai { get; set; }

    public decimal? GiaTien { get; set; }

    public string TenGiuong { get; set; }
    public virtual KhoaPhong? Khoa { get; set; }

    public virtual ICollection<NhapVien> NhapViens { get; set; } = new List<NhapVien>();
}
