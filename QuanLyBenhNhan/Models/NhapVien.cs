using System;
using System.Collections.Generic;

namespace QuanLyBenhNhan.Models;

public partial class NhapVien
{
    public Guid Id { get; set; }

    public Guid? BenhNhanId { get; set; }

    public DateTime? NgayNhap { get; set; }

    public DateTime? NgayXuat { get; set; }

    public Guid? KhoaId { get; set; }

    public string? LyDoNhap { get; set; }

    public string? TrangThai { get; set; }

    public virtual BenhNhan? BenhNhan { get; set; }

    public virtual ICollection<DichVuDieuTri> DichVuDieuTris { get; set; } = new List<DichVuDieuTri>();

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

    public virtual KhoaPhong? Khoa { get; set; }

    public virtual ICollection<PhauThuat> PhauThuats { get; set; } = new List<PhauThuat>();

    public virtual ICollection<XetNghiem> XetNghiems { get; set; } = new List<XetNghiem>();
}
