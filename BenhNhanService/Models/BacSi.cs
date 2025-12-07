using System;
using System.Collections.Generic;

namespace QuanLyBenhNhan.Models;

public partial class BacSi
{
    public Guid Id { get; set; }

    public string? HoTen { get; set; }

    public string? ChuyenKhoa { get; set; }

    public string? ThongTinLienHe { get; set; }

    public virtual ICollection<DichVuDieuTri> DichVuDieuTris { get; set; } = new List<DichVuDieuTri>();

    public virtual ICollection<PhauThuat> PhauThuats { get; set; } = new List<PhauThuat>();

    public virtual ICollection<XetNghiem> XetNghiems { get; set; } = new List<XetNghiem>();
}
