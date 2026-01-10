namespace E_PharmaHub.Dtos
{
    public class AdminDashboardDto
    {
        public AdminStats Today { get; set; } = new();
        public AdminStats Yesterday { get; set; } = new();
    }

    public class AdminStats
    {
        public decimal TotalRevenue { get; set; }
        public decimal DoctorRegistrationRevenue { get; set; }
        public decimal PharmacistRegistrationRevenue { get; set; }

        public int TotalRegistrations { get; set; }
        public int DoctorRegistrations { get; set; } 
        public int PharmacistRegistrations { get; set; } 

        public int PendingRegistrations { get; set; }
        public int ApprovedRegistrations { get; set; }
        public int RejectedRegistrations { get; set; }
    }

    public class AdminTopPerformersDto
    {
        public List<DoctorRevenueDto> TopDoctors { get; set; } = new();
        public List<PharmacistRevenueDto> TopPharmacists { get; set; } = new();
    }

    public class DoctorRevenueDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class PharmacistRevenueDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class DailyRevenueReportDto
    {
        public DateTime Date { get; set; }
        public decimal DoctorRevenue { get; set; }
        public decimal PharmacistRevenue { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class DailyCountReportDto
    {
        public DateTime Date { get; set; }
        public int DoctorCount { get; set; }
        public int PharmacistCount { get; set; }
        public int TotalCount { get; set; }
    }

    public class DailyOrdersReportDto
    {
        public DateTime Date { get; set; }
        public int PendingCount { get; set; }
        public int ConfirmedCount { get; set; }
        public int DeliveredCount { get; set; }
        public int CancelledCount { get; set; }
        public int TotalCount { get; set; }
    }

    public class DailyAppointmentsReportDto
    {
        public DateTime Date { get; set; }
        public int PendingCount { get; set; }
        public int ConfirmedCount { get; set; }
        public int CompletedCount { get; set; }
        public int CancelledCount { get; set; }
        public int TotalCount { get; set; }
    }
}
