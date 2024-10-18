using LinenManagementSystem;
using LinenManagementSystem.Controllers;
using LinenManagementSystem.Data;
using LinenManagementSystem.Models;
using LinenManagementSystem.Repository;
using LinenManagementSystem.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;

namespace UnitTests
{
    public class UnitTest1
    {
        public Mock<ICartLogDetailsRepository> mock = new Mock<ICartLogDetailsRepository>();
        public Mock<ILogger<CartLogDetailsController>> mockLogger = new Mock<ILogger<CartLogDetailsController>>();

        [Fact]
        public async Task GetCartLogById_Return_OkResult()
        {
            // Arrange
            var cartLogDetailsResponseDTO = new CartLogDetailsResponse()
            {
                CartLogId= 2,
                ReceiptNumber= "123ABC",
                ReportedWeight= 50,
                ActualWeight= 51,
                Comments = "Extra blanket received",
                DateWeighed = Convert.ToDateTime("2024-10-08T13:41:00"),
                Cart = new CartDto
                {
                    CartId = 1,
                    Name = "Cart - Small",
                    Weight = 20,
                    Type = "CLEAN"
                },
                Location = new LocationDto
                {
                    LocationId = 2,
                    Name = "101A",
                    Type = "CLEAN_ROOM"
                },
                Employee = new EmployeeDto
                {
                    EmployeeId = 2,
                    Name = "John"
                },
                Linen = new List<LinenDetailDto>
                {
                   new LinenDetailDto { CartLogDetailId = 1, LinenId = 1, Name = "Bedsheet - Small",Count = 10
                   },
                   new LinenDetailDto { CartLogDetailId = 2, LinenId = 6, Name = "Pillowcase - Large",Count = 5
                   },
                   new LinenDetailDto { CartLogDetailId = 3, LinenId = 16, Name = "Towel - Large",Count = 2
                   },
                   new LinenDetailDto { CartLogDetailId = 4, LinenId = 4, Name = "Pillowcase - Small",Count = 15
                   },
                   new LinenDetailDto { CartLogDetailId = 5, LinenId = 9, Name = "Blanket - Large",Count = 1
                   },
                   new LinenDetailDto { CartLogDetailId = 6, LinenId = 3, Name = "Bedsheet - Large",Count = 8
                   },

                }




            };
            mock.Setup(p => p.GetCartLog(2)).Returns(cartLogDetailsResponseDTO);
            int cartLogId = 2;
            // Act
            CartLogDetailsController crt = new CartLogDetailsController(mock.Object, mockLogger.Object);
            var data = crt.GetCartLog(cartLogId);
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(data);  // Ensure result is OkObjectResult
            var actualData = Assert.IsType<CartLogDetailsResponse>(okResult.Value); // Extract the actual response data

            Assert.NotNull(actualData);
            Assert.Equal(cartLogDetailsResponseDTO, actualData);
            Assert.True(cartLogDetailsResponseDTO.Equals(actualData));


        }


        [Fact]
        public async Task GetCartLogById_Return_NotNull()
        {
            // Arrange
            var cartLogDetailsResponseDTO = new CartLogDetailsResponse()
            {
                CartLogId = 2,
                ReceiptNumber = "123ABC",
                ReportedWeight = 50,
                ActualWeight = 51,
                Comments = "Extra blanket received",
                DateWeighed = Convert.ToDateTime("2024-10-08T13:41:00"),
                Cart = new CartDto
                {
                    CartId = 1,
                    Name = "Cart - Small",
                    Weight = 20,
                    Type = "CLEAN"
                },
                Location = new LocationDto
                {
                    LocationId = 2,
                    Name = "101A",
                    Type = "CLEAN_ROOM"
                },
                Employee = new EmployeeDto
                {
                    EmployeeId = 2,
                    Name = "John"
                },
                Linen = new List<LinenDetailDto>
                {
                   new LinenDetailDto { CartLogDetailId = 1, LinenId = 1, Name = "Bedsheet - Small",Count = 10
                   },
                   new LinenDetailDto { CartLogDetailId = 2, LinenId = 6, Name = "Pillowcase - Large",Count = 5
                   },
                   new LinenDetailDto { CartLogDetailId = 3, LinenId = 16, Name = "Towel - Large",Count = 2
                   },
                   new LinenDetailDto { CartLogDetailId = 4, LinenId = 4, Name = "Pillowcase - Small",Count = 15
                   },
                   new LinenDetailDto { CartLogDetailId = 5, LinenId = 9, Name = "Blanket - Large",Count = 1
                   },
                   new LinenDetailDto { CartLogDetailId = 6, LinenId = 3, Name = "Bedsheet - Large",Count = 8
                   },

                }




            };
            mock.Setup(p => p.GetCartLog(2)).Returns(cartLogDetailsResponseDTO);
            int cartLogId = 2;
            // Act
            CartLogDetailsController crt = new CartLogDetailsController(mock.Object, mockLogger.Object);
            var data = crt.GetCartLog(cartLogId);
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(data);  // Ensure result is OkObjectResult
            var actualData = Assert.IsType<CartLogDetailsResponse>(okResult.Value); // Extract the actual response data

            Assert.NotNull(actualData);

        }

