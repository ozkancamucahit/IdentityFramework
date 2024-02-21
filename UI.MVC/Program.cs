using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UI.MVC.Data;
using UI.MVC.Helpers;
using UI.MVC.Interfaces;
using UI.MVC.Models;
using UI.MVC.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(e =>
{
    e.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
})
    .AddTransient<ISendGridEmail, SendGridEmail>()
    .Configure<AuthMessageSenderOptions>(builder.Configuration.GetSection("SendGrid"))
    .AddIdentity<AppUser, IdentityRole>(opt =>
    {
        opt.Password.RequireDigit = true;
        opt.Password.RequireLowercase = true;
        opt.Password.RequireUppercase = true;
        opt.Password.RequireNonAlphanumeric = false;
        opt.Password.RequiredLength = 6;
        opt.Lockout.MaxFailedAccessAttempts = 5;
        opt.SignIn.RequireConfirmedPhoneNumber= false;
        //opt.SignIn.RequireConfirmedAccount= true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication()
    .AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
});
    //.AddFacebook(facebookOptions =>
    //{

    //});

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
