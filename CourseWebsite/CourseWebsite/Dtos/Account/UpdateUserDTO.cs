using System.ComponentModel.DataAnnotations;

namespace CourseWebsite.DTOs.Account
{
    public class UpdateUserDTO
    {
        public string userId { get; set; } 
        public string NewPassword { get; set; }
        public string Role { get; set; }

    }
}
