using System.IdentityModel.Tokens.Jwt;
using System.Text;
using HrmsApi.Data;
using HrmsApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace HrmsApi.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public JwtMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context, HrmsDbContext dbContext)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
                await AttachUserToContext(context, dbContext, token);

            await _next(context);
        }

        private async Task AttachUserToContext(HttpContext context, HrmsDbContext dbContext, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "HrmsDefaultSecretKeyForDevelopment12345");
                
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "sub").Value);
                var userRole = jwtToken.Claims.First(x => x.Type == "role").Value;

                // Attach user to context
                context.Items["User"] = await dbContext.Users.FindAsync(userId);
                context.Items["UserRole"] = userRole;
                
                Console.WriteLine($"User authenticated: ID={userId}, Role={userRole}");
            }
            catch (Exception ex)
            {
                // Do nothing if token validation fails
                // User is not attached to context so the request won't have access to secured endpoints
                Console.WriteLine($"Token validation failed: {ex.Message}");
            }
        }
    }
}
