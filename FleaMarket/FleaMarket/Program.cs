using Repository.Core;
using Repository.Data;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

string connectionString = "Host=localhost;Port=5433;Database=testDB;Username=postgres;Password=2077";
builder.Services.AddTransient<IUserRepository, UserRepository>(provider => new UserRepository(connectionString, new UserPasswordRepository(connectionString)));


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

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
