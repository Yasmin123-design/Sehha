using E_PharmaHub.Models;
using E_PharmaHub.Services.BloodRequestServ;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_PharmaHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "RegularUser,Admin")]

    public class BloodRequestController : ControllerBase
    {
        private readonly IBloodRequestService _bloodRequestService;

        public BloodRequestController(IBloodRequestService bloodRequestService)
        {
            _bloodRequestService = bloodRequestService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _bloodRequestService.GetAllBloodRequestsDtoAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var request = await _bloodRequestService.GetRequestByIdAsync(id);
            if (request == null) return NotFound();
            return Ok(request);
        }

        [HttpGet("unfulfilled")]
        public async Task<IActionResult> GetUnfulfilled()
        {
            var result = await _bloodRequestService.GetUnfulfilledRequestsAsync();
            return Ok(result);
        }

        [HttpGet("my-requests")]
        public async Task<IActionResult> GetMyRequests()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var result = await _bloodRequestService.GetMyRequestsAsync(userId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BloodRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            request.RequestedByUserId = userId;

            var newRequest = await _bloodRequestService.AddRequestAsync(request,userId);
            return Ok(new { message = "Blood request created successfully!"});
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] BloodRequest updatedRequest)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var existing = await _bloodRequestService.GetRequestByIdAsync(id);
            if (existing == null) return NotFound("Blood request not found.");

            if (existing.RequestedByUserId != userId)
                return Forbid("You are not allowed to update this request.");

            var success = await _bloodRequestService.UpdateRequestAsync(id, updatedRequest);
            if (!success) return BadRequest("Failed to update request.");

            return Ok(new { message = "Blood request Updated successfully!" });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _bloodRequestService.DeleteRequestAsync(id);
            if (!success) return NotFound();
            return Ok(new { message = "Blood request Deleted successfully!" });
        }
    }

}
