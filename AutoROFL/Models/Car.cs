namespace AutoROFL.Models
{
    public class Car
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Img { get; set; }
        public uint Price { get; set; }
        public uint Amount { get; set; }
        public string ShortDesc { get; set; }
        public string LongDesc { get; set; }
        public bool isFavorit { get; set; }
    }
}
