﻿using Mango.Web.Service;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddScoped<ITokenProvider, TokenProvider>();
builder.Services.AddHttpClient<ICouponService, CouponService>();
builder.Services.AddHttpClient<ICartService, CartService>();
builder.Services.AddHttpClient<IProductService, ProductService>();
builder.Services.AddHttpClient<IAuthService, AuthService>();
builder.Services.AddScoped<IBaseService, BaseService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<ICartService, CartService>();
Mango.Web.Utility.StaticDetails.CouponAPIBase = builder.Configuration.GetSection("ServiceUrls:CouponAPI").Value!;
Mango.Web.Utility.StaticDetails.AuthAPIBase = builder.Configuration.GetSection("ServiceUrls:AuthAPI").Value!;
Mango.Web.Utility.StaticDetails.ProductAPIBase = builder.Configuration.GetSection("ServiceUrls:ProductAPI").Value!;
Mango.Web.Utility.StaticDetails.ShoppingCartAPIBase = builder.Configuration.GetSection("ServiceUrls:ShoppingCartAPI").Value!;
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromHours(10);
    options.LoginPath = "/Auth/Login";
    options.AccessDeniedPath = "/Auth/AccessDenied";
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

