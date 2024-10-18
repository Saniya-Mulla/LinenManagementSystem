using Azure.Core;
using LinenManagementSystem.Data;
using LinenManagementSystem.Models;
using LinenManagementSystem.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace LinenManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly JwtSettings _jwtSettings;
        private readonly LinenManagementContext _context;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(JwtSettings jwtSettings,LinenManagementContext context, ITokenService tokenService, ILogger<AuthController> logger)
        {
            _jwtSettings = jwtSettings;
            _context = context;
            _tokenService = tokenService;
            _logger = logger;
        }

        [HttpPost("HashPassword")]
        public async Task<IActionResult> HashPassword(LoginDTO request)
        {
            var login = _context.Employees.FirstOrDefault(u => u.Email == request.Email);
       
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(login.Password);

            if (login != null)
            {
                login.Password = hashedPassword;

                await _context.SaveChangesAsync();
            }

            return Ok();
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO users)
        {
            _logger.LogTrace("Entered Login method.");
            var login = _context.Employees.FirstOrDefault(u => u.Email == users.Email);

            // Verifying password during authentication
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(users.Password,login.Password);
            if (login == null || !isPasswordValid)
            {
                _logger.LogError("An Unauthorized error occurred in Login method.");
                return Unauthorized();
            }

            var token = _tokenService.GenerateToken(login);
            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));


            if (login != null)
            {
                login.RefreshToken = refreshToken;

                 await _context.SaveChangesAsync();
            }
            else
            {
                _logger.LogError("An Employee not found error occurred in Login method.");
                throw new Exception("Employee not found.");

            }
            return Ok(new  { AccessToken = token , RefreshToken = refreshToken });

        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenDTO model)
        {
            _logger.LogTrace("Entered Refresh method.");
            var principal = _tokenService.GetPrincipalFromExpiredToken(model.AccessToken);
            if (principal == null)
            {
                _logger.LogError("An BadRequest error occurred in Refresh method.");
                return BadRequest("Invalid access token");
            }

            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var emp = new Employee
            {
                EmployeeId = int.Parse(principal.FindFirst(ClaimTypes.NameIdentifier)?.Value),
                Name = principal.FindFirst(ClaimTypes.Name)?.Value
            };
            var savedRefreshToken = await _tokenService.GetSavedRefreshTokenAsync(int.Parse(userId));

            if (savedRefreshToken.RefreshToken != model.RefreshToken)
            {
                _logger.LogError("An Unauthorized error occurred in Refresh method.");
                return Unauthorized();
            }

            var newAccessToken = _tokenService.GenerateToken(emp);
            var newRefreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)); 

            // Update refresh token in the database
            await _tokenService.UpdateRefreshTokenAsync(int.Parse(userId), newRefreshToken);

            return Ok(new { RefreshToken = newRefreshToken });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            _logger.LogTrace("Entered Logout method.");
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (userId == null)
            {
                _logger.LogError("An Unauthorized error occurred in Logout method.");
                return Unauthorized();
            }

            try
            {
                // Call method to remove or invalidate the refresh token
                await _tokenService.InvalidateRefreshTokenAsync(userId);

                return Ok(new { message = "Logged out successfully." });
            }
            catch (KeyNotFoundException)
            {
                _logger.LogError("A KeyNotFoundException error occurred in Logout method.");
                return NotFound(new { message = "Employee not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in Logout method.");
                return StatusCode(500, new { message = "An error occurred", details = ex.Message });
            }


        }

        



      

       










    }

}
