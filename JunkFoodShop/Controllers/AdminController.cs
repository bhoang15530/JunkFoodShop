using JunkFoodShop.Data;
using JunkFoodShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web;
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
            var FoodData = await (from food in _context.Foods
                                  join category in _context.FoodCategories on food.CategoryId equals category.Categoryid
                                  select new FoodManage
                                  {
                                      FoodId = food.FoodId,
                                      FoodImage = food.FoodImage,
                                      FoodName = food.FoodName,
                                      FoodPrice = food.FoodPrice,
                                      FoodStock = food.FoodStock,
                                      CategoryName = category.CategoryName,
                                      Categoryid = food.CategoryId,
                                  }).ToListAsync();

            ViewBag.EditSuccess = TempData["EditSuccess"]?.ToString();
            ViewBag.DeleteSuccess = TempData["DeleteSuccess"]?.ToString();
            ViewBag.FoodCreate = TempData["FoodCreate"]?.ToString();

            // Using ViewBag to display data without Model
            ViewBag.CategoryList = await _context.FoodCategories.ToListAsync();
            ViewBag.FoodData = FoodData;
            return View();
        }

        // GET - CREATE FOOD
        // CODE BY HOANG
        public async Task<IActionResult> CreateFood()
        {
            // Get category data
            var CategoryList = await _context.FoodCategories.ToListAsync();

            // Using ViewBag to display data without Model
            ViewBag.CategoryList = CategoryList;
            return View();
        }

        // POST - CREATE FOOD
        // CODE BY HOANG
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFood([Bind("FoodName,FoodImage,FoodPrice,FoodStock,FoodDescription,CategoryId")] CreateFood createFood)
        {

            ViewBag.CategoryList = await _context.FoodCategories.ToListAsync();

            if (createFood.CategoryId == 0)
            {
                ViewBag.CategoryError = "Please select the Category";
                return View();
            }

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
                    FoodStock = createFood.FoodStock,
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
        public async Task<IActionResult> FoodEdit(int fId)
        {
            // Get data of Food join Category where FoodId selected Specify Column
            var FoodData = await (from foods in _context.Foods
                                  join categories in _context.FoodCategories on foods.CategoryId equals categories.Categoryid
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
                                  }).Where(x => x.FoodId == fId).FirstOrDefaultAsync();
            // Using ViewBag to display data without Model
            ViewBag.CategoryList = await _context.FoodCategories.ToListAsync();
            ViewBag.FoodData = FoodData;
            return View();
        }

        // POST - FOOD EDIT
        // CODE BY HOANG
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FoodEdit([Bind("FoodId,FoodName,FoodImage,FoodPrice,FoodStock,FoodDescription,CategoryId")] EditFood editFood)
        {
            if (!ModelState.IsValid)
            {
                return View(editFood);
            }

            Food food = new()
            {
                FoodId = editFood.FoodId,
                FoodName = editFood.FoodName,
                FoodImage = editFood.FoodImage,
                FoodPrice = editFood.FoodPrice,
                FoodStock = editFood.FoodStock,
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
            if (FoodData == null)
            {
                return NotFound();
            }
            _context.Foods.Remove(FoodData);
            await _context.SaveChangesAsync();

            TempData["DeleteSuccess"] = "Delete successfully";
            return RedirectToAction(nameof(FoodManage));
        }
        #endregion

        #region CATEGORY
        // GET - CATEGORY 
        // CODE BY HOANG
        public async Task<IActionResult> CategoryManage()
        {
            // Get All Category
            var CategoryData = await (from categoryManage in _context.FoodCategories
                                      select new CategoryManage
                                      {
                                          Categoryid = categoryManage.Categoryid,
                                          CategoryImage = categoryManage.CategoryImage,
                                          CategoryName = categoryManage.CategoryName,
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
            if (!ModelState.IsValid)
            {
                return View(createCategory);
            }
            // Check CategoryName exist
            bool isCategoryExists = await _context.FoodCategories.AnyAsync(x => x.CategoryName == createCategory.CategoryName);
            if (isCategoryExists)
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
        public async Task<IActionResult> CategoryEdit([Bind("Categoryid,CategoryImage,CategoryName")] CategoryManage editCategory)
        {
            if (!ModelState.IsValid)
            {
                return View(editCategory);
            }

            FoodCategory category = new()
            {
                Categoryid = editCategory.Categoryid,
                CategoryImage = editCategory.CategoryImage,
                CategoryName = editCategory.CategoryName
            };

            _context.FoodCategories.Update(category);
            await _context.SaveChangesAsync();

            TempData["EditSuccess"] = "Edit successfully";
            return RedirectToAction(nameof(CategoryManage));
        }

        // POST - CATEGORY DELETE
        // CODE BY HOANG
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CategoryDelete(int cid)
        {
            // Get Category by cid
            var CategoryData = await _context.FoodCategories.Where(x => x.Categoryid == cid).FirstOrDefaultAsync();

            if (CategoryData == null)
            {
                return NotFound();
            }
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
                                Username = user.Username,
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserDelete(int uid)
        {
            // Get user by uid
            var UserData = await _context.UserAccounts.Where(x => x.UserId == uid).FirstOrDefaultAsync();

            if (UserData == null)
            {
                return NotFound();
            }

            // Get all relative to user
            var GetCart = await _context.Carts.Where(x => x.UserId == uid).ToArrayAsync();
            var GetComment = await _context.Comments.Where(x => x.UserId == uid).ToArrayAsync();
            var GetRating = await _context.Ratings.Where(x => x.UserId == uid).ToArrayAsync();
            // Get OrderId in OrderFoods and Orders
            var GetOrderFoods = await _context.OrderFoods.Where(x => x.UserId == uid).ToArrayAsync();

            var OrderId = await _context.OrderFoods.Where(x => x.UserId == uid).Select(x => x.OrderId).ToArrayAsync();
            var GetOrder = await _context.Orders.Where(x => OrderId.Contains(x.OrderId)).ToArrayAsync();

            _context.OrderFoods.RemoveRange(GetOrderFoods);
            _context.Orders.RemoveRange(GetOrder);
            _context.Carts.RemoveRange(GetCart);
            _context.Comments.RemoveRange(GetComment);
            _context.Ratings.RemoveRange(GetRating);

            _context.UserAccounts.Remove(UserData);
            await _context.SaveChangesAsync();

            TempData["DeleteUser"] = "User has been deleted";
            return RedirectToAction(nameof(UserManage));
        }
        #endregion

        #region ORDER-MANAGEMENT
        // GET - ORDER
        // CODE BY HOANG: 16/3/2023
        // Last update: 3/30/2023
        public async Task<IActionResult> OrderManage()
        {
            // Get all order
            var orders = _context.Orders
                    .Include(o => o.OrderFoods).ThenInclude(of => of.Food)
                    .Include(o => o.OrderFoods).ThenInclude(of => of.User)
                    .Include(o => o.Status)
                    .Include(o => o.Payment)
                    .ToList();

            ViewBag.StatusList = await _context.OrderStatuses.ToListAsync();

            ViewBag.OrderDelete = TempData["OrderDelete"]?.ToString();
            ViewBag.OrderStatus = TempData["Update"]?.ToString();

            // Using ViewBag to display data without Model
            ViewBag.OrderData = orders;
            return View();
        }

        // GET - UPDATE-ORDER-STATUS
        // CODE BY HOANG: 16/3/2023
        // UPDATE BY TRUONG: 31/3/2023
        public async Task<IActionResult> UpdateOrderStatus(int oid, int uid)
        {
            var OrderData = _context.Orders
                    .Include(o => o.OrderFoods)
                    .ThenInclude(of => of.Food)
                    .Include(o => o.Status)
                    .Include(o => o.Payment)
                    .Where(o => o.OrderFoods.Any(of => of.UserId == uid))
                    .Where(o => o.OrderId == oid)
                    .FirstOrDefault();
            if (OrderData == null)
            {
                return NotFound();
            }
            ViewBag.StatusList = _context.OrderStatuses.ToList();
            ViewBag.OrderData = OrderData;
            return View();
        }

        // POST 
        // CODE BY HOANG: 16/3/2023
        // UPDATE BY TRUONG: 31/3/2023
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetOrderStatus(int oid, string orderstatus)
        {
            var order = _context.Orders.Where(x => x.OrderId == oid).FirstOrDefault();
            if (order == null)
            {
                return NotFound();
            }

            // 3 = Delivery Complete
            if (orderstatus == "3")
            {
                var orderfoods = _context.OrderFoods.Where(x => x.OrderId == oid).ToList();
                bool isOutOfStock = false;
                List<string> outOfStockItems = new();

                // Loop though each item to check if is out of stock or not
                foreach (var item in orderfoods)
                {
                    var food = _context.Foods.Where(x => x.FoodId == item.FoodId).FirstOrDefault()!;
                    int foodStock = food.FoodStock;
                    int orderQuantity = item.Quantity;

                    if (foodStock < orderQuantity)
                    {
                        isOutOfStock = true;
                        outOfStockItems.Add(item.Food!.FoodName);
                    }
                }

                if (isOutOfStock)
                {
                    TempData["Update"] = $"The following items are currently out of stock: {string.Join(", ", outOfStockItems.ToArray())}";
                    return RedirectToAction(nameof(OrderManage));
                }

                foreach (var item in orderfoods)
                {
                    // 1. Take each FoodQuantity in order
                    // 2. Take each Quantity in Database
                    // 3. Subtract and Update if Quantity is greater or equal to FoodQuantity
                    var food = _context.Foods.Where(x => x.FoodId == item.FoodId).FirstOrDefault()!;
                    int foodStock = food.FoodStock;
                    int orderQuantity = item.Quantity;

                    food.FoodStock = foodStock - orderQuantity;
                    order.StatusId = int.Parse(orderstatus);
                    _context.Foods.Update(food);
                    _context.SaveChanges();
                }
            }
            order.StatusId = int.Parse(orderstatus);
            _context.Orders.Update(order);
            _context.SaveChanges();

            TempData["Update"] = "Update Status Successfully!!!";
            return RedirectToAction(nameof(OrderManage));
        }

        // POST - ORDER-DELETE
        // CODE BY HOANG: 16/3/2023
        // Last update: 31/3/2023
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OrderDelete(int oid)
        {
            // Get order by oid
            var OrderData = await _context.Orders.Where(x => x.OrderId == oid).FirstOrDefaultAsync();
            var OrderFoodData = await _context.OrderFoods.Where(x => x.OrderId == oid).ToArrayAsync();

            if (OrderData == null)
            {
                return NotFound();
            }

            _context.OrderFoods.RemoveRange(OrderFoodData);
            _context.Orders.Remove(OrderData);
            await _context.SaveChangesAsync();

            TempData["OrderDelete"] = "Delete Successful";
            return RedirectToAction(nameof(OrderManage));
        }
        #endregion
    }
}
