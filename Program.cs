using Memochka.Models.Entities;
using Memochka.Models.MemochkaDbContext;
using Memochka.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container. 
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDbContext<MemochkaContext>(db =>
    db.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
//Add Authentication 
builder.Services.AddAuthentication(options =>
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();
builder.Services.AddTransient<ICreateUser<User>, UserService>();
//builder.Services.AddIdentity<User, IdentityRole>();

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
    pattern: "{controller=Home}/{action=MainPage}/{id?}");

app.Run();
