using System;
using System.Collections.Generic;

namespace BacSiService.Models;

public partial class AuditHoSoBenhAn
{
    public Guid Id { get; set; }

    public Guid? HoSoBenhAnId { get; set; }

    public string? HanhDong { get; set; }

    public string? ChanDoanCu { get; set; }

    public string? KetQuaCu { get; set; }

    public string? NguoiSua { get; set; }

    public DateTime? ThoiGianSua { get; set; }
}
