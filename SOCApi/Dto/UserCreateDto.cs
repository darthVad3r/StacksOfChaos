using System.ComponentModel.DataAnnotations;
namespace SOCApi.Dto
{
    public class UserCreateDto
    {
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Email { get; set; }
        // Add other fields as needed
    }
}
