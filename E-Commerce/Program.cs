<<<<<<< HEAD
<<<<<<< HEAD
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

using System.Security.Claims;

using E_Commerce.Helpers;
using E_Commerce.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(3);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// USER LOGIN
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = "/User/Login";
    // Controller
    options.AccessDeniedPath = "/Home/AccessDenied";
});

// ADMIN ROLE
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policy => policy.RequireClaim(ClaimTypes.Role, "0"));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
=======
=======
>>>>>>> f08b466 (Setup peoject)
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
<<<<<<< HEAD
>>>>>>> f08b466 (Setup peoject)
=======
>>>>>>> f08b466 (Setup peoject)
    app.UseHsts();
}

app.UseHttpsRedirection();
<<<<<<< HEAD
<<<<<<< HEAD

=======
>>>>>>> f08b466 (Setup peoject)
=======
>>>>>>> f08b466 (Setup peoject)
app.UseStaticFiles();

app.UseRouting();

<<<<<<< HEAD
<<<<<<< HEAD
app.UseSession();

app.UseAuthentication();

=======
>>>>>>> f08b466 (Setup peoject)
=======
>>>>>>> f08b466 (Setup peoject)
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
<<<<<<< HEAD
<<<<<<< HEAD
    pattern: "{controller=Home}/{action=Index}/{id?}"
);
=======
    pattern: "{controller=Home}/{action=Index}/{id?}");
>>>>>>> f08b466 (Setup peoject)
=======
    pattern: "{controller=Home}/{action=Index}/{id?}");
>>>>>>> f08b466 (Setup peoject)

app.Run();
