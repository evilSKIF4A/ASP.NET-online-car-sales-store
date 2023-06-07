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
    public class BasketController : Controller
    {
        ARDbContext db;

        public BasketController(ARDbContext context)
        {
            db = context;
        }

        [HttpGet]
        public ActionResult Basket()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            List<Basket> basket = db.Baskets.Where(x => x.UserId == userId).Where(y => y.isBuy == false).ToList();
            BasketCarViewModel obj = new BasketCarViewModel();
            obj.Baskets = basket;
            obj.Cars = db.Cars;
            return View(obj);
        }

        [HttpPost]
        public async Task<ActionResult> Basket(int carId)
        {
            // Добавление в корзину
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            db.Baskets.Add(new Basket { UserId = userId, CarId = carId, isBuy = false });
            await db.SaveChangesAsync();
            List<Basket> basket = db.Baskets.Where(x => x.UserId == userId).Where(y => y.isBuy == false).ToList();
            BasketCarViewModel obj = new BasketCarViewModel();
            obj.Baskets = basket;
            obj.Cars = db.Cars;
            return View(obj);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int basketId)
        {
            List<Basket> basket = db.Baskets.Where(x => x.Id == basketId).ToList();
            if(basket.Count != 0)
            {
                db.Baskets.Remove(basket[0]);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Basket", "Basket");
        }

        [HttpPost]
        public async Task<ActionResult> Payment(int basketId)
        {
            List<Basket> basket = db.Baskets.Where(x => x.Id == basketId).ToList();
            if (basket.Count != 0)
            {
                List<Car> car = db.Cars.Where(x => x.Id == basket[0].CarId).ToList();
                car[0].Amount -= 1;
                db.Cars.Update(car[0]);

                basket[0].isBuy = true;
                db.Baskets.Update(basket[0]);
                db.PurchaseHistories.Add(new PurchaseHistory { CarId = car[0].Id, UserId = basket[0].UserId, PurchaseDate = DateTime.Now });
                await db.SaveChangesAsync();
            }
            return View();
        }
    }
}
