using System;
using System.Collections.Generic;

namespace BacSiService.Models;

public partial class BacSi
{
    public Guid Id { get; set; }

    public string? HoTen { get; set; }

    public string? ChuyenKhoa { get; set; }

    public string? ThongTinLienHe { get; set; }

    public Guid? KhoaId { get; set; }

    public virtual ICollection<DichVuDieuTri> DichVuDieuTris { get; set; } = new List<DichVuDieuTri>();

    public virtual ICollection<HoSoBenhAn> HoSoBenhAns { get; set; } = new List<HoSoBenhAn>();

    public virtual KhoaPhong? Khoa { get; set; }

    public virtual ICollection<PhauThuat> PhauThuats { get; set; } = new List<PhauThuat>();

    public virtual ICollection<XetNghiem> XetNghiems { get; set; } = new List<XetNghiem>();
}
