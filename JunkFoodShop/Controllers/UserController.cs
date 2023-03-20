using JunkFoodShop.Data;
using JunkFoodShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JunkFoodShop.Controllers
{
    public class UserController : Controller
    {
        private readonly JunkFoodShopContext _context;

        public UserController(JunkFoodShopContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> AccountSetting(string Username)
        {
            // Get UserData by UserId
            var UserData = await (from user in _context.UserAccounts select new UserSetting
                                {
                                    Username = user.Username,
                                    FullName = user.FullName,
                                    PhoneNumber = user.PhoneNumber,
                                    Email = user.Email,
                                }).Where(x => x.Username == Username).FirstOrDefaultAsync();

            ViewBag.UserData = UserData;
            return View();
        }

    }
}
