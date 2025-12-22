using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SOCApi.Models
{
    public class Role : IdentityRole
    {
        [MaxLength(500)]
        public string? Description { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public bool IsActive { get; set; } = true;
    }
}
