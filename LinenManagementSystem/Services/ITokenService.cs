using LinenManagementSystem.Models;
using System.Security.Claims;

namespace LinenManagementSystem.Services
{
    public interface ITokenService
    {
        string GenerateToken(Employee user);
        Task<Employee?> GetSavedRefreshTokenAsync(int userId);
        Task UpdateRefreshTokenAsync(int userId, string newRefreshToken);
        Task InvalidateRefreshTokenAsync(int userId);
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
