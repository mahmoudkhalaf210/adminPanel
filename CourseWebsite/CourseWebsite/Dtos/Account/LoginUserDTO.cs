using System.ComponentModel.DataAnnotations;

namespace CourseWebsite.DTOs.Account
{
    public class LoginUserDTO
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }

    }
}
