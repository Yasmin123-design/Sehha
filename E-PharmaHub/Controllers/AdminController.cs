using E_PharmaHub.Dtos;
using E_PharmaHub.Services.ClinicServ;
using E_PharmaHub.Services.DoctorServ;
using E_PharmaHub.Services.MedicineServ;
using E_PharmaHub.Services.PharmacistServ;
using E_PharmaHub.Services.PharmacyServ;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_PharmaHub.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IDoctorService _doctorService;
        private readonly IPharmacistService _pharmacistService;
        private readonly IPharmacyService _pharmacyService;
        private readonly IClinicService _clinicService;
        private readonly IMedicineService _medicineService;
        public AdminController(
            IDoctorService doctorService,
            IPharmacistService pharmacistService,
            IPharmacyService pharmacyService,
            IClinicService clinicService,
            IMedicineService medicineService
            )
        {
            _doctorService = doctorService;
            _pharmacistService = pharmacistService;
            _pharmacyService = pharmacyService;
            _clinicService = clinicService;
            _medicineService = medicineService;
        }
        [HttpGet("allDoctorsShowToAdmin")]
        public async Task<IActionResult> GetAllDoctorsShowToAdmin()
        {
            var doctor = await _doctorService.GetAllDoctorsShowToAdmin();
            if (doctor == null)
                return NotFound(new { message = "Doctor not found." });

            return Ok(doctor);
        }

        [HttpDelete("deletedoctorwithclinic/{id}")]
        public async Task<IActionResult> DeleteDoctorWithClinicWithAddressRelated(int id)
        {
            try
            {
                await _doctorService.DeleteDoctorAsync(id);
                return Ok(new { message = "Doctor deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("approvedoctor/{id}")]
        public async Task<IActionResult> ApproveDoctor(int id)
        {
            var (success, message) = await _doctorService.ApproveDoctorAsync(id);
            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }

        [HttpPut("rejectdoctor/{id}")]
        public async Task<IActionResult> RejectDoctor(int id)
        {
            var (success, message) = await _doctorService.RejectDoctorAsync(id);
            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }

        [HttpGet("allpharmacists")]
        public async Task<IActionResult> GetAllPharmacists()
        {
            var pharmacists = await _pharmacistService.GetAllPharmacistsAsync();
            return Ok(pharmacists);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPharmacistById(int id)
        {
            var pharmacist = await _pharmacistService.GetPharmacistByIdAsync(id);
            if (pharmacist == null)
                return NotFound(new { message = "Pharmacist not found." });

            return Ok(pharmacist);
        }

        [HttpDelete("deletepharmacistwithpharmacy/{id}")]
        public async Task<IActionResult> DeletePharmacistWithPharmacyRelated(int id)
        {
            try
            {
                await _pharmacistService.DeletePharmacistAsync(id);
                return Ok(new { message = "Pharmacist deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("approvepharmacist/{id}")]
        public async Task<IActionResult> ApprovePharmacist(int id)
        {
            var (success, message) = await _pharmacistService.ApprovePharmacistAsync(id);
            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }

        [HttpPut("rejectpharmacist/{id}")]
        public async Task<IActionResult> RejectPharmacist(int id)
        {
            var (success, message) = await _pharmacistService.RejectPharmacistAsync(id);
            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }

        [HttpPut("update-pharmacy/{userId}")]
        public async Task<IActionResult> UpdatePharmacy([FromForm] PharmacyUpdateDto dto, IFormFile? image,string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User not authenticated." });

            var pharmacistProfile = await _pharmacistService.GetPharmacistProfileByUserIdAsync(userId);
            if (pharmacistProfile == null)
                return NotFound(new { message = "Pharmacist profile not found." });

            if (pharmacistProfile.PharmacyId == null)
                return BadRequest(new { message = "You are not assigned to any pharmacy." });

            int pharmacyId = pharmacistProfile.PharmacyId;

            var (success, message) = await _pharmacyService.UpdatePharmacyAsync(pharmacyId, dto, image);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }

        [HttpPut("update-doctorprofile/{userId}")]
        public async Task<IActionResult> UpdateDoctorProfile(
[FromForm] DoctorUpdateDto dto ,string userId
)
        {
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _doctorService.UpdateDoctorProfileAsync(
                userId,
                dto
            );

            if (!result)
                return NotFound("Doctor not found");

            return Ok(new { message = "Doctor profile updated successfully" });
        }

        [HttpPut("updatepharmacist-profile/{userId}")]
        public async Task<IActionResult> UpdateProfile([FromForm] PharmacistUpdateDto dto, IFormFile? image , string userId)
        {
            try
            {
                var result = await _pharmacistService.UpdatePharmacistProfileAsync(userId, dto, image);

                if (!result)
                    return NotFound(new { message = "Pharmacist profile not found." });

                return Ok(new { message = "Pharmacist profile updated successfully ✅" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetPharmacyOfPharmacist/{userId}")]
        public async Task<IActionResult> GetPharmacyOfPharmacist(string userId)
        {
            var pharmacies = await _pharmacyService
                .GetMyPharmaciesAsync(userId!);

            return Ok(pharmacies);
        }

        [HttpGet("GetClinicOfDoctor/{userId}")]
        public async Task<IActionResult> GetClinicOfDoctor(string userId)
        {

            var clinic = await _clinicService.GetMyClinicsAsync(userId!);

            return Ok(clinic);
        }
        [HttpGet("pharmacy/{userId}")]
        [Authorize(
  AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme
)]
        public async Task<IActionResult> GetByPharmacy(string userId)
        {

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Invalid token");

            var pharmacyId = await _pharmacistService
                .GetPharmacyIdByUserIdAsync(userId);

            if (pharmacyId == null)
                return NotFound("Pharmacy not found for this pharmacist");

            var items = await _medicineService
                .GetMedicinesByPharmacyIdAsync(pharmacyId.Value);

            if (!items.Any())
                return NotFound("No medicines found for this pharmacy");

            return Ok(items);
        }
    }

}
