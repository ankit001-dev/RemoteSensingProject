using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace RemoteSensingProject.Models
{

    public class JwtAuthorizeAttribute : AuthorizationFilterAttribute
    {
        private readonly string _secretKey = System.Configuration.ConfigurationManager.AppSettings["JwtSecretKey"];
        private readonly string _issuer = System.Configuration.ConfigurationManager.AppSettings["JwtIssuer"];
        private readonly string _audience = System.Configuration.ConfigurationManager.AppSettings["JwtAudience"];

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            // Check if the action OR controller has AllowAnonymous
            bool skipAuthorization = actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any()
                                     || actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();

            if (skipAuthorization)
                return; // Skip JWT validation for login API or any AllowAnonymous API

            // JWT validation logic here
            var authHeader = actionContext.Request.Headers.Authorization;
            if (authHeader == null || authHeader.Scheme != "Bearer" || string.IsNullOrEmpty(authHeader.Parameter))
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, "Missing or invalid token");
                return;
            }

            var token = authHeader.Parameter;

            if (!ValidateToken(token, out ClaimsPrincipal principal))
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, "Invalid or expired token");
                return;
            }

            // Attach principal
            Thread.CurrentPrincipal = principal;
            if (actionContext.RequestContext.Principal != null)
                actionContext.RequestContext.Principal = principal;

            base.OnAuthorization(actionContext);
        }

        private bool ValidateToken(string token, out ClaimsPrincipal principal)
        {
            principal = null;
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public class RoleAuthorizeAttribute : AuthorizationFilterAttribute
    {
        private readonly string[] _roles;

        public RoleAuthorizeAttribute(string roles)
        {
            _roles = roles.Split(',').Select(r => r.Trim()).ToArray();
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var user = Thread.CurrentPrincipal as ClaimsPrincipal;

            if (user == null || !user.Identity.IsAuthenticated)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, "User not authenticated.");
                return;
            }

            // ✅ Support both "role" and ClaimTypes.Role
            var roles = user.Claims
                .Where(c => c.Type == ClaimTypes.Role || c.Type == "role")
                .Select(c => c.Value)
                .ToList();

            System.Diagnostics.Debug.WriteLine("User roles: " + string.Join(", ", roles));

            // Check if user has at least one of the required roles
            if (!_roles.Any(required => roles.Contains(required, StringComparer.OrdinalIgnoreCase)))
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden, "You do not have permission to perform this action.");
                return;
            }

            base.OnAuthorization(actionContext);
        }
    }


}