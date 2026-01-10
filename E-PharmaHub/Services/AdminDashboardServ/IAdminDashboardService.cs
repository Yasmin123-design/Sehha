using E_PharmaHub.Dtos;

namespace E_PharmaHub.Services.AdminDashboardServ
{
    public interface IAdminDashboardService
    {
        Task<AdminDashboardDto> GetAdminDashboardStatsAsync();
        Task<AdminTopPerformersDto> GetTopPerformingEntitiesAsync();
    }
}
