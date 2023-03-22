using JunkFoodShop.Data;
using JunkFoodShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JunkFoodShop.Controllers
{
    public class FoodsController : Controller
    {
        private readonly JunkFoodShopContext _context;
        public FoodsController(JunkFoodShopContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        // Get Food Details by FoodId
        public async Task<IActionResult> Details(int? foodId)
        {
            if (foodId == null)
            {
                return NotFound();
            }

            var commentList = await _context.Comments.Where(x => x.FoodId == foodId).ToListAsync();

            var FoodDetails = await (from food in _context.Foods
                                     join rating in _context.Ratings on food.FoodId equals rating.FoodId
                                     join comment in _context.Comments on food.FoodId equals comment.FoodId
                                     join category in _context.FoodCategories on food.CategoryId equals category.Categoryid
                                     select new FoodDetails
                                     {
                                         Comments = commentList,
                                         FoodDescription = food.FoodDescription,
                                         FoodImage = food.FoodImage,
                                         FoodId = food.FoodId,
                                         FoodName = food.FoodName,
                                         FoodPrice = food.FoodPrice,
                                         FoodStock = food.FoodStock,
                                         Star = rating.Star,
                                         CommentTime = comment.DateComment,
                                         CategoryId = category.Categoryid
                                     }).Where(f => f.FoodId == foodId).FirstOrDefaultAsync();
            if (FoodDetails == null)
            {
                return NotFound();
            }

            ViewBag.FoodDetails = FoodDetails;

            return View();
        }
    }
}
