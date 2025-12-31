using System;
using System.Collections.Generic;

namespace BacSiService.Models;

public partial class KhoaPhong
{
    public Guid Id { get; set; }

    public string? TenKhoa { get; set; }

    public string? LoaiKhoa { get; set; }

    public int? SoGiuongTieuChuan { get; set; }

    public virtual ICollection<DieuDuong> DieuDuongs { get; set; } = new List<DieuDuong>();

    public virtual ICollection<GiuongBenh> GiuongBenhs { get; set; } = new List<GiuongBenh>();

    public virtual ICollection<NhapVien> NhapViens { get; set; } = new List<NhapVien>();

    public virtual ICollection<Ytum> Yta { get; set; } = new List<Ytum>();
}
