using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace QuanLyBenhNhan.Models;
public class BenhNhan
{
    public Guid Id { get; set; }
    public string HoTen { get; set; }
    public DateOnly NgaySinh { get; set; }
    public string GioiTinh { get; set; }
    public string DiaChi { get; set; }
    public string SoTheBaoHiem { get; set; }
    public decimal? MucHuong { get; set; } // Mức hưởng (0.8, 0.95, 1.0)
    public DateTime? HanTheBHYT { get; set; } // Ngày hết hạn thẻ
    public string TrangThai { get; set; } // Trạng thái (Đang điều trị, Đã xuất viện, ...)

    // [QUAN TRỌNG] Thêm dòng này trước các danh sách
    [JsonIgnore]
    public virtual ICollection<HoaDon> HoaDons { get; set; }

    [JsonIgnore]
    public virtual ICollection<NhapVien> NhapViens { get; set; }
}

