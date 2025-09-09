using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SOCApi.Models
{
    public class User : IdentityUser
    {

        [MaxLength(50)]
        public string? FirstName { get; set; }

        [MaxLength(50)]
        public string? LastName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastLogin { get; set; }

        [MaxLength(500)]
        public string? ProfilePictureUrl { get; set; }

        [MaxLength(1000)]
        public string? Bio { get; set; }

        // Navigation properties
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

        // Helper property for full name
        public string FullName => $"{FirstName} {LastName}".Trim();
        public List<Book> MyBooks { get; set; } = new List<Book>();
    }
}