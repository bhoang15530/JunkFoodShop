using JunkFoodShop.Data;
using JunkFoodShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JunkFoodShop.Controllers
{
    public class UserController : Controller
    {
        private readonly JunkFoodShopContext _context;

        public UserController(JunkFoodShopContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> AccountSetting(string username)
        {
            // Get UserData by UserId
            var UserData = await (from user in _context.UserAccounts
                                  select new AccountSetting
                                  {
                                      Username = user.Username,
                                      FullName = user.FullName,
                                      PhoneNumber = user.PhoneNumber,
                                      Email = user.Email,
                                  }).Where(x => x.Username == username).FirstOrDefaultAsync();

            ViewBag.UserData = UserData;
            return View();
        }

        public async Task<IActionResult> OrdersList(string username)
        {
            // Get OrderList by username
            var OrderList = await (from order in _context.Orders
                                   join orderfood in _context.OrderFoods on order.OrderFoodId equals orderfood.OrderFoodId
                                   join user in _context.UserAccounts on orderfood.UserId equals user.UserId
                                   join orderstatus in _context.OrderStatuses on order.StatusId equals orderstatus.StatusId
                                   join orderpayment in _context.OrderPaymentTypes on order.PaymentId equals orderpayment.PaymentId
                                   select new OrderReview
                                   {
                                       DateOrder = order.DateOrder,
                                       TotalPrice = order.TotalPrice,
                                       Fullname = user.FullName,
                                       OrderId = order.OrderId,
                                       OrderStatus = orderstatus.StatusName,
                                       PaymentType = orderpayment.PaymentType
                                   }).Where(x => x.Username == username).ToListAsync();
            ViewBag.OrderList = OrderList;
            return View();
        }

        public async Task<IActionResult> Cart(string username)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return View(nameof(Index));
            }

            // Get CartItemList by username
            // TODO: Sửa dùm t đi =)))) cái join t ko biết viết
            var CartList = await (from cart in _context.Carts
                                  join food in _context.Foods on cart.FoodId equals food.FoodId
                                  join user in _context.UserAccounts on cart.UserId equals user.UserId
                                  join orderfood in _context.OrderFoods on user.UserId equals orderfood.UserId
                                  select new Models.Cart
                                  {
                                      FoodId = food.FoodId,
                                      FoodName = food.FoodName,
                                      FoodPrice = food.FoodPrice,
                                      Username = user.Username,
                                      Quantity = orderfood.Quantity,
                                  }).Where(x => x.Username == User.Identity.Name).ToListAsync();
            ViewBag.CartList = CartList;
            return View();
        }
        public IActionResult AddToCart(int fid, int? quantity)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return BadRequest();
            }

            // Get the FoodStock and Check if item is in stock
            var foodStock = _context.Foods.FirstOrDefault(x => x.FoodId == fid).FoodStock;

            if (foodStock <= 0)
            {
                ViewBag.OutOfStock = "This item is currently out of stock. Please shop another item.";
                return RedirectToAction("Index", "Home");
            }

            var user = _context.UserAccounts.Single(x => x.Username == User.Identity.Name);
            var foodId = _context.Foods.Single(x => x.FoodId == fid).FoodId;

            // Check if food is in cart
            var userCart = _context.Carts.Where(x => x.UserId == user.UserId).Where(x => x.FoodId == foodId).FirstOrDefault();
            if (userCart == null)
            {
                Data.Cart cart = new()
                {
                    FoodId = foodId,
                    UserId = user.UserId,
                    Quantity = 1,
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
            return View(nameof(Cart));
        }
        public IActionResult OrderList(string username)
        {
            // Get order list by username
            var OrderList = (from user in _context.UserAccounts
                            join orderfood in _context.OrderFoods on user.UserId equals orderfood.UserId
                            join order in _context.Orders on orderfood.OrderFoodId equals order.OrderFoodId
                            join orderstatus in _context.OrderStatuses on order.StatusId equals orderstatus.StatusId
                            join payment in _context.OrderPaymentTypes on order.PaymentId equals payment.PaymentId
                            select new
                            {
                                OrderId = order.OrderId,
                                StatusId = orderstatus.StatusId,
                                PaymentType = payment.PaymentType,
                                TotalPrice = order.TotalPrice,
                                DateOrder = order.DateOrder,
                                StatusName = orderstatus.StatusName,
                                Username = user.Username
                            }).Where(x => x.Username == username).ToList();
            ViewBag.OrderList = OrderList;
            return View();
        }
    }
}
