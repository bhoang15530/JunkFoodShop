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
            // Get CartItemList by username
            var CartItemsList = await (from cart in _context.Carts
                                       join food in _context.Foods on cart.FoodId equals food.FoodId
                                       join user in _context.UserAccounts on cart.UserId equals user.UserId
                                       select new Models.Cart
                                       {
                                           FoodId = food.FoodId,
                                           Foodname = food.FoodName,
                                           Fullname = user.FullName,
                                           FoodPrice = food.FoodPrice,
                                           Username = user.Username
                                       }).Where(x => x.Username == username).ToListAsync();
            ViewBag.CartItems = CartItemsList;
            return View();
        }
    }
}
