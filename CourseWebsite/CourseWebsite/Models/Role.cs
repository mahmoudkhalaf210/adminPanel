using System.ComponentModel.DataAnnotations;

namespace HospAPI.Models
{
    public class Role
    {
        [Key]
        public string Roleid { get; set; }
        [Required]
        public string RoleName { get; set; }

    }
}
