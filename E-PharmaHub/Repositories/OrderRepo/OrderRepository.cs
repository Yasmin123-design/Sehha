using E_PharmaHub.Dtos;
using E_PharmaHub.Helpers;
using E_PharmaHub.Models;
using E_PharmaHub.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace E_PharmaHub.Repositories.OrderRepo
{
    public class OrderRepository : IOrderRepository
    {
        private readonly EHealthDbContext _context;

        public OrderRepository(EHealthDbContext context)
        {
            _context = context;
        }
        public async Task<int> GetTotalOrdersAsync(int pharmacyId)
        {
            return await _context.Orders
                .Where(o =>
                    o.PharmacyId == pharmacyId)
                .CountAsync();
        }

        public async Task<decimal> GetTotalRevenueAsync(int pharmacyId)
        {
            return await _context.Orders
                .Where(o =>
                o.PharmacyId == pharmacyId &&
                (o.Status == OrderStatus.Confirmed
                         || o.Status == OrderStatus.Delivered))
                .SumAsync(o => o.TotalPrice);
        }
        public async Task<int> GetTotalCustomersAsync(int pharmacyId)
{
    return await _context.Orders
        .Where(o =>
            o.PharmacyId == pharmacyId)
        .Select(o => o.UserId)
        .Distinct()
        .CountAsync();
}
     
        public async Task AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
        }

        public async Task UpdateStatusAsync(int orderId, OrderStatus status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = status;
                _context.Orders.Update(order);
            }
        }

        public async Task MarkAsPaid(string userId)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.UserId == userId);

            if (order == null)
                throw new Exception("Order not found.");

            order.PaymentStatus = PaymentStatus.Paid;
        }
        public void Delete(Order order)
        {
            _context.Orders.Remove(order);
        }
        public async Task<Order?> GetByPaymentIdEntityAsync(int paymentId)
        {
            return await BaseOrderIncludes()
                .FirstOrDefaultAsync(o => o.PaymentId == paymentId);
        }

        private IQueryable<Order> BaseOrderIncludes()
        {
            return _context.Orders
                .Include(o => o.User)
                .Include(o => o.Pharmacy)
                .Include(o => o.Payment)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Medication)
                .AsNoTracking()
                    .OrderByDescending(o => o.CreatedAt);

        }


        public async Task<int> GetPendingOrdersAsync(int pharmacyId)
        {
            return await _context.Orders
                .OrderByDescending(o => o.CreatedAt)
                .CountAsync(o =>
                    o.PharmacyId == pharmacyId &&
                    o.Status == OrderStatus.Pending);

        }

        public async Task<int> GetConfirmedOrdersAsync(int pharmacyId)
        {
            return await _context.Orders
                .CountAsync(o =>
                    o.PharmacyId == pharmacyId &&
                    o.Status == OrderStatus.Confirmed);
        }

        public async Task<int> GetCancelledOrdersAsync(int pharmacyId)
        {
            return await _context.Orders
                .CountAsync(o =>
                    o.PharmacyId == pharmacyId &&
                    o.Status == OrderStatus.Cancelled);
        }
        public async Task<int> GetDelieveredOrdersAsync(int pharmacyId)
        {
            return await _context.Orders
                .CountAsync(o =>
                    o.PharmacyId == pharmacyId &&
                    o.Status == OrderStatus.Delivered);
        }
        public async Task<int> CountAsync(
      int pharmacyId,
      DateTime from,
      DateTime to)
        {
            return await _context.Orders.CountAsync(o =>
                o.PharmacyId == pharmacyId &&
                o.CreatedAt >= from &&
                o.CreatedAt <= to);
        }

        public async Task<int> CountByStatusAsync(
            int pharmacyId,
            OrderStatus status,
            DateTime from,
            DateTime to)
        {
            return await _context.Orders.CountAsync(o =>
                o.PharmacyId == pharmacyId &&
                o.Status == status &&
                o.CreatedAt >= from &&
                o.CreatedAt <= to);
        }
        private Expression<Func<Order, OrderResponseDto>> Selector =>
            OrderSelectors.GetOrderSelector();


        public async Task<IEnumerable<OrderResponseDto>> GetAllAsync()
        {
            return await BaseOrderIncludes()
                .Select(Selector)
                .ToListAsync();
        }

        public async Task<OrderResponseDto?> GetOrderResponseByIdAsync(int id)
        {
            return await BaseOrderIncludes()
                .Where(o => o.Id == id)
                .Select(Selector)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<OrderResponseDto>> GetByUserIdAsync(string userId)
        {
            return await BaseOrderIncludes()
                .Where(o => o.UserId == userId && o.PaymentId != null && o.Payment.PaymentIntentId != null)
                .Select(Selector)
                .ToListAsync();
        }

        public async Task<IEnumerable<OrderResponseDto>> GetByPharmacyIdAsync(int pharmacyId)
        {
            return await BaseOrderIncludes()
                .Where(o => o.PharmacyId == pharmacyId)
                .Select(Selector)
                .ToListAsync();
        }
        public async Task<Order> GetOrderByPaymentIdAsync(int paymentId)
        {
            return await BaseOrderIncludes()
                .Where(a => a.PaymentId == paymentId)
                .FirstOrDefaultAsync();
        }
        public async Task<OrderResponseDto?> GetByPaymentIdAsync(int paymentId)
        {
            return await BaseOrderIncludes()
                .Where(o => o.PaymentId == paymentId)
                .Select(Selector)
                .FirstOrDefaultAsync();
        }

        public async Task<OrderResponseDto?> GetPendingOrderByUserAsync(string userId, int pharmacyId)
        {
            return await BaseOrderIncludes()
                .Where(o =>
                    o.UserId == userId &&
                    o.PharmacyId == pharmacyId &&
                    o.Status == OrderStatus.Pending)
                .Select(Selector)
                .FirstOrDefaultAsync();
        }
        public async Task<List<DailyRevenueDto>>
    GetDailyRevenueAsync(int pharmacyId, int year, int? month)
        {
            var ordersQuery = _context.Orders
                .Where(o =>
    o.PharmacyId == pharmacyId &&
    (o.Status == OrderStatus.Confirmed ||
     o.Status == OrderStatus.Delivered) &&
    o.CreatedAt.Year == year
);


            if (month.HasValue)
                ordersQuery = ordersQuery
                    .Where(o => o.CreatedAt.Month == month.Value);

            var groupedOrders = await ordersQuery
                .GroupBy(o => o.CreatedAt.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Revenue = g.Sum(x => x.TotalPrice)
                })
                .ToListAsync();

            int daysCount = month.HasValue
                ? DateTime.DaysInMonth(year, month.Value)
                : DateTime.IsLeapYear(year) ? 366 : 365;

            DateTime startDate = month.HasValue
                ? new DateTime(year, month.Value, 1)
                : new DateTime(year, 1, 1);

            var result = new List<DailyRevenueDto>();

            for (int i = 0; i < daysCount; i++)
            {
                var date = startDate.AddDays(i);

                var revenueForDay = groupedOrders
                    .FirstOrDefault(x => x.Date == date)?.Revenue ?? 0;

                result.Add(new DailyRevenueDto
                {
                    Date = date.ToString("yyyy-MM-dd"),
                    TotalRevenue = revenueForDay
                });
            }

            return result;
        }

        public async Task<List<SalesByTimeSlotDto>>
    GetTodaySalesByTimeSlotsAsync(int pharmacyId)
        {
            var today = DateTime.UtcNow.Date;

            var orders = await _context.Orders
                .Where(o =>
    o.PharmacyId == pharmacyId &&
    (o.Status == OrderStatus.Confirmed || o.Status == OrderStatus.Delivered) &&
    o.CreatedAt.Date == today
    ).Select(o => o.CreatedAt.Hour)
                .ToListAsync();

            var slots = new List<SalesByTimeSlotDto>
    {
        new() { TimeSlot = "00:00 - 03:00" },
        new() { TimeSlot = "03:00 - 06:00" },
        new() { TimeSlot = "06:00 - 09:00" },
        new() { TimeSlot = "09:00 - 12:00" },
        new() { TimeSlot = "12:00 - 15:00" },
        new() { TimeSlot = "15:00 - 18:00" },
        new() { TimeSlot = "18:00 - 21:00" },
        new() { TimeSlot = "21:00 - 24:00" }
    };

            foreach (var hour in orders)
            {
                int index = hour / 3;  
                if (index < 8)
                    slots[index].SalesCount++;
            }

            return slots;
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
        }
        public async Task<Order?> GetPendingOrderEntityByUserAsync(string userId, int pharmacyId)
        {
            return await BaseOrderIncludes()
                .FirstOrDefaultAsync(o =>
                    o.UserId == userId &&
                    o.PharmacyId == pharmacyId &&
                    o.Status == OrderStatus.Pending);
        }
        public async Task<OrderResponseDto> AddOrderAndReturnResponseAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            return await BaseOrderIncludes()
                .Where(o => o.Id == order.Id)
                .Select(Selector)
                .FirstAsync();
        }
        public async Task<int> CountOrdersByDateAsync(int pharmacyId, DateTime date)
        {
            return await _context.Orders
                .CountAsync(o =>
                    o.PharmacyId == pharmacyId &&
                    o.CreatedAt.Date == date.Date);
        }

        public async Task<decimal> GetRevenueByDateAsync(int pharmacyId, DateTime date)
        {
            return await _context.Orders
                .Where(o =>
                    o.PharmacyId == pharmacyId &&
                    o.CreatedAt.Date == date.Date &&
                    o.Status == OrderStatus.Confirmed)
                .SumAsync(o => o.TotalPrice);
        }

        public async Task<int> CountPendingOrdersAsync(int pharmacyId)
        {
            return await _context.Orders
                .CountAsync(o =>
                    o.PharmacyId == pharmacyId &&
                    o.Status == OrderStatus.Pending);
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await BaseOrderIncludes()
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }
        public async Task<Order?> GetOrderByIdTrackingAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Pharmacy)
                .Include(o => o.Payment)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Medication)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<Order?> GetPendingOrderEntityByUserForUpdateAsync(string userId, int pharmacyId)
        {
            return await _context.Orders
                .Include(o => o.Items) 
                .FirstOrDefaultAsync(o =>
                    o.UserId == userId &&
                    o.PharmacyId == pharmacyId &&
                    o.Status == OrderStatus.Pending);
        }

        public async Task<Order?> GetByIdForUpdateAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);
        }
        public async Task<decimal> GetRevenueAsync(int pharmacyId, DateTime from, DateTime to, OrderStatus status)
        {
            return await _context.Orders
                .Where(o => o.PharmacyId == pharmacyId &&
                            o.Status == status &&
                            o.CreatedAt >= from && o.CreatedAt <= to)
                .SumAsync(o => o.TotalPrice); 
        }

        public async Task UpdateWithItemsAsync(Order order)
        {
            _context.Orders.Update(order);

            foreach (var item in order.Items)
            {
                if (item.Id > 0)
                {
                    _context.Entry(item).State = EntityState.Modified;
                }
                else
                {
                    _context.OrderItems.Add(item);
                }
            }
        }

        public async Task<Order?> GetPendingOrderEntityByUserAsync(string userId, int pharmacyId, bool asNoTracking = false)
        {
            var query = _context.Orders
                .Include(o => o.Items)
                .Where(o =>
                    o.UserId == userId &&
                    o.PharmacyId == pharmacyId &&
                    o.Status == OrderStatus.Pending);

            if (asNoTracking)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Order>> GetAllEntitiesAsync()
        {
            return await _context.Orders.AsNoTracking().ToListAsync();
        }
    }

}
