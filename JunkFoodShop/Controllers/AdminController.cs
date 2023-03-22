﻿using JunkFoodShop.Data;
using JunkFoodShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JunkFoodShop.Controllers
{
    [Authorize("Admin")]
    public class AdminController : Controller
    {
        private readonly JunkFoodShopContext _context;

        public AdminController(JunkFoodShopContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region FOOD
        // GET - FOOD MANAGE
        // CODE BY HOANG
        public async Task<IActionResult> FoodManage()
        {
            // Get food data follow specify column
            var FoodData = await _context.Foods.Select(x => new FoodManage
            {
                FoodId = x.FoodId,
                FoodImage = x.FoodImage,
                FoodName = x.FoodName,
            }).ToListAsync();

            ViewBag.EditSuccess = TempData["EditSuccess"]?.ToString();
            ViewBag.DeleteSuccess = TempData["DeleteSuccess"]?.ToString();
            ViewBag.FoodCreate = TempData["FoodCreate"]?.ToString();

            // Using ViewBag to display data without Model
            ViewBag.FoodData = FoodData;
            return View();
        }

        // GET - CREATE FOOD
        // CODE BY HOANG
        public async Task<IActionResult> CreateFood()
        {
            // Get category data
            var CategoryData = await _context.FoodCategories.ToListAsync();

            // Using ViewBag to display data without Model
            ViewBag.CategoryData = CategoryData;
            return View();
        }

        // POST - CREATE FOOD
        // CODE BY HOANG
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFood([Bind("FoodName,FoodImage,FoodPrice,FoodStock,FoodDescription,CategoryId")] CreateFood createFood)
        {
            // Check FoodName exist
            bool CheckFoodName = await _context.Foods.AnyAsync(x => x.FoodName == createFood.FoodName);

            if (CheckFoodName)
            {
                ViewBag.Error = "This food name already exist";
                return View(createFood);
            }
            else
            {
                Food food = new()
                {
                    FoodName = createFood.FoodName,
                    FoodImage = createFood.FoodImage,
                    FoodPrice = createFood.FoodPrice,
                    FoodDescription = createFood.FoodDescription,
                    CategoryId = createFood.CategoryId
                };

                await _context.Foods.AddAsync(food);
                await _context.SaveChangesAsync();

                TempData["FoodCreate"] = "Create new food successfully";
                return RedirectToAction(nameof(FoodManage));
            }
        }

        // GET - FOOD EDIT
        // CODE BY HOANG
        public IActionResult FoodEdit(int fId)
        {
            // Get data of Food join Category where FoodId selected Specify Column
            var FoodData = (from foods in _context.Foods join categories in _context.FoodCategories on foods.CategoryId equals categories.Categoryid
                            select new FoodEditViewModel
                            {
                                FoodId = foods.FoodId,
                                FoodImage = foods.FoodImage,
                                FoodName = foods.FoodName,
                                FoodDescription = foods.FoodDescription,
                                FoodPrice = foods.FoodPrice,
                                FoodStock = foods.FoodStock,
                                CategoryName = categories.CategoryName,
                                CategoryId = categories.Categoryid
                            }).Where(x => x.FoodId == fId).FirstOrDefault();

            // Using ViewBag to display data without Model
            ViewBag.FoodData = FoodData;
            return View();
        }

        // POST - FOOD EDIT
        // CODE BY HOANG
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FoodEdit([Bind("FoodId,FoodName,FoodImage,FoodPrice,FoodStock,FoodDescription,CategoryId")] EditFood editFood)
        {
            Food food = new()
            {
                FoodId = editFood.FoodId,
                FoodName = editFood.FoodName,
                FoodImage = editFood.FoodImage,
                FoodPrice = editFood.FoodPrice,
                FoodDescription = editFood.FoodDescription,
                CategoryId = editFood.CategoryId
            };

            _context.Foods.Update(food);
            await _context.SaveChangesAsync();

            TempData["EditSuccess"] = "Edit successfully";
            return RedirectToAction(nameof(FoodManage));
        }

        // POST - FOOD DELETE
        // CODE BY HOANG
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FoodDelete(int fId)
        {
            // Get food by fId
            var FoodData = await _context.Foods.Where(x => x.FoodId == fId).FirstOrDefaultAsync();

            _context.Foods.Remove(FoodData);
            await _context.SaveChangesAsync();

            TempData["DeleteSuccess"] = "Delete successfully";
            return RedirectToAction(nameof(FoodManage));
        }
        #endregion

        #region CATEGORY
        // GET - CATEGORY 
        // CODE BY HOANG
        public IActionResult CategoryManage()
        {
            // Get All Category
            var CategoryData = (from categoryManage in _context.FoodCategories
                                select new CategoryManage
                                {
                                    Categoryid = categoryManage.Categoryid,
                                    CategoryImage = categoryManage.CategoryImage,
                                    CategoryName = categoryManage.CategoryName
                                }).ToListAsync();

            ViewBag.EditSuccess = TempData["EditSuccess"]?.ToString();
            ViewBag.DeleteSuccess = TempData["DeleteSuccess"]?.ToString();

            // Using ViewBag to display data without Model
            ViewBag.CategoryData = CategoryData;
            return View();
        }

        // GET - CREATE CATEGORY
        // CODE BY HOANG
        public IActionResult CreateCategory()
        {
            return View();
        }

        // POST - CREATE CATEGORY
        // CODE BY HOANG
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory([Bind("CategoryName,CategoryImage")] CreateCategory createCategory)
        {
            // Check CategoryName exist
            bool CheckCategoryName = await _context.FoodCategories.AnyAsync(x => x.CategoryName == createCategory.CategoryName);
            if (CheckCategoryName)
            {
                ViewBag.Error = "This Category Name already exist";
                return View(createCategory);
            }
            else
            {
                FoodCategory foodCategory = new()
                {
                    CategoryName = createCategory.CategoryName,
                    CategoryImage = createCategory.CategoryImage
                };
                await _context.FoodCategories.AddAsync(foodCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(CategoryManage));
            }
        }

        // GET - CATEGORY EDIT
        // CODE BY HOANG
        public async Task<IActionResult> CategoryEdit(int cid)
        {
            // Get Category by cid
            var CategoryData = await _context.FoodCategories.Where(x => x.Categoryid == cid).FirstOrDefaultAsync();
            ViewBag.Success = TempData["Success"]?.ToString();

            // Using ViewBag to display data without Model
            ViewBag.CategoryData = CategoryData;
            return View();
        }

        // POST - CATEGORY EDIT
        //CODE BY HOANG
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CategoryEdit([Bind("CategoryId,CategoryImage,CategoryName")] CategoryManage editCategory )
        {
            FoodCategory category = new()
            {
                Categoryid = editCategory.Categoryid,
                CategoryImage = editCategory.CategoryImage,
                CategoryName = editCategory.CategoryName
            };

            _context.FoodCategories.Update(category);
            await _context.SaveChangesAsync();

            TempData["EditSuccess"] = "Edit successfully";
            return RedirectToAction(nameof(CategoryEdit));
        }

        // POST - CATEGORY DELETE
        // CODE BY HOANG
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CategoryDelete(int cid)
        {
            // Get Category by cid
            var CategoryData = await _context.FoodCategories.Where(x => x.Categoryid ==cid).FirstOrDefaultAsync();

            _context.FoodCategories.Remove(CategoryData);
            await _context.SaveChangesAsync();

            TempData["DeleteSuccess"] = "Delete successfully";
            return RedirectToAction(nameof(CategoryManage));
        }
        #endregion

        #region USER-MANAGEMENT

        // GET - USER
        // CODE BY HOANG
        public IActionResult UserManage()
        {
            // Get all User
            var UserData = (from user in _context.UserAccounts 
                            select new UserManage
                            {
                                UserId = user.UserId,
                                FullName = user.FullName,
                                Email = user.Email,
                                PhoneNumber = user.PhoneNumber,
                            }).ToList();

            ViewBag.DeleteUser = TempData["DeleteUser"]?.ToString();

            // Using ViewBag to display data without Model
            ViewBag.UserData = UserData;
            return View();
        }

        // POST - DELETE USER
        // CODE BY HOANG
        [HttpPost]
        public async Task<IActionResult> UserDelete(int uid)
        {
            // Get user by uid
            var UserData = await _context.UserAccounts.Where(x => x.UserId == uid).FirstOrDefaultAsync();

            _context.UserAccounts.Remove(UserData);
            await _context.SaveChangesAsync();

            TempData["DeleteUser"] = "User has been deleted";
            return RedirectToAction(nameof(UserManage));
        }
        #endregion

        #region ORDER-MANAGEMENT
        // GET - ORDER
        // CODE BY HOANG
        public async Task<IActionResult> OrderManage()
        {
            // Get all order
            var OrderData = await (from order in _context.Orders 
                             join status in _context.OrderStatuses on order.StatusId equals status.StatusId
                             join payment in _context.OrderPaymentTypes on order.PaymentId equals payment.PaymentId
                             select new OrderManage
                             {
                                 OrderId = order.OrderId,
                                 TotalPrice = order.TotalPrice,
                                 DateOrder = order.DateOrder,
                                 StatusId = status.StatusId,
                                 PaymentId = payment.PaymentId
                             }).ToListAsync();

            ViewBag.OrderDelete = TempData["OrderDelete"]?.ToString();

            // Using ViewBag to display data without Model
            ViewBag.OrderData = OrderData;
            return View();
        }

        // GET - ORDER-STATUS-SET
        // CODE BT HOANG
        public async Task<IActionResult> OrderStatusSet(int oid)
        {
            // Get details of order
            var OrderData = await (from foods in _context.Foods
                                   join orderfood in _context.OrderFoods on foods.FoodId equals orderfood.FoodId
                                   join order in _context.Orders on orderfood.OrderFoodId equals order.OrderFoodId
                                   join status in _context.OrderStatuses on order.StatusId equals status.StatusId
                                   select new OrderStatusSet
                                   {
                                       PhoneReceive = orderfood.PhoneReceive,
                                       Address = orderfood.Address,
                                       OrderId = order.OrderId,
                                       FoodName = foods.FoodName,
                                       FoodPrice = foods.FoodPrice,
                                       Quantity = orderfood.Quantity,
                                       StatusId = status.StatusId,
                                       TotalPrice = order.TotalPrice
                                   }).Where(x => x.OrderId == oid).FirstOrDefaultAsync();

            // Using ViewBag to display data without Model
            ViewBag.OrderData = OrderData;
            return View();
        }

        // POST - UPDATE-ORDER-STATUS
        // CODE BY HOANG
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(int sid)
        {

            Order order = new()
            {
                StatusId = sid
            };

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(OrderManage));

        }

        // POST - ORDER-DELETE
        // CODE BY HOANG
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OrderDelete(int oid)
        {
            // Get order by oid
            var OrderData = await _context.Orders.Where(x => x.OrderId == oid).FirstOrDefaultAsync();

            _context.Orders.Remove(OrderData);
            await _context.SaveChangesAsync();

            TempData["OrderDelete"] = "The order with id " + oid + " has been delete";
            return RedirectToAction(nameof(OrderManage));
        }
        #endregion
    }
}