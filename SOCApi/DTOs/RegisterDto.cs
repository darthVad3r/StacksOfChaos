using System.ComponentModel.DataAnnotations;

namespace SOCApi.DTOs
{
    public class RegisterDto 
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;
        
        [MaxLength(1000)]
        public string? Bio { get; set; }
    }
}
