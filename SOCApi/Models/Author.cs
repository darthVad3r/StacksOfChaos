namespace SOCApi.Models
{
    public class Author
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public List<Title>? Titles { get; set; }
    }
}
