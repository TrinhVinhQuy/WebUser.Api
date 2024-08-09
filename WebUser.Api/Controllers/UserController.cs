using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebUser.Api.ViewModel;
using WebUser.Application.Abstracts;
using WebUser.Application.DTOs.UserDTO;
using WebUser.Domain.Abstracts;
using WebUser.Domain.Entities;
using WebUser.Domain.Model;

namespace WebUser.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly PasswordHasher<ApplicationUser> _passwordHasher;
        private readonly IUserService _userService;
        public UserController(UserManager<ApplicationUser> userManager,
            PasswordHasher<ApplicationUser> passwordHasher,
            IEmailService emailService, IUserService userService)
        {
            _userManager = userManager;
            _emailService = emailService;
            _passwordHasher = passwordHasher;
            _userService = userService;
        }
        [HttpPost("register-user")]
        public async Task<IActionResult> Register(CancellationToken cancellationToken, [FromBody] UserModel userVM)
        {
            if (userVM is null)
            {
                return BadRequest("Invalid Data");
            }
            var user = new ApplicationUser
            {
                Fullname = userVM.Fullname,
                UserName = userVM.Username,
                Email = userVM.Email,
                PasswordHash = userVM.Password,
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, user.PasswordHash);

            var result = await _userManager.CreateAsync(user);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Admin");

                //Send Email for Confirm
                string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                //string url = Url.Action("ConfirmEmail", "User", new { memberKey = user.Id, tokenReset = token }, Request.Scheme);
                string url = $"https://localhost:7128/api/User/confirm-email?memberKey={user.Id}&&tokenReset={token}";

                string body = await _emailService.GetTemplate("Templates\\ConfirmEmail.html");
                body = string.Format(body, user.Fullname, url);

                await _emailService.SendEmailAsync(cancellationToken, new EmailRequest
                {
                    To = user.Email,
                    Subject = "Confirm Email For Register",
                    Content = body
                });

                return Ok(new
                {
                    memberKey = user.Id,
                    tokenReset = token,
                });
            }
            else
                return BadRequest(result.Errors);
        }
        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmail model, CancellationToken cancellationToken)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.MemberKey) || string.IsNullOrWhiteSpace(model.TokenReset))
            {
                return BadRequest("Invalid parameters");
            }

            var user = await _userManager.FindByIdAsync(model.MemberKey);

            if (user == null)
            {
                return BadRequest("User not found");
            }

            var result = await _userManager.ConfirmEmailAsync(user, model.TokenReset);

            if (result.Succeeded)
            {
                return Ok("Email confirmed successfully");
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpGet("list-user")]
        [AllowAnonymous]
        public async Task<IActionResult> ListUsers()
        {
            var users = await _userService.ListUsers();
            return Ok(users);
        }

        [HttpPut("edit/{id}")]
        public async Task<IActionResult> EditUser(string id, UpdatedUserDTO updatedUser)
        {
            var result = await _userService.UpdatedUser(id, updatedUser);
            if (result) return Ok(new { message = "User updated successfully" });
            return Ok(result);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _userService.DeletedUser(id);
            if (result) return Ok(new { message = "User deleted successfully" });
            return Ok(result);
        }
    }
}
