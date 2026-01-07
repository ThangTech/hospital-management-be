using System;
using System.Collections.Generic;

namespace IdentityService.Models;

/// <summary>
/// Entity NguoiDung - scaffold từ database hospital_manage
/// </summary>
public partial class NguoiDung
{
    public Guid Id { get; set; }

    public string? TenDangNhap { get; set; }

    public string? MatKhauHash { get; set; }

    public string? VaiTro { get; set; }
}
