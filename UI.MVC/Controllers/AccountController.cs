using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.MVC.Models;
using UI.MVC.ViewModels;

namespace UI.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> singinManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> singinManager)
        {
            this.userManager = userManager;
            this.singinManager = singinManager;
        }

        public async Task<IActionResult> Register(string? returnUrl = null)
        {
            var model = new RegisterViewModel { ReturnUrl = returnUrl };

            return View(model);
        }


        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
        {
            model.ReturnUrl = returnUrl;

            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                var user = new AppUser { UserName = model.UserName, Email = model.Email };
                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await singinManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                ModelState.AddModelError("Password", "User could not be created. ");
            }

            return View(model);
        }


    }
}
