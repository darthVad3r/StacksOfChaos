
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOCApi.Models
{
    public class Library
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        [ForeignKey("User")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Library Name is required.")]
        [MaxLength(100)]
        public string LibraryName { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsPublic { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual User User { get; set; } = null!;

        public virtual ICollection<Location> Locations { get; set; } = new List<Location>();

        [NotMapped]
        public int LocationCount => Locations?.Count ?? 0;

        public Library() { }

        public Library(string userId, string libraryName, string? description = null)
        {
            UserId = userId;
            LibraryName = libraryName;
            Description = description;
            Locations = new List<Location>();
        }
    }
}