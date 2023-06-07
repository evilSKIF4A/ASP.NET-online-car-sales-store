using AutoROFL.Models;
using System.Collections.Generic;

namespace AutoROFL.ViewModels
{
    public class ListCarsViewModel
    {
        public bool isOneBrand { get; set; }
        public string userId { get; set; } // используется только в AdminPanel
        public IEnumerable<Car> Cars { get; set; }
        public PageViewModel PageViewModel { get; set; }
    }
}
