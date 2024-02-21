using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using UI.MVC.Interfaces;
using UI.MVC.Models;
using UI.MVC.ViewModels;

namespace UI.MVC.Controllers
{
    public class AccountController : Controller
    {
        #region FIELDS
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signinManager;
        private readonly ISendGridEmail sendGridEmail;
        private readonly RoleManager<IdentityRole> roleManager; 
        #endregion

        #region CTOR
        public AccountController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signinManager,
            ISendGridEmail sendGridEmail,
            RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.signinManager = signinManager;
            this.sendGridEmail = sendGridEmail;
            this.roleManager = roleManager;
        }
        #endregion

        [HttpGet]
        public async Task<IActionResult> Register(string? returnUrl = null)
        {

            if(!await roleManager.RoleExistsAsync("Pokemon"))
            {
                await roleManager.CreateAsync(new IdentityRole("Pokemon"));
                await roleManager.CreateAsync(new IdentityRole("Trainer"));
            }

            List<SelectListItem> listItems = new ();

            listItems.Add(new SelectListItem()
            {
                Value = "Pokemon",
                Text = "Pokemon"
            });
            listItems.Add(new SelectListItem()
            {
                Value = "Trainer",
                Text = "Trainer"
            });

            var model = new RegisterViewModel { ReturnUrl = returnUrl, RoleList = listItems };

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
                    if(model.RoleSelected != null && model.RoleSelected.Length > 0 && model.RoleSelected == "Trainer")
                    {
                        await userManager.AddToRoleAsync(user, "Trainer");
                    }
                    else
                    {
                        await userManager.AddToRoleAsync(user, "Pokemon");
                    }


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
        public IActionResult ResetPassword(string? code = null)
        {
            var viewModel = new ResetPasswordViewModel
            {
                Code = code ?? string.Empty
            };
            return code == null ? View("Error") : View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(viewModel.Email);
                if (user == null)
                {
                    ModelState.AddModelError("Email", "User not found");
                    return View();
                }
                var result = await userManager.ResetPasswordAsync(user, viewModel.Code, viewModel.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("ResetPasswordConfirmation");
                }
            }
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
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


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginViewModel model, string? returnurl = null)
        {
            returnurl = returnurl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                //get the info about the user from external login provider
                var info = await signinManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("Error");
                }
                var user = new AppUser { UserName = model.Email, Email = model.Email };
                var result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    //await _userManager.AddToRoleAsync(user, "User");
                    result = await userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await signinManager.SignInAsync(user, isPersistent: false);
                        await signinManager.UpdateExternalAuthenticationTokensAsync(info);
                        return LocalRedirect(returnurl);
                    }
                }
                ModelState.AddModelError("Email", "Error occuresd");
            }
            ViewData["ReturnUrl"] = returnurl;
            return View(model);
        }


    }
}
