using E_PharmaHub.Dtos;
using E_PharmaHub.Models;

namespace E_PharmaHub.Helpers
{
    public static class DonorSelectors
    {
        public static DonorReadDto ToDonorReadDto(this DonorProfile donor)
        {
            if (donor == null) return null;

            return new DonorReadDto
            {
                Id = donor.Id,
                Email = donor.AppUser?.Email,
                BloodType = donor.BloodType.ToString(),
                City = donor.DonorCity,
                Country = donor.DonorCountry,
                Latitude = donor.DonorLatitude,
                Longitude = donor.DonorLongitude,
                IsAvailable = donor.IsAvailable,
                DonorTelephone = donor.DonorTelephone,
                LastDonationDate = donor.LastDonationDate,
                BloodRequest = donor.BloodRequest?.ToBloodRequestResponseDto()
            };
        }

        public static IQueryable<DonorReadDto> SelectDonorReadDto(this IQueryable<DonorProfile> query)
        {
            return query.Select(d => new DonorReadDto
            {
                Id = d.Id,
                Email = d.AppUser != null ? d.AppUser.Email : null,
                BloodType = d.BloodType.ToString(),
                City = d.DonorCity,
                Country = d.DonorCountry,
                Latitude = d.DonorLatitude,
                Longitude = d.DonorLongitude,
                IsAvailable = d.IsAvailable,
                DonorTelephone = d.DonorTelephone,
                LastDonationDate = d.LastDonationDate,
                BloodRequest = d.BloodRequest != null ? new BloodRequestResponseDto
                {
                    Id = d.BloodRequest.Id,
                    RequestedByUserId = d.BloodRequest.RequestedByUserId,
                    RequestedByUserName = d.BloodRequest.RequestedBy.UserName,
                    RequiredType = d.BloodRequest.RequiredType,
                    City = d.BloodRequest.HospitalCity,
                    Country = d.BloodRequest.HospitalCountry,
                    Latitude = d.BloodRequest.HospitalLatitude,
                    Longitude = d.BloodRequest.HospitalLongitude,
                    HospitalName = d.BloodRequest.HospitalName,
                    Units = d.BloodRequest.Units,
                    NeedWithin = d.BloodRequest.NeedWithin,
                    Fulfilled = d.BloodRequest.Fulfilled,
                    CreatedAt = d.BloodRequest.CreatedAt
                } : null
            });
        }
    }
}
