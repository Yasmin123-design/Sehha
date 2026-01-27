using E_PharmaHub.Dtos;
using E_PharmaHub.Helpers;
using E_PharmaHub.Models;
using E_PharmaHub.Models.Enums;
using E_PharmaHub.Services.PaymentServ;
using E_PharmaHub.Services.NotificationServ;
using E_PharmaHub.UnitOfWorkes;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace E_PharmaHub.Services.DonorServ
{
    public class DonorService : IDonorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly IPaymentService _paymentService;
        private readonly INotificationService _notificationService;

        public DonorService(IUnitOfWork unitOfWork,
            UserManager<AppUser> userManager,
            IPaymentService paymentService,
            INotificationService notificationService
            )
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _paymentService = paymentService;
            _notificationService = notificationService;
        }

        public async Task<IEnumerable<DonorReadDto>> GetAllDetailsAsync()
        {
            return await _unitOfWork.Donors.GetQueryable()
                .SelectDonorReadDto()
                .ToListAsync();
        }

        public async Task<IEnumerable<DonorReadDto>> GetByFilterAsync(BloodType? type, string? city)
        {
            var query = _unitOfWork.Donors.GetQueryable();

            if (type.HasValue)
                query = query.Where(d => d.BloodType == type.Value);

            if (!string.IsNullOrEmpty(city))
                query = query.Where(d => d.DonorCity.ToLower() == city.ToLower());

            return await query
                .SelectDonorReadDto()
                .ToListAsync();
        }

        public async Task<IEnumerable<DonorReadDto>> GetDonorsByRequestIdAsync(int requestId)
        {
            return await _unitOfWork.Donors.GetQueryable()
                .Where(d => d.BloodRequestId == requestId)
                .SelectDonorReadDto()
                .ToListAsync();
        }

        public async Task<IEnumerable<MyDonationResponseDto>> GetMyDonationsAsync(string userId)
        {
            return await _unitOfWork.Donors.GetQueryable()
                .Include(d => d.BloodRequest)
                .ThenInclude(r => r.RequestedBy)
                .Where(d => d.AppUserId == userId)
                .SelectMyDonationResponseDto()
                .ToListAsync();
        }
        public async Task<DonorProfile?> GetByUserIdAsync(string userId)
        {
            return await _unitOfWork.Donors.GetByUserIdAsync(userId);
        }

        public async Task<DonorReadDto> RegisterAsync(DonorRegisterDto dto)
        {
            var request = await _unitOfWork.BloodRequest.GetByIdAsync(dto.BloodRequestId);
            var donor = new DonorProfile
            {
                AppUserId = dto.UserId,
                BloodRequestId = dto.BloodRequestId,
                BloodType = request.RequiredType,
                DonorTelephone = dto.PhoneNumber,
                DonorCity = dto.City,
                DonorCountry = dto.Country,
                DonorLatitude = dto.Latitude,
                DonorLongitude = dto.Longitude,
                IsAvailable = true
            };

            await _unitOfWork.Donors.AddAsync(donor);
            await _unitOfWork.CompleteAsync();

            if (request != null && !string.IsNullOrEmpty(request.RequestedByUserId))
            {
                await _notificationService.CreateAndSendAsync(
                    userId: request.RequestedByUserId,
                    title: "Blood Donation Received",
                    message: $"Someone has donated for your {request.RequiredType} blood request.Check your blood donations part.",
                    type: NotificationType.BloodDonation
                );
            }

            return donor.ToDonorReadDto();
        }

        public async Task<bool> UpdateAvailabilityAsync(string userId, int donorId, bool isAvailable)
        {
            var donor = await _unitOfWork.Donors.GetByIdAsync(donorId);
            if (donor == null || donor.AppUserId != userId) return false;

            var result = await _unitOfWork.Donors.UpdateAvailabilityAsync(donorId, isAvailable);
            await _unitOfWork.CompleteAsync();
            return result;
        }
        public async Task UpdateAsync(DonorProfile donor)
        {
            _unitOfWork.Donors.Update(donor);
            await _unitOfWork.CompleteAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var donor = await _unitOfWork.Donors.GetByIdAsync(id);
            if (donor == null)
                throw new Exception("Donor not found");
            _unitOfWork.Donors.Delete(donor);
            await _unitOfWork.CompleteAsync();
        }
    }
}
