using AdminService.DTOs;

namespace AdminService.BLL;

public interface IUserManagementService
{
    PagedResult<UserDTO> Search(UserSearchDTO search);
    UserDTO? GetById(Guid id);
    (bool Success, string Message, UserDTO? User) Create(CreateUserDTO dto);
    (bool Success, string Message, UserDTO? User) Update(UpdateUserDTO dto);
    (bool Success, string Message) Delete(Guid id);
    (bool Success, string Message) ResetPassword(ResetPasswordDTO dto);
    List<UserDTO> GetAll();
}
