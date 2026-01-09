using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using E_PharmaHub.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace E_PharmaHub.Helpers
{
    public static class DoctorSelectors
    {
        public static Expression<Func<DoctorProfile, DoctorReadDto>> GetDoctorSelector(
            DbSet<Appointment> appointments,
            DbSet<FavoriteDoctor> favouriteDoctors)
        {
            return d => new DoctorReadDto
            {
                Id = d.Id,
                Email = d.AppUser.Email,
                CreatedAt = d.CreatedAt,
                UserId = d.AppUser.Id,
                Specialty = d.Specialty,
                IsApproved = d.IsApproved,
                IsRejected = d.IsRejected,
                ClinicName = d.Clinic.Name,
                ClinicPhone = d.Clinic.Phone,
                ClinicImagePath = d.Clinic.ImagePath,
                City = d.Clinic.Address.City,
                DoctorImage = d.AppUser.ProfileImage,
                Gender = d.Gender,
                ConsultationType = d.ConsultationType,
                ConsultationPrice = d.ConsultationPrice,
                Username = d.AppUser.UserName,
                Country = d.Clinic.Address.Country,
                Latitude = d.Clinic.Address.Latitude,
                Longitude = d.Clinic.Address.Longitude,
                Street = d.Clinic.Address.Street,
                PostalCode = d.Clinic.Address.PostalCode,
                ClinicId = d.Clinic.Id,
                AverageRating = d.Reviews.Any() ? d.Reviews.Average(r => r.Rating) : 0,
                CountReviews = d.Reviews.Count,

                CountPatient = appointments.Count(
                    a => a.DoctorId == d.AppUserId
                    && 
                    a.PaymentId != null &&
                    a.Payment != null &&
                    a.Payment.PaymentIntentId != null &&
                    a.Status == AppointmentStatus.Confirmed),
                CountFavourite = favouriteDoctors.Count(f => f.DoctorId == d.Id)
            };
        }
    }

}