        [Fact]
        public async Task GetCartLogById_Return_SoiledLinen()
        {
            // Arrange
            var cartLogDetailsResponseDTO = new CartLogDetailsResponse()
            {
                CartLogId = 4,
                ReceiptNumber = "123def",
                ReportedWeight = 60,
                ActualWeight = 130,
                Comments = "No comments",
                DateWeighed = Convert.ToDateTime("2024-10-14T20:36:00"),
                Cart = new CartDto
                {
                    CartId = 6,
                    Name = "Cart - Medium",
                    Weight = 40,
                    Type = "SOILED"
                },
                Location = new LocationDto
                {
                    LocationId = 5,
                    Name = "105A",
                    Type = "SOILED_ROOM"
                },
                Employee = new EmployeeDto
                {
                    EmployeeId = 2,
                    Name = "John"
                },
                Linen = []

            };
            mock.Setup(p => p.GetCartLog(4)).Returns(cartLogDetailsResponseDTO);
            int cartLogId = 4;
            // Act
            CartLogDetailsController crt = new CartLogDetailsController(mock.Object, mockLogger.Object);
            var data = crt.GetCartLog(cartLogId);
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(data);  // Ensure result is OkObjectResult
            var actualData = Assert.IsType<CartLogDetailsResponse>(okResult.Value); // Extract the actual response data

            Assert.Equal(cartLogDetailsResponseDTO, actualData);

        }

        [Fact]
        public async Task GetCartLogs_Return_OkResult()
        {
            // Arrange
            var cartLogDetailsResponseDTO = new List<CartLogDetailsResponse>
            {   new CartLogDetailsResponse{
                CartLogId = 2,
                ReceiptNumber = "123ABC",
                ReportedWeight = 50,
                ActualWeight = 51,
                Comments = "Extra blanket received",
                DateWeighed = Convert.ToDateTime("2024-10-08T13:41:00"),
                Cart = new CartDto
                {
                    CartId = 1,
                    Name = "Cart - Small",
                    Weight = 20,
                    Type = "CLEAN"
                },
                Location = new LocationDto
                {
                    LocationId = 2,
                    Name = "101A",
                    Type = "CLEAN_ROOM"
                },
                Employee = new EmployeeDto
                {
                    EmployeeId = 2,
                    Name = "John"
                },
                Linen = new List<LinenDetailDto>
                {
                   new LinenDetailDto { CartLogDetailId = 1, LinenId = 1, Name = "Bedsheet - Small",Count = 10
                   },
                   new LinenDetailDto { CartLogDetailId = 2, LinenId = 6, Name = "Pillowcase - Large",Count = 5
                   },
                   new LinenDetailDto { CartLogDetailId = 3, LinenId = 16, Name = "Towel - Large",Count = 2
                   },
                   new LinenDetailDto { CartLogDetailId = 4, LinenId = 4, Name = "Pillowcase - Small",Count = 15
                   },
                   new LinenDetailDto { CartLogDetailId = 5, LinenId = 9, Name = "Blanket - Large",Count = 1
                   },
                   new LinenDetailDto { CartLogDetailId = 6, LinenId = 3, Name = "Bedsheet - Large",Count = 8
                   },

                }

            }


            };
            mock.Setup(p => p.GetCartLogs("CLEAN",1,2)).Returns(cartLogDetailsResponseDTO);
            var cartLogType = "CLEAN";
            var locationId = 1;
            var empId = 2;
            // Act
            CartLogDetailsController crt = new CartLogDetailsController(mock.Object, mockLogger.Object);
            var data = crt.GetCartLogs(cartLogType,locationId, empId);
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(data);  // Ensure result is OkObjectResult
            var actualData = Assert.IsType<List<CartLogDetailsResponse>>(okResult.Value); // Extract the actual response data

            Assert.NotNull(actualData);
            Assert.Equal(cartLogDetailsResponseDTO, actualData);
            Assert.True(cartLogDetailsResponseDTO.Equals(actualData));


        }

