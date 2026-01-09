using E_PharmaHub.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace E_PharmaHub.Models
{
    public class DoctorProfile
    {
        [Key] public int Id { get; set; }
        [Required(ErrorMessage = "Specialty is required.")]
        public Speciality Specialty { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "ClinicId must be a valid positive number.")]
        public int? ClinicId { get; set; }
        public decimal ConsultationPrice { get; set; }
        public Gender Gender { get; set; }

        public ConsultationType ConsultationType { get; set; }
        public bool IsApproved { get; set; } = false;
        public bool IsRejected { get; set; } = false;
        public bool HasPaid { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public virtual Clinic? Clinic { get; set; }
        public string? AppUserId { get; set; }
        public virtual AppUser? AppUser { get; set; }
        public virtual ICollection<Review>? Reviews { get; set; }
        public virtual ICollection<DoctorAvailability> Availabilities { get; set; }
        = new List<DoctorAvailability>();

    }
}
