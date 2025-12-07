using System;
using System.Collections.Generic;

namespace QuanLyBenhNhan.Models;

public partial class GiuongBenh
{
    public Guid Id { get; set; }

    public Guid? KhoaId { get; set; }

    public string? LoaiGiuong { get; set; }

    public string? TrangThai { get; set; }

    public virtual KhoaPhong? Khoa { get; set; }
}
