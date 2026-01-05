using E_PharmaHub.Dtos;
using E_PharmaHub.Models.Enums;
using E_PharmaHub.Repositories.InventoryItemRepo;
using E_PharmaHub.Services.PharmacistServ;
using E_PharmaHub.UnitOfWorkes;

namespace E_PharmaHub.Services.PharmacistAnalyticsServ
{
    public class PharmacistDashboardService : IPharmacistDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPharmacistService _pharmacistService;
        private readonly IInventoryItemRepository _inventoryItemRepository;
        public PharmacistDashboardService(
            IUnitOfWork unitOfWork,
            IPharmacistService pharmacistService,
            IInventoryItemRepository inventoryItemRepository
            )
        {
            _unitOfWork = unitOfWork;
            _inventoryItemRepository = inventoryItemRepository;
            _pharmacistService = pharmacistService;
        }
        public async Task<List<DailyRevenueDto>>
    GetDailyRevenueAsync(string userId, int? year, int? month)
        {
            int resolvedYear = year ?? DateTime.UtcNow.Year;

            var pharmacist =
                await _pharmacistService.GetPharmacistProfileByUserIdAsync(userId);

            if (pharmacist?.PharmacyId == null)
                throw new Exception("Pharmacist has no pharmacy");

            return await _unitOfWork.Order
                .GetDailyRevenueAsync(
                    pharmacist.PharmacyId,
                    resolvedYear,
                    month
                );
        }
        public async Task<List<SalesByTimeSlotDto>>
    GetTodaySalesByTimeSlotsAsync(string userId)
        {
            var pharmacist =
                await _pharmacistService.GetPharmacistProfileByUserIdAsync(userId);

            if (pharmacist?.PharmacyId == null)
                throw new Exception("Pharmacist has no pharmacy");

            return await _unitOfWork.Order
                .GetTodaySalesByTimeSlotsAsync(pharmacist.PharmacyId);
        }

        public async Task<List<DailyOutOfStockDto>> GetOutOfStockLast30DaysAsync()
        {
            return await _unitOfWork.IinventoryItem.GetOutOfStockLast30DaysAsync();
        }
        public async Task<List<DailyInventoryDto>> GetLast30DaysInventoryReportAsync()
        {
            return await _unitOfWork.IinventoryItem.GetLast30DaysInventoryAsync();
        }
        public async Task<List<BestSellingMedicationDto>>
    GetBestSellingMedicationsAsync(string userId)
        {
            var pharmacist =
                await _pharmacistService.GetPharmacistProfileByUserIdAsync(userId);

            if (pharmacist?.PharmacyId == null)
                throw new Exception("Pharmacist has no pharmacy");

            return await _unitOfWork.OrderItems
                .GetTopSellingMedicationsAsync(pharmacist.PharmacyId);
        }

        public async Task<WeeklyOrdersDashboardDto> GetWeeklyOrdersAsync(string userId)
        {
            var pharmacist =
                await _pharmacistService.GetPharmacistProfileByUserIdAsync(userId);

            if (pharmacist?.PharmacyId == null)
                throw new Exception("Pharmacist has no pharmacy");

            int pharmacyId = pharmacist.PharmacyId;

            var nowUtc = DateTime.UtcNow;  

            var thisWeekFrom = nowUtc.AddDays(-7);
            var thisWeekTo = nowUtc;

            var lastWeekFrom = nowUtc.AddDays(-14);
            var lastWeekTo = nowUtc.AddDays(-7);

            return new WeeklyOrdersDashboardDto
            {
                ThisWeek = await BuildSummary(pharmacyId, thisWeekFrom, thisWeekTo),
                LastWeek = await BuildSummary(pharmacyId, lastWeekFrom, lastWeekTo)
            };
        }
        public async Task<WeeklyCategoryItemsDto> GetWeeklyCategoryItemsAsync(string userId)
        {
            var pharmacist =
                await _pharmacistService.GetPharmacistProfileByUserIdAsync(userId);

            if (pharmacist?.PharmacyId == null)
                throw new Exception("Pharmacist has no pharmacy");

            int pharmacyId = pharmacist.PharmacyId;

            var nowUtc = DateTime.UtcNow;

            var thisWeekFrom = nowUtc.AddDays(-7);
            var thisWeekTo = nowUtc;

            var lastWeekFrom = nowUtc.AddDays(-14);
            var lastWeekTo = nowUtc.AddDays(-7);

            var thisWeekData = await _unitOfWork.IinventoryItem
                .GetItemsCountByCategoryAsync(pharmacyId, thisWeekFrom, thisWeekTo);

            var lastWeekData = await _unitOfWork.IinventoryItem
                .GetItemsCountByCategoryAsync(pharmacyId, lastWeekFrom, lastWeekTo);

            var allCategories = Enum.GetValues(typeof(MedicationCategory)).Cast<MedicationCategory>();

            var thisWeekResult = allCategories
                .Select(cat => new CategoryItemsCountDto
                {
                    CategoryName = cat,
                    ItemsCount = thisWeekData.FirstOrDefault(d => d.CategoryName == cat)?.ItemsCount ?? 0
                }).ToList();

            var lastWeekResult = allCategories
                .Select(cat => new CategoryItemsCountDto
                {
                    CategoryName = cat,
                    ItemsCount = lastWeekData.FirstOrDefault(d => d.CategoryName == cat)?.ItemsCount ?? 0
                }).ToList();

            return new WeeklyCategoryItemsDto
            {
                ThisWeek = thisWeekResult,
                LastWeek = lastWeekResult
            };
        }


