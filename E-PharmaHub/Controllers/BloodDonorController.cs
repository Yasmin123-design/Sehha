using E_PharmaHub.Dtos;
using E_PharmaHub.Models.Enums;
using E_PharmaHub.Services.DonorServ;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_PharmaHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "RegularUser,Admin")]
    public class BloodDonorController : ControllerBase
    {
        private readonly IDonorService _donorService;

        public BloodDonorController(IDonorService donorService)
        {
            _donorService = donorService;
        }

        //[HttpGet]
        //public async Task<IActionResult> GetAll()
        //{
        //    var donors = await _donorService.GetAllDetailsAsync();
        //    return Ok(donors);
        //}

        //[HttpGet("filter")]
        //public async Task<IActionResult> Filter([FromQuery] BloodType? type, [FromQuery] string? city)
        //{
        //    var donors = await _donorService.GetByFilterAsync(type, city);
        //    return Ok(donors);
        //}

        [HttpGet("by-request/{requestId}")]
        public async Task<IActionResult> GetByRequestId(int requestId)
        {
            var donors = await _donorService.GetDonorsByRequestIdAsync(requestId);
            return Ok(donors);
        }

        [HttpPost("Donate")]
        public async Task<IActionResult> Register([FromBody] DonorRegisterDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            dto.UserId = userId;
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _donorService.RegisterAsync(dto);
                return Ok(new
                {
                    message = "Donor Donated successfully!",
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    error = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }

        }

        [HttpPut("availability/{donorId}")]
        public async Task<IActionResult> UpdateAvailability([FromBody] bool isAvailable , int donorId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var success = await _donorService.UpdateAvailabilityAsync(userId, donorId, isAvailable);
            if (!success) return NotFound("Donor not found or unauthorized.");
            return Ok(new { message = "Availability updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _donorService.DeleteAsync(id);
            return Ok(new { message = "Donor deleted successfully" });
        }
    }
}
