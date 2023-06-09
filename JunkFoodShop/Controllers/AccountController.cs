﻿using JunkFoodShop.Data;
using JunkFoodShop.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace JunkFoodShop.Controllers
{
    public class AccountController : Controller
    {
        private readonly JunkFoodShopContext _context;
        private const int KeyLen = 10000;
        private const string KeyName = "JunkFoodShop";

        public AccountController(JunkFoodShopContext context)
        {
            _context = context;
        }

        #region SignUp

        // Display sign up
        public IActionResult SignUp()
        {
            return View();
        }

        // Function Sign Up
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp([Bind("Username,FullName,Email,PhoneNumber,Password,ConfirmPassword")] SignUp signUp)
        {
            // Checking Username and Email
            bool CheckUsername = await _context.UserAccounts.AnyAsync(x => x.Username == signUp.Username);
            bool CheckEmail = await _context.UserAccounts.AnyAsync(x => x.Email == signUp.Email);
            bool CheckPhoneNumber = await _context.UserAccounts.AnyAsync(x => x.PhoneNumber == signUp.PhoneNumber);

            if (CheckUsername)
            {
                ViewBag.Error = "Username is already exist!";
                return View(signUp);
            }
            else if (CheckPhoneNumber)
            {
                ViewBag.Error = "Your phone number is already exist!";
                return View(signUp);
            }
           
            else if (CheckEmail)
            {
                ViewBag.Error = "Email is already exist";
                return View(signUp);
            }
            else
            {
                // Encode Password 
                #region Encode Password
                byte[] encode = new byte[KeyLen];
                encode = System.Text.Encoding.UTF8.GetBytes(KeyName);

                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: signUp.Password!,
                    salt: encode,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8));

                signUp.Password = hashed;
                #endregion

                // Save account to database
                #region Save account
                UserAccount user = new()
                {
                    Username = signUp.Username,
                    Email = signUp.Email,
                    Password = signUp.Password,
                    PhoneNumber = signUp.PhoneNumber,
                    FullName = signUp.FullName,
                };

                await _context.AddAsync(user);
                await _context.SaveChangesAsync();
                #endregion

                TempData["Success"] = "Sign Up Successfully";
                return RedirectToAction(nameof(SignIn));
            }

        }
        #endregion

        #region SignIn

        // Display Sign In
        public IActionResult SignIn()
        {
            ViewBag.SignUpSuccess = TempData["Success"]?.ToString();
            ViewBag.Message = TempData["Message"]?.ToString();
            return View();
        }

        // Function Sign In
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn([Bind("UsernameEmail,Password")] SignIn signIn)
        {
            // Check Username and Email
            var isUsernameOrEmail = await _context.UserAccounts.Where(x => x.Username == signIn.UsernameEmail || x.Email == signIn.UsernameEmail).FirstOrDefaultAsync();
            
            // Check if sign in as Admin
            if (signIn.UsernameEmail == "Admin" && signIn.Password == "123456")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, "Admin"),
                    new Claim(ClaimTypes.Name, "Admin"),
                    new Claim(ClaimTypes.Role, "Admin")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync(claimsPrincipal);
                return RedirectToAction("Index", "Admin");
            }

            // Check if sign in as User
            else if (isUsernameOrEmail != null)
            {
                // Encode
                byte[] encode = new byte[KeyLen];
                encode = System.Text.Encoding.UTF8.GetBytes(KeyName);

                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: signIn.Password!,
                    salt: encode,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8));

                signIn.Password = hashed;
            }
            else
            {
                ViewBag.Error = "Username or Email is not exist";
                return View(signIn);
            }

            // Check password
            bool isPasswordMatch = string.Equals(signIn.Password, isUsernameOrEmail!.Password);

            if (isPasswordMatch)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, signIn.UsernameEmail),
                    new Claim(ClaimTypes.Name, signIn.UsernameEmail),
                    new Claim(ClaimTypes.Role, "User")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync(claimsPrincipal);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ErrorPassword = "Password is not match";
            return View(signIn);
        }
        #endregion

        #region SignOut
        // Sign Out
        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region Access Denied
        // Access Denied
        public IActionResult Denied()
        {
            if (User.Identity!.IsAuthenticated)
            {
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("Index", "Admin");
                }
            }
            return View();
        }
        #endregion

    }
}
