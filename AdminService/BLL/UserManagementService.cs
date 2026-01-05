using AdminService.DAL;
using AdminService.DTOs;

namespace AdminService.BLL;

public class UserManagementService : IUserManagementService
{
    private readonly IUserManagementRepository _repo;

    public UserManagementService(IUserManagementRepository repo)
    {
        _repo = repo;
    }

    public PagedResult<UserDTO> Search(UserSearchDTO search)
    {
        if (search.PageNumber < 1) search.PageNumber = 1;
        if (search.PageSize < 1) search.PageSize = 20;
        if (search.PageSize > 100) search.PageSize = 100;

        return _repo.Search(search);
    }

    public UserDTO? GetById(Guid id) => _repo.GetById(id);

    public List<UserDTO> GetAll() => _repo.GetAll();

    public (bool Success, string Message, UserDTO? User) Create(CreateUserDTO dto)
    {
        // Validate vai trò
        if (!Roles.IsValid(dto.VaiTro))
        {
            return (false, $"Vai trò không hợp lệ. Các vai trò hợp lệ: {string.Join(", ", Roles.AllRoles)}", null);
        }

        // Kiểm tra username đã tồn tại
        if (_repo.Exists(dto.TenDangNhap))
        {
            return (false, "Tên đăng nhập đã tồn tại", null);
        }

        // Validate mật khẩu
        if (dto.MatKhau.Length < 6)
        {
            return (false, "Mật khẩu phải có ít nhất 6 ký tự", null);
        }

        try
        {
            var user = _repo.Create(dto);
            return (true, "Tạo user thành công", user);
        }
        catch (Exception ex)
        {
            return (false, ex.Message, null);
        }
    }

    public (bool Success, string Message, UserDTO? User) Update(UpdateUserDTO dto)
    {
        // Kiểm tra user tồn tại
        var existing = _repo.GetById(dto.Id);
        if (existing == null)
        {
            return (false, "User không tồn tại", null);
        }

        // Validate vai trò nếu có thay đổi
        if (!string.IsNullOrEmpty(dto.VaiTro) && !Roles.IsValid(dto.VaiTro))
        {
            return (false, $"Vai trò không hợp lệ. Các vai trò hợp lệ: {string.Join(", ", Roles.AllRoles)}", null);
        }

        // Kiểm tra username mới nếu có thay đổi
        if (!string.IsNullOrEmpty(dto.TenDangNhap) && dto.TenDangNhap != existing.TenDangNhap)
        {
            if (_repo.Exists(dto.TenDangNhap))
            {
                return (false, "Tên đăng nhập đã tồn tại", null);
            }
        }

        // Set default values if not provided
        dto.TenDangNhap ??= existing.TenDangNhap;
        dto.VaiTro ??= existing.VaiTro;

        var updated = _repo.Update(dto);
        return updated != null 
            ? (true, "Cập nhật user thành công", updated) 
            : (false, "Cập nhật thất bại", null);
    }

    public (bool Success, string Message) Delete(Guid id)
    {
        var existing = _repo.GetById(id);
        if (existing == null)
        {
            return (false, "User không tồn tại");
        }

        // Không cho xóa Admin cuối cùng (optional protection)
        if (existing.VaiTro == Roles.Admin)
        {
            var allUsers = _repo.GetAll();
            var adminCount = allUsers.Count(u => u.VaiTro == Roles.Admin);
            if (adminCount <= 1)
            {
                return (false, "Không thể xóa Admin cuối cùng");
            }
        }

        return _repo.Delete(id) 
            ? (true, "Xóa user thành công") 
            : (false, "Xóa thất bại");
    }

    public (bool Success, string Message) ResetPassword(ResetPasswordDTO dto)
    {
        var existing = _repo.GetById(dto.UserId);
        if (existing == null)
        {
            return (false, "User không tồn tại");
        }

        if (dto.MatKhauMoi.Length < 6)
        {
            return (false, "Mật khẩu mới phải có ít nhất 6 ký tự");
        }

        return _repo.ResetPassword(dto.UserId, dto.MatKhauMoi) 
            ? (true, "Reset mật khẩu thành công") 
            : (false, "Reset mật khẩu thất bại");
    }
}
