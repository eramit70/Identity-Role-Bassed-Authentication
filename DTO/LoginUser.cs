using System.ComponentModel.DataAnnotations;

namespace RoleBassedAuthentication.DTO
{
    public class LoginUser
    {
        [Required]
        public string? Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
