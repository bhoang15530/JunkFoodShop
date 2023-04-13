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

        #region Category
        public async Task<IActionResult> Index(int page = 1)
        {
            var CategoryList = await _context.FoodCategories.ToListAsync();
            var FoodList = await _context.FoodCategories.ToListAsync();
            if (CategoryList == null)
            {
                return NotFound();
            }

            // Set page size
            const int pageSize = 6;
            // Get total food count
            var totalCategory = await _context.FoodCategories.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCategory / pageSize);
            // Get paginated food list
            var paginatedCategory = await _context.FoodCategories.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.CategoryList = paginatedCategory;

            //ViewBag.FoodList = FoodList;

            ViewBag.NotFound = TempData["NotFound"]?.ToString();

            return View();
        }
        #endregion

        #region Category Details
        public async Task<IActionResult> Details(int? cid, int page = 1)
        {
            if (cid == null)
            {
                TempData["NotFound"] = "Category not found";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.CategoryInfo = _context.FoodCategories.FirstOrDefault(x => x.Categoryid == cid);

            var FoodListByCategory = (from food in _context.Foods
                                            join foodCategory in _context.FoodCategories on food.CategoryId equals foodCategory.Categoryid
                                            select new
                                            {
                                                food.FoodId,
                                                food.FoodImage,
                                                food.FoodName,
                                                food.FoodPrice,
                                                foodCategory.Categoryid,
                                                foodCategory.CategoryName
                                            }).Where(x => x.Categoryid == cid);

            // Set page size
            const int pageSize = 6;
            // Get total food count
            var totalFoodListByCategory = FoodListByCategory.Count();
            var totalPages = (int)Math.Ceiling((double)totalFoodListByCategory / pageSize);
            // Get paginated food list
            var paginatedFoodListByCategory = await FoodListByCategory.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.FoodListByCategory = paginatedFoodListByCategory;
            return View();
        }
        #endregion
    }
}
