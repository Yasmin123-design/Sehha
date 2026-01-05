using E_PharmaHub.Dtos;

namespace E_PharmaHub.Services.PharmacistAnalyticsServ
{
    public interface IPharmacistDashboardService
    {
        Task<List<DailyOutOfStockDto>> GetOutOfStockLast30DaysAsync();
        Task<List<DailyInventoryDto>> GetLast30DaysInventoryReportAsync();
        Task<List<SalesByTimeSlotDto>> GetTodaySalesByTimeSlotsAsync(string userId);
        Task<List<DailyRevenueDto>> GetDailyRevenueAsync(string userId, int? year, int? month);
        Task<List<BestSellingMedicationDto>> GetBestSellingMedicationsAsync(string userId);

        Task<List<SalesByCategoryDto>> GetSalesByCategoryForPharmacistAsync(string userId);
        Task<DashboardStatsDto> GetDashboardStatsAsync(string userId);
        Task<WeeklyCategoryItemsDto> GetWeeklyCategoryItemsAsync(string userId);
        Task<WeeklyOrdersDashboardDto> GetWeeklyOrdersAsync(string userId);
        Task<InventoryDashboardDto> GetInventoryDashboardAsync(string userId);

    }
}
