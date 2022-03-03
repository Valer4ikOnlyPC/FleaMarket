using Domain.Core;
using Repository.Data;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using Domain.IServices;
using Services.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.FileProviders;
using Mg;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IUserPasswordRepository, UserPasswordRepository>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<ICityRepository, CityRepository>();
builder.Services.AddTransient<IDealRepository, DealRepository>();
builder.Services.AddTransient<IRatingRepository, RatingRepository>();
builder.Services.AddTransient<IFavoritesRepository, FavoritesRepository>();
builder.Services.AddTransient<IFavoritesService, FavoritesService>();
builder.Services.AddTransient<IRatingService, RatingService>();
builder.Services.AddTransient<ICityService, CityService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IFileService, FileService>();
builder.Services.AddTransient<IDealService, DealService>();
builder.Services.AddTransient<IProductPhotoRepository, ProductPhotoRepository>();
builder.Services.AddTransient<ICategoryRepository, CategoryRepository>();
builder.Services.AddTransient<ICategoryService, CategoryService>();
builder.Services.AddTransient<IProductRepository, ProductRepository>();
builder.Services.AddTransient<IProductService, ProductService>();
builder.Services.AddTransient<Migration>();


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
                });


// Add services to the container.
builder.Services.AddControllersWithViews();

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
app.UseDefaultFiles();

if (!Directory.Exists(app.Configuration["FileDirectory"]))
    Directory.CreateDirectory(app.Configuration["FileDirectory"]);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(app.Configuration["FileDirectory"]),
    RequestPath = "/img"
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//var migration = new Migration(app.Configuration);

app.Run();
