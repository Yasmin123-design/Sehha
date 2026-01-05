using E_PharmaHub.Dtos;
using E_PharmaHub.Helpers;
using E_PharmaHub.Models;
using E_PharmaHub.Models.Enums;
using E_PharmaHub.Repositories.MedicineRepo;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace E_PharmaHub.Repositories.InventoryItemRepo
{
    public class InventoryItemRepository : IInventoryItemRepository
    {
        private readonly EHealthDbContext _context;
        private readonly IMedicineRepository _medicineRepository;

        public InventoryItemRepository(EHealthDbContext context,IMedicineRepository medicineRepository)
        {
            _context = context;
            _medicineRepository = medicineRepository;
        }
        public async Task<int> GetTotalProductsAsync(int pharmacyId)
        {
            return await _context.InventoryItems
                .Where(i => i.PharmacyId == pharmacyId && i.Quantity > 0)
                .CountAsync();
        }

        public async Task<List<DailyInventoryDto>> GetLast30DaysInventoryAsync()
        {
            var today = DateTime.UtcNow.Date;
            var startDate = today.AddDays(-29);

            var dbData = await _context.InventoryItems
                .Where(i => i.CreatedAt.Date >= startDate && i.CreatedAt.Date <= today)
                .GroupBy(i => i.CreatedAt.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Quantity = g.Sum(i => i.Quantity)
                })
                .ToListAsync();

            var allDates = Enumerable.Range(0, 30)
                .Select(offset => startDate.AddDays(offset))
                .ToList();

            var result = allDates.Select(date => new DailyInventoryDto
            {
                Date = date.ToString("yyyy-MM-dd"),
                Quantity = dbData.FirstOrDefault(d => d.Date == date)?.Quantity ?? 0
            })
            .ToList();

            return result;
        }


        private IQueryable<InventoryItem> BaseInventoryIncludes()
        {
            return _context.InventoryItems
                .AsNoTracking()
                .Include(i => i.Medication)
                    .ThenInclude(m => m.Reviews)
                .Include(i => i.Pharmacy)
                    .ThenInclude(p => p.Address);
        }

        public async Task<int> CountAvailableStockAsync(int pharmacyId)
        {
            return await _context.InventoryItems
                .CountAsync(i =>
                    i.PharmacyId == pharmacyId &&
                    i.Quantity > 0);
        }

        public async Task<List<SalesByCategoryDto>>
    GetSalesByCategoryAsync(int pharmacyId)
        {
            var totalSales = await _context.OrderItems
                .Where(oi =>
                    oi.Order.PharmacyId == pharmacyId &&
                    oi.Order.Status == OrderStatus.Confirmed)
                .SumAsync(oi => oi.UnitPrice * oi.Quantity);

            if (totalSales == 0)
            {
                return Enum.GetValues(typeof(MedicationCategory))
                    .Cast<MedicationCategory>()
                    .Select(c => new SalesByCategoryDto
                    {
                        Category = c,
                        TotalSales = 0,
                        Percentage = 0
                    })
                    .ToList();
            }

            var salesByCategory = await _context.OrderItems
                .Where(oi =>
                    oi.Order.PharmacyId == pharmacyId &&
                    oi.Order.Status == OrderStatus.Confirmed)
                .Include(oi => oi.Medication)
                .GroupBy(oi => oi.Medication!.Category)
                .Select(g => new
                {
                    Category = g.Key,
                    TotalSales = g.Sum(x => x.UnitPrice * x.Quantity)
                })
                .ToListAsync();

            var allCategories = Enum.GetValues(typeof(MedicationCategory))
                .Cast<MedicationCategory>();

            var result = allCategories.Select(category =>
            {
                var match = salesByCategory.FirstOrDefault(x => x.Category == category);
                var sales = match?.TotalSales ?? 0;

                return new SalesByCategoryDto
                {
                    Category = category,
                    TotalSales = sales,
                    Percentage = Math.Round(
                        (double)(sales / totalSales) * 100,
                        2)
                };
            }).ToList();

            return result;
        }

        public async Task<int> CountOutOfStockAsync(int pharmacyId)
        {
            return await _context.InventoryItems
                .CountAsync(i =>
                    i.PharmacyId == pharmacyId &&
                    i.Quantity == 0);
        }


        public async Task<InventoryItem?> GetByPharmacyAndMedicationWithoutIncludesAsync(int pharmacyId, int medicationId)
        {
            return await _context.InventoryItems
                .AsNoTracking() 
                .FirstOrDefaultAsync(x => x.PharmacyId == pharmacyId && x.MedicationId == medicationId);
        }

        
        public async Task<int> GetLowStockCountAsync(int pharmacyId, int threshold = 5)
        {
            return await _context.InventoryItems
                .CountAsync(p =>
                    p.PharmacyId == pharmacyId &&
                    p.Quantity > 0 &&
                    p.Quantity <= threshold);
        }

        public async Task<int> GetOutOfStockCountAsync(int pharmacyId)
        {
            return await _context.InventoryItems
                .CountAsync(p =>
                    p.PharmacyId == pharmacyId &&
                    p.Quantity == 0);
        }
        public async Task<IEnumerable<InventoryItem>> GetAllAsync()
        {
            return await BaseInventoryIncludes().ToListAsync();
        }
        public async Task<IEnumerable<MedicineDto>> GetAlternativeMedicinesAsync(string name)
        {
            var originalMedicine = await _medicineRepository.FindAsync(m => m.BrandName == name);
            if (originalMedicine == null)
                return Enumerable.Empty<MedicineDto>();

            var alternatives = await BaseInventoryIncludes()
    .Where(i =>
        (i.Medication.BrandName != originalMedicine.BrandName &&
         i.Medication.GenericName == originalMedicine.GenericName) ||
        (i.Medication.BrandName != originalMedicine.BrandName &&
         i.Medication.ATCCode == originalMedicine.ATCCode))
    .ToListAsync();



            return alternatives
                .Select(MedicineSelector.MapInventoryToDto)
                .ToList();
        }
        public async Task<MedicineDto?> GetByIdAsync(int id)
        {
            var inventoryItem = await BaseInventoryIncludes()
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inventoryItem == null) return null;

            return MedicineSelector.MapInventoryToDto(inventoryItem);
        }
        public async Task<InventoryItem?> GetInventoryItemByIdAsync(int id)
        {
            var inventoryItem = await BaseInventoryIncludes()
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inventoryItem == null) return null;

            return inventoryItem;
        }
        public async Task AddAsync(InventoryItem entity)
        {
            await _context.InventoryItems.AddAsync(entity);
        }

        public async Task<InventoryItem?> FindAsync(Expression<Func<InventoryItem, bool>> predicate)
        {
            return await BaseInventoryIncludes()
                .FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<InventoryItem>> FindAllAsync(Expression<Func<InventoryItem, bool>> predicate)
        {
            return await BaseInventoryIncludes()
                .Where(predicate)
                .ToListAsync();
        }

        public async Task Update(InventoryItem entity)
        {
            _context.InventoryItems.Update(entity);
        }

        public void Delete(InventoryItem entity)
        {
            _context.InventoryItems.Remove(entity);
        }

        public async Task<IEnumerable<MedicineDto>> GetByPharmacyIdAsync(int pharmacyId)
        {
            var items = await BaseInventoryIncludes()
                .Where(i => i.PharmacyId == pharmacyId)
                .ToListAsync();

            return items.Select(MedicineSelector.MapInventoryToDto).ToList();
        }

        public async Task<IEnumerable<MedicineDto>> GetByMedicationIdAsync(int medicationId)
        {
            var items = await BaseInventoryIncludes()
                .Where(i => i.MedicationId == medicationId)
                .ToListAsync();

            return items.Select(MedicineSelector.MapInventoryToDto).ToList();
        }

        public async Task<InventoryItem?> GetByPharmacyAndMedicationAsync(int pharmacyId, int medicationId)
        {
            var item = await BaseInventoryIncludes()
                .FirstOrDefaultAsync(i => i.PharmacyId == pharmacyId && i.MedicationId == medicationId);

            if (item == null) return null;

            return item;
        }

        public async Task<InventoryItem> GetInventoryForCheckoutAsync(int medicationId, int pharmacyId, decimal price)
        {
            return await _context.InventoryItems
                .AsNoTracking()
                .FirstOrDefaultAsync(i =>
                    i.MedicationId == medicationId &&
                    i.PharmacyId == pharmacyId &&
                    i.Price == price);
        }

        public async Task DecreaseQuantityAsync(int inventoryId, int quantity)
        {
            var inventory = await _context.InventoryItems.FindAsync(inventoryId);
            if (inventory != null)
            {
                inventory.Quantity -= quantity;
                _context.InventoryItems.Update(inventory);
            }
        }
        public async Task<List<CategoryItemsCountDto>> GetItemsCountByCategoryAsync(
     int pharmacyId,
     DateTime from,
     DateTime to)
        {
            return await _context.InventoryItems
                .Where(p =>
                    p.PharmacyId == pharmacyId &&
                    p.CreatedAt >= from &&
                    p.CreatedAt <= to)
                .GroupBy(p => p.Medication.Category) 
                .Select(g => new CategoryItemsCountDto
                {
                    CategoryName = g.Key,
                    ItemsCount = g.Count()
                })
                .ToListAsync();
        }
        public async Task<List<DailyOutOfStockDto>> GetOutOfStockLast30DaysAsync()
        {
            var today = DateTime.UtcNow.Date;
            var startDate = today.AddDays(-29);

            var zeroStockItems = await _context.InventoryItems
                .Where(i =>
                    i.Quantity == 0 &&
                    i.LastUpdated.Date >= startDate &&
                    i.LastUpdated.Date <= today
                )
                .GroupBy(i => i.LastUpdated.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            var result = Enumerable.Range(0, 30)
                .Select(i =>
                {
                    var date = startDate.AddDays(i);
                    var dayData = zeroStockItems.FirstOrDefault(x => x.Date == date);

                    return new DailyOutOfStockDto
                    {
                        Date = date.ToString("yyyy-MM-dd"),
                        Count = dayData?.Count ?? 0
                    };
                })
                .ToList();

            return result;
        }

    }
}
