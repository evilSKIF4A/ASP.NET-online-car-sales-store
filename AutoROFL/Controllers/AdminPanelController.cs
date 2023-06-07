using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoROFL.ViewModels;
using AutoROFL.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AutoROFL.Controllers
{
    public class AdminPanelController : Controller
    {
        ARDbContext db;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public AdminPanelController(ARDbContext context, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            db = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public ActionResult AdminPanelUsers()
        {
            List<ProfileViewModel> users = new List<ProfileViewModel>();
            foreach(var x in db.Users)
            {
                if (x.UserName != "admin@mail.ru")
                    users.Add(new ProfileViewModel { User = x, UserRole = db.UserRoles.Where(y => y.UserId == x.Id).ToList()[0].RoleId });
            }
            return View(users);
        }

        [HttpPost]
        public ActionResult AdminPanelUsers(string FName = "", string Email = "", string PhoneNumber = "Не указан")
        {
            var users = db.Users.AsEnumerable();
            if (!string.IsNullOrEmpty(FName))
                users = users.Where(x => x.FName.Contains(FName));
            if (!string.IsNullOrEmpty(Email))
                users = users.Where(x => x.UserName.Contains(Email));
            if (PhoneNumber != "Не указан" && !string.IsNullOrEmpty(PhoneNumber))
                users = users.Where(x => x.PhoneNumber == PhoneNumber);

            List<ProfileViewModel> users_roles = new List<ProfileViewModel>();
            foreach (var x in users)
            {
                if (x.UserName != "admin@mail.ru")
                    users_roles.Add(new ProfileViewModel { User = x, UserRole = db.UserRoles.Where(y => y.UserId == x.Id).ToList()[0].RoleId });
            }

            return View(users_roles);
        }

        [HttpGet]
        public async Task<ActionResult> AdminPanelCar(int page = 1)
        {
            int pageSize = 10;   // количество элементов на странице
            IQueryable<Car> source = db.Cars;
            var count = await source.CountAsync();
            var items = await source.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);
            ListCarsViewModel viewModel = new ListCarsViewModel
            {
                PageViewModel = pageViewModel,
                Cars = items.OrderBy(p => p.Brand)
            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AdminPanelCar(int page = 1, string Brand = "", int Price = 0)
        {
            IQueryable<Car> source = db.Cars;
            if(!string.IsNullOrEmpty(Brand))
                source = source.Where(x => x.Brand == Brand);
            if(Price != 0)
                source = source.Where(x => x.Price <= Price);
            PageViewModel pageViewModel = new PageViewModel(0, 0, 1); // убрать навигацию
            ListCarsViewModel viewModel = new ListCarsViewModel
            {
                PageViewModel = pageViewModel,
                Cars = source.OrderBy(p => p.Brand)
            };
            return View(viewModel);
        }

        [HttpGet]
        public ActionResult EditInfoCar(int carId)
        {
            Car car = db.Cars.Where(x => x.Id == carId).ToList()[0];
            return View(car);
        }

        [HttpPost]
        public async Task<ActionResult> EditInfoCar(Car car)
        {
            db.Cars.Update(car);
            await db.SaveChangesAsync();
            return View(car);
        }

        [HttpGet]
        public ActionResult LoadCar() => View();

        [HttpPost]
        public async Task<ActionResult> LoadCar(Car car)
        {
            await db.Cars.AddAsync(car);
            await db.SaveChangesAsync();
            return RedirectToAction("AdminPanelCar", "AdminPanel");
        }

        [HttpGet]
        public async Task<ActionResult> DeleteCar(int carId)
        {
            var car = db.Cars.Where(x => x.Id == carId).ToList()[0];
            db.Cars.Remove(car);
            await db.SaveChangesAsync();
            return RedirectToAction("AdminPanelCar", "AdminPanel");
        }

        [HttpPost]
        public ActionResult AdminPanelPurchaseHistory(string userId, int page = 1)
        {
            List<PurchaseHistory> PH = db.PurchaseHistories.Where(x => x.UserId == userId).ToList();
            List<Car> cars = new List<Car>();

            for (int i = 0; i < PH.Count; i++)
                cars.Add(db.Cars.Where(x => x.Id == PH[i].CarId).ToList()[0] as Car);

            int pageSize = 3;   // количество элементов на странице

            List<Car> source = cars;
            var count = source.Count();
            var items = source.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);
            ListCarsViewModel viewModel = new ListCarsViewModel
            {
                PageViewModel = pageViewModel,
                Cars = items,
                userId = userId
            };

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult AdminPanelRemoteAccount()
        {
            List<RemoteAccount> RemoteUsers = new List<RemoteAccount>();
            foreach (var x in db.RemoteAccounts)
            {
                RemoteUsers.Add(new RemoteAccount
                {
                    Id = x.Id,
                    userId = x.userId,
                    FName = x.FName,
                    SName = x.SName,
                    MName = x.MName,
                    Year = x.Year,
                    Adress = x.Adress,
                    UserName = x.UserName,
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber
                });
            }
            return View(RemoteUsers);
        }

        [HttpPost]
        public ActionResult AdminPanelRemoteAccount(string FName = "", string Email = "", string PhoneNumber = "Не указан")
        {
            var RemoteUsers = db.RemoteAccounts.AsEnumerable();
            if (!string.IsNullOrEmpty(FName))
                RemoteUsers = RemoteUsers.Where(x => x.FName.Contains(FName));
            if (!string.IsNullOrEmpty(Email))
                RemoteUsers = RemoteUsers.Where(x => x.UserName.Contains(Email));
            if (PhoneNumber != "Не указан" && !string.IsNullOrEmpty(PhoneNumber))
                RemoteUsers = RemoteUsers.Where(x => x.PhoneNumber == PhoneNumber);

            List<RemoteAccount> RemoteAccountUsers = new List<RemoteAccount>();
            foreach (var x in RemoteUsers)
            {
                RemoteAccountUsers.Add(new RemoteAccount
                {
                    Id = x.Id,
                    userId = x.userId,
                    FName = x.FName,
                    SName = x.SName,
                    MName = x.MName,
                    Year = x.Year,
                    Adress = x.Adress,
                    UserName = x.UserName,
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber
                });
            }
            return View(RemoteAccountUsers);
        }

        [HttpPost]
        public async Task<ActionResult> AccountDelete(string userId)
        {
            var user = db.Users.Where(x => x.Id == userId).ToList()[0];

            await db.RemoteAccounts.AddAsync(new RemoteAccount
            {
                userId = user.Id,
                FName = user.FName,
                SName = user.SName,
                MName = user.MName,
                Year = user.Year,
                Adress = user.Adress,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            });

            await _userManager.DeleteAsync(user);
            if (User.Identity.Name == user.UserName)
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("AdminPanelUsers", "AdminPanel");
        }

        [HttpPost]
        public ActionResult AdminRights(string userId)
        {
            var userRole = db.UserRoles.Where(x => x.UserId == userId).ToList()[0];
            db.UserRoles.Remove(userRole);
            db.SaveChanges();
            bool isUser = true;
            if (userRole.RoleId == "1")
                isUser = false;
            var newUserRole = new IdentityUserRole<string> { UserId = userId, RoleId = isUser ? "1" : "2" };
            db.UserRoles.Add(newUserRole);
            db.SaveChanges();
            return RedirectToAction("AdminPanelUsers", "AdminPanel");
        }
    }
}
