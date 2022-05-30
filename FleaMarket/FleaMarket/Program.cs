using Domain.Core;
using Repository.Data;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using Domain.IServices;
using Services.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.FileProviders;
using Mg;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using FleaMarket.Models;
using FleaMarket.Hubs;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using Serilog.Events;
using SerilogWeb.Classic.Enrichers;
using AspNetCore.Yandex.ObjectStorage.Extensions;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration()
  .ReadFrom.Configuration(builder.Configuration)
  .Enrich.FromLogContext()
  .Enrich.WithHttpRequestId()
  .WriteTo.Console()
  .WriteTo.Seq("http://localhost:7163")
  .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);


builder.Services.AddTransient<IUserPasswordRepository, UserPasswordRepository>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<ICityRepository, CityRepository>();
builder.Services.AddTransient<IDealRepository, DealRepository>();
builder.Services.AddTransient<IRatingRepository, RatingRepository>();
builder.Services.AddTransient<IFavoritesRepository, FavoritesRepository>();
builder.Services.AddTransient<IDialogRepository, DialogRepository>();
builder.Services.AddTransient<IMessageRepository, MessageRepository>();
builder.Services.AddTransient<IDialogService, DialogService>();
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
builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
builder.Services.AddSingleton<IFfmpegService, FfmpegService>();
builder.Services.AddSingleton<IStorageService, StorageService>();
builder.Services.AddMemoryCache();
builder.Services.AddYandexObjectStorage(options =>
{
    options.BucketName = "flea-market-backet";
    options.AccessKey = "YCAJEjfNlPTDm4tQyMZo9Zv2v";
    options.SecretKey = "YCM3Ttllmxgjo7cOAHp02mA9Z-9bOPaiZY-LaEG9";
});


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
                });
builder.Services.AddDataProtection()
    .SetApplicationName("FleaMarket")
    .PersistKeysToFileSystem(new DirectoryInfo("C:\\inetpub\\vhosts\\u1687197.plsk.regruhosting.ru\\temp-keys"))
    .SetDefaultKeyLifetime(TimeSpan.FromDays(30));


builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseStatusCodePagesWithReExecute("/Home/Error404");
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseDefaultFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ChatHub>("/chat");
    endpoints.MapDefaultControllerRoute();
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
