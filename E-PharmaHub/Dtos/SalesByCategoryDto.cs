using E_PharmaHub.Models.Enums;

namespace E_PharmaHub.Dtos
{
    public class SalesByCategoryDto
    {
        public MedicationCategory Category { get; set; }
        public decimal TotalSales { get; set; }
        public double Percentage { get; set; }
    }

}
