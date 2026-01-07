using AdminService.DTOs;

namespace AdminService.DAL;

public interface IUserManagementRepository
{
    PagedResult<UserDTO> Search(UserSearchDTO search);
    UserDTO? GetById(Guid id);
    UserDTO? GetByTenDangNhap(string tenDangNhap);
    bool Exists(string tenDangNhap);
    UserDTO Create(CreateUserDTO dto);
    UserDTO? Update(UpdateUserDTO dto);
    bool Delete(Guid id);
    bool ResetPassword(Guid userId, string hashedPassword);
    List<UserDTO> GetAll();
}
