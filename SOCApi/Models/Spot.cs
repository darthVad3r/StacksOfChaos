namespace SOCApi.Models
{
    public class Spot
    {
        public int Id { get; set; }
        public string? SpotName { get; set; }
        public List<Title>? Titles { get; set; }
    }
}
