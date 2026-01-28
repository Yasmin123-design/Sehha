using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using System.Linq.Expressions;

namespace E_PharmaHub.Repositories.InventoryItemRepo
{
    public interface IInventoryItemRepository
    {
        Task<int> GetTotalProductsAsync(int pharmacyId);
        Task<List<DailyOutOfStockDto>> GetOutOfStockLast30DaysAsync();
        Task<List<DailyInventoryDto>> GetLast30DaysInventoryAsync();
        Task<List<SalesByCategoryDto>> GetSalesByCategoryAsync(int pharmacyId);
        Task<int> CountAvailableStockAsync(int pharmacyId);
        Task<int> CountOutOfStockAsync(int pharmacyId);
        Task<List<CategoryItemsCountDto>> GetItemsCountByCategoryAsync(
    int pharmacyId,
    DateTime from,
    DateTime to);
        Task<int> GetLowStockCountAsync(int pharmacyId, int threshold = 5);
        Task<int> GetOutOfStockCountAsync(int pharmacyId);
        Task<InventoryItem> GetInventoryForCheckoutAsync(int medicationId, int pharmacyId, decimal price);
        Task<List<InventoryItem>> GetInventoriesForCheckoutAsync(IEnumerable<int> medicationIds, int pharmacyId);
        Task DecreaseQuantityAsync(int inventoryId, int quantity);
        Task<IEnumerable<InventoryItem>> GetAllAsync();
        Task<IEnumerable<MedicineDto>> GetAlternativeMedicinesAsync(string name);
        Task<MedicineDto?> GetByIdAsync(int id);
        Task AddAsync(InventoryItem entity);

        Task<InventoryItem?> FindAsync(Expression<Func<InventoryItem, bool>> predicate);
        Task<IEnumerable<InventoryItem>> FindAllAsync(Expression<Func<InventoryItem, bool>> predicate);
        Task Update(InventoryItem entity);
        void Delete(InventoryItem entity);
        Task<IEnumerable<MedicineDto>> GetByPharmacyIdAsync(int pharmacyId);
        Task<IEnumerable<MedicineDto>> GetByMedicationIdAsync(int medicationId);

        Task<InventoryItem?> GetByPharmacyAndMedicationAsync(int pharmacyId, int medicationId);
        Task<InventoryItem?> GetInventoryItemByIdAsync(int id);
        Task<InventoryItem?> GetByPharmacyAndMedicationWithoutIncludesAsync(int pharmacyId, int medicationId);

    }

}
