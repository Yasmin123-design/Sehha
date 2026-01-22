using E_PharmaHub.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace E_PharmaHub.Models
{
    public class BloodRequest
    {
        [Key] public int Id { get; set; }
        public string? RequestedByUserId { get; set; } 
        public BloodType RequiredType { get; set; }
        public double HospitalLatitude { get; set; }
        public double HospitalLongitude { get; set; }
        public string HospitalCity { get; set; }
        public string HospitalCountry { get; set; }
        public string HospitalName { get; set; }
        public int Units { get; set; }
        public string NeedWithin { get; set; }
        public bool Fulfilled { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public virtual AppUser? RequestedBy { get; set; }
    }
}
