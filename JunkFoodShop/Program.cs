using JunkFoodShop.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Lowercase Route URL
builder.Services.AddRouting(options => options.LowercaseUrls = true);

// Connect Database
builder.Services.AddDbContext<JunkFoodShopContext>(option
    => option.UseSqlServer(builder.Configuration.GetConnectionString("JunkFoodShop") ?? throw new InvalidOperationException("The connect string is wrong or database not found")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(
    option =>
    {
        option.LoginPath = "/Account/SignIn";
        option.AccessDeniedPath = "/Account/Denied";
    }
);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", authBuilder =>
    {
        authBuilder.RequireRole("Admin");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
