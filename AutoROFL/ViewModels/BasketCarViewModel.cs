using AutoROFL.Models;
using System.Collections.Generic;

namespace AutoROFL.ViewModels
{
    public class BasketCarViewModel
    {
        public List<Basket> Baskets { get; set; }
        public IEnumerable<Car> Cars { get; set; }
        public bool isOneBrand { get; set; }
    }
}
