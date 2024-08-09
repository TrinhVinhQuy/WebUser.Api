using Microsoft.AspNetCore.Authentication.JwtBearer;
using WebUser.Domain.Entities;
using WebUser.Domain.Model;

namespace WebUser.Domain.Abstracts
{
    public interface ITokenHandler
    {
        /// <summary>
        /// Validate token
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task ValidateToken(TokenValidatedContext context);
        Task<string> CreateAccessToken(ApplicationUser user);
        Task<string> CreateRefreshToken(ApplicationUser user);
        Task<JwtModel> ValidateRefreshToken(string refreshToken);
    }
}
