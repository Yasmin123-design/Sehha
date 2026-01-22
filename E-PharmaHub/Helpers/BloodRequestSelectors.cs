using E_PharmaHub.Dtos;
using E_PharmaHub.Models;

namespace E_PharmaHub.Helpers
{
    public static class BloodRequestSelectors
    {
        public static BloodRequestResponseDto ToBloodRequestResponseDto(this BloodRequest request)
        {
            return new BloodRequestResponseDto
            {
                Id = request.Id,
                RequestedByUserId = request.RequestedByUserId,
                RequiredType = request.RequiredType,
                City = request.HospitalCity,
                Country = request.HospitalCountry,
                Longitude = request.HospitalLongitude,
                Latitude = request.HospitalLatitude,
                HospitalName = request.HospitalName,
                Units = request.Units,
                NeedWithin = request.NeedWithin,
                Fulfilled = request.Fulfilled,
                CreatedAt = request.CreatedAt
            };
        }

        public static IQueryable<BloodRequestResponseDto> SelectBloodRequestResponseDto(this IQueryable<BloodRequest> query)
        {
            return query.Select(request => new BloodRequestResponseDto
            {
                Id = request.Id,
                RequestedByUserId = request.RequestedByUserId,
                RequiredType = request.RequiredType,
                City = request.HospitalCity,
                Country = request.HospitalCountry,
                Latitude = request.HospitalLatitude,
                Longitude = request.HospitalLongitude,
                HospitalName = request.HospitalName,
                Units = request.Units,
                NeedWithin = request.NeedWithin,
                Fulfilled = request.Fulfilled,
                CreatedAt = request.CreatedAt
            });
        }
    }
}
