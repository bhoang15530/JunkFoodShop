﻿using System.Drawing.Printing;
using JunkFoodShop.Data;
using JunkFoodShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;

namespace JunkFoodShop.Controllers
{
    public class FoodsController : Controller
    {
        private readonly JunkFoodShopContext _context;
        public FoodsController(JunkFoodShopContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            // Set page size
            const int pageSize = 15;
            // Get total food count
            var totalFood = await _context.Foods.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalFood / pageSize);
            // Get paginated food list
            var paginatedFood = await _context.Foods.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.FoodList = paginatedFood;

            ViewBag.NotFound = TempData["NotFound"]?.ToString();
            return View();
        }

        #region Get Food Details and comment
        // Get Food Details by FoodId
        public async Task<IActionResult> Details(int? foodId)
        {
            if (foodId == null)
            {
                TempData["NotFound"] = "Food not found";
                return RedirectToAction(nameof(Index));
            }

            var commentList = await _context.Comments.Where(x => x.FoodId == foodId).ToListAsync();

            var FoodDetails = await _context.Foods.Include(x => x.Category).Where(x => x.FoodId == foodId).FirstOrDefaultAsync();
            if (FoodDetails == null)
            {
                TempData["NotFound"] = "Food not found";
                return RedirectToAction(nameof(Index));
            }

            var RandomFoodList = _context.Foods.OrderBy(x => Guid.NewGuid()).Take(3).ToList();

            if (User.Identity!.IsAuthenticated)
            {
                var userId = _context.UserAccounts.Where(x => x.Username == User.Identity.Name || x.Email == User.Identity.Name ).FirstOrDefault()!.UserId;

                bool isUserRated = _context.Ratings.Where(x => x.UserId == userId && x.FoodId == foodId).Any();
                bool isUserCommented = _context.Comments.Where(x => x.UserId == userId && x.FoodId == foodId).Any();

                var order = _context.OrderFoods.Where(x => x.UserId == userId && x.FoodId == foodId).FirstOrDefault();

                // Check if the user bought & the order status is set to "Delivery Complete", then we will let user rate the product
                if (order == null)
                {
                    ViewBag.IsBoughtProduct = false;
                }
                else
                {
                    var orderStatus = _context.Orders.Find(order.OrderId).StatusId;
                    if (orderStatus == 3)
                    {
                        ViewBag.IsBoughtProduct = true;
                    }
                    else
                    {
                        ViewBag.IsBoughtProduct = false;
                    }
                }

                // If the user is already rated the product, they will not able to rate it again
                if (isUserRated || isUserCommented)
                {
                    ViewBag.IsRatedOrCommented = true;
                }
                else
                {
                    ViewBag.IsRatedOrCommented = false;
                }
            }

            var StarList = _context.Ratings.Where(x => x.FoodId == foodId).ToList();

            var CommentList = await (from comment in _context.Comments
                                     join user in _context.UserAccounts on comment.UserId equals user.UserId
                                     join rating in _context.Ratings on user.UserId equals rating.UserId
                                     where comment.FoodId == foodId
                                     where rating.FoodId == foodId
                                     select new
                                     {
                                         user.FullName,
                                         user.UserId,
                                         comment.DateComment,
                                         comment.Content,
                                         rating.Star
                                     }).ToListAsync();

            // Check if any users rate the product yet
            if (!_context.Ratings.Where(x => x.FoodId == foodId).Any())
            {
                ViewBag.StarRatingAvg = null;
            }
            else
            {
                var starRatingAvg = _context.Ratings.Where(x => x.FoodId == foodId).Average(x => x.Star);
                ViewBag.StarRatingAvg = Math.Floor(starRatingAvg);
            }

            ViewBag.FoodDetails = FoodDetails;
            ViewBag.RandomFoodList = RandomFoodList;
            ViewBag.CommentList = CommentList;

            return View();
        }
        #endregion

        #region Searching
        public IActionResult SearchFood(string keyword)
        {

            var FoodData = from f in _context.Foods select f;

            if (!String.IsNullOrEmpty(keyword))
            {
                ViewBag.FoodList = FoodData.Where(f => f.FoodName.Contains(keyword)).ToList();
                ViewBag.Keyword = keyword;
            }
            else
            {
                ViewBag.FoodList = FoodData.OrderBy(x => Guid.NewGuid()).ToList();
            }
            return View();
        }
        #endregion
    }
}
