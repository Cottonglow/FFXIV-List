using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace ffxivList
{
    public class AuthorizationHandler
    {
    }

    public class AdminRequirement : IAuthorizationRequirement
    {
        public AdminRequirement(string role)
        {
            Role = role;
        }

        protected String Role { get; set; }
    }

    public class AdminRoleHandler : AuthorizationHandler<AdminRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminRequirement requirement)
        {
            if (!context.User.HasClaim(c => c.Type == "role"))
            {
                // .NET 4.x -> return Task.FromResult(0);
                return Task.CompletedTask;
            }

            var role = context.User.FindFirst(c => c.Type == "role")
                .Value;

            if (role.Equals("Admin"))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
