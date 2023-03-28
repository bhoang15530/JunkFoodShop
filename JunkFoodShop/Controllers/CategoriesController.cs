using JunkFoodShop.Data;
using JunkFoodShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JunkFoodShop.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly JunkFoodShopContext _context;

        public CategoriesController(JunkFoodShopContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var CategoryList = await _context.FoodCategories.ToListAsync();
            var FoodList = await _context.FoodCategories.ToListAsync();
            if (CategoryList == null)
            {
                return NotFound();
            }
            ViewBag.FoodList = FoodList;
            ViewBag.CategoryList = CategoryList;


            return View();
        }
        public async Task<IActionResult> Details(int? cid)
        {
            if (cid == null)
            {
                return NotFound();
            }
            ViewBag.CategoryName = _context.FoodCategories.FirstOrDefault(x => x.Categoryid == cid).CategoryName;

            var FoodListByCategory = await (from food in _context.Foods
                                            join foodCategory in _context.FoodCategories on food.CategoryId equals foodCategory.Categoryid
                                            select new FoodManage
                                            {
                                                FoodId = food.FoodId,
                                                FoodImage = food.FoodImage,
                                                FoodName = food.FoodName,
                                                FoodPrice = food.FoodPrice,
                                                Categoryid = foodCategory.Categoryid,
                                                CategoryName = foodCategory.CategoryName
                                            }).Where(x => x.Categoryid == cid).ToListAsync();
            ViewBag.FoodListByCategory = FoodListByCategory;
            return View();
        }
    }
}
