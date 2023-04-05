using System.Security.Claims;
using JunkFoodShop.Data;
using JunkFoodShop.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace JunkFoodShop.Controllers
{
    [Authorize(Roles = "User")]
    public class UserController : Controller
    {
        private readonly JunkFoodShopContext _context;
        private const int KeyLen = 10000;
        private const string KeyName = "JunkFoodShop";

        public UserController(JunkFoodShopContext context)
        {
            _context = context;
        }

        #region Account Setting
        public async Task<IActionResult> AccountSetting()
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("SignIn", "Account");
            }

            // Check if account exist
            var UserEXIST = User.FindFirstValue(ClaimTypes.Name);
            if (UserEXIST == null)
            {
                TempData["Message"] = "Your account has been deleted. Please contact an administrator for more information.";
                await HttpContext.SignOutAsync();
                return RedirectToAction("SignIn", "Account");
            }

            // Get UserData by UserId
            var UserData = await (from user in _context.UserAccounts
                                  select new
                                  {
                                      user.Username,
                                      user.FullName,
                                      user.PhoneNumber,
                                      user.Email,
                                  }).Where(x => x.Username == User.Identity.Name || x.Email == User.Identity.Name).FirstOrDefaultAsync();
            ViewBag.UserData = UserData;

            ViewBag.WrongPasswrod = TempData["WrongPassword"]?.ToString();
            ViewBag.InputNewPassword = TempData["TypeNewPassword"]?.ToString();
            ViewBag.InputOldPassword = TempData["TypeOldPassword"]?.ToString();

            return View();
        }
        #endregion

        #region View Order List of User
        public async Task<IActionResult> OrderList()
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("SignIn", "Account");
            }

            // Check if account exist
            var UserEXIST = User.FindFirstValue(ClaimTypes.Name);
            if (UserEXIST == null)
            {
                TempData["Message"] = "Your account has been deleted. Please contact an administrator for more information.";
                await HttpContext.SignOutAsync();
                return RedirectToAction("SignIn", "Account");
            }

            // Get userID
            var userId = _context.UserAccounts.Where(x => x.Username == User.Identity.Name || x.Email == User.Identity.Name).Select(x => x.UserId).FirstOrDefault();

            // Get OrderList by UserId sort by OrderDate
            var orders = _context.Orders
                    .Include(o => o.OrderFoods)
                    .ThenInclude(of => of.Food)
                    .Include(o => o.Status)
                    .Include(o => o.Payment)
                    .Where(o => o.OrderFoods.Any(of => of.UserId == userId))
                    .ToList();

            ViewBag.OrderData = orders;
            return View();
        }
        #endregion

        #region View Cart
        public async Task<IActionResult> Cart()
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("SignIn", "Account");
            }

            // Check if account exist
            var UserEXIST = User.FindFirstValue(ClaimTypes.Name);
            if (UserEXIST == null)
            {
                TempData["Message"] = "Your account has been deleted. Please contact an administrator for more information.";
                await HttpContext.SignOutAsync();
                return RedirectToAction("SignIn", "Account");
            }

            // Get CartItemList by username
            var userId = _context.UserAccounts.Where(x => x.Username == User.Identity.Name || x.Email == User.Identity.Name).Select(x => x.UserId).FirstOrDefault();

            var CartList = await (from cart in _context.Carts
                                  where cart.UserId == userId
                                  join foods in _context.Foods on cart.FoodId equals foods.FoodId
                                  select new
                                  {
                                      foods.FoodName,
                                      foods.FoodId,
                                      foods.FoodPrice,
                                      cart.Quantity,
                                      cart.UserId,
                                      foods.FoodImage
                                  }).ToListAsync();
            ViewBag.CartList = CartList;
            return View();
        }
        #endregion

        #region Function Add Item To Cart
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int fid, int? quantity)
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("SignIn", "Account");
            }

            // Check if account exist
            var UserEXIST = User.FindFirstValue(ClaimTypes.Name);
            if (UserEXIST == null)
            {
                TempData["Message"] = "Your account has been deleted. Please contact an administrator for more information.";
                await HttpContext.SignOutAsync();
                return RedirectToAction("SignIn", "Account");
            }

            // Get the FoodStock and Check if item is in stock
            var food = _context.Foods.FirstOrDefault(x => x.FoodId == fid);
            if (food == null)
            {
                return NotFound();
            }

            var foodStock = food.FoodStock;

            if (foodStock <= 0)
            {
                ViewBag.OutOfStock = "This item is currently out of stock. Please shop another item.";
                return RedirectToAction("Index", "Home");
            }

            var user = _context.UserAccounts.Single(x => x.Username == User.Identity.Name || x.Email == User.Identity.Name);
            var foodId = _context.Foods.Single(x => x.FoodId == fid).FoodId;

            // Check if food is in cart
            var userCart = _context.Carts.Where(x => x.UserId == user.UserId).Where(x => x.FoodId == foodId).FirstOrDefault();
            if (userCart == null)
            {
                Data.Cart cart = new()
                {
                    FoodId = foodId,
                    UserId = user.UserId,
                    Quantity = quantity ?? 1,
                    Address = "VN",
                    PhoneReceive = user.PhoneNumber,
                };
                _context.Carts.Add(cart);
                _context.SaveChanges();
            }
            else
            {
                // if food exists, increment by one or by quantity
                userCart.Quantity = userCart.Quantity + quantity ?? userCart.Quantity + 1;
                _context.Carts.Update(userCart);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Cart));
        }
        #endregion

        #region Function Update Cart
        [HttpPost]
        public async Task<IActionResult> UpdateCartItem([FromBody] UpdateCartItem updateCartItem)
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("SignIn", "Account");
            }

            // Check if account exist
            var UserEXIST = User.FindFirstValue(ClaimTypes.Name);
            if (UserEXIST == null)
            {
                TempData["Message"] = "Your account has been deleted. Please contact an administrator for more information.";
                await HttpContext.SignOutAsync();
                return RedirectToAction("SignIn", "Account");
            }

            int foodId = int.Parse(updateCartItem.fid);
            int foodQuantity = int.Parse(updateCartItem.quantity);
            var userId = _context.UserAccounts.Where(x => x.Username == User.Identity.Name || x.Email == User.Identity.Name).FirstOrDefault()!.UserId;
            var userCart = _context.Carts.Where(x => x.UserId == userId).Where(x => x.FoodId == foodId).FirstOrDefault();

            if (userCart == null)
            {
                return NotFound();
            }

            userCart.Quantity = foodQuantity;
            _context.Carts.Update(userCart);
            _context.SaveChanges();

            return Ok();
        }
        #endregion

        #region Function Remove Item From Cart
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromCart(int fid)
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("SignIn", "Account");
            }

            // Check if account exist
            var UserEXIST = User.FindFirstValue(ClaimTypes.Name);
            if (UserEXIST == null)
            {
                TempData["Message"] = "Your account has been deleted. Please contact an administrator for more information.";
                await HttpContext.SignOutAsync();
                return RedirectToAction("SignIn", "Account");
            }

            // Get userID
            var userId = _context.UserAccounts.Where(x => x.Username == User.Identity.Name || x.Email == User.Identity.Name).Select(x => x.UserId).FirstOrDefault();

            // Check if item is in cart
            var userCart = _context.Carts.Where(x => x.UserId == userId).Where(x => x.FoodId == fid).FirstOrDefault();
            if (userCart == null)
            {
                return NotFound();
            }
            else
            {
                _context.Carts.Remove(userCart);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Cart));
        }
        #endregion

        #region Function Check Out
        public async Task<IActionResult> CheckOut()
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            // Check if account exist
            var UserEXIST = User.FindFirstValue(ClaimTypes.Name);
            if (UserEXIST == null)
            {
                TempData["Message"] = "Your account has been deleted. Please contact an administrator for more information.";
                await HttpContext.SignOutAsync();
                return RedirectToAction("SignIn", "Account");
            }

            // Get CartItemList by username
            var userId = _context.UserAccounts.Where(x => x.Username == User.Identity.Name || x.Email == User.Identity.Name).Select(x => x.UserId).FirstOrDefault();

            var CartList = await (from cart in _context.Carts
                                  where cart.UserId == userId
                                  join foods in _context.Foods on cart.FoodId equals foods.FoodId
                                  select new
                                  {
                                      foods.FoodName,
                                      foods.FoodId,
                                      foods.FoodPrice,
                                      cart.Quantity,
                                      cart.UserId,
                                      foods.FoodImage
                                  }).ToListAsync();
            ViewBag.CartList = CartList;
            return View();
        }
        #endregion

        #region Function Pay
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pay(string address, int phone, string paymentType)
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("SignIn", "Account");
            }

            // Check if account exist
            var UserEXIST = User.FindFirstValue(ClaimTypes.Name);
            if (UserEXIST == null)
            {
                TempData["Message"] = "Your account has been deleted. Please contact an administrator for more information.";
                await HttpContext.SignOutAsync();
                return RedirectToAction("SignIn", "Account");
            }

            float total = 0;
            var userId = _context.UserAccounts.Where(x => x.Username == User.Identity.Name || x.Email == User.Identity.Name).FirstOrDefault()!.UserId;

            var cart = _context.Carts.Where(x => x.UserId == userId).ToList();
            if (cart == null)
            {
                return NotFound();
            }

            Order o = new()
            {
                DateOrder = DateTime.Now,
                PaymentId = int.Parse(paymentType),
                StatusId = 1,
            };

            _context.Orders.Add(o);
            _context.SaveChanges();

            foreach (var cartItem in cart)
            {
                float FoodCost = _context.Foods.Where(x => x.FoodId == cartItem.FoodId).FirstOrDefault()!.FoodPrice;
                total += cartItem.Quantity * FoodCost;

                OrderFood c = new()
                {
                    Address = address,
                    FoodId = cartItem.FoodId,
                    PhoneReceive = phone,
                    Quantity = cartItem.Quantity,
                    UserId = cartItem.UserId,
                    OrderId = o.OrderId,
                };

                _context.Carts.Remove(cartItem);
                _context.OrderFoods.Add(c);
            }

            o.TotalPrice = (int)total;
            _context.SaveChanges();

            return View();
        }
        #endregion

        #region View Order Details
        public async Task<IActionResult> OrderDetails(int oid)
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("SignIn", "Account");
            }

            // Check if account exist
            var UserEXIST = User.FindFirstValue(ClaimTypes.Name);
            if (UserEXIST == null)
            {
                TempData["Message"] = "Your account has been deleted. Please contact an administrator for more information.";
                await HttpContext.SignOutAsync();
                return RedirectToAction("SignIn", "Account");
            }

            var userId = _context.UserAccounts.Where(x => x.Username == User.Identity.Name || x.Email == User.Identity.Name).FirstOrDefault()!.UserId;
            var OrderData = _context.Orders
                    .Include(o => o.OrderFoods)
                    .ThenInclude(of => of.Food)
                    .Include(o => o.Status)
                    .Include(o => o.Payment)
                    .Where(o => o.OrderFoods.Any(of => of.UserId == userId))
                    .Where(o => o.OrderId == oid)
                    .FirstOrDefault();
            if (OrderData == null)
            {
                return NotFound();
            }
            ViewBag.OrderData = OrderData;
            return View();
        }
        #endregion

        #region Function Update User Info
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUserData([Bind("FullName,Email,PhoneNumber")] AccountSetting accountSetting, string? OldPassword, string? NewPassword)
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("SignIn", "Account");
            }

            // Check if account exist
            var UserExist = User.FindFirstValue(ClaimTypes.Name);
            if (UserExist == null)
            {
                TempData["Message"] = "Your account has been deleted. Please contact an administrator for more information.";
                await HttpContext.SignOutAsync();
                return RedirectToAction("SignIn", "Account");
            }

            var user = _context.UserAccounts.Where(x => x.Username == User.Identity.Name || x.Email == User.Identity.Name).FirstOrDefault();

            if (user == null)
            {
                return NotFound();
            }
            user.FullName = accountSetting.FullName;
            user.PhoneNumber = accountSetting.PhoneNumber;
            user.Email = accountSetting.Email;

            if (OldPassword != null)
            {
                byte[] encode = new byte[KeyLen];
                encode = System.Text.Encoding.UTF8.GetBytes(KeyName);

                string hashedOldPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: OldPassword!,
                    salt: encode,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8));

                // Check if hashed old password is the same with hashed password in user database
                if (!String.Equals(user.Password, hashedOldPassword))
                {
                    TempData["WrongPassword"] = "Your password is incorrect!, Please try again!";
                    return RedirectToAction(nameof(AccountSetting));
                }

                if (NewPassword != null)
                {
                    string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: NewPassword!,
                        salt: encode,
                        prf: KeyDerivationPrf.HMACSHA256,
                        iterationCount: 100000,
                        numBytesRequested: 256 / 8));
                    user.Password = hashed;
                }
                else
                {
                    TempData["TypeNewPassword"] = "Please input your new password!";
                    return RedirectToAction(nameof(AccountSetting));
                }
            }
            else
            {
                TempData["TypeOldPassword"] = "Please input your old password!";
                return RedirectToAction(nameof(AccountSetting));
            }

            _context.UserAccounts.Update(user);
            _context.SaveChanges();
            return RedirectToAction(nameof(AccountSetting));
        }
        #endregion

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveRatings(int foodId, int? commentId, int? ratingId, string? commentContent, string? star)
        {
            // check user is login or not
            if (!User.Identity!.IsAuthenticated)
            {
                TempData["NotLoggedIn"] = "You need to log in first";
                return RedirectToAction("Details", "Foods");
            }

            var userId = _context.UserAccounts.Where(x => x.Username == User.Identity.Name || x.Email == User.Identity.Name).FirstOrDefault()!.UserId;

            // Lots of checking
            if (commentId != null)
            {
                var comment = _context.Comments.Where(x => x.FoodId == foodId && x.CommentId == commentId).FirstOrDefault();
                if (comment != null)
                {
                    if (commentContent != null)
                    {
                        comment.Content = commentContent;
                        _context.Comments.Update(comment);
                    }
                }
            }
            else
            {
                var comment = new Comment
                {
                    FoodId = foodId,
                    Content = commentContent ?? "",
                    DateComment = DateTime.Now,
                    UserId = userId
                };
                _context.Comments.Add(comment);
            }

            if (ratingId != null)
            {
                var userRating = _context.Ratings.Where(x => x.UserId == userId && x.FoodId == foodId).FirstOrDefault();
                if (userRating != null)
                {
                    if (star != null)
                    {
                        userRating.Star = int.Parse(star);
                    }
                }
            }
            else
            {
                if (star != null)
                {
                    var rating = new Rating
                    {
                        FoodId = foodId,
                        Star = int.Parse(star),
                        UserId = userId
                    };
                    _context.Ratings.Add(rating);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Foods", new { foodId });
        }
        public IActionResult ViewComment(int? foodId)
        {
            if (foodId == null)
            {
                return NotFound();
            }

            var user = _context.UserAccounts.Where(x => x.Username == User.Identity.Name || x.Email == User.Identity.Name).FirstOrDefault();

            if (user == null)
            {
                return NotFound();
            }

            var userId = user.UserId;

            var Comment = _context.Comments.Where(x => x.FoodId == foodId && x.UserId == userId).FirstOrDefault();
            var Rating = _context.Ratings.Where(x => x.FoodId == foodId && x.UserId == userId).FirstOrDefault();

            ViewBag.Comment = Comment;
            ViewBag.Rating = Rating;
            ViewBag.FoodId = foodId;

            return View();
        }
    }
}
