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

        // View all food
        public async Task<IActionResult> FoodManage()
        {
            // Get all food
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
            ViewBag.NotFound = TempData["NotFound"]?.ToString();

            ViewBag.CategoryList = await _context.FoodCategories.ToListAsync();
            ViewBag.FoodData = FoodData;
            return View();
        }

        // Display Create new food
        public async Task<IActionResult> CreateFood()
        {
            // Get category data
            var CategoryList = await _context.FoodCategories.ToListAsync();

            ViewBag.CategoryList = CategoryList;
            return View();
        }

        // Function Create new food
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFood([Bind("FoodName,FoodImage,FoodPrice,FoodStock,FoodDescription,CategoryId")] CreateFood createFood)
        {
            // Get and display category data
            ViewBag.CategoryList = await _context.FoodCategories.ToListAsync();

            // Check if admin selected category or not
            if (createFood.CategoryId == 0)
            {
                ViewBag.CategoryError = "Please select the Category";
                return View();
            }

            // Check FoodName exist in database
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

        // Display food edit
        public async Task<IActionResult> FoodEdit(int fId)
        {
            // Check if food exist
            bool IsFoodExist = _context.Foods.Any(x => x.FoodId == fId);

            if (!IsFoodExist)
            {
                TempData["NotFound"] = "Cannot find that Food";
                return RedirectToAction(nameof(FoodManage));
            }

            // Get data of Food by foodid
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

            ViewBag.CategoryList = await _context.FoodCategories.ToListAsync();
            ViewBag.FoodData = FoodData;
            return View();
        }

        // Function edit food
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
                FoodStock = editFood.FoodStock,
                FoodDescription = editFood.FoodDescription,
                CategoryId = editFood.CategoryId
            };

            _context.Foods.Update(food);
            await _context.SaveChangesAsync();

            TempData["EditSuccess"] = "Edit successfully";
            return RedirectToAction(nameof(FoodManage));
        }

        // Function delete food
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FoodDelete(int fId)
        {
            // Get food by fId
            var FoodData = await _context.Foods.Where(x => x.FoodId == fId).FirstOrDefaultAsync();

            if (FoodData == null)
            {
                TempData["NotFound"] = "Cannot find that Food";
                return RedirectToAction(nameof(FoodManage));
            }
            try
            {
                _context.Foods.Remove(FoodData);
            }
            catch (Exception)
            {
                TempData["NotFound"] = "You cannot delete this food because it's maybe exist in cart, order of user. You maybe set the quantity to zero instead or delete order!";
                return RedirectToAction(nameof(FoodManage));
            }
            await _context.SaveChangesAsync();

            TempData["DeleteSuccess"] = "Delete successfully";
            return RedirectToAction(nameof(FoodManage));
        }
        #endregion

        #region CATEGORY
        // Display all category
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
            ViewBag.NotFound = TempData["NotFound"]?.ToString();

            ViewBag.CategoryData = CategoryData;
            return View();
        }

        // View create category
        public IActionResult CreateCategory()
        {
            return View();
        }

        // Function create category
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory([Bind("CategoryName,CategoryImage")] CreateCategory createCategory)
        {
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

        // View category edit
        public async Task<IActionResult> CategoryEdit(int cid)
        {
            // Get Category by cid
            var CategoryData = await _context.FoodCategories.Where(x => x.Categoryid == cid).FirstOrDefaultAsync();
            
            if (CategoryData == null)
            {
                TempData["NotFound"] = "Category not found";
                return RedirectToAction(nameof(CategoryManage));
            }

            ViewBag.CategoryData = CategoryData;
            return View();
        }

        // Function category edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CategoryEdit([Bind("Categoryid,CategoryImage,CategoryName")] CategoryManage editCategory)
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
            return RedirectToAction(nameof(CategoryManage));
        }

        // Function delete category
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CategoryDelete(int cid)
        {
            // Get Category by cid
            var CategoryData = await _context.FoodCategories.Where(x => x.Categoryid == cid).FirstOrDefaultAsync();

            // Get food by cid
            var FoodData = await _context.Foods.Where(x => x.CategoryId == cid).ToListAsync();

            if (CategoryData == null)
            {
                TempData["NotFound"] = "Category not found";
                return RedirectToAction(nameof(CategoryManage));
            }

            try
            {
                _context.Foods.RemoveRange(FoodData);
                _context.FoodCategories.Remove(CategoryData);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                TempData["NotFound"] = "You cannot delete this category. It maybe because there is food exist in Cart, Order of user. You may need to delete Order instead or set food quantity to zero if you do not want anyone can buy this!";
                return RedirectToAction(nameof(CategoryManage));
            }

            TempData["DeleteSuccess"] = "Delete successfully";
            return RedirectToAction(nameof(CategoryManage));
        }
        #endregion

        #region USER-MANAGEMENT

        // Display all User
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
            ViewBag.NotFound = TempData["NotFound"]?.ToString();

            ViewBag.UserData = UserData;
            return View();
        }

        // Function delete User
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserDelete(int uid)
        {
            // Get user by uid
            var UserData = await _context.UserAccounts.Where(x => x.UserId == uid).FirstOrDefaultAsync();

            if (UserData == null)
            {
                TempData["NotFound"] = "User not found";
                return RedirectToAction(nameof(UserManage));
            }

            // Get all relative to user
            var GetCart = await _context.Carts.Where(x => x.UserId == uid).ToArrayAsync();
            var GetComment = await _context.Comments.Where(x => x.UserId == uid).ToArrayAsync();
            var GetRating = await _context.Ratings.Where(x => x.UserId == uid).ToArrayAsync();
            var GetOrderFoods = await _context.OrderFoods.Where(x => x.UserId == uid).ToArrayAsync();
            var OrderId = await _context.OrderFoods.Where(x => x.UserId == uid).Select(x => x.OrderId).ToArrayAsync();
            var GetOrder = await _context.Orders.Where(x => OrderId.Contains(x.OrderId)).ToArrayAsync();

            // Delete all relative
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
        // Display all Orders
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
            ViewBag.NotFound = TempData["NotFound"]?.ToString();

            ViewBag.OrderData = orders;
            return View();
        }

        // Display update status
        public async Task<IActionResult> UpdateOrderStatus(int oid, int uid)
        {
            // Get order by oid and uid
            var OrderData = await _context.Orders
                    .Include(o => o.OrderFoods)
                    .ThenInclude(of => of.Food)
                    .Include(o => o.Status)
                    .Include(o => o.Payment)
                    .Where(o => o.OrderFoods.Any(of => of.UserId == uid))
                    .Where(o => o.OrderId == oid)
                    .FirstOrDefaultAsync();

            if (OrderData == null)
            {
                TempData["NotFound"] = "Orders not found";
                return RedirectToAction(nameof(OrderManage));
            }

            ViewBag.StatusList = _context.OrderStatuses.ToList();
            ViewBag.OrderData = OrderData;
            return View();
        }

        // Function set order status
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetOrderStatus(int oid, string orderstatus)
        {
            var order = await _context.Orders.Where(x => x.OrderId == oid).FirstOrDefaultAsync();

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
                    order!.StatusId = int.Parse(orderstatus);
                    _context.Foods.Update(food);
                    await _context.SaveChangesAsync();
                }
            }
            // 2 == Delivering
            else if (orderstatus == "2")
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
            }
            order!.StatusId = int.Parse(orderstatus);
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            TempData["Update"] = "Update Status Successfully!!!";
            return RedirectToAction(nameof(OrderManage));
        }

        // Function delete orders
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OrderDelete(int oid)
        {
            // Get order by oid and their relative
            var OrderData = await _context.Orders.Where(x => x.OrderId == oid).FirstOrDefaultAsync();
            var OrderFoodData = await _context.OrderFoods.Where(x => x.OrderId == oid).ToArrayAsync();

            if (OrderData == null)
            {
                TempData["NotFound"] = "Orders not found";
                return RedirectToAction(nameof(OrderManage));
            }

            _context.OrderFoods.RemoveRange(OrderFoodData);
            _context.Orders.Remove(OrderData);
            await _context.SaveChangesAsync();

            TempData["OrderDelete"] = "Delete Successful";
            return RedirectToAction(nameof(OrderManage));
        }
        #endregion

        #region USER-COMMENTS-MANAGE
        // Display all comment
        public IActionResult UserCommentManage()
        {
            // Get all comment
            var CommentList = _context.Comments.Include(x => x.User).Select(x => new
            {
                x.CommentId,
                x.Content,
                x.DateComment,
                x.Food.FoodName,
                x.User.FullName,
            }).OrderBy(x => x.FoodName).ToList();

            ViewBag.CommentList = CommentList;

            ViewBag.NotFound = TempData["NotFound"]?.ToString();
            return View();
        }

        // Function delete comment
        public IActionResult DeleteComment(int? commentId)
        {
            if (commentId == null)
            {
                TempData["NotFound"] = "Comment not found";
                return RedirectToAction(nameof(UserCommentManage));
            }

            // Get comment by ID
            var comment = _context.Comments.Where(x => x.CommentId == commentId).FirstOrDefault();

            if (comment == null)
            {
                TempData["NotFound"] = "Comment not found";
                return RedirectToAction(nameof(UserCommentManage));
            }

            var userId = comment.UserId;
            var foodId = comment.FoodId;

            var rating = _context.Ratings.Where(x => x.FoodId == foodId && x.UserId == userId).FirstOrDefault();

            if (rating != null)
            {
                _context.Ratings.Remove(rating);
                _context.SaveChanges();
            }

            _context.Comments.Remove(comment);
            _context.SaveChanges();

            return RedirectToAction(nameof(UserCommentManage));
        }
        #endregion
    }
}
