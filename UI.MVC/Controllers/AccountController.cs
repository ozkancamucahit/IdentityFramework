using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.MVC.Interfaces;
using UI.MVC.Models;
using UI.MVC.ViewModels;

namespace UI.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signinManager;
        private readonly ISendGridEmail sendGridEmail;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signinManager,
            ISendGridEmail sendGridEmail)
        {
            this.userManager = userManager;
            this.signinManager = signinManager;
            this.sendGridEmail = sendGridEmail;
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
                    await signinManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                ModelState.AddModelError("Password", "User could not be created. ");
            }

            return View(model);
        }


        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {

            LoginViewModel loginViewModel = new();
            loginViewModel.ReturnUrl = returnUrl ?? Url.Content("~/");

            return View(loginViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Invalid login attempt");
                return View();
            }

            var result = await signinManager.PasswordSignInAsync(loginViewModel.UserName, loginViewModel.Password, loginViewModel.RememberMe, lockoutOnFailure: true);

            if(result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            if (result.IsLockedOut)
            {
                return View("Lockout");
            }
            else
            {
                ModelState.AddModelError("", "Invalid login attempt");
                return View(loginViewModel);
            }
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if(user is null)
                {
                    return RedirectToAction("ForgotPasswordConfirmation");
                }
                string? code = await userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code}, protocol: HttpContext.Request.Scheme);

                string link = $"""
                    <a href='{callbackUrl}' target='_blank'> Click to reset your password</a>
                    """;

                await sendGridEmail.SendEmailAsync(user.Email, "Reset your password", link);

                return RedirectToAction("ForgotPasswordConfirmation");
            }
            else
            {
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }


        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await signinManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }


    }
}
