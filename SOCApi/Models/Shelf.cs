using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOCApi.Models
{
    public class Shelf
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Location")]
        public int LocationId { get; set; }

        [Required]
        [MaxLength(100)]
        public string ShelfName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(20)]
        public string? ShelfCode { get; set; } // e.g., "A1-01", "B2-TOP"

        public int? MaxCapacity { get; set; } // Optional: maximum number of books

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Location Location { get; set; } = null!;

        public virtual ICollection<Book> Books { get; set; } = new List<Book>();

        // Computed properties
        [NotMapped]
        public int BookCount => Books?.Count ?? 0;

        [NotMapped]
        public bool IsFull => MaxCapacity.HasValue && BookCount >= MaxCapacity.Value;

        [NotMapped]
        public int AvailableSpace => MaxCapacity.HasValue ? Math.Max(0, MaxCapacity.Value - BookCount) : int.MaxValue;

        [NotMapped]
        public decimal CapacityUtilization => MaxCapacity.HasValue && MaxCapacity.Value > 0 
            ? (decimal)BookCount / MaxCapacity.Value * 100 
            : 0;

        [NotMapped]
        public string FullShelfName => !string.IsNullOrEmpty(ShelfCode) 
            ? $"{ShelfName} ({ShelfCode})" 
            : ShelfName;

        // Constructors
        public Shelf() { }

        public Shelf(int locationId, string shelfName, string? description = null, string? shelfCode = null, int? maxCapacity = null)
        {
            LocationId = locationId;
            ShelfName = shelfName;
            Description = description;
            ShelfCode = shelfCode;
            MaxCapacity = maxCapacity;
            Books = new List<Book>();
        }

        // Helper methods
        public void UpdateTimestamp()
        {
            UpdatedAt = DateTime.UtcNow;
        }

        public bool HasBooks()
        {
            return Books?.Any() == true;
        }

        public bool CanAddBook()
        {
            return IsActive && (!MaxCapacity.HasValue || BookCount < MaxCapacity.Value);
        }

        public bool CanAddBooks(int count)
        {
            return IsActive && (!MaxCapacity.HasValue || (BookCount + count) <= MaxCapacity.Value);
        }

        public IEnumerable<Book> GetBooksByTitle(string title)
        {
            return Books?.Where(b => b.Title.Contains(title, StringComparison.OrdinalIgnoreCase)) ?? new List<Book>();
        }

        public IEnumerable<Book> GetBooksByAuthor(string author)
        {
            return Books?.Where(b => b.Author.Contains(author, StringComparison.OrdinalIgnoreCase)) ?? new List<Book>();
        }

        public Book? FindBookByIsbn(string isbn)
        {
            return Books?.FirstOrDefault(b => b.ISBN == isbn);
        }
    }
}