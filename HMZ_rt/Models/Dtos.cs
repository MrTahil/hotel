using System.ComponentModel.DataAnnotations;
namespace HMZ_rt.Models
{
    public class CreateUserDto
    {
        [Required(ErrorMessage = "Username is required.")]
        [MinLength(5, ErrorMessage = "Username must be at least 5 characters long.")]
        [MaxLength(20, ErrorMessage = "Username cannot exceed 20 characters.")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$", ErrorMessage = "Password must include at least one uppercase letter, one lowercase letter, and one number.")]
        public string? Password { get; set; }
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }
        public string? Status { get; set; }
        public string? Notes { get; set; }









    }


        //(string? Username, string? Password, string? Email, string? Status, string? Notes);
    }
