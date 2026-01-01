using System;
using System.Collections.Generic;

namespace KhoaPhongService.Models;

public partial class DieuDuong
{
    public Guid Id { get; set; }

    public string? HoTen { get; set; }

    public Guid? KhoaId { get; set; }

    public virtual ICollection<DichVuDieuTri> DichVuDieuTris { get; set; } = new List<DichVuDieuTri>();

    public virtual KhoaPhong? Khoa { get; set; }
}
