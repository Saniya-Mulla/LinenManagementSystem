using LinenManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace LinenManagementSystem.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;

        public TokenService(JwtSettings jwtSettings)
        {
            _jwtSettings = jwtSettings;
        }

        public string GenerateToken(Employee user)
        {
            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.EmployeeId.ToString()),
                new Claim(ClaimTypes.Name, user.Name)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task InvalidateRefreshTokenAsync(int userId)
        {
            using (var context = new LinenManagementContext())
            {
                var employee = await context.Employees.FirstOrDefaultAsync(e => e.EmployeeId == userId);
                if (employee == null)
                {
                    throw new KeyNotFoundException("Employee Not Found");
                }
                if (employee != null)
                {
                    // Clear the refresh token and expiry time
                    employee.RefreshToken = null;

                    // Save changes to the database
                    await context.SaveChangesAsync();
                }
            }
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)),
                ValidateLifetime = false 
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        public async Task<Employee?> GetSavedRefreshTokenAsync(int userId)
        {
            using (var context = new LinenManagementContext())
            {
                // Find the employee by their string Id (from IdentityUser)
                return await context.Employees.FirstOrDefaultAsync(t => t.EmployeeId == userId && t.RefreshToken != null);
            }
        }

        public async Task UpdateRefreshTokenAsync(int userId, string newRefreshToken)
        {
            using (var context = new LinenManagementContext())
            {
                // Retrieve the user's refresh token entry
                var refreshToken = await context.Employees
                    .FirstOrDefaultAsync(t => t.EmployeeId == userId);

                if (refreshToken != null)
                {
                    // Invalidate the old refresh token
                    refreshToken.RefreshToken = newRefreshToken;

                    // Save changes to the database
                    await context.SaveChangesAsync();
                }
                
            }


        }
    }
}
