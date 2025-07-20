using Newtonsoft.Json;

namespace SOCApi.Models
{
    public class Book
    {
        public int? Id { get; set; }
        public string? Title { get; set; }
        public Author? Author { get; set; }
        public string? Genre { get; set; }
        public string? Description { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? ISBN { get; set; }
        public DateTime? PublishedDate { get; set; }
        public int? PageCount { get; set; }
        public string? Language { get; set; }
        public string? Publisher { get; set; }
        public string? Format { get; set; } // e.g., Hardcover, Paperback, eBook
        public string? Edition { get; set; } // e.g., First Edition, Revised Edition
        public string? Tags { get; set; } // Comma-separated tags for easy searching

    }

}
