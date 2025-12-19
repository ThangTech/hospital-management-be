using System;
using System.Collections.Generic;

namespace BacSiService.Models;

public partial class XetNghiem
{
    public Guid Id { get; set; }

    public Guid? NhapVienId { get; set; }

    public string? LoaiXetNghiem { get; set; }

    public string? KetQua { get; set; }

    public DateTime? Ngay { get; set; }

    public Guid? BacSiId { get; set; }

    public virtual BacSi? BacSi { get; set; }

    public virtual NhapVien? NhapVien { get; set; }
}
