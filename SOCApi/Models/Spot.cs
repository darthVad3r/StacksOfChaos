namespace SOCApi.Models
{
    public class Spot
    {
        public int Id { get; set; }
        public string? SpotName { get; set; }
        public List<Book>? Books { get; set; }
    }
}
