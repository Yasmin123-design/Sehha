using E_PharmaHub.Dtos;

namespace E_PharmaHub.Repositories.OrderItemRepo
{
    public interface IOrderItemRepository
    {
        Task<List<BestSellingMedicationDto>>
    GetTopSellingMedicationsAsync(int pharmacyId, int top = 10);
    }
}
