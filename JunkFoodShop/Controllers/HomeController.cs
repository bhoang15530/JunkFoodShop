using JunkFoodShop.Data;
using JunkFoodShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace JunkFoodShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly JunkFoodShopContext _context;

        public HomeController(ILogger<HomeController> logger, JunkFoodShopContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.Identity.Name == "Admin")
                {
                    return RedirectToAction("Index", "Admin");
                }
            }

            // Only take 8 as it'll slow the Homepage as the Food table is too large
            var FoodList = await _context.Foods.Select(f => new Food
            {
                FoodId = f.FoodId,
                FoodImage = f.FoodImage,
                FoodName = f.FoodName,
                FoodPrice = f.FoodPrice,
                FoodDescription = f.FoodDescription,
            }).Take(8).ToListAsync();

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

        // Get All Food
        public async Task<IActionResult> AllFoods()
        {
            // Get all Food
            var FoodList = await _context.Foods.Select(f => new Food
            {
                FoodId = f.FoodId,
                FoodImage = f.FoodImage,
                FoodName = f.FoodName,
                FoodPrice = f.FoodPrice,
                FoodDescription = f.FoodDescription,
            }).ToListAsync();

            if (FoodList == null)
            {
                return NoContent();
            }
            ViewBag.FoodList = FoodList;
            return View();
        }

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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}