using System;
using System.Collections.Generic;

namespace YtaService.Models;

public partial class KhoaPhong
{
    public Guid Id { get; set; }

    public string? TenKhoa { get; set; }

    public string? LoaiKhoa { get; set; }

    public int? SoGiuongTieuChuan { get; set; }

    // Các danh sách liên quan
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual ICollection<BacSi> BacSis { get; set; } = new List<BacSi>();

    [System.Text.Json.Serialization.JsonIgnore]
    public virtual ICollection<DieuDuong> DieuDuongs { get; set; } = new List<DieuDuong>();

    [System.Text.Json.Serialization.JsonIgnore]
    public virtual ICollection<GiuongBenh> GiuongBenhs { get; set; } = new List<GiuongBenh>();

    [System.Text.Json.Serialization.JsonIgnore]
    public virtual ICollection<NhapVien> NhapViens { get; set; } = new List<NhapVien>();

    // --- CHỈ GIỮ LẠI 1 DÒNG DUY NHẤT NÀY ---
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual ICollection<YTa> YTa { get; set; } = new List<YTa>();
}