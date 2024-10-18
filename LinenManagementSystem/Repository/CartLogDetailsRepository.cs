using LinenManagementSystem.Data;
using LinenManagementSystem.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Security.Claims;

namespace LinenManagementSystem.Repository
{
    public class CartLogDetailsRepository : ICartLogDetailsRepository
    {
        private readonly LinenManagementContext _context;

        public CartLogDetailsRepository(LinenManagementContext context)
        {
            _context = context;
        }


        public CartLogDetailsResponse GetCartLog(int cartLogId)
        {
            // Fetching the cartLog based on cartLogId
            var cartLog = _context.CartLogs
                                .Include(cl => cl.Cart)
                                .Include(cl => cl.Location)
                                .Include(cl => cl.Employee)
                                .Include(cl => cl.CartLogDetails)
                                    .ThenInclude(cld => cld.Linen)
                                .FirstOrDefault(cl => cl.CartLogId == cartLogId);

            if(cartLog == null)
            {
                throw new KeyNotFoundException("CartLog Doesnt Exists");
            }

            // If the linen is clean than return linen details else empty property
            var linenDetails = cartLog.Cart.Type == "CLEAN"
                                ? cartLog.CartLogDetails.Select(cld => new LinenDetailDto
                                {
                                    CartLogDetailId = cld.CartLogDetailId,
                                    LinenId = cld.LinenId,
                                    Name = cld?.Linen?.Name ?? "",
                                    Count = cld.Count
                                }).ToList()
                                : new List<LinenDetailDto>();


            CartLogDetailsResponse response =  new CartLogDetailsResponse
            {
                
                    CartLogId = cartLog.CartLogId,
                    ReceiptNumber = cartLog.ReceiptNumber,
                    ReportedWeight = cartLog.ReportedWeight,
                    ActualWeight = cartLog.ActualWeight,
                    Comments = cartLog.Comments,
                    DateWeighed = cartLog.DateWeighed,
                    Cart = new CartDto
                    {
                        CartId = cartLog.Cart.CartId,
                        Name = cartLog.Cart.Name,
                        Weight = cartLog.Cart.Weight,
                        Type = cartLog.Cart.Type
                    },
                    Location = new LocationDto
                    {
                        LocationId = cartLog.Location.LocationId,
                        Name = cartLog.Location.Name,
                        Type = cartLog.Location.Type
                    },
                    Employee = new EmployeeDto
                    {
                        EmployeeId = cartLog.Employee.EmployeeId,
                        Name = cartLog.Employee.Name
                    },
                    Linen = linenDetails

            };

            return response;

        }

        public List<CartLogDetailsResponse> GetCartLogs(string? cartType, int? locationId,  int? employeeId)
        {

            var cartLogsQuery = _context.CartLogs
                                        .Include(cl => cl.Cart)
                                        .Include(cl => cl.Location)
                                        .Include(cl => cl.Employee)
                                        .Include(cl => cl.CartLogDetails)
                                            .ThenInclude(cld => cld.Linen)
                                        .AsQueryable();
            // Applying filters
            if (!string.IsNullOrEmpty(cartType))
            {
                cartLogsQuery = cartLogsQuery.Where(cl => cl.Cart.Type == cartType.ToUpper());
                if(cartLogsQuery == null)
                {
                    throw new KeyNotFoundException("Cart Not Found for this type");
                }
            }

            if (locationId.HasValue)
            {
                cartLogsQuery = cartLogsQuery.Where(cl => cl.LocationId == locationId.Value);
                if (cartLogsQuery == null)
                {
                    throw new KeyNotFoundException("Cart Not Found for this location");
                }
            }

            if (employeeId.HasValue)
            {
                cartLogsQuery = cartLogsQuery.Where(cl => cl.EmployeeId == employeeId.Value);
                if (cartLogsQuery == null)
                {
                    throw new KeyNotFoundException("Cart Not Found for this Employee");
                }
            }

            // Order by DateWeighed DESC
            cartLogsQuery = cartLogsQuery.OrderByDescending(cl => cl.DateWeighed);
            if (cartLogsQuery == null)
            {
                throw new KeyNotFoundException("Cart Not Found");
            }

            List<CartLogDetailsResponse> response = cartLogsQuery.ToList().Select(cartLog => new CartLogDetailsResponse
            {

                CartLogId = cartLog.CartLogId,
                ReceiptNumber = cartLog.ReceiptNumber,
                ReportedWeight = cartLog.ReportedWeight,
                ActualWeight = cartLog.ActualWeight,
                Comments = cartLog.Comments,
                DateWeighed = cartLog.DateWeighed,
                Cart = new CartDto
                {
                    CartId = cartLog.Cart.CartId,
                    Name = cartLog.Cart.Name,
                    Weight = cartLog.Cart.Weight,
                    Type = cartLog.Cart.Type
                },
                Location = new LocationDto
                {
                    LocationId = cartLog.Location.LocationId,
                    Name = cartLog.Location.Name,
                    Type = cartLog.Location.Type
                },
                Employee = new EmployeeDto
                {
                    EmployeeId = cartLog.Employee.EmployeeId,
                    Name = cartLog.Employee.Name
                },
                Linen = cartLog.CartLogDetails != null ? cartLog.CartLogDetails.Select(cld => new  LinenDetailDto{
                    CartLogDetailId = cld.CartLogDetailId,
                    LinenId = cld.LinenId,
                    Name = cld?.Linen?.Name ?? "" ,
                    Count = cld.Count
                }).ToList() : new List<LinenDetailDto>()

            }).ToList();

            return response;

        }
        public async Task<bool> DeleteCartLog(int cartLogId, int currentEmployeeId)
        {
            var cartLog = _context.CartLogs.FirstOrDefault(cl => cl.CartLogId == cartLogId);

            if (cartLog == null)
            {
                throw new KeyNotFoundException("Cart log not found");
            }

            // Ensure the employee trying to delete the cart log is the one who created it
            if (cartLog.EmployeeId != currentEmployeeId)
            {
                throw new UnauthorizedAccessException("You are not allowed to delete this cart log.");
            }

            // Remove the cart log details first
            var cartLogDetails = _context.CartLogDetails.Where(cld => cld.CartLogId == cartLogId);
            if (cartLogDetails != null)
            {
                _context.CartLogDetails.RemoveRange(cartLogDetails);
            }

            // Remove the cart log itself
            _context.CartLogs.Remove(cartLog);
           
            return await _context.SaveChangesAsync() > 0;

        }

