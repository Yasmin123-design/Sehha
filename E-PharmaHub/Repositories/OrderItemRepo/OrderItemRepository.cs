using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using E_PharmaHub.Models.Enums;
using E_PharmaHub.Repositories.MedicineRepo;
using Microsoft.EntityFrameworkCore;

namespace E_PharmaHub.Repositories.OrderItemRepo
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly EHealthDbContext _context;

        public OrderItemRepository(EHealthDbContext context)
        {
            _context = context;
        }
        public async Task<List<BestSellingMedicationDto>>
    GetTopSellingMedicationsAsync(int pharmacyId, int top = 10)
        {
            return await _context.OrderItems
                .Where(oi =>
                    oi.Order.PharmacyId == pharmacyId &&
                    oi.Order.Status == OrderStatus.Delivered
                )
                .GroupBy(oi => oi.Medication.BrandName)
                .Select(g => new BestSellingMedicationDto
                {
                    Name = g.Key,
                    Sales = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.Sales)
                .Take(top)
                .ToListAsync();
        }

    }
}
