namespace SOCApi.Models
{
    public class Title
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Author { get; set; }
        public Spot? spot { get; set; }
    }
}
