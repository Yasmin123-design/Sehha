using E_PharmaHub.Dtos;
using E_PharmaHub.Models;

namespace E_PharmaHub.Services.BloodRequestServ
{
    public interface IBloodRequestService
    {
        Task<IEnumerable<BloodRequestResponseDto>> GetAllBloodRequestsDtoAsync();
        Task<BloodRequestResponseDto?> GetRequestByIdAsync(int id);
        Task<IEnumerable<BloodRequestResponseDto>> GetUnfulfilledRequestsAsync();
        Task<BloodRequest> AddRequestAsync(BloodRequest request);
        Task<bool> UpdateRequestAsync(int id, BloodRequest updatedRequest);
        Task<bool> DeleteRequestAsync(int id);
    }

}
