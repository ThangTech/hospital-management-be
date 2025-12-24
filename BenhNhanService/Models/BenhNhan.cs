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

    // [QUAN TRỌNG] Thêm dòng này trước các danh sách
    [JsonIgnore]
    public virtual ICollection<HoaDon> HoaDons { get; set; }

    [JsonIgnore]
    public virtual ICollection<NhapVien> NhapViens { get; set; }
}

