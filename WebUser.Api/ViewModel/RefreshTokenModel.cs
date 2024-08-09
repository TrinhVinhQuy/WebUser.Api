using System.ComponentModel.DataAnnotations;

namespace WebUser.Api.ViewModel
{
    public class RefreshTokenModel
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
