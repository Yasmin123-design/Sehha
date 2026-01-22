using E_PharmaHub.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_PharmaHub.Models
{
    public class DonorProfile
    {
        [Key] public int Id { get; set; }
        public virtual AppUser? AppUser { get; set; }
        public string AppUserId { get; set; }
        public BloodType BloodType { get; set; }
        public string DonorCity { get; set; }
        public string DonorCountry { get; set; }
        public double DonorLatitude { get; set; }
        public double DonorLongitude { get; set; }
        public bool IsAvailable { get; set; } = true;
        public DateTime? LastDonationDate { get; set; }
        public int BloodRequestId { get; set; }
        public virtual BloodRequest? BloodRequest { get; set; }
    }
}