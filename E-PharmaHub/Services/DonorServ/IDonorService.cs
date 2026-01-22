using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using E_PharmaHub.Models.Enums;

namespace E_PharmaHub.Services.DonorServ
{
    public interface IDonorService
    {
        Task<IEnumerable<DonorReadDto>> GetAllDetailsAsync();
        Task<IEnumerable<DonorReadDto>> GetByFilterAsync(BloodType? type, string? city);
        Task<IEnumerable<DonorReadDto>> GetDonorsByRequestIdAsync(int requestId);
        Task<DonorProfile?> GetByUserIdAsync(string userId);
        Task<DonorReadDto> RegisterAsync(DonorRegisterDto donor);
        Task<bool> UpdateAvailabilityAsync(string userId, bool isAvailable);
        Task UpdateAsync(DonorProfile donor);
        Task DeleteAsync(int id);
    }
}
