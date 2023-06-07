using AutoROFL.Models;
using AutoROFL.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AutoROFL.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger _logger;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        ARDbContext db;
        public UserController(ILogger<HomeController> logger, ARDbContext context, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _logger = logger;
            db = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public ActionResult Profile()
        {
            ProfileViewModel obj = new ProfileViewModel();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            obj.User = db.Users.Where(x => x.Id == userId).ToList()[0];
            obj.UserRole = db.UserRoles.Where(x => x.UserId == userId).ToList()[0].RoleId;
            return View(obj);
        }

        [HttpPost]
        public async Task<ActionResult> EditProfile(string FName = "", string SName = "", string MName = "", string BirthDay = "", string Adress = "", string PhoneNumber = "")
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User user = db.Users.Where(x => x.Id == userId).ToList()[0];
            user.FName = FName;
            user.SName = SName;
            user.MName = MName;
            user.Year = DateTime.ParseExact(BirthDay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            user.Adress = Adress;
            user.PhoneNumber = PhoneNumber;
            db.Users.Update(user);
            await db.SaveChangesAsync();
            return RedirectToAction("Profile", "User");
        }

        public ActionResult PurchaseHistory(int page = 1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            List<PurchaseHistory> PH = db.PurchaseHistories.Where(x => x.UserId == userId).ToList();
            List<Car> cars = new List<Car>();

            for (int i = 0; i < PH.Count; i++)
                cars.Add(db.Cars.Where(x => x.Id == PH[i].CarId).ToList()[0] as Car);

            int pageSize = 2;   // количество элементов на странице

            List<Car> source = cars;
            var count = source.Count();
            var items = source.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);
            ListCarsViewModel viewModel = new ListCarsViewModel
            {
                PageViewModel = pageViewModel,
                Cars = items
            };

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Email()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            FreeEmailViewModel obj = new FreeEmailViewModel();
            obj.Email = db.Users.Where(x => x.Id == userId).ToList()[0].Email;
            obj.isChangeEmail = false;
            obj.isFreeEmail = false;
            return View(obj);
        }

        [HttpPost]
        public ActionResult Email(string email)
        {
            // Изменение Email'a
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            FreeEmailViewModel obj = new FreeEmailViewModel();
            obj.isChangeEmail = true;
            obj.Email = db.Users.Where(x => x.Id == userId).ToList()[0].Email;
            if (!string.IsNullOrEmpty(email) && new EmailAddressAttribute().IsValid(email))
            {
                // Если Email валидный
                List<User> user = db.Users.Where(x => x.UserName.Trim() == email.Trim()).ToList();
                if(user.Count != 0)
                {
                    // Если Email уже занят
                    obj.isFreeEmail = false;
                    return View(obj);
                }
                user = db.Users.Where(x => x.Id == userId).ToList();
                obj.Email = user[0].UserName = user[0].Email = email.Trim();
                user[0].NormalizedUserName = user[0].NormalizedEmail = email.Trim().ToUpper();
                db.Users.Update(user[0]);
                db.SaveChanges();
                obj.isFreeEmail = true;
                return View(obj);
            }
            else
            {
                obj.isFreeEmail = false;
                return View(obj);
            }
        }

        [HttpGet]
        public ActionResult Password()
        {
            ChangePasswordViewModel obj = new ChangePasswordViewModel();
            obj.isChangePassword = false;
            obj.isValidPassword = false;
            return View(obj);
        }

        [HttpPost]
        public async Task<IActionResult> Password(ChangePasswordViewModel model)
        {
            // Изменение пароля
            model.isChangePassword = true;
            model.isValidPassword = false;
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                User user = db.Users.Where(x => x.Id == userId).ToList()[0];
                if (user != null)
                {
                    var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        model.isValidPassword = true;
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult DeleteAccount()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> AccountDelete()
        {
            var user = await _userManager.GetUserAsync(User);

            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out prior to account deletion.");

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

            return RedirectToAction("Index", "Home");
        }
    }
}
