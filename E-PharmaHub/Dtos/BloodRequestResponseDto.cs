using E_PharmaHub.Models.Enums;

namespace E_PharmaHub.Dtos
{
    public class BloodRequestResponseDto
    {
        public int Id { get; set; }
        public string? RequestedByUserId { get; set; }
        public BloodType RequiredType { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string HospitalName { get; set; }
        public int Units { get; set; }
        public string NeedWithin { get; set; }
        public bool Fulfilled { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
