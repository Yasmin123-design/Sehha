using E_PharmaHub.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace E_PharmaHub.Dtos
{
    public class DonorRegisterDto
    {
        public string? UserId { get; set; }
        public int BloodRequestId { get; set; }

        [EnumDataType(typeof(BloodType), ErrorMessage = "Invalid blood type value.")]
        public BloodType? BloodType { get; set; }

        [Required(ErrorMessage = "City is required.")]
        [StringLength(50, ErrorMessage = "City name must not exceed 50 characters.")]
        public string City { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        public string Country { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
