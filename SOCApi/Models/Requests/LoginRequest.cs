using System.ComponentModel.DataAnnotations;

namespace SOCApi.Models.Requests
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public required string Password { get; set; }

        public bool RememberMe { get; set; } = false;
    }
}
