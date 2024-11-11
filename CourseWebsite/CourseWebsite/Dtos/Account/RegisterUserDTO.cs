using System.ComponentModel.DataAnnotations;

namespace CourseWebsite.DTOs.Account
{
    public class RegisterUserDTO
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string Email { get; set; }

        public string Role { get; set; } = "";




    }
}
