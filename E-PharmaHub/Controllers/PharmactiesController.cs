using E_PharmaHub.Dtos;
using E_PharmaHub.Services.PharmacistAnalyticsServ;
using E_PharmaHub.Services.PharmacistServ;
using E_PharmaHub.Services.PharmacyServ;
using E_PharmaHub.Services.ChatServ;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_PharmaHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PharmactiesController : ControllerBase
    {
        private readonly IPharmacistService _pharmacistService;
        private readonly IPharmacyService _pharmacyService;
        private readonly IPharmacistDashboardService _pharmacistDashboardService;
        private readonly IChatService _chatService;
        public PharmactiesController(
            IPharmacistService pharmacistService,
            IPharmacyService pharmacyService,
            IPharmacistDashboardService pharmacistDashboardService,
            IChatService chatService
            )
        {
            _pharmacistService = pharmacistService;
            _pharmacyService = pharmacyService;
            _pharmacistDashboardService = pharmacistDashboardService;
            _chatService = chatService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(
    [FromForm] PharmacistRegisterDto dto,
    IFormFile? pharmacyImage,
    IFormFile? pharmacistImage)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var response = await _pharmacistService.RegisterPharmacistAsync(
                    dto,
                    pharmacyImage,
                    pharmacistImage);

                return Ok(new
                {
                    message = "Pharmacist registered successfully! Awaiting admin approval.",
                    name = response.UserName,
                    email = response.Email,
                    pharmacistProfileId = response.PharmacistProfileId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Pharmacist")]
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdatePharmacistProfile(
      [FromForm] PharmacistUpdateDto dto,
      IFormFile? image)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User not authenticated." });

            var (success, message) = await _pharmacistService.UpdatePharmacistProfileAsync(userId, dto, image);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message = "Pharmacist profile updated successfully ✅" });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Pharmacist")]
        [HttpPut("update-pharmacy")]
        public async Task<IActionResult> UpdatePharmacy([FromForm] PharmacyUpdateDto dto, IFormFile? image)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
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

        [Authorize(
    AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
    Roles = "Pharmacist")]
        [HttpGet("orders-dashboard")]
        public async Task<IActionResult> GetOrdersDashboard()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _pharmacistDashboardService.GetWeeklyOrdersAsync(userId);
            return Ok(result);
        }

        [Authorize(
    AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
    Roles = "Pharmacist")]
        [HttpGet("categories-dashboard")]
        public async Task<IActionResult> GetCategoriesDashboard()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _pharmacistDashboardService.GetWeeklyCategoryItemsAsync(userId);
            return Ok(result);
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Pharmacist")]
        [HttpGet("dashboard/inventory")]
        public async Task<IActionResult> GetInventoryDashboard()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { message = "User not authenticated." });

                var dashboard = await _pharmacistDashboardService.GetInventoryDashboardAsync(userId);

                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("my-stats")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Pharmacist")]
        public async Task<IActionResult> GetMyDashboard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var stats = await _pharmacistDashboardService
                .GetDashboardStatsAsync(userId!);

            return Ok(stats);
        }
        [HttpGet("GetAnalyticsStats")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Pharmacist")]

        public async Task<IActionResult> GetDashboard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var pharmacist =
               await _pharmacistService.GetPharmacistProfileByUserIdAsync(userId);

            var dashboard = 
                await _pharmacistDashboardService.GetDashboardAsync(pharmacist.PharmacyId);
            return Ok(dashboard);
        }
        [HttpGet("sales-by-category")]
        [Authorize(
       AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
       Roles = "Pharmacist"
   )]
        public async Task<IActionResult> GetSalesByCategory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _pharmacistDashboardService
                .GetSalesByCategoryForPharmacistAsync(userId!);

            return Ok(result);
        }

        [HttpGet("best-selling")]
        [Authorize(
    AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
    Roles = "Pharmacist"
)]
        public async Task<IActionResult> GetBestSelling()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var result =
                await _pharmacistDashboardService.GetBestSellingMedicationsAsync(userId);

            return Ok(result);
        }
        [HttpGet("daily-revenue")]
        [Authorize(
     AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
     Roles = "Pharmacist"
 )]
        public async Task<IActionResult> GetDailyRevenue(
     [FromQuery] int? year,
     [FromQuery] int? month)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            if (year.HasValue && (year < 1 || year > 9999))
                return BadRequest("Year must be between 1 and 9999");

            if (month.HasValue && (month < 1 || month > 12))
                return BadRequest("Month must be between 1 and 12");

            var result =
                await _pharmacistDashboardService
                    .GetDailyRevenueAsync(userId, year, month);

            return Ok(result);
        }
        [HttpGet("today-sales-by-time")]
        [Authorize(
    AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
    Roles = "Pharmacist"
)]
        public async Task<IActionResult> GetTodaySalesByTime()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var result =
                await _pharmacistDashboardService.GetTodaySalesByTimeSlotsAsync(userId);

            return Ok(result);
        }
        [HttpGet("inventoryreport-last-30-days")]
        [Authorize(
    AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
    Roles = "Pharmacist"
)]
        public async Task<IActionResult> GetLast30DaysInventory()
        {
            var report = await _pharmacistDashboardService.GetLast30DaysInventoryReportAsync();
            return Ok(report);
        }
        [HttpGet("out-of-stock/last-30-days")]
        [Authorize(
    AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
    Roles = "Pharmacist"
)]
        public async Task<IActionResult> GetOutOfStockLast30Days()
        {
            var result = await _pharmacistDashboardService.GetOutOfStockLast30DaysAsync();
            return Ok(result);
        }

     
    }

}
