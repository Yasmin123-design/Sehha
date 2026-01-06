using E_PharmaHub.Dtos;
using E_PharmaHub.Models;

namespace E_PharmaHub.Helpers
{
    public static class PharmacistSelector
    {
        public static PharmacistReadDto MapToDto(PharmacistProfile p)
        {
            if (p == null) return null;

            return new PharmacistReadDto
            {
                Id = p.Id,
                Email = p.AppUser.Email,
                UserId = p.AppUser.Id,
                LicenseNumber = p.LicenseNumber,
                IsApproved = p.IsApproved,
                IsReject = p.IsRejected,
                PharmacyId = p.Pharmacy?.Id,
                PharmacyName = p.Pharmacy?.Name,
                PharmacyPhone = p.Pharmacy?.Phone,
                PharmacyImagePath = p.Pharmacy?.ImagePath,
                City = p.Pharmacy?.Address?.City,
                PharmacistImage = p.AppUser.ProfileImage
            };
        }
    }
}