        private async Task<OrdersSummaryDto> BuildSummary(
    int pharmacyId,
    DateTime from,
    DateTime to)
        {
            return new OrdersSummaryDto
            {
                TotalOrders = await _unitOfWork.Order
                    .CountAsync(pharmacyId, from, to),

                PendingOrders = await _unitOfWork.Order
                    .CountByStatusAsync(pharmacyId, OrderStatus.Pending, from, to),

                ConfirmedOrders = await _unitOfWork.Order
                    .CountByStatusAsync(pharmacyId, OrderStatus.Confirmed, from, to),

                CancelledOrders = await _unitOfWork.Order
                    .CountByStatusAsync(pharmacyId, OrderStatus.Cancelled, from, to),
                DelieveredOrders = await _unitOfWork.Order
                    .CountByStatusAsync(pharmacyId,OrderStatus.Delivered,from,to),
                TotalRevenue = await _unitOfWork.Order
        .GetRevenueAsync(pharmacyId, from, to, OrderStatus.Confirmed)

        };
        }
        public async Task<List<SalesByCategoryDto>>
    GetSalesByCategoryForPharmacistAsync(string userId)
        {
            var pharmacy = await _unitOfWork.Pharmacies
                .GetBriefByPharmacistUserIdAsync(userId);

            if (pharmacy == null)
                throw new Exception("Pharmacy not found");

            return await _unitOfWork.IinventoryItem
                .GetSalesByCategoryAsync(pharmacy.Id);
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync(string userId)
        {
            var pharmacy = await _unitOfWork.Pharmacies
                .GetPharmacyByPharmacistUserIdAsync(userId);

            if (pharmacy.Id == null)
                throw new Exception("Pharmacy not found for this pharmacist");

            var today = DateTime.UtcNow.Date;
            var yesterday = today.AddDays(-1);

            return new DashboardStatsDto
            {
                TodayOrders =
                    await _unitOfWork.Order.CountOrdersByDateAsync(pharmacy.Id, today),

                YesterdayOrders =
                    await _unitOfWork.Order.CountOrdersByDateAsync(pharmacy.Id, yesterday),

                TodayRevenue =
                    await _unitOfWork.Order.GetRevenueByDateAsync(pharmacy.Id, today),

                YesterdayRevenue =
                    await _unitOfWork.Order.GetRevenueByDateAsync(pharmacy.Id, yesterday),

                PendingOrders =
                    await _unitOfWork.Order.CountPendingOrdersAsync(pharmacy.Id),

                AvailableStock =
                    await _unitOfWork.IinventoryItem.CountAvailableStockAsync(pharmacy.Id),

                OutOfStock =
                    await _unitOfWork.IinventoryItem.CountOutOfStockAsync(pharmacy.Id)
            };
        }
        public async Task<InventoryDashboardDto> GetInventoryDashboardAsync(string userId)
        {
            var pharmacist =
                await _pharmacistService.GetPharmacistProfileByUserIdAsync(userId);

            if (pharmacist?.PharmacyId == null)
                throw new Exception("Pharmacist has no pharmacy");

            int pharmacyId = pharmacist.PharmacyId;
            return new InventoryDashboardDto
            {
                LowStockCount = await _inventoryItemRepository.GetLowStockCountAsync(pharmacyId),
                OutOfStockCount = await _inventoryItemRepository.GetOutOfStockCountAsync(pharmacyId),
                TotalProducts = await _inventoryItemRepository.GetTotalProductsAsync(pharmacyId)
            };
        }

        
    }

}
