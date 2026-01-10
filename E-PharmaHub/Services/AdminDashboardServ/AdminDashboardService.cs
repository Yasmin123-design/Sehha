using E_PharmaHub.Dtos;
using E_PharmaHub.Helpers;
using E_PharmaHub.Models.Enums;
using E_PharmaHub.UnitOfWorkes;

namespace E_PharmaHub.Services.AdminDashboardServ
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminDashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AdminDashboardDto> GetAdminDashboardStatsAsync()
        {
            var nowEgypt = DateTime.UtcNow.ToEgyptTime();
            var today = nowEgypt.Date;
            var yesterday = today.AddDays(-1);

            var payments = await _unitOfWork.Payments.GetAllAsync();
            var doctors = await _unitOfWork.Doctors.GetAllAsync();
            var pharmacists = await _unitOfWork.PharmasistsProfile.GetAllAsync();

            var result = new AdminDashboardDto
            {
                Today = CalculateStats(payments, doctors, pharmacists, today),
                Yesterday = CalculateStats(payments, doctors, pharmacists, yesterday)
            };

            return result;
        }

        public async Task<AdminTopPerformersDto> GetTopPerformingEntitiesAsync()
        {
            var payments = await _unitOfWork.Payments.GetAllAsync();
            var doctors = await _unitOfWork.Doctors.GetAllAsync();
            var pharmacists = await _unitOfWork.PharmasistsProfile.GetAllAsync();
            var orders = await _unitOfWork.Order.GetAllEntitiesAsync();
            var appointments = await _unitOfWork.Appointments.GetAllAsync();

            var result = new AdminTopPerformersDto();

            // Top 5 Doctors based on Appointment revenue
            result.TopDoctors = doctors
                .Select(d =>
                {
                    var doctorAppointments = appointments.Where(a => a.DoctorId == d.AppUserId);
                    var validAppointments = doctorAppointments.Where(a =>
                        (a.Status == AppointmentStatus.Confirmed || a.Status == AppointmentStatus.Completed) &&
                        payments.Any(p => p.Id == a.PaymentId && p.Status == PaymentStatus.Paid)
                    );

                    decimal revenue = 0;
                    foreach (var app in validAppointments)
                    {
                        var pay = payments.FirstOrDefault(p => p.Id == app.PaymentId);
                        if (pay != null) revenue += pay.Amount;
                    }

                    return new DoctorRevenueDto
                    {
                        FullName = d.AppUser?.UserName ?? "Unknown",
                        Email = d.AppUser?.Email ?? "N/A",
                        TotalRevenue = revenue
                    };
                })
                .OrderByDescending(d => d.TotalRevenue)
                .Take(5)
                .ToList();

            result.TopPharmacists = pharmacists
                .Select(p =>
                {
                    var pharmacyOrders = orders.Where(o => o.PharmacyId == p.PharmacyId);
                    var validOrders = pharmacyOrders.Where(o =>
                        (o.Status != OrderStatus.Cancelled && o.Status != OrderStatus.Returned && o.Status != OrderStatus.Pending) &&
                        o.PaymentStatus == PaymentStatus.Paid
                    );

                    return new PharmacistRevenueDto
                    {
                        FullName = p.AppUser?.UserName ?? "Unknown",
                        Email = p.AppUser?.Email ?? "N/A",
                        TotalRevenue = validOrders.Sum(o => o.TotalPrice)
                    };
                })
                .OrderByDescending(p => p.TotalRevenue)
                .Take(5)
                .ToList();

            return result;
        }

        private AdminStats CalculateStats(
            IEnumerable<E_PharmaHub.Models.Payment> payments,
            IEnumerable<E_PharmaHub.Models.DoctorProfile> doctors,
            IEnumerable<E_PharmaHub.Models.PharmacistProfile> pharmacists,
            DateTime date)
        {
            var stats = new AdminStats();

            var regPayments = payments.Where(p => p.ProcessedAt.ToEgyptTime().Date == date.Date && p.Status == PaymentStatus.Paid);

            stats.DoctorRegistrationRevenue = regPayments
                .Where(p => p.PaymentFor == PaymentForType.DoctorRegistration)
                .Sum(p => p.Amount);

            stats.PharmacistRegistrationRevenue = regPayments
                .Where(p => p.PaymentFor == PaymentForType.PharmacistRegistration)
                .Sum(p => p.Amount);

            stats.TotalRevenue = stats.DoctorRegistrationRevenue + stats.PharmacistRegistrationRevenue;

            var usersWithPayment = payments
                .Where(p => !string.IsNullOrEmpty(p.PaymentIntentId))
                .Select(p => p.ReferenceId)
                .ToHashSet();

            var dayDoctors = doctors.Where(d =>
                d.CreatedAt.ToEgyptTime().Date == date.Date &&
                !string.IsNullOrEmpty(d.AppUserId) &&
                usersWithPayment.Contains(d.AppUserId));

            var dayPharmacists = pharmacists.Where(p =>
                p.CreatedAt.ToEgyptTime().Date == date.Date &&
                !string.IsNullOrEmpty(p.AppUserId) &&
                usersWithPayment.Contains(p.AppUserId));

            stats.DoctorRegistrations = dayDoctors.Count();
            stats.PharmacistRegistrations = dayPharmacists.Count();
            stats.TotalRegistrations = stats.DoctorRegistrations + stats.PharmacistRegistrations;

            stats.PendingRegistrations = dayDoctors.Count(d => !d.IsApproved && !d.IsRejected)
                                       + dayPharmacists.Count(p => !p.IsApproved && !p.IsRejected);

            stats.ApprovedRegistrations = dayDoctors.Count(d => d.IsApproved)
                                        + dayPharmacists.Count(p => p.IsApproved);

            stats.RejectedRegistrations = dayDoctors.Count(d => d.IsRejected)
                                        + dayPharmacists.Count(p => p.IsRejected);

            return stats;
        }
    }
}