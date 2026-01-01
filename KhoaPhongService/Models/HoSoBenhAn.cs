using System;
using System.Collections.Generic;

namespace KhoaPhongService.Models;

public partial class HoSoBenhAn
{
    public Guid Id { get; set; }

    public Guid? NhapVienId { get; set; }

    public string? TienSuBenh { get; set; }

    public string? ChanDoanBanDau { get; set; }

    public string? ChanDoanRaVien { get; set; }

    public string? PhuongAnDieuTri { get; set; }

    public string? KetQuaDieuTri { get; set; }

    public DateTime? NgayLap { get; set; }

    public Guid? BacSiPhuTrachId { get; set; }

    public virtual BacSi? BacSiPhuTrach { get; set; }

    public virtual NhapVien? NhapVien { get; set; }
}
