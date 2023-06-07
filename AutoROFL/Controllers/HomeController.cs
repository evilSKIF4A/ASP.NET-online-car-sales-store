using AutoROFL.Models;
using AutoROFL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AutoROFL.Controllers
{
    public class HomeController : Controller
    {
        ARDbContext db;
        public HomeController(ARDbContext context)
        {
            db = context;
        }

        public IActionResult Index()
        {
            IEnumerable<Car> cars = db.Cars.Where(isF => isF.isFavorit == true);
            return View(cars);
        }

        public IActionResult Snake()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> ListCars(int page = 1, string brand = "", string category = "Не указано", string amount = "Не указано", int price = 0)
        {
            int pageSize = 3;   // количество элементов на странице
            bool isOneBrand = true;

            IQueryable<Car> source = db.Cars;
            if (brand != "" && brand.Trim() != "Не указано")
                source = source.Where(x => x.Brand.Trim() == brand.Trim()).AsQueryable();
            else
                isOneBrand = false;
            if (category.Trim() != "Не указано")
                source = source.Where(x => x.Category.Trim() == category.Trim()).AsQueryable();
            if (amount.Trim() != "Не указано")
            {
                if(amount.Trim() == "Есть в наличии")
                    source = source.Where(x => x.Amount > 0).AsQueryable();
                else
                    source = source.Where(x => x.Amount == 0).AsQueryable();
            }
            if (price != 0)
                source = source.Where(x => x.Price <= price).AsQueryable();

            ListCarsViewModel viewModel;
            if (isOneBrand)
            {
                var count = await source.CountAsync();
                var items = await source.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
                PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);
                viewModel = new ListCarsViewModel
                {
                    PageViewModel = pageViewModel,
                    Cars = items,
                    isOneBrand = isOneBrand

                };
            }
            else
            {
                viewModel = new ListCarsViewModel
                {
                    Cars = source,
                    isOneBrand = isOneBrand
                };
            }
            
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult FullInfoCar(string carIdandIsOneBrand)
        {
            string[] carIdandIsOB = carIdandIsOneBrand.Split(' ');
            int carId = int.Parse(carIdandIsOB[0]);
            bool isOneBrand = bool.Parse(carIdandIsOB[1]);
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            BasketCarViewModel obj = new BasketCarViewModel();
            obj.Cars = db.Cars.Where(x => x.Id == carId);
            obj.Baskets = db.Baskets.Where(y => y.CarId == carId).Where(t => t.UserId == userId).Where(g => g.isBuy == false).ToList();
            obj.isOneBrand = isOneBrand;
            return View(obj);
        }

        public ActionResult Search() => View();

    }
}
