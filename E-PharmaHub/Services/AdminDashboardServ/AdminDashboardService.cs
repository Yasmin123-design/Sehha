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

            // Paid Payment IDs (using PaymentIntentId)
            var paidPaymentIds = payments.Where(p => !string.IsNullOrEmpty(p.PaymentIntentId)).Select(p => (int?)p.Id).ToHashSet();

            result.TopDoctors = doctors
                .Select(d =>
                {
                    var doctorAppointments = appointments.Where(a => a.DoctorId == d.AppUserId);
                    var validAppointments = doctorAppointments.Where(a =>
                        (a.Status == AppointmentStatus.Confirmed || a.Status == AppointmentStatus.Completed) &&
                        paidPaymentIds.Contains(a.PaymentId)
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
                        paidPaymentIds.Contains(o.PaymentId)
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

        public async Task<IEnumerable<DailyRevenueReportDto>> GetDailyRevenueReportAsync(int? month, int? year)
        {
            var payments = await _unitOfWork.Payments.GetAllAsync();

            var query = payments
                .Where(p => !string.IsNullOrEmpty(p.PaymentIntentId) &&
                           (p.PaymentFor == PaymentForType.DoctorRegistration || p.PaymentFor == PaymentForType.PharmacistRegistration));

            if (year.HasValue)
                query = query.Where(p => p.ProcessedAt.ToEgyptTime().Year == year.Value);

            if (month.HasValue)
                query = query.Where(p => p.ProcessedAt.ToEgyptTime().Month == month.Value);

            var report = query
                .GroupBy(p => p.ProcessedAt.ToEgyptTime().Date)
                .Select(g => new DailyRevenueReportDto
                {
                    Date = g.Key,
                    DoctorRevenue = g.Where(p => p.PaymentFor == PaymentForType.DoctorRegistration).Sum(p => p.Amount),
                    PharmacistRevenue = g.Where(p => p.PaymentFor == PaymentForType.PharmacistRegistration).Sum(p => p.Amount),
                    TotalRevenue = g.Sum(p => p.Amount)
                })
                .OrderBy(r => r.Date)
                .ToList();

            return report;
        }

        public async Task<IEnumerable<DailyCountReportDto>> GetDailyRegistrationCountReportAsync(int? month, int? year)
        {
            var payments = await _unitOfWork.Payments.GetAllAsync();
            var doctors = await _unitOfWork.Doctors.GetAllAsync();
            var pharmacists = await _unitOfWork.PharmasistsProfile.GetAllAsync();

            var usersWithPayment = payments
                .Where(p => !string.IsNullOrEmpty(p.PaymentIntentId))
                .Select(p => p.ReferenceId)
                .ToHashSet();

            var validDoctors = doctors.Where(d => !string.IsNullOrEmpty(d.AppUserId) && usersWithPayment.Contains(d.AppUserId));
            var validPharmacists = pharmacists.Where(p => !string.IsNullOrEmpty(p.AppUserId) && usersWithPayment.Contains(p.AppUserId));

            if (year.HasValue)
            {
                validDoctors = validDoctors.Where(d => d.CreatedAt.ToEgyptTime().Year == year.Value);
                validPharmacists = validPharmacists.Where(p => p.CreatedAt.ToEgyptTime().Year == year.Value);
            }

            if (month.HasValue)
            {
                validDoctors = validDoctors.Where(d => d.CreatedAt.ToEgyptTime().Month == month.Value);
                validPharmacists = validPharmacists.Where(p => p.CreatedAt.ToEgyptTime().Month == month.Value);
            }

            var doctorGroups = validDoctors
                .GroupBy(d => d.CreatedAt.ToEgyptTime().Date)
                .Select(g => new { Date = g.Key, Count = g.Count() });

            var pharmacistGroups = validPharmacists
                .GroupBy(p => p.CreatedAt.ToEgyptTime().Date)
                .Select(g => new { Date = g.Key, Count = g.Count() });

            var allDates = doctorGroups.Select(g => g.Date)
                .Union(pharmacistGroups.Select(g => g.Date))
                .OrderBy(d => d);

            var report = allDates.Select(date => new DailyCountReportDto
            {
                Date = date,
                DoctorCount = doctorGroups.FirstOrDefault(g => g.Date == date)?.Count ?? 0,
                PharmacistCount = pharmacistGroups.FirstOrDefault(g => g.Date == date)?.Count ?? 0,
                TotalCount = (doctorGroups.FirstOrDefault(g => g.Date == date)?.Count ?? 0) +
                             (pharmacistGroups.FirstOrDefault(g => g.Date == date)?.Count ?? 0)
            }).ToList();

            return report;
        }

        public async Task<IEnumerable<DailyOrdersReportDto>> GetDailyOrdersReportAsync(int? month, int? year)
        {
            var orders = await _unitOfWork.Order.GetAllEntitiesAsync();
            var payments = await _unitOfWork.Payments.GetAllAsync();

            var paidPaymentIds = payments.Where(p => !string.IsNullOrEmpty(p.PaymentIntentId)).Select(p => (int?)p.Id).ToHashSet();

            var query = orders.Where(o => paidPaymentIds.Contains(o.PaymentId));

            if (year.HasValue)
                query = query.Where(o => o.CreatedAt.ToEgyptTime().Year == year.Value);

            if (month.HasValue)
                query = query.Where(o => o.CreatedAt.ToEgyptTime().Month == month.Value);

            var report = query
                .GroupBy(o => o.CreatedAt.ToEgyptTime().Date)
                .Select(g => new DailyOrdersReportDto
                {
                    Date = g.Key,
                    PendingCount = g.Count(o => o.Status == OrderStatus.Pending),
                    ConfirmedCount = g.Count(o => o.Status == OrderStatus.Confirmed),
                    DeliveredCount = g.Count(o => o.Status == OrderStatus.Delivered),
                    CancelledCount = g.Count(o => o.Status == OrderStatus.Cancelled),
                    TotalCount = g.Count()
                })
                .OrderBy(r => r.Date)
                .ToList();

            return report;
        }

        public async Task<IEnumerable<DailyAppointmentsReportDto>> GetDailyAppointmentsReportAsync(int? month, int? year)
        {
            var appointments = await _unitOfWork.Appointments.GetAllAsync();
            var payments = await _unitOfWork.Payments.GetAllAsync();

            var paidPaymentIds = payments.Where(p => !string.IsNullOrEmpty(p.PaymentIntentId)).Select(p => (int?)p.Id).ToHashSet();

            var query = appointments.Where(a => paidPaymentIds.Contains(a.PaymentId));

            if (year.HasValue)
                query = query.Where(a => a.CreatedAt.ToEgyptTime().Year == year.Value);

            if (month.HasValue)
                query = query.Where(a => a.CreatedAt.ToEgyptTime().Month == month.Value);

            var report = query
                .GroupBy(a => a.CreatedAt.ToEgyptTime().Date)
                .Select(g => new DailyAppointmentsReportDto
                {
                    Date = g.Key,
                    PendingCount = g.Count(a => a.Status == AppointmentStatus.Pending),
                    ConfirmedCount = g.Count(a => a.Status == AppointmentStatus.Confirmed),
                    CompletedCount = g.Count(a => a.Status == AppointmentStatus.Completed),
                    CancelledCount = g.Count(a => a.Status == AppointmentStatus.Cancelled),
                    TotalCount = g.Count()
                })
                .OrderBy(r => r.Date)
                .ToList();

            return report;
        }

        public async Task<IEnumerable<SpecialtyDoctorCountDto>> GetSpecialtyDoctorCountReportAsync()
        {
            var doctors = await _unitOfWork.Doctors.GetAllAsync();
            var payments = await _unitOfWork.Payments.GetAllAsync();

            var usersWithPayment = payments
                .Where(p => !string.IsNullOrEmpty(p.PaymentIntentId))
                .Select(p => p.ReferenceId)
                .ToHashSet();

            var validDoctors = doctors
                .Where(d => !string.IsNullOrEmpty(d.AppUserId) && usersWithPayment.Contains(d.AppUserId))
                .ToList();

            var specialties = Enum.GetValues(typeof(Speciality)).Cast<Speciality>();

            var report = specialties.Select(s => new SpecialtyDoctorCountDto
            {
                SpecialtyName = s.ToString(),
                DoctorCount = validDoctors.Count(d => d.Specialty == s)
            }).ToList();

            return report;
        }

        public async Task<IEnumerable<DailyRegistrationStatusReportDto>> GetDailyDoctorRegistrationStatusReportAsync(int? month, int? year)
        {
            var doctors = await _unitOfWork.Doctors.GetAllAsync();
            var payments = await _unitOfWork.Payments.GetAllAsync();

            var usersWithPayment = payments
                .Where(p => !string.IsNullOrEmpty(p.PaymentIntentId))
                .Select(p => p.ReferenceId)
                .ToHashSet();

            var validDoctors = doctors.Where(d => !string.IsNullOrEmpty(d.AppUserId) && usersWithPayment.Contains(d.AppUserId));

            if (year.HasValue)
                validDoctors = validDoctors.Where(d => d.CreatedAt.ToEgyptTime().Year == year.Value);

            if (month.HasValue)
                validDoctors = validDoctors.Where(d => d.CreatedAt.ToEgyptTime().Month == month.Value);

            var report = validDoctors
                .GroupBy(d => d.CreatedAt.ToEgyptTime().Date)
                .Select(g => new DailyRegistrationStatusReportDto
                {
                    Date = g.Key,
                    PendingCount = g.Count(d => !d.IsApproved && !d.IsRejected),
                    ApprovedCount = g.Count(d => d.IsApproved),
                    RejectedCount = g.Count(d => d.IsRejected),
                    TotalCount = g.Count()
                })
                .OrderBy(r => r.Date)
                .ToList();

            return report;
        }

        public async Task<IEnumerable<DailyRegistrationStatusReportDto>> GetDailyPharmacistRegistrationStatusReportAsync(int? month, int? year)
        {
            var pharmacists = await _unitOfWork.PharmasistsProfile.GetAllAsync();
            var payments = await _unitOfWork.Payments.GetAllAsync();

            var usersWithPayment = payments
                .Where(p => !string.IsNullOrEmpty(p.PaymentIntentId))
                .Select(p => p.ReferenceId)
                .ToHashSet();

            var validPharmacists = pharmacists.Where(p => !string.IsNullOrEmpty(p.AppUserId) && usersWithPayment.Contains(p.AppUserId));

            if (year.HasValue)
                validPharmacists = validPharmacists.Where(p => p.CreatedAt.ToEgyptTime().Year == year.Value);

            if (month.HasValue)
                validPharmacists = validPharmacists.Where(p => p.CreatedAt.ToEgyptTime().Month == month.Value);

            var report = validPharmacists
                .GroupBy(p => p.CreatedAt.ToEgyptTime().Date)
                .Select(g => new DailyRegistrationStatusReportDto
                {
                    Date = g.Key,
                    PendingCount = g.Count(p => !p.IsApproved && !p.IsRejected),
                    ApprovedCount = g.Count(p => p.IsApproved),
                    RejectedCount = g.Count(p => p.IsRejected),
                    TotalCount = g.Count()
                })
                .OrderBy(r => r.Date)
                .ToList();

            return report;
        }

        private AdminStats CalculateStats(
            IEnumerable<E_PharmaHub.Models.Payment> payments,
            IEnumerable<E_PharmaHub.Models.DoctorProfile> doctors,
            IEnumerable<E_PharmaHub.Models.PharmacistProfile> pharmacists,
            DateTime date)
        {
            var stats = new AdminStats();

            var regPayments = payments.Where(p => p.ProcessedAt.ToEgyptTime().Date == date.Date && !string.IsNullOrEmpty(p.PaymentIntentId));

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
