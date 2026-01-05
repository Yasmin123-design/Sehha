namespace E_PharmaHub.Dtos
{
    public class GetAnalyticsStats
    {
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalProducts { get; set; }
        public int TotalCustomers { get; set; }
    }
}