        [Fact]
        public async Task InsertCartLogAsync_WhenCalled()
        {
            // Arrange
            var requestCartLog = new InsertUpdateCartLogDTO
            {
                CartLogId = null,
                ReceiptNumber = "123def",
                ReportedWeight = 60,
                ActualWeight = 130,
                Comments = "No comments",
                DateWeighed = DateTime.UtcNow,
                CartId = 6,
                LocationId = 5,
                EmployeeId = 2,
                Linen = new List<LinenUpdInsertDTO>
                {
                    new LinenUpdInsertDTO { LinenId = 1, Count = 10 }
                }
            };

            var responseCartLog = new InsertUpdateCartLogResponse
            {
                CartLogId = 12,
                ReceiptNumber = "123def",
                ReportedWeight = 60,
                ActualWeight = 130,
                Comments = "No comments",
                DateWeighed = DateTime.UtcNow,
                CartId = 6,
                LocationId = 5,
                EmployeeId = 2,
                Linen = new List<LinenDetailDto>
                {
                    new LinenDetailDto { CartLogDetailId = 19, LinenId = 1, Name = "Bedsheet - Small", Count = 10 }
                }
            };

            mock.Setup(p => p.UpsertCartLog(requestCartLog,2)).Returns(responseCartLog);
            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "2"),  // Mock employee/user ID
                new Claim(ClaimTypes.Name, "John")
            };

            var identity = new ClaimsIdentity(userClaims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            // Act
            // Create controller and set the User property
            var crt = new CartLogDetailsController(mock.Object, mockLogger.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = claimsPrincipal }
                }
            };
            var data = crt.UpsertCartLog(requestCartLog);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(data);  // Ensure result is OkObjectResult
            var actualData = Assert.IsType<InsertUpdateCartLogResponse>(okResult.Value); // Extract the actual response data

            Assert.NotNull(actualData);
            Assert.Equal(responseCartLog, actualData);
            Assert.True(responseCartLog.Equals(actualData));
        }

        [Fact]
        public async Task UpdateCartLogAsync_WhenCalled()
        {
            // Arrange
            var requestCartLog = new InsertUpdateCartLogDTO
            {
                CartLogId = 11,
                ReceiptNumber = "123def",
                ReportedWeight = 600,
                ActualWeight = 13,
                Comments = "New comments",
                DateWeighed = DateTime.UtcNow,
                CartId = 6,
                LocationId = 5,
                EmployeeId = 2,
                Linen = new List<LinenUpdInsertDTO>
                {
                    new LinenUpdInsertDTO { LinenId = 1, Count = 10 }
                }
            };

            var responseCartLog = new InsertUpdateCartLogResponse
            {
                CartLogId = 11,
                ReceiptNumber = "123def",
                ReportedWeight = 600,
                ActualWeight = 13,
                Comments = "New comments",
                DateWeighed = DateTime.UtcNow,
                CartId = 6,
                LocationId = 5,
                EmployeeId = 2,
                Linen = new List<LinenDetailDto>
                {
                    new LinenDetailDto { CartLogDetailId = 19, LinenId = 1, Name = "Bedsheet - Small", Count = 10 }
                }
            };

            mock.Setup(p => p.UpsertCartLog(requestCartLog, 2)).Returns(responseCartLog);
            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "2"),  // Mock employee/user ID
                new Claim(ClaimTypes.Name, "John")
            };

            var identity = new ClaimsIdentity(userClaims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            // Act
            // Create controller and set the User property
            var crt = new CartLogDetailsController(mock.Object, mockLogger.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = claimsPrincipal }
                }
            };
            var data = crt.UpsertCartLog(requestCartLog);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(data);  // Ensure result is OkObjectResult
            var actualData = Assert.IsType<InsertUpdateCartLogResponse>(okResult.Value); // Extract the actual response data

            Assert.NotNull(actualData);
            Assert.Equal(responseCartLog, actualData);
            Assert.True(responseCartLog.Equals(actualData));
        }

        [Fact]
        public async Task DeleteCartLog_ReturnsOk_WhenCartLogIsDeletedSuccessfully()
        {
            // Arrange
            var cartLogId = 9;
            mock.Setup(p => p.DeleteCartLog(cartLogId, 2)).ReturnsAsync(true);
            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "2"),  // Mock employee/user ID
                new Claim(ClaimTypes.Name, "John")
            };

            var identity = new ClaimsIdentity(userClaims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            // Act
            // Create controller and set the User property
            var crt = new CartLogDetailsController(mock.Object, mockLogger.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = claimsPrincipal }
                }
            };
            var data = await crt.DeleteCartLog(cartLogId);

            // Assert
            Assert.IsType<OkObjectResult>(data);
        }

        [Fact]
        public async Task DeleteCartLog_ReturnsNotFound_WhenCartLogDoesNotExist()
        {
            // Arrange
            var cartLogId = 11;
            mock.Setup(p => p.DeleteCartLog(cartLogId, 2)).ReturnsAsync(false);
            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "2"),  // Mock employee/user ID
                new Claim(ClaimTypes.Name, "John")
            };

            var identity = new ClaimsIdentity(userClaims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            // Act
            // Create controller and set the User property
            var crt = new CartLogDetailsController(mock.Object, mockLogger.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = claimsPrincipal }
                }
            };
            var data = await crt.DeleteCartLog(cartLogId);

            // Assert
            Assert.IsType<BadRequestObjectResult>(data);
        }


    }
}