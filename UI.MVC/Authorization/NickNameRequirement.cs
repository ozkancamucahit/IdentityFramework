using Microsoft.AspNetCore.Authorization;

namespace UI.MVC.Authorization
{
    public sealed class NickNameRequirement : IAuthorizationRequirement
    {
        public string Name { get; set; }

        public NickNameRequirement(string name)
        {
            Name = name;
        }
    }
}
