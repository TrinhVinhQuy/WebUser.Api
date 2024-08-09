using Microsoft.AspNetCore.Mvc;
using WebUser.Application.DTOs.UserDTO;
using WebUser.Domain.Entities;

namespace WebUser.Application.Abstracts
{
    public interface IUserService
    {
        Task<ApplicationUser> CheckLogin(string username, string password);
        Task<IEnumerable<UserModelDTO>> ListUsers();
        Task<bool> UpdatedUser(string id, UpdatedUserDTO model);
        Task<bool> DeletedUser(string id);
        Task<UserModelDTO> GetUserById(string id);
        Task<bool> ChangePassword([FromBody] UserChangePasswordDTO request);
    }
}
