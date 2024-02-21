using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UI.MVC.Data;
using UI.MVC.Models;

namespace UI.MVC.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<AppUser> userManager;

        #region CTOR

        public UserController(ApplicationDbContext dbContext, UserManager<AppUser> userManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
        }

        #endregion


        public IActionResult Index()
        {
            var userlist = dbContext.AppUser.ToList();
            var roleList = dbContext.UserRoles.ToList();
            var roles = dbContext.Roles.ToList();

            foreach (var user in userlist)
            {
                var role = roleList.FirstOrDefault(r => r.UserId == user.Id);
                if (role == null)
                {
                    user.Role = "None";
                }
                else
                {
                    user.Role = roles.FirstOrDefault(u => u.Id == role.RoleId)?.Name ?? "";
                }
            }

            return View(userlist);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AppUser user)
        {
            if (ModelState.IsValid)
            {
                var userDbValue = dbContext.AppUser.FirstOrDefault(u => u.Id == user.Id);
                if (userDbValue == null)
                {
                    return NotFound();
                }
                var userRole = dbContext.UserRoles.FirstOrDefault(u => u.UserId == userDbValue.Id);
                if (userRole != null)
                {
                    var previousRoleName = dbContext.Roles.Where(u => u.Id == userRole.RoleId).Select(e => e.Name).FirstOrDefault();
                    await userManager.RemoveFromRoleAsync(userDbValue, previousRoleName);

                }

                await userManager.AddToRoleAsync(userDbValue, dbContext.Roles.FirstOrDefault(u => u.Id == user.RoleId).Name);
                dbContext.SaveChanges();
                return RedirectToAction(nameof(Index));
            }


            user.RoleList = dbContext.Roles.Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id
            });
            return View(user);
        }

        public IActionResult Edit(string userId)
        {
            var user = dbContext.AppUser.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return NotFound();
            }
            var userRole = dbContext.UserRoles.ToList();
            var roles = dbContext.Roles.ToList();
            var role = userRole.FirstOrDefault(u => u.UserId == user.Id);
            if (role != null)
            {
                user.RoleId = roles.FirstOrDefault(u => u.Id == role.RoleId).Id;
            }
            user.RoleList = dbContext.Roles.Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id
            });
            return View(user);
        }


        [HttpPost]
        public IActionResult Delete(string userId)
        {
            var user = dbContext.AppUser.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return NotFound();
            }
            dbContext.AppUser.Remove(user);
            dbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }






    }
}
