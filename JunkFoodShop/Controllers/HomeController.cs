using JunkFoodShop.Data;
using JunkFoodShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace JunkFoodShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly JunkFoodShopContext _context;

        public HomeController(JunkFoodShopContext context)
        {
            _context = context;
        }

        #region HOME
        // Home Page
        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.Identity.Name == "Admin")
                {
                    return RedirectToAction("Index", "Admin");
                }
            }

            // Display 9 random food
            var FoodList = await _context.Foods.Select(f => new Food
            {
                FoodId = f.FoodId,
                FoodImage = f.FoodImage,
                FoodName = f.FoodName,
                FoodPrice = f.FoodPrice,
            }).OrderBy(x => Guid.NewGuid()).Take(9).ToListAsync();

            // Display all category
            var CategoryList = await _context.FoodCategories.Select(c => new FoodCategory
            {
                Categoryid = c.Categoryid,
                CategoryImage = c.CategoryImage,
                CategoryName = c.CategoryName,
            }).ToListAsync();

            ViewBag.FoodList = FoodList;
            ViewBag.CategoryList = CategoryList;
            return View();
        }
        #endregion

        #region List all categories
        // Display all category 
        public async Task<IActionResult> AllFoodCategories()
        {
            // Get all Categories
            var CategoryList = await _context.FoodCategories.Select(c => new FoodCategory
            {
                Categoryid = c.Categoryid,
                CategoryImage = c.CategoryImage,
                CategoryName = c.CategoryName,
            }).ToListAsync();

            if (CategoryList == null)
            {
                return NoContent();
            }

            ViewBag.CategoryList = CategoryList;
            return View();
        }
        #endregion

        // Display Privacy, Contact, AboutUs
        #region Privacy/ Contact/ AboutUs
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult AboutUs()
        {
            return View();
        }
        #endregion

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}