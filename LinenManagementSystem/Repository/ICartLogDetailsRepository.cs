using LinenManagementSystem.Data;
using LinenManagementSystem.Models;

namespace LinenManagementSystem.Repository
{
    public interface ICartLogDetailsRepository
    {
        CartLogDetailsResponse GetCartLog(int cartLogId);
        List<CartLogDetailsResponse> GetCartLogs(string? cartType, int? locationId, int? employeeId);

        InsertUpdateCartLogResponse UpsertCartLog(InsertUpdateCartLogDTO cartLogDto, int currentEmployeeId);

        Task<bool> DeleteCartLog(int cartLogId, int currentEmployeeId);

    }
}
