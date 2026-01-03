using E_PharmaHub.Dtos;

namespace E_PharmaHub.Services.PharmacistAnalyticsServ
{
    public interface IPharmacistDashboardService
    {
        Task<List<SalesByCategoryDto>>
     GetSalesByCategoryForPharmacistAsync(string userId);
        Task<DashboardStatsDto> GetDashboardStatsAsync(string userId);
        Task<WeeklyCategoryItemsDto> GetWeeklyCategoryItemsAsync(string userId);
        Task<WeeklyOrdersDashboardDto> GetWeeklyOrdersAsync(string userId);
        Task<InventoryDashboardDto> GetInventoryDashboardAsync(string userId);

    }
}
