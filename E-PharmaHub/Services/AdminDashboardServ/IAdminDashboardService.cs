using E_PharmaHub.Dtos;

namespace E_PharmaHub.Services.AdminDashboardServ
{
    public interface IAdminDashboardService
    {
        Task<AdminDashboardDto> GetAdminDashboardStatsAsync();
        Task<AdminTopPerformersDto> GetTopPerformingEntitiesAsync();
        Task<IEnumerable<DailyRevenueReportDto>> GetDailyRevenueReportAsync(int? month, int? year);
        Task<IEnumerable<DailyCountReportDto>> GetDailyRegistrationCountReportAsync(int? month, int? year);
        Task<IEnumerable<DailyOrdersReportDto>> GetDailyOrdersReportAsync(int? month, int? year);
        Task<IEnumerable<DailyAppointmentsReportDto>> GetDailyAppointmentsReportAsync(int? month, int? year);
    }
}
