using System.ComponentModel.DataAnnotations;

namespace webResfulAPIs.Models.DTO
{
    public class UserLoginDTO
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