        public InsertUpdateCartLogResponse UpsertCartLog(InsertUpdateCartLogDTO cartLogDto,int currentEmployeeId)
        {
            // Check if cartLogId is provided (update operation) or null (create operation)
            if (cartLogDto.CartLogId.HasValue)
            {
                // Fetch the cart log from the database
                var existingCartLog = _context.CartLogs
                                        .Include(cl => cl.CartLogDetails)
                                        .ThenInclude(cld => cld.Linen)
                                        .FirstOrDefault(cl => cl.CartLogId == cartLogDto.CartLogId);

                if (existingCartLog == null)
                {
                    throw new KeyNotFoundException("the cart You want to update couldnt be found");
                }

                // Ensure the employee trying to update the cart log is the one who created it
                if (existingCartLog.EmployeeId != currentEmployeeId)
                {
                    throw new UnauthorizedAccessException("You are not allowed to edit this cart log.");
                }

                // Update the existing cart log with new values
                existingCartLog.ReceiptNumber = cartLogDto.ReceiptNumber;
                existingCartLog.ReportedWeight = cartLogDto.ReportedWeight;
                existingCartLog.ActualWeight = cartLogDto.ActualWeight;
                existingCartLog.Comments = cartLogDto.Comments;
                existingCartLog.DateWeighed = cartLogDto.DateWeighed;
                existingCartLog.CartId = cartLogDto.CartId;
                existingCartLog.LocationId = cartLogDto.LocationId;

                // Update CartLogDetails
                _context.CartLogDetails.RemoveRange(existingCartLog.CartLogDetails); // Remove old details
                foreach (var detail in cartLogDto.Linen)
                {
                    _context.CartLogDetails.Add(new CartLogDetail
                    {
                        CartLogId = existingCartLog.CartLogId,
                        LinenId = detail.LinenId,
                        Count = detail.Count
                    });
                }

                _context.SaveChanges();

                var response = new InsertUpdateCartLogResponse
                {
                    CartLogId = existingCartLog.CartLogId,
                    ReceiptNumber = existingCartLog.ReceiptNumber,
                    ReportedWeight = existingCartLog.ReportedWeight,
                    ActualWeight = existingCartLog.ActualWeight,
                    Comments = existingCartLog.Comments,
                    DateWeighed = existingCartLog.DateWeighed,
                    CartId = existingCartLog.CartId,
                    LocationId = existingCartLog.LocationId,
                    EmployeeId = existingCartLog.EmployeeId,
                    Linen = existingCartLog.CartLogDetails != null? existingCartLog.CartLogDetails.Select(cld => new LinenDetailDto
                    {
                        CartLogDetailId = cld.CartLogDetailId,
                        LinenId = cld.LinenId,
                        Name = cld?.Linen?.Name ?? "",
                        Count = cld.Count
                    }).ToList() : new List<LinenDetailDto>()


                };
                 return response;
            }
            else
            {
                // Create a new cart log
                var newCartLog = new CartLog
                {
                    ReceiptNumber = cartLogDto.ReceiptNumber,
                    ReportedWeight = cartLogDto.ReportedWeight,
                    ActualWeight = cartLogDto.ActualWeight,
                    Comments = cartLogDto.Comments,
                    DateWeighed = cartLogDto.DateWeighed,
                    CartId = cartLogDto.CartId,
                    LocationId = cartLogDto.LocationId,
                    EmployeeId = cartLogDto.EmployeeId  // Set the creator as the current employee
                };

                _context.CartLogs.Add(newCartLog);
                _context.SaveChanges();

                // Add CartLogDetails
                foreach (var detail in cartLogDto.Linen)
                {
                    _context.CartLogDetails.Add(new CartLogDetail
                    {
                        CartLogId = newCartLog.CartLogId,
                        LinenId = detail.LinenId,
                        Count = detail.Count
                    });
                }

                _context.SaveChanges();

                var response = new InsertUpdateCartLogResponse
                {
                    CartLogId = newCartLog.CartLogId,
                    ReceiptNumber = newCartLog.ReceiptNumber,
                    ReportedWeight = newCartLog.ReportedWeight,
                    ActualWeight = newCartLog.ActualWeight,
                    Comments = newCartLog.Comments,
                    DateWeighed = newCartLog.DateWeighed,
                    CartId = newCartLog.CartId,
                    LocationId = newCartLog.LocationId,
                    EmployeeId = newCartLog.EmployeeId,
                    Linen = (from cld in newCartLog.CartLogDetails
                                     join linen in _context.Linens on cld.LinenId equals linen.LinenId
                                     select new LinenDetailDto
                                     {
                                         CartLogDetailId = cld.CartLogDetailId,
                                         LinenId = cld.LinenId,
                                         Name = linen.Name,
                                         Count = cld.Count
                                     }).ToList()



                };
                return response;
            }

            
        }

    }
}
