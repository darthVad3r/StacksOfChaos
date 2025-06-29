using System.ComponentModel.DataAnnotations;

namespace SOCApi.Models
{
    public class Email
    {
        public int Id { get; set; }
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public User User { get; set; } = null!;
    }
}
