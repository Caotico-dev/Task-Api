using System.ComponentModel.DataAnnotations;

namespace Mi_Task_Api.ModelDto
{
    public class Login
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; } = null!;
        [Required]
        [MinLength(8, ErrorMessage = "The password has to have a size of 8")]
        public string Password { get; set; } = null!;
    }
}
