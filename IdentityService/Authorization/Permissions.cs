namespace IdentityService.Authorization;

/// <summary>
/// Định nghĩa các quyền trong hệ thống
/// Format: [ManHinh].[HanhDong]
/// </summary>
public static class Permissions
{
    // ===== BỆNH NHÂN =====
    public const string BenhNhan_Xem = "BenhNhan.Xem";
    public const string BenhNhan_Them = "BenhNhan.Them";
    public const string BenhNhan_Sua = "BenhNhan.Sua";
    public const string BenhNhan_Xoa = "BenhNhan.Xoa";
    
    // ===== BÁC SĨ =====
    public const string BacSi_Xem = "BacSi.Xem";
    public const string BacSi_Them = "BacSi.Them";
    public const string BacSi_Sua = "BacSi.Sua";
    public const string BacSi_Xoa = "BacSi.Xoa";
    
    // ===== Y TÁ =====
    public const string YTa_Xem = "YTa.Xem";
    public const string YTa_Them = "YTa.Them";
    public const string YTa_Sua = "YTa.Sua";
    public const string YTa_Xoa = "YTa.Xoa";
    
    // ===== HỒ SƠ BỆNH ÁN =====
    public const string HoSoBenhAn_Xem = "HoSoBenhAn.Xem";
    public const string HoSoBenhAn_Them = "HoSoBenhAn.Them";
    public const string HoSoBenhAn_Sua = "HoSoBenhAn.Sua";
    public const string HoSoBenhAn_Xoa = "HoSoBenhAn.Xoa";
    
    // ===== HÓA ĐƠN =====
    public const string HoaDon_Xem = "HoaDon.Xem";
    public const string HoaDon_Them = "HoaDon.Them";
    public const string HoaDon_Sua = "HoaDon.Sua";
    public const string HoaDon_Xoa = "HoaDon.Xoa";
    
    // ===== KHOA PHÒNG =====
    public const string KhoaPhong_Xem = "KhoaPhong.Xem";
    public const string KhoaPhong_Them = "KhoaPhong.Them";
    public const string KhoaPhong_Sua = "KhoaPhong.Sua";
    public const string KhoaPhong_Xoa = "KhoaPhong.Xoa";
    
    // ===== PHẪU THUẬT =====
    public const string PhauThuat_Xem = "PhauThuat.Xem";
    public const string PhauThuat_Them = "PhauThuat.Them";
    public const string PhauThuat_Sua = "PhauThuat.Sua";
    public const string PhauThuat_Xoa = "PhauThuat.Xoa";
    
    // ===== XÉT NGHIỆM =====
    public const string XetNghiem_Xem = "XetNghiem.Xem";
    public const string XetNghiem_Them = "XetNghiem.Them";
    public const string XetNghiem_Sua = "XetNghiem.Sua";
    public const string XetNghiem_Xoa = "XetNghiem.Xoa";
    
    // ===== NGƯỜI DÙNG =====
    public const string NguoiDung_Xem = "NguoiDung.Xem";
    public const string NguoiDung_Them = "NguoiDung.Them";
    public const string NguoiDung_Sua = "NguoiDung.Sua";
    public const string NguoiDung_Xoa = "NguoiDung.Xoa";
    
    // ===== THỐNG KÊ =====
    public const string ThongKe_Xem = "ThongKe.Xem";
}

