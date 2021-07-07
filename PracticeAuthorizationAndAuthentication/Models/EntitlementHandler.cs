using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PracticeAuthorizationAndAuthentication.Models
{
    public class EntitlementHandler : AuthorizationHandler<EntitlementRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, EntitlementRequirement requirement)
        {
            //    var x = context.User.Claims.Where(x => x.Type.Equals("Tech"));
            //    var y = context.User.Claims.Where(x => x.Type.Equals("Stud"));

            //    if (x.Any() && y.Any())
            //    {
            //        context.Succeed(requirement);
            //    }

            //    return Task.CompletedTask;
            //

            var hasSendClaim = context.User.Claims.Any(x => x.Type.Equals("Mail"));

            if (hasSendClaim)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}