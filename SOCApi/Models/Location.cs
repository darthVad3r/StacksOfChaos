using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOCApi.Models
{
    public class Location
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Library")]
        public int LibraryId { get; set; }

        [Required(ErrorMessage = "Location name is required.")]
        [MaxLength(100, ErrorMessage = "Location name cannot exceed 100 characters.")]
        public string LocationName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Location description is required.")]
        [MaxLength(500, ErrorMessage = "Location description cannot exceed 500 characters.")]
        public string LocationDescription { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Library Library { get; set; }

        public ICollection<Shelf> Shelves { get; set; } = new List<Shelf>();

        [NotMapped]
        public int ShelfCount => Shelves?.Count ?? 0;

        [NotMapped]
        public int BookCount => Shelves?.SelectMany(shelf => shelf.Books ?? new List<Book>()).Count() ?? 0;
    
        public Location() {}

        public Location(int libraryId, string locationName, string locationDescription)
        {
            LibraryId = libraryId;
            LocationName = locationName;
            LocationDescription = locationDescription;
            Shelves = new List<Shelf>();
        }

        public void UpdateTimeStamp()
        {
            UpdatedAt = DateTime.UtcNow;
        }

        public bool HasBooks()
        {
            return Shelves?.Any(shelf => shelf.Books?.Any() == true) == true;
        }

        public IEnumerable<Book> GetBooks()
        {
            return Shelves?.SelectMany(shelf => shelf.Books ?? new List<Book>()) ?? Enumerable.Empty<Book>();
        }
    }
}