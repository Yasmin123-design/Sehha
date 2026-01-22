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
                LastDonationDate = donor.LastDonationDate
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
                LastDonationDate = d.LastDonationDate
            });
        }
    }
}
