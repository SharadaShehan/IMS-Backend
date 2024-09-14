using System.Diagnostics;
using IMS.Infrastructure.Extensions;
using IMS.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace IMS.Presentation.Filters
{
    public class AuthorizationFilter : Attribute, IAsyncAuthorizationFilter
    {
        private readonly string[] authorizedRoles;

        public AuthorizationFilter(string[] authorizedRoles)
        {
            this.authorizedRoles = authorizedRoles;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            try
            {
                var contextUser = context.HttpContext.User;
                if (!contextUser.Identity.IsAuthenticated)
                {
                    // User is not authenticated
                    context.Result = new UnauthorizedResult();
                    return;
                }
                else
                {
                    string email = contextUser
                        .Claims.FirstOrDefault(c =>
                            c.Type
                                == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"
                            || c.Type == "email"
                        )
                        ?.Value;
                    // Get the database context via service locator
                    var dbContext = ServiceLocator.GetService<DataBaseContext>();
                    var dbUser = await dbContext
                        .users.Where(dbUser => dbUser.IsActive)
                        .FirstOrDefaultAsync(u => u.Email == email);
                    if (dbUser == null)
                    {
                        // User not found in database
                        context.Result = new UnauthorizedResult();
                        return;
                    }
                    else if (!authorizedRoles.Contains(dbUser.Role))
                    {
                        // User does not have any of required roles
                        context.Result = new ForbidResult();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                context.Result = new UnauthorizedResult();
            }
        }
    }
}
