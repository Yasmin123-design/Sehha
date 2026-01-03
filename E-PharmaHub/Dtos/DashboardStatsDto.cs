namespace E_PharmaHub.Dtos
{
    public class DashboardStatsDto
    {
        public int TodayOrders { get; set; }
        public int YesterdayOrders { get; set; }

        public decimal TodayRevenue { get; set; }
        public decimal YesterdayRevenue { get; set; }

        public int AvailableStock { get; set; }
        public int OutOfStock { get; set; }

        public int PendingOrders { get; set; }
    }

}
