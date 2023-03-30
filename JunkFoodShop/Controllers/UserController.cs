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

        public async Task<IActionResult> AccountSetting()
        {
            // Get UserData by UserId
            var UserData = await (from user in _context.UserAccounts
                                  select new AccountSetting
                                  {
                                      Username = user.Username,
                                      FullName = user.FullName,
                                      PhoneNumber = user.PhoneNumber,
                                      Email = user.Email,
                                  }).Where(x => x.Username == User.Identity.Name).FirstOrDefaultAsync();

            ViewBag.UserData = UserData;
            return View();
        }

        public async Task<IActionResult> OrderList()
        {
            // Get userID
            var userId = _context.UserAccounts.Where(x => x.Username == User.Identity.Name).Select(x => x.UserId).FirstOrDefault();

            // Get OrderList by UserId sort by OrderDate
            var OrderData = await (from order in _context.Orders
                                   join orderfood in _context.OrderFoods on order.OrderFoodId equals orderfood.OrderFoodId
                                   join orderstatus in _context.OrderStatuses on order.StatusId equals orderstatus.StatusId
                                   join payment in _context.OrderPaymentTypes on order.PaymentId equals payment.PaymentId
                                   where orderfood.UserId == userId
                                   group new { order, payment, orderstatus } by order.DateOrder into dateGroup
                                   select new
                                   {
                                       DateOrder = dateGroup.Key,
                                       Orders = dateGroup.Select(x => new
                                       {
                                           x.order.TotalPrice,
                                           x.payment.PaymentId,
                                           x.orderstatus.StatusId,
                                           x.orderstatus.StatusName
                                       })
                                   }).ToListAsync();
            ViewBag.OrderData = OrderData;
            return View();
        }

        public async Task<IActionResult> Cart()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            // Get CartItemList by username
            var userId = _context.UserAccounts.Where(x => x.Username == User.Identity.Name).Select(x => x.UserId).FirstOrDefault();

            var CartList = await (from cart in _context.Carts
                                  where cart.UserId == userId
                                  join foods in _context.Foods on cart.FoodId equals foods.FoodId
                                  select new Models.Cart
                                  {
                                      FoodName = foods.FoodName,
                                      FoodId = foods.FoodId,
                                      FoodPrice = foods.FoodPrice,
                                      Quantity = cart.Quantity,
                                      UserId = cart.UserId
                                  }).ToListAsync();
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
        [HttpPost]
        public IActionResult UpdateCartItem([FromBody] UpdateCartItem updateCartItem)
        {
            int foodId = Int32.Parse(updateCartItem.fid);
            int foodQuantity = Int32.Parse(updateCartItem.quantity);
            var userId = _context.UserAccounts.Where(x => x.Username == User.Identity.Name).FirstOrDefault().UserId;
            var userCart = _context.Carts.Where(x => x.UserId == userId).Where(x => x.FoodId == foodId).FirstOrDefault();
            if (userCart != null)
            {
                userCart.Quantity = foodQuantity;
                _context.Carts.Update(userCart);
                _context.SaveChanges();
            }
            else
            {
                return NotFound();
            }
            return Ok();
        }

        public IActionResult RemoveFromCart(int fid)
        {
            // Get userID
            var userId = _context.UserAccounts.Where(x => x.Username == User.Identity.Name).Select(x => x.UserId).FirstOrDefault();

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

        public async Task<IActionResult> CheckOut()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            // Get CartItemList by username
            var userId = _context.UserAccounts.Where(x => x.Username == User.Identity.Name).Select(x => x.UserId).FirstOrDefault();

            var CartList = await (from cart in _context.Carts
                                  where cart.UserId == userId
                                  join foods in _context.Foods on cart.FoodId equals foods.FoodId
                                  select new Models.Cart
                                  {
                                      FoodName = foods.FoodName,
                                      FoodId = foods.FoodId,
                                      FoodPrice = foods.FoodPrice,
                                      Quantity = cart.Quantity,
                                      UserId = cart.UserId
                                  }).ToListAsync();
            ViewBag.CartList = CartList;
            return View();
        }

        // TODO: Need fixing
        public IActionResult Pay(string address, int phone, string paymentType)
        {
            float total = 0;
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            var userId = _context.UserAccounts.Where(x => x.Username == User.Identity.Name).FirstOrDefault().UserId;
            
            var cart = _context.Carts.Where(x => x.UserId == userId).ToList();
            if (cart == null)
            {
                return NotFound();
            }

            foreach (var cartItem in cart)
            {
                float FoodCost = _context.Foods.Where(x => x.FoodId == cartItem.FoodId).FirstOrDefault().FoodPrice;
                total += cartItem.Quantity * FoodCost;
                
                OrderFood c = new()
                {
                    Address = address,
                    FoodId = cartItem.FoodId,
                    PhoneReceive = phone,
                    Quantity = cartItem.Quantity,
                    UserId = cartItem.UserId,
                };

                _context.Carts.Remove(cartItem);
                _context.OrderFoods.Add(c);
                _context.SaveChanges();

                Order o = new()
                {
                    DateOrder = DateTime.Now,
                    PaymentId = int.Parse(paymentType),
                    TotalPrice = (int)total,
                    StatusId = 1,
                    OrderFoodId = c.OrderFoodId,
                };

                _context.Orders.Add(o);
                _context.SaveChanges();
            }
            return View();
        }
    }
}
