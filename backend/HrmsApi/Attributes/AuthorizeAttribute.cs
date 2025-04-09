using HrmsApi.Modules.Auth.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HrmsApi.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _roles;

        public AuthorizeAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Skip authorization if action is decorated with [AllowAnonymous] attribute
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.Any(em => 
                em.GetType() == typeof(Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute));
            
            if (allowAnonymous)
                return;

            // Authorization
            var user = (User?)context.HttpContext.Items["User"];
            var userRole = (string?)context.HttpContext.Items["UserRole"];
            
            if (user == null)
            {
                // Not logged in or token expired/invalid
                context.Result = new JsonResult(new { message = "Unauthorized" }) 
                { 
                    StatusCode = StatusCodes.Status401Unauthorized 
                };
                return;
            }

            // Check if role is required and user has the required role
            if (_roles.Any() && !_roles.Contains(userRole))
            {
                // User doesn't have the required role
                context.Result = new JsonResult(new { message = "Forbidden" }) 
                { 
                    StatusCode = StatusCodes.Status403Forbidden 
                };
                return;
            }
        }
    }
}
