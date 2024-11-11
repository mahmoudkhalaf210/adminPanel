using CourseWebsite.DTOs.Account;
using CourseWebsite.Models;
using HospAPI.Models;

namespace CourseWebsite.Repositories.AccountRepository
{
    public interface IAccountRepoistory
    {
        Task<string> register(RegisterUserDTO user, string role);

        Task<ResponseTokenLoginDTO> login(LoginUserDTO userDto);

        Task<ResponseService<bool>> ResetAndChangePassword(string userId, string newPassword);

        Task<ResponseService<List<GetUserDTO>>> GetAllUsers();
        Task<ResponseService<List<string>>> GetAllRoles();
        Task<ResponseService<List<Role>>> GetAllRolesWithId();


        Task<ResponseService<GetUserDTO>> GetUserById(string id);

        Task<string> DeleteUser(string Id);
        Task<string> UpdateUser(UpdateUserDTO userDTO);
    }
}