/// <summary>
/// Map quyền theo vai trò
/// </summary>
public static class RolePermissions
{
    private static readonly Dictionary<string, List<string>> _rolePermissions = new()
    {
        // ADMIN - Toàn quyền
        [Roles.Admin] = new List<string>
        {
            // Bệnh nhân
            Permissions.BenhNhan_Xem, Permissions.BenhNhan_Them, 
            Permissions.BenhNhan_Sua, Permissions.BenhNhan_Xoa,
            // Bác sĩ
            Permissions.BacSi_Xem, Permissions.BacSi_Them, 
            Permissions.BacSi_Sua, Permissions.BacSi_Xoa,
            // Y tá
            Permissions.YTa_Xem, Permissions.YTa_Them, 
            Permissions.YTa_Sua, Permissions.YTa_Xoa,
            // Hồ sơ bệnh án
            Permissions.HoSoBenhAn_Xem, Permissions.HoSoBenhAn_Them, 
            Permissions.HoSoBenhAn_Sua, Permissions.HoSoBenhAn_Xoa,
            // Hóa đơn
            Permissions.HoaDon_Xem, Permissions.HoaDon_Them, 
            Permissions.HoaDon_Sua, Permissions.HoaDon_Xoa,
            // Khoa phòng
            Permissions.KhoaPhong_Xem, Permissions.KhoaPhong_Them, 
            Permissions.KhoaPhong_Sua, Permissions.KhoaPhong_Xoa,
            // Phẫu thuật
            Permissions.PhauThuat_Xem, Permissions.PhauThuat_Them, 
            Permissions.PhauThuat_Sua, Permissions.PhauThuat_Xoa,
            // Xét nghiệm
            Permissions.XetNghiem_Xem, Permissions.XetNghiem_Them, 
            Permissions.XetNghiem_Sua, Permissions.XetNghiem_Xoa,
            // Người dùng
            Permissions.NguoiDung_Xem, Permissions.NguoiDung_Them, 
            Permissions.NguoiDung_Sua, Permissions.NguoiDung_Xoa,
            // Thống kê
            Permissions.ThongKe_Xem
        },
        
        // BÁC SĨ - Quản lý bệnh nhân, hồ sơ, phẫu thuật, xét nghiệm
        [Roles.BacSi] = new List<string>
        {
            Permissions.BenhNhan_Xem, Permissions.BenhNhan_Them, Permissions.BenhNhan_Sua,
            Permissions.BacSi_Xem,
            Permissions.YTa_Xem,
            Permissions.HoSoBenhAn_Xem, Permissions.HoSoBenhAn_Them, Permissions.HoSoBenhAn_Sua,
            Permissions.KhoaPhong_Xem,
            Permissions.PhauThuat_Xem, Permissions.PhauThuat_Them, Permissions.PhauThuat_Sua,
            Permissions.XetNghiem_Xem, Permissions.XetNghiem_Them, Permissions.XetNghiem_Sua
        },
        
        // Y TÁ - Hỗ trợ quản lý bệnh nhân
        [Roles.YTa] = new List<string>
        {
            Permissions.BenhNhan_Xem, Permissions.BenhNhan_Them, Permissions.BenhNhan_Sua,
            Permissions.BacSi_Xem,
            Permissions.YTa_Xem,
            Permissions.HoSoBenhAn_Xem,
            Permissions.KhoaPhong_Xem,
            Permissions.XetNghiem_Xem
        },
        
        // KẾ TOÁN - Quản lý hóa đơn, thống kê
        [Roles.KeToan] = new List<string>
        {
            Permissions.BenhNhan_Xem,
            Permissions.HoaDon_Xem, Permissions.HoaDon_Them, Permissions.HoaDon_Sua,
            Permissions.ThongKe_Xem
        },
        
        // BỆNH NHÂN - Chỉ xem thông tin cá nhân
        [Roles.BenhNhan] = new List<string>
        {
            Permissions.BenhNhan_Xem,  // Chỉ xem thông tin của chính mình
            Permissions.HoSoBenhAn_Xem, // Chỉ xem hồ sơ của chính mình
            Permissions.HoaDon_Xem      // Chỉ xem hóa đơn của chính mình
        }
    };

    /// <summary>
    /// Lấy danh sách quyền của một vai trò
    /// </summary>
    public static List<string> GetPermissions(string role)
    {
        if (_rolePermissions.TryGetValue(role, out var permissions))
        {
            return permissions;
        }
        return new List<string>();
    }

    /// <summary>
    /// Kiểm tra vai trò có quyền cụ thể không
    /// </summary>
    public static bool HasPermission(string role, string permission)
    {
        return GetPermissions(role).Contains(permission);
    }
}
