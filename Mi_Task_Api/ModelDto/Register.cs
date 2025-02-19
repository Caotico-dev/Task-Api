using System.ComponentModel.DataAnnotations;

namespace Mi_Task_Api.ModelDto
{
    public class Register
    {
        [Required]
        [MinLength(3)]
        public string Name { get; set; } = null!;
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; } = null!;
        [Required]
        [MinLength(8)]
        public string Password { get; set; } = null!;   
    }
}
