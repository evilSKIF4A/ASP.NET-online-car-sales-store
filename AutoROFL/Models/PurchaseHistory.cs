using System;

namespace AutoROFL.Models
{
    public class PurchaseHistory
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public string UserId { get; set; }
        public DateTime PurchaseDate { get; set; }
    }
}
