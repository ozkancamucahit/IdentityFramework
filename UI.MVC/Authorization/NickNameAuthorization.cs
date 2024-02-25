using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using UI.MVC.Data;
using UI.MVC.Models;

namespace UI.MVC.Authorization
{
    public sealed class NickNameAuthorization : AuthorizationHandler<NickNameRequirement>
    {
        private readonly UserManager<AppUser> userManager;
        private readonly ApplicationDbContext db;

        public NickNameAuthorization(UserManager<AppUser> userManager, ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.db = context;
        }


        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, NickNameRequirement requirement)
        {
            string userId = context.User.FindFirst(ClaimTypes.NameIdentifier).Value;


            var user = db.AppUser.FirstOrDefault(u => u.Id == userId);
            var claims = Task.Run(async () => await userManager.GetClaimsAsync(user)).Result;
            var claim = claims.FirstOrDefault(c => c.Type == "NickName");

            if(claim != null)
            {
                if (claim.Value.ToLower().Contains(requirement.Name.ToLower()))
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }
            return Task.CompletedTask;


        }
    }
}
