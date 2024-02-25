using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UI.MVC.Authorization;
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

builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    opt.AddPolicy("UserAndAdmin", policy => policy.RequireRole("Admin").RequireRole("User"));
    opt.AddPolicy("Admin_CreateAccess", policy => policy.RequireRole("Admin").RequireClaim("create", "True"));
    opt.AddPolicy("Admin_Create_Edit_DeleteAccess", policy => policy.RequireRole("Admin").RequireClaim("create", "True")
    .RequireClaim("edit", "True")
    .RequireClaim("Delete", "True"));
    opt.AddPolicy("Admin_Create_Edit_DeleteAccess_OR_SuperAdmin", policy => policy.RequireAssertion(context =>
    context.User.IsInRole("Admin") && context.User.HasClaim(c => c.Type == "Create" && c.Value == "True")
                        && context.User.HasClaim(c => c.Type == "Edit" && c.Value == "True")
                        && context.User.HasClaim(c => c.Type == "Delete" && c.Value == "True")
                        || context.User.IsInRole("SuperAdmin")));
    opt.AddPolicy("FirstNameAuth", policy => policy.Requirements.Add(new NickNameRequirement("billy")));
    opt.AddPolicy("OnlyBloggerChecker", policy => policy.Requirements.Add(new OnlyBloggerAuthorization()) );
    opt.AddPolicy("CheckNickNameTeddy", policy => policy.Requirements.Add(new NickNameRequirement("Teddy")) );
});

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
