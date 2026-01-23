using E_PharmaHub.Dtos;
using E_PharmaHub.Services.AppointmentServ;
using E_PharmaHub.Services.ClinicServ;
using E_PharmaHub.Services.DoctorServ;
using E_PharmaHub.Services.MedicineServ;
using E_PharmaHub.Services.NotificationServ;
using E_PharmaHub.Services.OrderServ;
using E_PharmaHub.Services.PaymentServ;
using E_PharmaHub.Services.PharmacistServ;
using E_PharmaHub.Services.PharmacyServ;
using E_PharmaHub.Services.UserServ;
using E_PharmaHub.Services.AdminDashboardServ;
using E_PharmaHub.Services.ChatServ;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using E_PharmaHub.Services.DonorServ;
using E_PharmaHub.Services.BloodRequestServ;
using E_PharmaHub.Models;

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
        private readonly IAppointmentService _appointmentService;
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        private readonly IPaymentService _paymentService;
        private readonly INotificationService _notificationService;
        private readonly IAdminDashboardService _adminDashboardService;
        private readonly IChatService _chatService;
        private readonly IDonorService _donorService;
        private readonly IBloodRequestService _bloodRequestService;
        public AdminController(
            IDoctorService doctorService,
            IPharmacistService pharmacistService,
            IPharmacyService pharmacyService,
            IClinicService clinicService,
            IMedicineService medicineService,
            IAppointmentService appointmentService,
            IOrderService orderService,
            IUserService userService,
            IPaymentService paymentService,
            INotificationService notificationService,
            IAdminDashboardService adminDashboardService,
            IChatService chatService,
            IDonorService donorService,
            IBloodRequestService bloodRequestService
            )
        {
            _doctorService = doctorService;
            _paymentService = paymentService;
            _pharmacistService = pharmacistService;
            _pharmacyService = pharmacyService;
            _notificationService = notificationService;
            _clinicService = clinicService;
            _medicineService = medicineService;
            _appointmentService = appointmentService;
            _orderService = orderService;
            _userService = userService;
            _adminDashboardService = adminDashboardService;
            _chatService = chatService;
            _donorService = donorService;
            _bloodRequestService = bloodRequestService;
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
        public async Task<IActionResult> UpdateDoctorProfileByAdmin(
    string userId,
    [FromForm] DoctorUpdateDto dto,
    [FromForm] IFormFile? image
)
        {
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var (success, errorMessage) = await _doctorService.UpdateDoctorProfileAsync(userId, dto, image);

            if (!success)
                return BadRequest(new { message = errorMessage });

            return Ok(new { message = "Doctor profile updated successfully" });
        }

        [HttpPut("updatepharmacist-profile/{userId}")]
        public async Task<IActionResult> UpdatePharmacistProfileByAdmin(
    [FromForm] PharmacistUpdateDto dto,
    IFormFile? image,
    string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User not authenticated." });

            var (success, message) = await _pharmacistService.UpdatePharmacistProfileAsync(userId, dto, image);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message = "Pharmacist profile updated successfully ✅" });
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
        [HttpGet("pharmacy/{pharmacyId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetByPharmacy(int pharmacyId)
        {
            if (pharmacyId == null)
                return NotFound("Pharmacy not found for this pharmacist");

            var items = await _medicineService
                .GetMedicinesByPharmacyIdAsync(pharmacyId);

            if (!items.Any())
                return NotFound("No medicines found for this pharmacy");

            return Ok(items);
        }

        [HttpPost("addmedicinetopharmacy/{pharmacyId}")]
        public async Task<IActionResult> Add([FromForm] MedicineDto dto, IFormFile? image,int pharmacyId)
        {
           
            var result = await _medicineService.AddMedicineWithInventoryAsync(dto, image,pharmacyId);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        [HttpPut("updatemedicine/{id}/{pharmacyId}")]
        public async Task<IActionResult> Update(int id, [FromForm] MedicineDto dto, IFormFile? image, int pharmacyId)
        {
           
                await _medicineService.UpdateMedicineAsync(id, dto, image, pharmacyId);
            return Ok(new { message = "Medicine updated successfully." });
        }

        [HttpDelete("deletemedicine/{id}/{pharmacyId}")]
        public async Task<IActionResult> Delete(int id,int pharmacyId)
        {
           
            await _medicineService.DeleteMedicineAsync(id, pharmacyId);
            return Ok(new { message = "Medicine deleted successfully." });
        }

        [HttpGet("doctorappointments/{userId}")]
        public async Task<IActionResult> GetByDoctor(string userId)

        {
            var appointments = await _appointmentService.GetAppointmentsByDoctorAsync(userId);
            if (!appointments.Any())
                return NotFound(new { message = "No appointments found for this doctor." });

            return Ok(appointments);
        }

        [HttpPatch("completeappointment/{id}")]
        public async Task<IActionResult> CompleteAppointment(int id)
        {
            var result = await _appointmentService.CompleteAppointmentAsync(id);
            if (!result) return NotFound(new { message = "Appointment not found" });

            return Ok(new { message = $"Appointment status updated to Completed" });
        }

        [HttpPut("approveappointment/{appointmentId}/{userId}")]
        public async Task<IActionResult> ApproveAppointment(int appointmentId,string userId)
        {
            var (success, message) = await _appointmentService.ApproveAppointmentAsync(appointmentId, userId);
            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }

        [HttpPut("rejectappointment/{appointmentId}")]
        public async Task<IActionResult> RejectAppointment(int appointmentId)
        {
            var (success, message) = await _appointmentService.RejectAppointmentAsync(appointmentId);
            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }

        [HttpGet("getordersforpharmacy/{pharmacistId}")]
        public async Task<IActionResult> GetOrdersForMyPharmacy(string pharmacistId)
        {

            var pharmacist = await _pharmacistService.GetPharmacistProfileByUserIdAsync(pharmacistId);
            if (pharmacist == null || pharmacist.PharmacyId == null)
                return BadRequest("No pharmacy found for this pharmacist.");

            var orders = await _orderService.GetOrdersByPharmacyIdAsync(pharmacist.PharmacyId);
            if (orders == null || !orders.Any())
                return NotFound("No orders found for this pharmacy.");

            return Ok(orders);
        }

        [HttpGet("getorder/{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null )
                return NotFound("Order not found or does not belong to your pharmacy.");

            return Ok(order);
        }

        [HttpPut("{id}/acceptorder")]
        public async Task<IActionResult> AcceptOrder(int id)
        {
            var (success, message) = await _orderService.AcceptOrderAsync(id);
            if (!success)
                return BadRequest(message);

            return Ok(message);
        }

        [HttpPut("{id}/cancelorder")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var (success, message) = await _orderService.CancelOrderAsync(id);
            if (!success)
                return BadRequest(message);

            return Ok(message);
        }

        [HttpPut("{id}/deliveredorder")]
        public async Task<IActionResult> MarkAsDelivered(int id)
        {
            var (success, message) = await _orderService.MarkAsDeliveredAsync(id);
            if (!success)
                return BadRequest(message);

            return Ok(message);
        }
        [HttpGet("getallregularusers")]
        public async Task<IActionResult> GetRegularUsers()
        {
            var users = await _userService.GetRegularUsersAsync();
            return Ok(users);
        }

        [HttpPost("change-password-for-regularuser")]
        public async Task<IActionResult> ChangePassword(ChangeUserPasswordDto dto)
        {
            var result = await _userService.ChangeUserPasswordAsync(dto.UserId, dto.NewPassword);

            if (!result)
                return BadRequest("Failed to change password");

            return Ok("Password changed successfully");
        }
        [HttpPut("update-clinic/{userId}")]
        public async Task<IActionResult> UpdateClinic([FromForm] ClinicUpdateDto dto,string userId, IFormFile? image)
        {    
            var (success, message) = await _clinicService.UpdateClinicAsync(userId, dto, image);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }
        [HttpGet("getdoctor/{userId}")]
        public async Task<IActionResult> GetDoctorByUserId(string userId)
        {
            var doctor = await _doctorService.GetDoctorByUserIdAsync(userId);

            if (doctor == null)
                return NotFound(new { message = "Doctor not found for this user id" });

            return Ok(doctor);
        }
        [HttpGet("getpharmacist/{userId}")]
        public async Task<IActionResult> GetPharmacistByUserId(string userId)
        {
            var result = await _pharmacistService.GetPharmacistByUserIdAsync(userId);

            if (result == null)
                return NotFound(new { message = "Pharmacist not found for this user id" });

            return Ok(result);
        }

        [HttpGet("user-orders/{userId}")]
        public async Task<IActionResult> GetUserOrders(string userId)
        {
            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            return Ok(orders);
        }

        [HttpGet("user-appointments/{userId}")]
        public async Task<IActionResult> GetByUser(string userId)
        {
            var appointments = await _appointmentService.GetAppointmentsByUserAsync(userId);
            if (!appointments.Any())
                return NotFound(new { message = "No appointments found for this patient." });

            return Ok(appointments);
        }
        [HttpGet("patientsofdoctor/{doctorId}")]
        public async Task<IActionResult> GetMyPatients(string doctorId)
        {            
            var patients = await _appointmentService.GetDoctorPatientsAsync(doctorId);

            return Ok(patients);
        }
        [HttpGet("payment/doctor-registrations")]
        public async Task<IActionResult> GetDoctorRegistrations()
        => Ok(await _paymentService.GetDoctorRegistrationPaymentsAsync());

        [HttpGet("payment/pharmacist-registrations")]
        public async Task<IActionResult> GetPharmacistRegistrations()
            => Ok(await _paymentService.GetPharmacistRegistrationPaymentsAsync());

        [HttpGet("payment/orders")]
        public async Task<IActionResult> GetOrdersPayments()
            => Ok(await _paymentService.GetOrderPaymentsAsync());

        [HttpGet("payment/appointments")]
        public async Task<IActionResult> GetAppointmentsPayments()
            => Ok(await _paymentService.GetAppointmentPaymentsAsync());

        [HttpGet("admin-notifications")]
        public async Task<IActionResult> GetUserNotifications()
        {
            var adminId = await _userService.GetAdminUserIdAsync();

            var result = await _notificationService
                .GetUserNotificationsByCategoryAsync(adminId, "Admin");

            return Ok(result);
        }

        [HttpGet("profile")]

        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var profile = await _userService.GetUserProfileAsync(userId);

            if (profile == null)
                return NotFound(new { message = "User not found ❌" });

            return Ok(profile);
        }

        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromForm] UserProfileDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var (success, message) = await _userService.UpdateProfileAsync(userId, dto);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }

        [HttpPut("update-password")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdatePassword([FromBody] UserPasswordUpdateDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var (success, message) = await _userService.UpdatePasswordAsync(userId, dto);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }
        [HttpGet("dashboard-stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var stats = await _adminDashboardService.GetAdminDashboardStatsAsync();
            return Ok(stats);
        }

        [HttpGet("top-performers")]
        public async Task<IActionResult> GetTopPerformers()
        {
            var result = await _adminDashboardService.GetTopPerformingEntitiesAsync();
            return Ok(result);
        }

        [HttpGet("daily-revenue-report")]
        public async Task<IActionResult> GetDailyRevenueReport([FromQuery] int? month, [FromQuery] int? year)
        {
            var report = await _adminDashboardService.GetDailyRevenueReportAsync(month, year);
            return Ok(report);
        }

        [HttpGet("daily-registration-count-report")]
        public async Task<IActionResult> GetDailyRegistrationCountReport([FromQuery] int? month, [FromQuery] int? year)
        {
            var report = await _adminDashboardService.GetDailyRegistrationCountReportAsync(month, year);
            return Ok(report);
        }

        [HttpGet("daily-orders-report")]
        public async Task<IActionResult> GetDailyOrdersReport([FromQuery] int? month, [FromQuery] int? year)
        {
            var report = await _adminDashboardService.GetDailyOrdersReportAsync(month, year);
            return Ok(report);
        }

        [HttpGet("daily-appointments-report")]
        public async Task<IActionResult> GetDailyAppointmentsReport([FromQuery] int? month, [FromQuery] int? year)
        {
            var report = await _adminDashboardService.GetDailyAppointmentsReportAsync(month, year);
            return Ok(report);
        }

        [HttpGet("specialty-doctor-count-report")]
        public async Task<IActionResult> GetSpecialtyDoctorCountReport()
        {
            var report = await _adminDashboardService.GetSpecialtyDoctorCountReportAsync();
            return Ok(report);
        }

        [HttpGet("daily-doctor-registration-status-report")]
        public async Task<IActionResult> GetDailyDoctorRegistrationStatusReport([FromQuery] int? month, [FromQuery] int? year)
        {
            var report = await _adminDashboardService.GetDailyDoctorRegistrationStatusReportAsync(month, year);
            return Ok(report);
        }

        [HttpGet("daily-pharmacist-registration-status-report")]
        public async Task<IActionResult> GetDailyPharmacistRegistrationStatusReport([FromQuery] int? month, [FromQuery] int? year)
        {
            var report = await _adminDashboardService.GetDailyPharmacistRegistrationStatusReportAsync(month, year);
            return Ok(report);
        }

        [HttpGet("admin-overview")]
        public async Task<IActionResult> GetAdminOverview()
        {
            var result = await _adminDashboardService.GetAdminOverviewAsync();
            return Ok(result);
        }

        [HttpPost("chat/start-with-pharmacist")]
        public async Task<IActionResult> StartConversationWithPharmacist([FromQuery] int pharmacistId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var thread = await _chatService.StartConversationWithPharmacistAsync(userId, pharmacistId);
            return Ok(thread);
        }

        [HttpPost("chat/start-with-doctor")]
        public async Task<IActionResult> StartConversationWithDoctor([FromQuery] int doctorId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var thread = await _chatService.StartConversationWithDoctorAsync(userId, doctorId);
            return Ok(thread);
        }


        [HttpPost("chat/send")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto)
        {
            var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var message = await _chatService.SendMessageAsync(dto.ThreadId, senderId, dto.Text);
            return Ok(message);
        }

        [HttpGet("chat/{threadId}/messages")]
        public async Task<IActionResult> GetMessages(int threadId)
        {
            var messages = await _chatService.GetMessagesAsync(threadId);
            return Ok(messages);
        }

        [HttpGet("chat/my-threads")]
        public async Task<IActionResult> GetMyThreads()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var threads = await _chatService.GetUserThreadsAsync(userId);
            return Ok(threads);
        }
        [HttpGet("blooddonor/getall")]
        public async Task<IActionResult> GetAllBloodDonor()
        {
            var donors = await _donorService.GetAllDetailsAsync();
            return Ok(donors);
        }
        [HttpDelete("blooddonor/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _donorService.DeleteAsync(id);
            return Ok(new { message = "Donor deleted successfully" });
        }
        [HttpGet("blooddonor/getbyrequestid/{requestId}")]
        public async Task<IActionResult> GetByRequestId(int requestId)
        {
            var donors = await _donorService.GetDonorsByRequestIdAsync(requestId);
            return Ok(donors);
        }
        [HttpGet("bloodrequest/getall")]
        public async Task<IActionResult> GetAllBloodRequest()
        {
            var result = await _bloodRequestService.GetAllBloodRequestsDtoAsync();
            return Ok(result);
        }
        [HttpGet("bloodrequest/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var request = await _bloodRequestService.GetRequestByIdAsync(id);
            if (request == null) return NotFound();
            return Ok(request);
        }
        [HttpPut("bloodrequest/{id}")]
        public async Task<IActionResult> UpdateBloodRequest(int id, [FromBody] BloodRequest updatedRequest)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var existing = await _bloodRequestService.GetRequestByIdAsync(id);
            if (existing == null) return NotFound("Blood request not found.");

            var isAdmin = User.IsInRole("Admin");

            if (existing.RequestedByUserId != userId && !isAdmin)
                return StatusCode(403, "You are not allowed to update this request.");

            var success = await _bloodRequestService.UpdateRequestAsync(id, updatedRequest);
            if (!success) return BadRequest("Failed to update request.");

            return Ok(new { message = "Blood request Updated successfully!" });
        }

        [HttpDelete("bloodrequest/{id}")]
        public async Task<IActionResult> DeleteBloodRequest(int id)
        {
            var success = await _bloodRequestService.DeleteRequestAsync(id);
            if (!success) return NotFound();
            return Ok(new { message = "Blood request Deleted successfully!" });
        }
    }
}
