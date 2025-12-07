using System;
using System.Collections.Generic;

namespace QuanLyBenhNhan.Models;

public partial class DichVuDieuTri
{
    public Guid Id { get; set; }

    public Guid? NhapVienId { get; set; }

    public string? LoaiDichVu { get; set; }

    public int? SoLuong { get; set; }

    public DateTime? Ngay { get; set; }

    public Guid? BacSiId { get; set; }

    public Guid? DieuDuongId { get; set; }

    public virtual BacSi? BacSi { get; set; }

    public virtual DieuDuong? DieuDuong { get; set; }

    public virtual NhapVien? NhapVien { get; set; }
}
