using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SOCApi.Models.Enums;

namespace SOCApi.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Author { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Genre { get; set; }

        [Range(1000, 9999, ErrorMessage = "Year must be between 1000 and 9999")]
        public DateTime? YearPublished { get; set; }

        [MaxLength(17)] // ISBN-13 format: 978-0-123456-78-9
        public string? ISBN { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(500)]
        public string? CoverImageUrl { get; set; }

        [MaxLength(100)]
        public string? Publisher { get; set; }

        [Range(1, int.MaxValue)]
        public int? PageCount { get; set; }

        [MaxLength(10)]
        public string? Language { get; set; } = "English";

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Price { get; set; }

        public DateTime? PurchaseDate { get; set; }

        public BookCondition Condition { get; set; } = BookCondition.Good;

        public BookStatus Status { get; set; } = BookStatus.Available;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        // Foreign Keys
        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("Shelf")]
        public int? ShelfId { get; set; }

        // Navigation properties
        public virtual User User { get; set; } = null!;
        public virtual Shelf? Shelf { get; set; }

        // Constructors only - NO business logic
        public Book() 
        {
            var now = DateTime.UtcNow;
            CreatedAt = now;
            UpdatedAt = now;
        }

        public Book(string title, string author, string userId) : this()
        {
            Title = title;
            Author = author;
            UserId = userId;
        }

        public Book(string title, string author, string userId, string? isbn = null, string? genre = null, DateTime? yearPublished = null) : this()
        {
            Title = title;
            Author = author;
            UserId = userId;
            ISBN = isbn;
            Genre = genre;
            YearPublished = yearPublished;
        }
    }
}