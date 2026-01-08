using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using E_PharmaHub.Models.Enums;
using System.Linq.Expressions;

namespace E_PharmaHub.Helpers
{
    public static class AppointmentSelectors
    {
        public static Expression<Func<Appointment, AppointmentResponseDto>> GetAppointmentSelector()
        {
            return a => new AppointmentResponseDto
            {
                Id = a.Id,

                CreatedAt = a.CreatedAt,

                ClinicId = a.ClinicId,
                ClinicName = a.Clinic != null ? a.Clinic.Name : "",
                ClinicImage = a.Clinic != null ? a.Clinic.ImagePath : null,

                AppointmentAmount = a.Doctor.DoctorProfile.ConsultationPrice,

                DoctorId = a.Doctor.DoctorProfile != null
                    ? a.Doctor.DoctorProfile.Id
                    : 0,

                DoctorAppUserId = a.Doctor.DoctorProfile != null
                    ? a.Doctor.DoctorProfile.AppUserId
                    : null,

                DoctorName = a.Doctor != null ? a.Doctor.UserName : "",
                DoctorImage = a.Doctor.ProfileImage,

                DoctorSpeciality = a.Doctor.DoctorProfile != null
                    ? a.Doctor.DoctorProfile.Specialty : Speciality.GeneralMedicine,
                UserId = a.UserId,
                UserNameLogged = a.User != null ? a.User.UserName : "",
                UserImageLogged = a.User.ProfileImage,
                PatientAge = a.PatientAge,
                PatientPhone = a.PatientPhone,
                PatientName = a.PatientName,
                PatientGender = a.PatientGender ,
                StartAt = a.StartAt ,
                EndAt = a.EndAt ,
                Status = a.Status 
            };
        }

        public static DoctorPatientDto ToDoctorPatientDto(Appointment appointment)
        {
            return new DoctorPatientDto
            {
                AppointmentId = appointment.Id,
                PatientName = appointment.PatientName,
                PatientId = appointment.UserId,
                PatientPhone = appointment.PatientPhone,
                PatientAge = appointment.PatientAge,
                PatientGender = appointment.PatientGender,
                StartAt = appointment.StartAt,
                EndAt = appointment.EndAt,
                Status = appointment.Status,
                IsPaid = appointment.IsPaid,
                ClinicName = appointment.Clinic?.Name
            };
        }

    }

}
