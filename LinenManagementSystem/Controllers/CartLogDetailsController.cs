using LinenManagementSystem.Data;
using LinenManagementSystem.Models;
using LinenManagementSystem.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LinenManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartLogDetailsController : ControllerBase
    {
        private readonly ICartLogDetailsRepository _repository;
        private readonly ILogger<CartLogDetailsController> _logger;
        public CartLogDetailsController(ICartLogDetailsRepository repository, ILogger<CartLogDetailsController> logger) { 
           
            _repository = repository;
            _logger = logger;
        
        }

        [Authorize]
        [HttpGet("{cartLogId}")]
        public IActionResult GetCartLog(int cartLogId)
        {
            try
            {
                _logger.LogTrace("Entered GetCartLog method.");
                var response = _repository.GetCartLog(cartLogId);
                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogError("An UnauthorizedAccessException error occurred in GetCartLog method.");
                return Forbid();
            }
            catch (KeyNotFoundException) {
                _logger.LogError("A KeyNotFoundException error occurred in GetCartLog method.");
                return NotFound(new { message = "Cart log not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetCartLog method.");
                return StatusCode(500, new { message = "An error occurred", details = ex.Message });
            }

        }

        [Authorize]
        [HttpGet]
        public IActionResult GetCartLogs(string? cartType, int? locationId,int? employeeId)
        {
            try {
                _logger.LogTrace("Entered GetCartLogs method.");
                // Execute the query and prepare the response
                var cartLogs = _repository.GetCartLogs(cartType, locationId, employeeId);

                return Ok(cartLogs);
            }
            catch (KeyNotFoundException)
            {
                _logger.LogError("A KeyNotFoundException error occurred in GetCartLogs method.");
                return NotFound(new { message = "Cart log not found" });
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex,"An error occurred in GetCartLogs method.");
                return StatusCode(500, new { message = "An error occurred", details = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("upsert")]
        public IActionResult UpsertCartLog(InsertUpdateCartLogDTO cartLogDto)
        {
            try
            {
                _logger.LogTrace("Entered UpsertCartLog method.");
                // Retrieve the currently authenticated employee (assuming JWT is used for authentication)
                var currentEmployeeId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                var response = _repository.UpsertCartLog(cartLogDto, currentEmployeeId);

                return Ok(response);

            }
            catch (UnauthorizedAccessException) 
            {
                _logger.LogError("An UnauthorizedAccessException error occurred in UpsertCartLog method.");
                return Forbid();
            }
            catch (KeyNotFoundException) 
            {
                _logger.LogError("A KeyNotFoundException error occurred in UpsertCartLog method.");
                return NotFound(new { message = "Cart log not found" });
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex,"An error occurred in UpsertCartLog method.");
                return StatusCode(500, new { message = "An error occurred", details = ex.Message });
            }
            
        }

        [Authorize]
        [HttpDelete("{cartLogId}")]
        public async Task<IActionResult> DeleteCartLog(int cartLogId)
        {
            try
            {
                _logger.LogTrace("Entered DeleteCartLog method.");
                // Retrieve the currently authenticated employee (assuming JWT is used for authentication)
                var currentEmployeeId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                // Call service to delete the cart log
                var isDeleted = await _repository.DeleteCartLog(cartLogId, currentEmployeeId);

                if (isDeleted)
                {
                    return Ok(new { message = "Cart log deleted successfully" });
                }
                _logger.LogError("A BadRequest error occurred in DeleteCartLog method.");
                return BadRequest(new { message = "Error deleting cart log" });
                
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogError("An UnauthorizedAccessException error occurred in DeleteCartLog method.");
                return Forbid();
            }
            catch (KeyNotFoundException)
            {
                _logger.LogError("A KeyNotFoundException error occurred in DeleteCartLog method.");
                return NotFound(new { message = "Cart log not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"An error occurred in DeleteCartLog method.");
                return StatusCode(500, new { message = "An error occurred", details = ex.Message });
            }
        }

       








    }
}
