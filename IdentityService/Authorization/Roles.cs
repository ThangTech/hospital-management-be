namespace IdentityService.Authorization;

/// <summary>
/// Định nghĩa các vai trò trong hệ thống
/// Sử dụng: [Authorize(Roles = Roles.Admin)]
/// </summary>
public static class Roles
{
    /// <summary>
    /// Quản trị viên - toàn quyền
    /// </summary>
    public const string Admin = "Admin";
    
    /// <summary>
    /// Bác sĩ - quản lý bệnh nhân, hồ sơ bệnh án
    /// </summary>
    public const string BacSi = "BacSi";
    
    /// <summary>
    /// Y tá - hỗ trợ bác sĩ, quản lý bệnh nhân
    /// </summary>
    public const string YTa = "YTa";
    

    /// <summary>
    /// Kế toán - quản lý hóa đơn, thanh toán
    /// </summary>
    public const string KeToan = "KeToan";
    
    /// <summary>
    /// Bệnh nhân - xem thông tin cá nhân
    /// </summary>
    public const string BenhNhan = "BenhNhan";
    
    /// <summary>
    /// Danh sách tất cả vai trò hợp lệ
    /// </summary>
    public static readonly string[] AllRoles = { Admin, BacSi, YTa, KeToan, BenhNhan };
    
    /// <summary>
    /// Kiểm tra vai trò có hợp lệ không
    /// </summary>
    public static bool IsValidRole(string role)
    {
        return AllRoles.Contains(role, StringComparer.OrdinalIgnoreCase);
    }
}
