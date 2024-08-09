using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebUser.Application.Abstracts;
using WebUser.Application.DTOs.UserDTO;
using WebUser.Domain.Entities;

namespace WebUser.Application.Services
{
    public class UserService : IUserService
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private IMapper _mapper;
        public UserService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }
        public async Task<ApplicationUser> CheckLogin(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return null;
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, true);

            if (!result.Succeeded)
            {
                return null;
            }

            return user;
        }

        public async Task<IEnumerable<UserModelDTO>> ListUsers()
        {
            var users = await Task.FromResult(_userManager.Users.ToList());
            return _mapper.Map<IEnumerable<UserModelDTO>>(users);
        }

        public async Task<bool> UpdatedUser(string id, UpdatedUserDTO model)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return false; // User not found
            }

            _mapper.Map(model, user);

            // Optionally, update other fields if needed

            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded;
        }

        public async Task<bool> DeletedUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return false; // User not found
            }

            var result = await _userManager.DeleteAsync(user);

            return result.Succeeded;
        }

        public async Task<UserModelDTO> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return null; // User not found
            }

            return _mapper.Map<UserModelDTO>(user);
        }

        public async Task<bool> ChangePassword(UserChangePasswordDTO request)
        {
            var user = await _userManager.FindByIdAsync(request.Id);

            if (user == null) return false;

            if (request.NewPassword != request.ConfirmPassword) return false;

            var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);

            return result.Succeeded;
        }

    }
}
