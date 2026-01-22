using E_PharmaHub.Dtos;
using E_PharmaHub.Helpers;
using E_PharmaHub.Models;
using E_PharmaHub.Repositories;
using E_PharmaHub.UnitOfWorkes;

namespace E_PharmaHub.Services.BloodRequestServ
{
    public class BloodRequestService : IBloodRequestService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BloodRequestService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<IEnumerable<BloodRequestResponseDto>> GetAllBloodRequestsDtoAsync()
        {
            var requests = await _unitOfWork.BloodRequest.GetAllAsync();
            return requests.Select(r => r.ToBloodRequestResponseDto());
        }

        public async Task<BloodRequestResponseDto?> GetRequestByIdAsync(int id)
        {
            var request = await _unitOfWork.BloodRequest.GetByIdAsync(id);
            return request?.ToBloodRequestResponseDto();
        }

        public async Task<IEnumerable<BloodRequestResponseDto>> GetMyRequestsAsync(string userId)
        {
            var requests = await _unitOfWork.BloodRequest.GetByUserIdAsync(userId);
            return requests.Select(r => r.ToBloodRequestResponseDto());
        }

        public async Task<IEnumerable<BloodRequestResponseDto>> GetUnfulfilledRequestsAsync()
        {
            var requests = await _unitOfWork.BloodRequest.GetUnfulfilledRequestsAsync();
            return requests.Select(r => r.ToBloodRequestResponseDto());
        }

        public async Task<BloodRequest> AddRequestAsync(BloodRequest request)
        {
            await _unitOfWork.BloodRequest.AddAsync(request);
            await _unitOfWork.CompleteAsync();
            return request;
        }

        public async Task<bool> UpdateRequestAsync(int id, BloodRequest updatedRequest)
        {
            var existing = await _unitOfWork.BloodRequest.GetByIdAsync(id);
            if (existing == null) return false;

            existing.RequiredType = updatedRequest.RequiredType;
            existing.HospitalCity = updatedRequest.HospitalCity;
            existing.HospitalCountry = updatedRequest.HospitalCountry;
            existing.HospitalLongitude = updatedRequest.HospitalLongitude;
            existing.HospitalLatitude = updatedRequest.HospitalLatitude;
            existing.HospitalName = updatedRequest.HospitalName;
            existing.Units = updatedRequest.Units;
            existing.NeedWithin = updatedRequest.NeedWithin;
            existing.Fulfilled = updatedRequest.Fulfilled;

            _unitOfWork.BloodRequest.Update(existing);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> DeleteRequestAsync(int id)
        {
            var existing = await _unitOfWork.BloodRequest.GetByIdAsync(id);
            if (existing == null) return false;

            _unitOfWork.BloodRequest.Delete(existing);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }

}
