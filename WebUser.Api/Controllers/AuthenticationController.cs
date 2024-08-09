using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebUser.Api.ViewModel;
using WebUser.Application.Abstracts;
using WebUser.Application.DTOs.UserDTO;
using WebUser.Domain.Abstracts;
using WebUser.Domain.Model;

namespace WebUser.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private ITokenHandler _tokenHandler;
        private IUserService _userService;
        private readonly IEmailService _emailService;

        public AuthenticationController(ITokenHandler tokenHandler, IUserService userService, IEmailService emailService)
        {
            _tokenHandler = tokenHandler;
            _userService = userService;
            _emailService = emailService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] AccountModel accountModel)
        {
            var user = await _userService.CheckLogin(accountModel.Username, accountModel.Password);

            if (user == null)
            {
                return BadRequest("The username or password is incorrect.");
            }

            if (!user.EmailConfirmed)
            {
                return BadRequest("Your account is inactive.");
            }


            string accessToken = await _tokenHandler.CreateAccessToken(user);
            string refreshToken = await _tokenHandler.CreateRefreshToken(user);

            return Ok(new JwtModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenModel token)
        {
            if (token == null)
                return BadRequest("Could not get refresh token");

            return Ok(await _tokenHandler.ValidateRefreshToken(token.RefreshToken));
        }
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] UserChangePasswordDTO token)
        {
            var result = await _userService.ChangePassword(token);
            return Ok(result);
        }
    }
}
