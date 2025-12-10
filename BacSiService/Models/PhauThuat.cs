using System;
using System.Collections.Generic;

namespace BacSiService.Models;

public partial class PhauThuat
{
    public Guid Id { get; set; }

    public Guid? NhapVienId { get; set; }

    public string? LoaiPhauThuat { get; set; }

    public Guid? BacSiChinhId { get; set; }

    public string? Ekip { get; set; }

    public DateTime? Ngay { get; set; }

    public string? PhongMo { get; set; }

    public string? TrangThai { get; set; }

    public virtual BacSi? BacSiChinh { get; set; }

    public virtual NhapVien? NhapVien { get; set; }
}
